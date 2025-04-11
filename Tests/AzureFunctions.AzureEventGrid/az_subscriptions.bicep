param functionAppName string = 'azure-function-event-grid-${uniqueString(resourceGroup().id)}'

resource functionAppAzureFunctionEventGrid 'Microsoft.Web/sites@2021-02-01' existing = {
    name: functionAppName
}

resource eventGridTopicClient 'Microsoft.EventGrid/topics@2021-12-01' existing = {
  name: 'client-created-event'
}

resource eventGridTopicSpecific 'Microsoft.EventGrid/topics@2021-12-01' existing = {
  name: 'specific-topic'
}

resource eventGridSub 'Microsoft.EventGrid/eventSubscriptions@2021-12-01' = {
  name: '${functionAppName}-client-created-event-sub'
  scope: eventGridTopicClient
  properties: {
      destination: {
        endpointType: 'AzureFunction'
        properties: {
            resourceId: '${functionAppAzureFunctionEventGrid.id}/functions/ClientCreatedEventConsumer'
        }
      }
      filter: {
        includedEventTypes: [
          'AzureFunctions.AzureEventGrid.Eventing.Messages.ClientCreatedEvent'
        ]
      }
    }
}

resource eventGridTopicSpecificSub 'Microsoft.EventGrid/eventSubscriptions@2021-12-01' = {
  name: '${functionAppName}-specific-topic-sub'
  scope: eventGridTopicSpecific
  properties: {
      destination: {
        endpointType: 'AzureFunction'
        properties: {
            resourceId: '${functionAppAzureFunctionEventGrid.id}/functions/SpecificTopicMessageConsumer'
        }
      }
      filter: {
        includedEventTypes: [
          'AzureFunctions.AzureEventGrid.Eventing.Messages.SpecificTopicOneMessageEvent'
          'AzureFunctions.AzureEventGrid.Eventing.Messages.SpecificTopicTwoMessageEvent'
        ]
      }
    }
}
