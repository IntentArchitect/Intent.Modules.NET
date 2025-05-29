param functionAppName string = 'azure-functions-azure-service-bus-group-b-${uniqueString(resourceGroup().id)}'
param appInsightsName string = 'app-insights-${uniqueString(resourceGroup().id)}'
param storageName string = 'storage${uniqueString(resourceGroup().id)}'
param serviceBusNamespaceName string = 'service-bus-${uniqueString(resourceGroup().id)}'

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: resourceGroup().location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

resource functionAppHostingPlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: 'asp-${functionAppName}'
  location: resourceGroup().location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageName
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2021-06-01-preview' = {
  name: serviceBusNamespaceName
  location: resourceGroup().location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}

resource specificQueueQueue 'Microsoft.ServiceBus/namespaces/queues@2021-06-01-preview' = {
  parent: serviceBusNamespace
  name: 'specific-queue'
}

resource clientCreatedSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-06-01-preview' = {
  parent: clientCreatedTopic
  name: 'client-created-subscription'
}

resource clientCreatedTopic 'Microsoft.ServiceBus/namespaces/topics@2021-06-01-preview' existing = {
  parent: serviceBusNamespace
  name: 'client-created'
}

resource specificTopicTopic 'Microsoft.ServiceBus/namespaces/topics@2021-06-01-preview' = {
  parent: serviceBusNamespace
  name: 'specific-topic'
}

var storageAccountKey = storageAccount.listKeys().keys[0].value
var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccountKey};EndpointSuffix=${environment().suffixes.storage}'

var serviceBusKey = listKeys(
  '${serviceBusNamespace.id}/AuthorizationRules/RootManageSharedAccessKey',
  serviceBusNamespace.apiVersion
).primaryKey
var serviceBusConnectionString = 'Endpoint=sb://${serviceBusNamespace.name}.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=${serviceBusKey}'

resource functionApp 'Microsoft.Web/sites@2021-02-01' = {
  name: functionAppName
  location: resourceGroup().location
  kind: 'functionapp'
  properties: {
    serverFarmId: functionAppHostingPlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: storageConnectionString
        }
        {
          name: 'AzureServiceBus:ClientCreated'
          value: clientCreatedTopic.name
        }
        {
          name: 'AzureServiceBus:ClientCreatedSubscription'
          value: clientCreatedSubscription.name
        }
        {
          name: 'AzureServiceBus:ConnectionString'
          value: serviceBusConnectionString
        }
        {
          name: 'AzureServiceBus:SpecificQueue'
          value: specificQueueQueue.name
        }
        {
          name: 'AzureServiceBus:SpecificTopic'
          value: specificTopicTopic.name
        }
        {
          name: 'CqrsSettings:LogRequestPayload'
          value: 'True'
        }
        {
          name: 'CqrsSettings:LogRequestPayload'
          value: 'True'
        }
        {
          name: 'CqrsSettings:LogRequestPayload'
          value: 'True'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
      ]
      cors: {
        allowedOrigins: [
          'https://portal.azure.com'
        ]
      }
      netFrameworkVersion: 'v8.0'
    }
  }
}

