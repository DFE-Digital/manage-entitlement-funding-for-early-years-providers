targetScope = 'subscription'

param resourcePrefix string
param serviceCode string
param location string
param locationShortCode string
param tags object = {}
param storageAccountSku string = 'Standard_ZRS'
param vnetAddressPrefix string
param subnetAddressPrefix string
param workflowPrincipalId string

var servicePrefix = '${resourcePrefix}-${locationShortCode}-${serviceCode}'
var resourceGroupName = '${resourcePrefix}rg-${locationShortCode}-${serviceCode}-terraform'
var storageAccountName = '${resourcePrefix}tfstatestorage'
var workspaceName = '${servicePrefix}-law-tf-state'
var vnetName = '${servicePrefix}-vnet-tf-state'
var subnetName = '${servicePrefix}-snet-tf-state'

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
  tags: tags
}

module network './network.bicep' = {
  name: 'networkDeployment'
  scope: rg
  params: {
    vnetName: vnetName
    vnetAddressPrefix: vnetAddressPrefix
    subnetName: subnetName
    subnetAddressPrefix: subnetAddressPrefix
    location: location
    tags: tags
  }
}

module storage './storage.bicep' = {
  name: 'storageDeployment'
  scope: rg
  params: {
    storageAccountName: storageAccountName
    location: location
    tags: tags
    storageAccountSku: storageAccountSku
    workspaceName: workspaceName
    subnetId: network.outputs.subnetId
    privateDnsZoneId: network.outputs.privateDnsZoneId
    workflowPrincipalId: workflowPrincipalId
  }
}

output resourceGroupName string = rg.name
output storageAccountName string = storageAccountName
