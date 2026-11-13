mock_provider "azurerm" {}
mock_provider "random" {}

variables {
  deployment          = "main"
  resource_group_name = "deployment-auth-tests"
}

run "development_creates_key_and_settings" {
  command = plan

  override_resource {
    target          = azurerm_linux_web_app.web
    override_during = plan
    values = {
      outbound_ip_address_list          = ["192.0.2.1"]
      possible_outbound_ip_address_list = ["192.0.2.1", "192.0.2.2"]
    }
  }

  variables {
    environment                     = "development"
    development_basic_auth_password = "test-only-basic-auth-password-with-at-least-32-characters"
  }

  assert {
    condition = (
      length(random_password.development_jwt_signing_key) == 1 &&
      contains(keys(azurerm_linux_web_app.api.app_settings), "DevelopmentJwt__SigningKey") &&
      contains(keys(azurerm_linux_web_app.web.app_settings), "DevelopmentJwt__SigningKey") &&
      azurerm_linux_web_app.web.app_settings["DevelopmentBasicAuthEnabled"] == "true"
    )
    error_message = "Development must provision the signing key and configure the development authentication gates."
  }

  assert {
    condition = (
      azurerm_linux_web_app.api.site_config[0].health_check_path == "/openapi/v1.json" &&
      azurerm_linux_web_app.api.site_config[0].health_check_eviction_time_in_min == 10
    )
    error_message = "Development must configure both the API health-check path and its required eviction time."
  }

  assert {
    condition = (
      toset([for rule in azurerm_linux_web_app.api.site_config[0].ip_restriction : rule.ip_address]) ==
      toset(["192.0.2.1/32", "192.0.2.2/32"]) &&
      alltrue([for rule in azurerm_linux_web_app.api.site_config[0].ip_restriction : rule.action == "Allow"]) &&
      azurerm_linux_web_app.api.site_config[0].ip_restriction_default_action == "Deny"
    )
    error_message = "API ingress must allow the Web app's full possible outbound IPv4 pool and deny other callers."
  }
}

run "staging_omits_development_credentials" {
  command = plan

  variables {
    environment = "staging"
  }

  assert {
    condition = (
      length(random_password.development_jwt_signing_key) == 0 &&
      !contains(keys(azurerm_linux_web_app.api.app_settings), "DevelopmentJwt__SigningKey") &&
      !contains(keys(azurerm_linux_web_app.web.app_settings), "DevelopmentJwt__SigningKey") &&
      !contains(keys(azurerm_linux_web_app.web.app_settings), "DevelopmentBasicAuthEnabled") &&
      !contains(keys(azurerm_linux_web_app.web.app_settings), "DevelopmentBasicAuthPassword") &&
      azurerm_linux_web_app.api.app_settings["ASPNETCORE_ENVIRONMENT"] == "Staging" &&
      azurerm_linux_web_app.web.app_settings["ASPNETCORE_ENVIRONMENT"] == "Staging"
    )
    error_message = "Staging must use its own runtime environment without provisioning or injecting development credentials."
  }

  assert {
    condition = (
      azurerm_linux_web_app.api.site_config[0].health_check_path == null &&
      azurerm_linux_web_app.api.site_config[0].health_check_eviction_time_in_min == null
    )
    error_message = "Staging must omit both development-only API health-check settings."
  }
}

run "production_omits_development_credentials" {
  command = plan

  variables {
    environment = "production"
  }

  assert {
    condition = (
      length(random_password.development_jwt_signing_key) == 0 &&
      !contains(keys(azurerm_linux_web_app.api.app_settings), "DevelopmentJwt__SigningKey") &&
      !contains(keys(azurerm_linux_web_app.web.app_settings), "DevelopmentJwt__SigningKey") &&
      !contains(keys(azurerm_linux_web_app.web.app_settings), "DevelopmentBasicAuthEnabled") &&
      !contains(keys(azurerm_linux_web_app.web.app_settings), "DevelopmentBasicAuthPassword") &&
      azurerm_linux_web_app.api.app_settings["ASPNETCORE_ENVIRONMENT"] == "Production" &&
      azurerm_linux_web_app.web.app_settings["ASPNETCORE_ENVIRONMENT"] == "Production"
    )
    error_message = "Production must use its own runtime environment without provisioning or injecting development credentials."
  }

  assert {
    condition = (
      azurerm_linux_web_app.api.site_config[0].health_check_path == null &&
      azurerm_linux_web_app.api.site_config[0].health_check_eviction_time_in_min == null
    )
    error_message = "Production must omit both development-only API health-check settings."
  }
}
