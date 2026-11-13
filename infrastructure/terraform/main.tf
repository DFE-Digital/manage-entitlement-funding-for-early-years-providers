locals {
  is_development = var.environment == "development"
  name_prefix    = local.is_development ? "eyentitlementfunding-${var.deployment}" : "eyentitlementfunding-${var.environment}-${var.deployment}"
  api_app_name   = "${local.name_prefix}-api"

  aspnetcore_environment = {
    development = "Development"
    staging     = "Staging"
    production  = "Production"
  }[var.environment]

  development_jwt_settings = local.is_development ? {
    DevelopmentJwt__SigningKey = random_password.development_jwt_signing_key[0].result
  } : {}

  tags = {
    Environment = var.environment
    Deployment  = var.deployment
    Source      = "terraform"
    Product     = "Manage Entitlement Funding for Early Years Providers"
  }
}

resource "azurerm_resource_group" "application" {
  name     = var.resource_group_name
  location = var.location
  tags     = local.tags
}

resource "random_password" "development_jwt_signing_key" {
  count   = local.is_development ? 1 : 0
  length  = 48
  special = false
}

moved {
  from = random_password.development_jwt_signing_key
  to   = random_password.development_jwt_signing_key[0]
}

resource "azurerm_service_plan" "application" {
  name                = "${local.name_prefix}-asp"
  resource_group_name = azurerm_resource_group.application.name
  location            = azurerm_resource_group.application.location
  os_type             = "Linux"
  sku_name            = var.app_service_sku
  tags                = local.tags
}

resource "azurerm_linux_web_app" "api" {
  name                                           = local.api_app_name
  resource_group_name                            = azurerm_resource_group.application.name
  location                                       = azurerm_resource_group.application.location
  service_plan_id                                = azurerm_service_plan.application.id
  https_only                                     = true
  ftp_publish_basic_authentication_enabled       = false
  webdeploy_publish_basic_authentication_enabled = false
  tags                                           = local.tags

  site_config {
    always_on                         = false
    health_check_path                 = local.is_development ? "/openapi/v1.json" : null
    minimum_tls_version               = "1.2"
    scm_minimum_tls_version           = "1.2"
    ftps_state                        = "Disabled"
    ip_restriction_default_action     = "Deny"
    scm_use_main_ip_restriction       = false
    scm_ip_restriction_default_action = "Allow"

    dynamic "ip_restriction" {
      for_each = azurerm_linux_web_app.web.outbound_ip_address_list
      content {
        name       = "web-outbound-${ip_restriction.key}"
        ip_address = "${ip_restriction.value}/32"
        action     = "Allow"
        priority   = 100 + ip_restriction.key
      }
    }

    application_stack {
      dotnet_version = "10.0"
    }
  }

  app_settings = merge({
    ASPNETCORE_ENVIRONMENT = local.aspnetcore_environment
  }, local.development_jwt_settings)
}

resource "azurerm_linux_web_app" "web" {
  name                                           = "${local.name_prefix}-web"
  resource_group_name                            = azurerm_resource_group.application.name
  location                                       = azurerm_resource_group.application.location
  service_plan_id                                = azurerm_service_plan.application.id
  https_only                                     = true
  ftp_publish_basic_authentication_enabled       = false
  webdeploy_publish_basic_authentication_enabled = false
  tags                                           = local.tags

  site_config {
    always_on               = false
    minimum_tls_version     = "1.2"
    scm_minimum_tls_version = "1.2"
    ftps_state              = "Disabled"

    application_stack {
      dotnet_version = "10.0"
    }
  }

  app_settings = merge({
    ASPNETCORE_ENVIRONMENT = local.aspnetcore_environment
    # Use the standard public-Azure hostname - we can't get this from the web api,
    # because it needs the webapp to exist first in order to unlock the ip
    ApiBaseUrl = "https://${local.api_app_name}.azurewebsites.net"
    }, local.development_jwt_settings, local.is_development ? {
    DevelopmentBasicAuthEnabled  = "true"
    DevelopmentBasicAuthPassword = var.development_basic_auth_password
  } : {})
}
