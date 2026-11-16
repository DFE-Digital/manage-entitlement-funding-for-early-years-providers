resource "azurerm_resource_group" "application" {
  name     = var.resource_group_name
  location = var.location

  tags = {
    Environment = var.environment
    Deployment  = var.deployment
    Source      = "terraform"
    Product     = "Manage Entitlement Funding for Early Years Providers"
  }
}
