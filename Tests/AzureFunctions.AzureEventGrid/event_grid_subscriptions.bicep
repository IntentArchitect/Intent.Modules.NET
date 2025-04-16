param functionAppName string = 'azure-function-event-grid-${uniqueString(resourceGroup().id)}'

resource functionAppEventGrid 'Microsoft.Web/sites@2021-02-01' existing = {
  name: functionAppName
}

resource eventGridTopicClientCreatedEvent 'Microsoft.EventGrid/topics@2021-12-01' existing = {
  name: 'client-created-event'
}

resource eventGridTopicSpecificTopic 'Microsoft.EventGrid/topics@2021-12-01' existing = {
  name: 'specific-topic'
}

resource eventGridSubscriptionClientCreatedEvent 'Microsoft.EventGrid/eventSubscriptions@2021-12-01' = {
  name: '${functionAppName}-client-created-event-sub'
  scope: eventGridTopicClientCreatedEvent
  properties: {
    destination: {
      endpointType: 'AzureFunction'
      properties: {
        resourceId: '${functionAppEventGrid.id}/functions/ClientCreatedEventConsumer'
      }
    }
    filter: {
      includedEventTypes: [
        'AzureFunctions.AzureEventGrid.Eventing.Messages.ClientCreatedEvent'
      ]
    }
  }
}

resource eventGridSubscriptionSpecificTopic 'Microsoft.EventGrid/eventSubscriptions@2021-12-01' = {
  name: '${functionAppName}-specific-topic-sub'
  scope: eventGridTopicSpecificTopic
  properties: {
    destination: {
      endpointType: 'AzureFunction'
      properties: {
        resourceId: '${functionAppEventGrid.id}/functions/SpecificTopicMessageConsumer'
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

