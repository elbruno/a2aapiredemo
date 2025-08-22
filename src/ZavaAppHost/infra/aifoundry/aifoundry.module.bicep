@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource aifoundry 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: take('aifoundry-${uniqueString(resourceGroup().id)}', 64)
  location: location
  kind: 'OpenAI'
  properties: {
    customSubDomainName: toLower(take(concat('aifoundry', uniqueString(resourceGroup().id)), 24))
    publicNetworkAccess: 'Enabled'
    // Allow key-based (local) authentication for demo and backwards compatibility.
    // The Products project currently uses key-based calls to the embeddings endpoint
    // (received HTTP 403 AuthenticationTypeDisabled). Set to false to enable
    // key issuance. In production consider enforcing AAD-only auth and updating
    // services to use managed identities.
    disableLocalAuth: false
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
