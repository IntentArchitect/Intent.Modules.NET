param functionAppName string = 'azure-function-event-grid-${uniqueString(resourceGroup().id)}'

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'storage${uniqueString(resourceGroup().id)}'
  location: resourceGroup().location
  sku: { name: 'Standard_LRS' }
  kind: 'StorageV2'
}

resource appServicePlan 'Microsoft.Web/serverfarms@2021-02-01' = {
  name: 'asp-${functionAppName}'
  location: resourceGroup().location
  sku: { name: 'Y1', tier: 'Dynamic' }
  kind: 'functionapp'
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
    name: 'app-insights-${functionAppName}'
    kind: 'web'
    location: resourceGroup().location
    properties: {
      Application_Type: 'web'
      DisableIpMasking: false
      Flow_Type: 'Bluefield'
      Request_Source: 'rest'
      publicNetworkAccessForIngestion: 'Enabled'
      publicNetworkAccessForQuery: 'Enabled'
    }
}

resource eventGridTopicClient 'Microsoft.EventGrid/topics@2021-12-01' = {
  name: 'client-created-event'
  location: resourceGroup().location
  properties: {}
}

resource eventGridTopicSpecific 'Microsoft.EventGrid/topics@2021-12-01' = {
  name: 'specific-topic'
  location: resourceGroup().location
  properties: {}
}

var storageAccountKey = storageAccount.listKeys().keys[0].value
var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccountKey};EndpointSuffix=${environment().suffixes.storage}'

resource functionAppAzureFunctionEventGrid 'Microsoft.Web/sites@2021-02-01' = {
  name: functionAppName
  location: resourceGroup().location
  kind: 'functionapp'
  properties: {
    serverFarmId: appServicePlan.id
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
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'EventGrid:Topics:ClientCreatedEvent:Endpoint'
          value: eventGridTopicClient.properties.endpoint
        }
        {
          name: 'EventGrid:Topics:ClientCreatedEvent:Key'
          value: eventGridTopicClient.listKeys().key1
        }
        {
          name: 'EventGrid:Topics:SpecificTopic:Endpoint'
          value: eventGridTopicSpecific.properties.endpoint
        }
        {
          name: 'EventGrid:Topics:SpecificTopic:Key'
          value: eventGridTopicSpecific.listKeys().key1
        }
      ]
    }
  }
}