targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment used in resource naming (resource group will be rg-<environmentName>)')
param environmentName string

@description('The location used for deployed resources')
param location string

@description('Optional principal id (user or service principal) to assign roles to the AI Foundry resource')
param principalId string = ''

var tags = {
  'azd-env-name': environmentName
}

resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}

module aifoundry 'aifoundry/aifoundry.module.bicep' = {
  name: 'aifoundry'
  scope: rg
  params: {
    location: location
  }
}

module aifoundry_roles 'aifoundry-roles/aifoundry-roles.module.bicep' = {
  name: 'aifoundry-roles'
  scope: rg
  params: {
    aifoundry_outputs_name: aifoundry.outputs.name
    location: location
    principalId: principalId
    principalType: 'ServicePrincipal'
  }
  // only deploy role assignment when a principalId is provided
  if: principalId != ''
}

output AIFOUNDRY_CONNECTIONSTRING string = aifoundry.outputs.connectionString
output AIFOUNDRY_NAME string = aifoundry.outputs.name
