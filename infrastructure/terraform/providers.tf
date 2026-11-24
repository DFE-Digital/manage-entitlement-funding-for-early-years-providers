terraform {
  required_version = ">= 1.10.5"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "5.4.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.7"
    }
  }

  backend "azurerm" {
    use_oidc         = true
    use_azuread_auth = true
  }
}

provider "azurerm" {
  # azurerm 5.x registers no resource providers by default; register only those we use.
  resource_provider_registrations = "none"
  resource_providers_to_register  = ["Microsoft.Sql", "Microsoft.Web"]

  features {}
}
