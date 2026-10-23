# Terraform remote state bootstrap

This directory contains a PowerShell script for bootstrapping the Azure resources
required to store Terraform remote state.

These resources must exist before the main Terraform configuration can be
initialised with the Azure Storage backend. They are therefore created outside
Terraform to avoid the chicken-and-egg problem of needing somewhere to store the
Terraform state for the resources that will themselves store Terraform state.

The script creates:

- an Azure resource group;
- an Azure Storage account;
- a private Blob Storage container.

The Blob Storage container can then be configured as the `azurerm` backend for
the service's Terraform configuration.

## Prerequisites

Before running the script you will need:

- PowerShell;
- the Azure CLI (`az`);
- access to the Azure subscription in which the resources will be created; and
- permission to create resource groups and Storage accounts in that subscription.

## 1. Sign in to Azure

Sign in using the Azure CLI:

```powershell
az login
```

Check which subscription is currently selected:

```powershell
az account show
```

If necessary, select the subscription in which the Terraform state resources should be created:

```powershell
az account set --subscription "<subscription name or ID>"
```

Check the selected subscription again before continuing:

```powershell
az account show
```

Take particular care with this step. The script creates resources in the currently selected Azure subscription.

## 2. Define the resource names and tags

In the same PowerShell session, define the variables used by the script.

For example:

```powershell
$tagsToAdd = @(
    "Enviroment=The Environment",
    "Parent Business=The Parent Business",
    "Region=The Region",
    "Product=The Service Name",
    "Service Offering=The Service Name"
)
```

Replace these example values with the appropriate values for the service.

The Storage account name must satisfy Azure's Storage account naming requirements, including being globally unique.

## 3. Run the bootstrap script

Run the script from PowerShell:

```powershell
./create-terraform-state-storage.ps1 
```

You can pass in parameter values, or the script will prompt you for the name of the resource group, Storage account
and Blob container.

It also:

- configures the Storage account to use a minimum TLS version of TLS 1.2;
- disables public Blob access;
- creates the Terraform state container with public access disabled.

## 4. Configure the Terraform backend

Once the script has completed successfully, configure the main Terraform configuration to use the Storage account and container.

For example:

```terraform
terraform {
  backend "azurerm" {
    resource_group_name  = "<resource group name>"
    storage_account_name = "<storage account name>"
    container_name       = "<container name>"
    key                  = "terraform.tfstate"
  }
}
```

The exact key should be chosen to identify the state belonging to the particular service or environment.

For example, if the test subscription contains separate environments for staging/pre-prod and test, they might use different keys:

```text
test.terraform.tfstate
staging.terraform.tfstate
```

## 5. Initialise Terraform

After configuring the backend, initialise the Terraform working directory:

```powershell
terraform init
```

Terraform will then use the Azure Blob Storage container as its remote backend rather than creating a local terraform.tfstate file for the service infrastructure.

## Why isn't this created using Terraform?

Terraform needs a backend in which to store its state.

If the Storage account used as that backend were itself created by the service's Terraform configuration, Terraform would need the Storage account to exist before it could run the configuration that creates the Storage account.

This script deliberately bootstraps this small set of prerequisite resources outside Terraform:

```text
Bootstrap script
      |
      v
Azure resource group
      |
      v
Azure Storage account
      |
      v
Blob container
      |
      v
Terraform remote state
      |
      v
Service infrastructure managed by Terraform
```

Once these resources have been created, normal infrastructure changes should be made through Terraform rather than through this bootstrap script.

## GitHub Actions

The bootstrap script is intended to establish the Storage resources required before the service's normal Terraform deployment pipeline can run.

The GitHub Actions Terraform workflow should authenticate to Azure and configure the azurerm backend to use the Storage account and container created here.

Credentials, Storage account keys and other secrets should not be committed to this repository.

## Important

The Storage account contains the Terraform state used to manage the service's Azure infrastructure. Deleting the Storage account, container or state Blob can therefore prevent Terraform from correctly tracking the resources it manages.

Treat these bootstrap resources as long-lived infrastructure and do not delete them as part of the normal lifecycle of the service's Azure resources.
