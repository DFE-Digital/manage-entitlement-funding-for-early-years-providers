locals {
  # Common tags to be assigned to resources
  common_tags = {
    "Environment"      = var.environment
    "Parent Business"  = "Children's Care"
    "Product"          = "Manage Entitlement Funding for Early Years Providers"
    "Service Offering" = "Manage Entitlement Funding for Early Years Providers"
  }
}
