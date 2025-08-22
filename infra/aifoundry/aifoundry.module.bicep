@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource aifoundry 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: take('aifoundry-${uniqueString(resourceGroup().id)}', 64)
  location: location
  kind: 'OpenAI'
  properties: {
    customSubDomainName: toLower(take(concat('aifoundry', uniqueString(resourceGroup().id)), 24))
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: true
  }
  sku: {
    name: 'S0'
  }
  tags: {
    'aspire-resource-name': 'aifoundry'
  }
}

resource chat 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  name: 'chat'
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4.1-mini'
      version: '2025-04-14'
    }
  }
  sku: {
    name: 'GlobalStandard'
    capacity: 8
  }
  parent: aifoundry
}

resource embeddings 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  name: 'embeddings'
  properties: {
    model: {
      format: 'OpenAI'
      name: 'text-embedding-ada-002'
      version: '2'
    }
  }
  sku: {
    name: 'Standard'
    capacity: 8
  }
  parent: aifoundry
  dependsOn: [
    chat
  ]
}

output connectionString string = 'Endpoint=${aifoundry.properties.endpoint}'

output name string = aifoundry.name
