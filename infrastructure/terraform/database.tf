resource "random_password" "sql_admin" {
  length      = 32
  special     = false
  min_upper   = 1
  min_lower   = 1
  min_numeric = 1
}

resource "azurerm_mssql_server" "application" {
  name                         = "${local.name_prefix}-sql"
  resource_group_name          = azurerm_resource_group.application.name
  location                     = azurerm_resource_group.application.location
  version                      = "12.0"
  administrator_login          = "fundingadmin"
  administrator_login_password = random_password.sql_admin.result
  minimum_tls_version          = "1.2"
  tags                         = local.tags
}

resource "azurerm_mssql_database" "application" {
  name        = "Funding"
  server_id   = azurerm_mssql_server.application.id
  sku_name    = "Basic"
  max_size_gb = 2
  tags        = local.tags
}

# Temporary development access; authentication is still required.
resource "azurerm_mssql_firewall_rule" "azure_services" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.application.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}
