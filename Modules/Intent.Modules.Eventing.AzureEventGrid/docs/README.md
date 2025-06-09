# Intent.Eventing.AzureEventGrid

This module provides patterns for working with Azure Event Grid using a flexible pipeline-based architecture that supports middleware behaviors for cross-cutting concerns.

## What is Azure Event Grid?

Azure Event Grid is a cloud-based event routing service that enables event-driven architectures by facilitating the seamless communication of events between different services and applications. It abstracts the complexities of event routing, allowing developers to focus on building scalable and decoupled systems. Azure Event Grid supports various event sources and handlers, making it a versatile choice for integrating disparate systems and services.

For more information on Azure Event Grid, check out their [official docs](https://docs.microsoft.com/en-us/azure/event-grid/).

## Pipeline Architecture

This module implements a behavior-driven pipeline pattern that allows you to add middleware behaviors for both publishing and consuming messages. This enables clean separation of concerns for cross-cutting functionality like logging, retry logic, validation, and custom processing.

### Key Components

- **Publisher Pipeline**: Processes outbound messages before sending to Event Grid
- **Consumer Pipeline**: Processes inbound messages before handler execution
- **Behaviors**: Middleware components that can inspect, modify, or act on messages
- **Cloud Event Context**: Provides access to Event Grid extension attributes and metadata

## Modeling Integration Events

Modeling Integration Events can be achieved from within the Services designer.
This module automatically installs the `Intent.Modelers.Eventing` module which provides designer modeling capabilities for integration events and commands.
For details on modeling integration events and commands, refer to its [README](https://docs.intentarchitect.com/articles/modules-common/intent-modelers-eventing/intent-modelers-eventing.html).

## Specifying Topic Names

Working with Azure Event Grid requires that Messages be assigned to an Event Grid Topic.
Since Topics need to be created in advance, unless you are using Infrastructure as Code to maintain this for you, you will need to specify the Topic Name when creating a new Message.

This module will automatically be assigned a Topic Name when you create a new Message and you can alter it by capturing the `Topic Name` property on the selected Message.

![Event Message](images/event-client-created-event.png)

![Azure Event Grid Topic Name](images/event-topic-name.png)

## Message Publishing

Message publishing can be done through the `IEventBus` interface using the `Publish` method. The interface supports publishing with additional metadata:

```csharp
public interface IEventBus
{
    void Publish<T>(T message) where T : class;
    void Publish<T>(T message, IDictionary<string, object> additionalData) where T : class;
    Task FlushAllAsync(CancellationToken cancellationToken = default);
}

Publishing with Extension Attributes
You can include Event Grid extension attributes and subject information:

// Basic publishing
_eventBus.Publish(new ClientCreatedEvent { Id = clientId });

// Publishing with extension attributes
_eventBus.Publish(new ClientCreatedEvent { Id = clientId }, new Dictionary<string, object>
{
    ["Subject"] = $"clients/{clientId}",
    ["Priority"] = "High",
    ["CorrelationId"] = correlationId
});

// Flush all queued messages
await _eventBus.FlushAllAsync();
```

### Message Consumption

For every message subscribed to in the `Services Designer` will receive its own Integration Event handler.

### Accessing Cloud Event Context

In your handlers, you can access Event Grid extension attributes through the `ICloudEventContext`:

```csharp
public class ClientCreatedIntegrationEventHandler : IIntegrationEventHandler<ClientCreatedEvent>
{
    private readonly ICloudEventContext _eventContext;

    public ClientCreatedIntegrationEventHandler(ICloudEventContext eventContext)
    {
        _eventContext = eventContext;
    }

    public async Task HandleAsync(ClientCreatedEvent message, CancellationToken cancellationToken = default)
    {
        // Access extension attributes
        if (_eventContext.AdditionalData.TryGetValue("Priority", out var priority))
        {
            // Handle high priority messages differently
        }
        
        // Business logic here
        throw new NotImplementedException();
    }
}
```

### Extensibility with Custom Behaviors

You can create custom behaviors to handle cross-cutting concerns:

#### Publisher Behaviors

```csharp
public class LoggingPublisherBehavior : IAzureEventGridPublisherBehavior
{
    private readonly ILogger<LoggingPublisherBehavior> _logger;

    public LoggingPublisherBehavior(ILogger<LoggingPublisherBehavior> logger)
    {
        _logger = logger;
    }

    public async Task<CloudEvent> HandleAsync(CloudEvent cloudEvent, CloudEventBehaviorDelegate next, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing event {EventType} with ID {EventId}", cloudEvent.Type, cloudEvent.Id);
        
        var result = await next(cloudEvent, cancellationToken);
        
        _logger.LogInformation("Successfully published event {EventId}", cloudEvent.Id);
        return result;
    }
}
```

#### Consumer Behaviors

```csharp
public class ValidationConsumerBehavior : IAzureEventGridConsumerBehavior
{
    public async Task<CloudEvent> HandleAsync(CloudEvent cloudEvent, CloudEventBehaviorDelegate next, CancellationToken cancellationToken = default)
    {
        // Validate cloud event structure
        if (string.IsNullOrEmpty(cloudEvent.Source))
        {
            throw new InvalidOperationException("Cloud event source is required");
        }

        return await next(cloudEvent, cancellationToken);
    }
}
```

#### Registering Custom Behaviors

```csharp
public static IServiceCollection ConfigureEventGrid(this IServiceCollection services, IConfiguration configuration)
{
    // ... existing configuration ...
    
    // Add custom behaviors
    services.AddScoped<IAzureEventGridPublisherBehavior, LoggingPublisherBehavior>();
    services.AddScoped<IAzureEventGridConsumerBehavior, ValidationConsumerBehavior>();
    
    return services;
}
```

## Configuring Event Grid

When you're publishing an Event Grid Message, you will need to configure it in your `appsettings.json` file.

```json
{
  "EventGrid": {
    "Topics": {
      "ClientCreatedEvent": {
        "Source": "client-created-event",
        "Key": "4L6y6Nk8LFHXm0KnbK7gYpLtD0OL6Ear9VnY5ihQio8DhtljnGAdJQQJ99BDACrIdLPXJ3w3AAABAZEGvWZM",
        "Endpoint": "https://client-created-event.your-region.eventgrid.azure.net/api/events"
      }
    }
  }
}
```

> [!NOTE]
>
> This module will not generate the consumer code for you. Look at the Related Modules section to see which modules can provide you with that capability.

## Related Modules

### Intent.AzureFunctions.AzureEventGrid

This module handles the consumer code for Azure Event Grid when Azure Functions is selected as the hosting technology. It includes support for the new pipeline architecture and behavior system.

> [!NOTE]
>
> Not seeing the hosting technology you're looking for? Please reach out to us on [GitHub](https://github.com/IntentArchitect/Support) or email us at [support@intentarchitect.com](mailto://support@intentarchitect.com), and we'll be happy to help. 
