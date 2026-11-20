mock_provider "azurerm" {}
mock_provider "random" {}

variables {
  environment                     = "development"
  deployment                      = "main"
  resource_group_name             = "deployment-database-tests"
  development_basic_auth_password = "test-only-basic-auth-password-with-at-least-32-characters"
}

run "database_is_wired_only_to_api" {
  command = plan

  override_resource {
    target          = azurerm_mssql_server.application
    override_during = plan
    values = {
      id                          = "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/deployment-database-tests/providers/Microsoft.Sql/servers/test"
      fully_qualified_domain_name = "test.database.windows.net"
    }
  }

  override_resource {
    target          = random_password.sql_admin
    override_during = plan
    values = {
      result = "test-only-generated-sql-password"
    }
  }

  assert {
    condition = (
      azurerm_mssql_database.application.sku_name == "Basic" &&
      azurerm_mssql_database.application.max_size_gb == 2 &&
      azurerm_mssql_server.application.minimum_tls_version == "1.2" &&
      azurerm_mssql_firewall_rule.azure_services.start_ip_address == "0.0.0.0" &&
      azurerm_mssql_firewall_rule.azure_services.end_ip_address == "0.0.0.0"
    )
    error_message = "Provision a small SQL database with TLS and the agreed Azure-services firewall rule."
  }

  assert {
    condition = (
      azurerm_linux_web_app.api.app_settings["ConnectionStrings__Funding"] ==
      "Server=tcp:test.database.windows.net,1433;Database=Funding;User ID=fundingadmin;Password=test-only-generated-sql-password;Encrypt=True;TrustServerCertificate=False;Connection Timeout=15;" &&
      !contains(keys(azurerm_linux_web_app.web.app_settings), "ConnectionStrings__Funding")
    )
    error_message = "Only the API must receive the encrypted SQL connection configuration."
  }
}
