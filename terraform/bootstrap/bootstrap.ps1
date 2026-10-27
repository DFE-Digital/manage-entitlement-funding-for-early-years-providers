<#
.SYNOPSIS
Creates the Azure resources required to store Terraform remote state.

.DESCRIPTION
Creates an Azure resource group, storage account and blob container
for use as an Azure backend for Terraform state. It sets tags on
these resources too, edit this file to set those tags to the appropriate
but do not commit those values to the repository.

.PARAMETER ResourceGroup
The name of the Azure resource group to create.

.PARAMETER StorageAccount
The name of the Azure Storage account to create.
The name must meet Azure Storage account naming requirements and be globally unique.

.PARAMETER Container
The name of the blob container in which Terraform state will be stored.

.PARAMETER Location
The Azure region in which the resources will be created, for example "uksouth".

.EXAMPLE
.\create-terraform-state-storage.ps1 `
    -ResourceGroup "rg-myservice-tfstate" `
    -StorageAccount "stmyservicetfstate" `
    -Container "tfstate" `
    -Location "uksouth"
#>

param (

    [Parameter(Mandatory)]
    [string]$ResourceGroup,

    [Parameter(Mandatory)]
    [string]$StorageAccount,

    [Parameter(Mandatory)]
    [string]$Container,

    [Parameter(Mandatory)]
    [string]$Location
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true

# Ideally, do not set these to the real values in the repository.
$tagsToAdd = @(
    "Environment=The Environment",
    "Parent Business=The Parent Business",
    "Region=The Region",
    "Product=The Service Name",
    "Service Offering=The Service Name"
)

if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    throw "Azure CLI ('az') is required but was not found."
}

az group create `
    --name $ResourceGroup `
    --location $Location `
    --tags $tagsToAdd

az storage account create `
    --name $StorageAccount `
    --resource-group $ResourceGroup `
    --location $Location `
    --sku Standard_LRS `
    --min-tls-version TLS1_2 `
    --allow-blob-public-access false `
    --tags $tagsToAdd

az storage container create `
    --name $Container `
    --account-name $StorageAccount `
    --public-access off