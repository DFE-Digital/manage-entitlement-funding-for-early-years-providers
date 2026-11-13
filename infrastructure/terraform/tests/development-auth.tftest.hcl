mock_provider "azurerm" {}
mock_provider "random" {}

variables {
  deployment          = "main"
  resource_group_name = "deployment-auth-tests"
}

run "development_creates_key_and_settings" {
  command = plan

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
}
