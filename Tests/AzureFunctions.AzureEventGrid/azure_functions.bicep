param functionAppName string = 'azure-functions-azure-event-grid-${uniqueString(resourceGroup().id)}'
param appInsightsName string = 'app-insights-${uniqueString(resourceGroup().id)}'
param storageName string = 'storage${uniqueString(resourceGroup().id)}'

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

resource eventGridTopicClientCreatedEvent 'Microsoft.EventGrid/topics@2021-12-01' = {
  name: 'client-created-event'
  location: resourceGroup().location
  properties: {}
}

resource eventGridTopicSpecificTopic 'Microsoft.EventGrid/topics@2021-12-01' = {
  name: 'specific-topic'
  location: resourceGroup().location
  properties: {}
}

var storageAccountKey = storageAccount.listKeys().keys[0].value
var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccountKey};EndpointSuffix=${environment().suffixes.storage}'

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
          name: 'EventGrid:Topics:ClientCreatedEvent:Endpoint'
          value: eventGridTopicClientCreatedEvent.properties.endpoint
        }
        {
          name: 'EventGrid:Topics:ClientCreatedEvent:Key'
          value: eventGridTopicClientCreatedEvent.listKeys().key1
        }
        {
          name: 'EventGrid:Topics:SpecificTopic:Endpoint'
          value: eventGridTopicSpecificTopic.properties.endpoint
        }
        {
          name: 'EventGrid:Topics:SpecificTopic:Key'
          value: eventGridTopicSpecificTopic.listKeys().key1
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
      ]
    }
  }
}

