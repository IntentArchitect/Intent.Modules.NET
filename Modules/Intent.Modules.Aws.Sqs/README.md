# Intent.Aws.Sqs

> **📚 For Implementation:** See `GETTING_STARTED.md` for workspace navigation and `IMPLEMENTATION_PLAN.md` for complete specifications.

This module provides AWS SQS eventing infrastructure for publishing and consuming messages in Intent Architect applications.

## Features

- **IEventBus Implementation**: `SqsEventBus` for publishing events to AWS SQS queues
- **Message Dispatcher**: `SqsMessageDispatcher` for routing SQS messages to typed handlers
- **Configuration Support**: Auto-configuration with LocalStack detection for local development
- **Hosting-Agnostic**: Works with Lambda, console apps, web apps, and more

## Components

### Templates

1. **SqsEventBus** - Implements IEventBus to publish events to SQS queues
2. **SqsMessageDispatcher** - Routes incoming SQS messages to typed handlers
3. **ISqsMessageDispatcher** - Interface for message dispatching
4. **SqsConfiguration** - Dependency injection and service registration
5. **SqsPublisherOptions** - Configuration for mapping message types to queue URLs
6. **SqsSubscriptionOptions** - Configuration for mapping message types to handlers

### NuGet Packages

The module automatically manages the following NuGet dependencies:

- `AWSSDK.SQS` - AWS SQS SDK
- `AWSSDK.Extensions.NETCore.Setup` - AWS SDK DI extensions
- `Amazon.Lambda.SQSEvents` - SQS event types for Lambda
- `Amazon.Lambda.Core` - Lambda core types

## Usage

### Configuration

The module generates a `SqsConfiguration.ConfigureSqs()` extension method that:

1. Registers the AWS SQS client with LocalStack support
2. Registers `IEventBus` → `SqsEventBus`
3. Registers `ISqsMessageDispatcher` → `SqsMessageDispatcher`
4. Configures publisher and subscription options (metadata-driven)

### LocalStack Support

For local development, set the following in your `appsettings.Development.json`:

```json
{
  "AWS": {
    "ServiceURL": "http://localhost:4566",
    "Region": "us-east-1"
  }
}
```

The module automatically detects the `ServiceURL` setting and configures the SQS client accordingly.

### Publishing Events

```csharp
public class MyService
{
    private readonly IEventBus _eventBus;
    
    public MyService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }
    
    public async Task DoSomething()
    {
        _eventBus.Publish(new MyEvent { Data = "value" });
        await _eventBus.FlushAllAsync();
    }
}
```

### Message Attributes

The module automatically sets the `MessageType` attribute on published messages for routing purposes.

## Architecture

This is a **core module** that provides infrastructure only. Lambda function generation is handled by the companion `Intent.AwsLambda.Sqs` module (future).

This follows the same pattern as the Azure Service Bus modules:
- **Core**: Intent.Modules.Eventing.AzureServiceBus
- **Bridge**: Intent.Modules.AzureFunctions.AzureServiceBus

## Dependencies

- Intent.Common (3.9.0+)
- Intent.Common.CSharp (3.8.3+)
- Intent.Eventing.Contracts (5.2.0+)
- Intent.Modelers.Eventing (6.0.2+)
- Intent.Modelers.Services (3.9.3+)
- Intent.Modelers.Services.EventInteractions (1.2.1+)

## Metadata Support

### AWS SQS Stereotype

The module provides an `AWS SQS` stereotype that can be applied to message models in the Eventing designer:

- **Queue Name**: The name of the SQS queue (optional - defaults to kebab-case message name)
- **Queue URL**: The full queue URL (optional - can be configured via appsettings)

### Metadata-Driven Configuration

The `SqsConfiguration` template uses the `IntegrationManager` to automatically:
- Detect published and subscribed messages in your application
- Generate appropriate publisher options (message type → queue URL mappings)
- Generate subscription options (message type → handler mappings)
- Register all event handlers in the DI container

No manual configuration is needed - the module generates everything based on your Intent Architect models.

## Future Enhancements

- Bridge module for Lambda function generation (`Intent.AwsLambda.Sqs`)

## Related Modules

- `Intent.Eventing.Contracts` - Base eventing interfaces
- `Intent.AwsLambda.Sqs` (future) - Lambda consumer generation
