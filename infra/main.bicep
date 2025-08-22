targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment used in resource naming (resource group will be rg-<environmentName>)')
param environmentName string

@description('The location used for deployed resources')
param location string

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

// Role assignment to grant a principal access to the AI Foundry account is
// intentionally left out of the automatic deployment. If you need to assign a
// role to a principal, run the optional post-deploy command shown in the
// repository docs. This avoids conditional module/resource patterns that can
// be problematic across different Bicep versions and resource types.

output AIFOUNDRY_CONNECTIONSTRING string = aifoundry.outputs.connectionString
output AIFOUNDRY_NAME string = aifoundry.outputs.name
