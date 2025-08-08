@description('Application name prefix for all resources')
param appName string

@description('Resource group location')
param location string = resourceGroup().location

@description('Environment name (dev, staging, prod)')
@allowed(['dev', 'staging', 'prod'])
param environment string = 'dev'

@description('App Service Plan name')
param appServicePlanName string = '${appName}-${environment}-plan'

@description('Log Analytics workspace name')
param logAnalyticsWorkspaceName string = '${appName}-${environment}-logs'

@description('Application Insights name')
param appInsightsName string = '${appName}-${environment}-insights'

@description('Key Vault name')
param keyVaultName string = '${appName}-${environment}-kv'

@description('NLWeb API endpoint (if using external service)')
param nlWebApiEndpoint string = ''

@description('NLWeb API key (will be stored in Key Vault)')
@secure()
param nlWebApiKey string = ''

@description('SQL Server name')
param sqlServerName string = '${appName}-${environment}-sql'

@description('SQL Database name')
param sqlDatabaseName string = '${appName}-${environment}-db'

@description('SQL Administrator login')
param sqlAdminLogin string = 'sqladmin'

@description('SQL Administrator password')
@secure()
param sqlAdminPassword string

// Variables
var tags = {
  application: appName
  environment: environment
  'created-by': 'bicep'
}

var appServicePlanSku = environment == 'prod' ? 'P1v3' : (environment == 'staging' ? 'S1' : 'B1')
var sqlDatabaseSku = environment == 'prod' ? 'S1' : 'Basic'

// Log Analytics Workspace
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: environment == 'prod' ? 90 : 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
}

// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
    IngestionMode: 'LogAnalytics'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

// Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    enablePurgeProtection: false
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

// Store NLWeb API key in Key Vault if provided
resource nlWebApiKeySecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = if (!empty(nlWebApiKey)) {
  name: 'nlweb-api-key'
  parent: keyVault
  properties: {
    value: nlWebApiKey
  }
}

// Store SQL connection string in Key Vault
resource sqlConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: 'sql-connection-string'
  parent: keyVault
  properties: {
    value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabaseName};Persist Security Info=False;User ID=${sqlAdminLogin};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
  }
}

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  tags: tags
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

// SQL Database
resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  name: sqlDatabaseName
  parent: sqlServer
  location: location
  tags: tags
  sku: {
    name: sqlDatabaseSku
    tier: sqlDatabaseSku == 'Basic' ? 'Basic' : 'Standard'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: environment == 'prod' ? 268435456000 : 2147483648 // 250GB for prod, 2GB for others
  }
}

// SQL Server Firewall Rule for Azure Services
resource sqlFirewallRule 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  name: 'AllowAzureServices'
  parent: sqlServer
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: {
    name: appServicePlanSku
    capacity: environment == 'prod' ? 2 : 1
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

// Products API App Service
resource productsApi 'Microsoft.Web/sites@2023-12-01' = {
  name: '${appName}-${environment}-products'
  location: location
  tags: tags
  kind: 'app,linux,container'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOCKER|mcr.microsoft.com/dotnet/aspnet:9.0'
      alwaysOn: environment != 'dev'
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment == 'prod' ? 'Production' : 'Development'
        }
        {
          name: 'ConnectionStrings__DefaultConnection'
          value: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=sql-connection-string)'
        }
      ]
    }
    httpsOnly: true
    clientAffinityEnabled: false
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Search API App Service
resource searchApi 'Microsoft.Web/sites@2023-12-01' = {
  name: '${appName}-${environment}-search'
  location: location
  tags: tags
  kind: 'app,linux,container'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOCKER|mcr.microsoft.com/dotnet/aspnet:9.0'
      alwaysOn: environment != 'dev'
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'NLWEB_API_ENDPOINT'
          value: !empty(nlWebApiEndpoint) ? nlWebApiEndpoint : 'https://api.nlweb.dev'
        }
        {
          name: 'NLWEB_API_KEY'
          value: !empty(nlWebApiKey) ? '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=nlweb-api-key)' : 'mock-key'
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment == 'prod' ? 'Production' : 'Development'
        }
      ]
    }
    httpsOnly: true
    clientAffinityEnabled: false
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Chat API App Service
resource chatApi 'Microsoft.Web/sites@2023-12-01' = {
  name: '${appName}-${environment}-chat'
  location: location
  tags: tags
  kind: 'app,linux,container'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOCKER|mcr.microsoft.com/dotnet/aspnet:9.0'
      alwaysOn: environment != 'dev'
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'NLWEB_API_ENDPOINT'
          value: !empty(nlWebApiEndpoint) ? nlWebApiEndpoint : 'https://api.nlweb.dev'
        }
        {
          name: 'NLWEB_API_KEY'
          value: !empty(nlWebApiKey) ? '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=nlweb-api-key)' : 'mock-key'
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment == 'prod' ? 'Production' : 'Development'
        }
      ]
    }
    httpsOnly: true
    clientAffinityEnabled: false
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Store Web App Service
resource storeApp 'Microsoft.Web/sites@2023-12-01' = {
  name: '${appName}-${environment}-store'
  location: location
  tags: tags
  kind: 'app,linux,container'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOCKER|mcr.microsoft.com/dotnet/aspnet:9.0'
      alwaysOn: environment != 'dev'
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'PRODUCTS_API_URL'
          value: 'https://${productsApi.properties.defaultHostName}'
        }
        {
          name: 'SEARCH_API_URL'
          value: 'https://${searchApi.properties.defaultHostName}'
        }
        {
          name: 'CHAT_API_URL'
          value: 'https://${chatApi.properties.defaultHostName}'
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment == 'prod' ? 'Production' : 'Development'
        }
      ]
    }
    httpsOnly: true
    clientAffinityEnabled: false
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Grant Key Vault access to App Services
resource productsApiKeyVaultAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, productsApi.id, 'Key Vault Secrets User')
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // Key Vault Secrets User
    principalId: productsApi.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource searchApiKeyVaultAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, searchApi.id, 'Key Vault Secrets User')
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // Key Vault Secrets User
    principalId: searchApi.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource chatApiKeyVaultAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, chatApi.id, 'Key Vault Secrets User')
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // Key Vault Secrets User
    principalId: chatApi.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// Outputs
output resourceGroupName string = resourceGroup().name
output appServicePlanName string = appServicePlan.name
output logAnalyticsWorkspaceName string = logAnalyticsWorkspace.name
output appInsightsName string = appInsights.name
output keyVaultName string = keyVault.name
output sqlServerName string = sqlServer.name
output sqlDatabaseName string = sqlDatabase.name

output storeAppUrl string = 'https://${storeApp.properties.defaultHostName}'
output productsApiUrl string = 'https://${productsApi.properties.defaultHostName}'
output searchApiUrl string = 'https://${searchApi.properties.defaultHostName}'
output chatApiUrl string = 'https://${chatApi.properties.defaultHostName}'

output appInsightsConnectionString string = appInsights.properties.ConnectionString
output appInsightsInstrumentationKey string = appInsights.properties.InstrumentationKey