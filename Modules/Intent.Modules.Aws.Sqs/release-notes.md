# Release Notes

## Version 1.0.0-pre.0

### New Features

- **SqsEventBus**: Implements `IEventBus` for publishing events to AWS SQS queues
  - In-memory queue for batching messages
  - `Publish<T>()` method to queue messages
  - `FlushAllAsync()` to send all queued messages to SQS
  - Automatic `MessageType` attribute setting for routing

- **SqsMessageDispatcher**: Routes incoming SQS messages to typed handlers
  - Dictionary-based lookup by message type
  - Deserializes JSON message bodies
  - Invokes handlers via dependency injection
  - Public `InvokeDispatchHandler<TMessage, THandler>()` method for bridge modules

- **ISqsMessageDispatcher**: Interface for message dispatching
  - `DispatchAsync()` method accepting `SQSEvent.SQSMessage`

- **SqsConfiguration**: Dependency injection and service registration
  - Auto-detects LocalStack via `AWS:ServiceURL` configuration
  - Registers AWS SQS client with appropriate credentials
  - Registers `IEventBus` → `SqsEventBus`
  - Registers `ISqsMessageDispatcher` → `SqsMessageDispatcher`
  - Configures publisher and subscription options

- **SqsPublisherOptions**: Configuration class for publisher mappings
  - Maps message types to queue URLs
  - `AddQueue<TMessage>(string queueUrl)` method

- **SqsSubscriptionOptions**: Configuration class for subscription mappings
  - Maps message types to handler delegates
  - `Add<TMessage, THandler>()` method

### Architecture

- Hosting-agnostic design (works with Lambda, console apps, web apps)
- Follows Azure Service Bus module patterns for consistency
- Core module only - Lambda function generation will be in bridge module

### Dependencies

- AWSSDK.SQS (3.7.400.61)
- AWSSDK.Extensions.NETCore.Setup (3.7.301)
- Amazon.Lambda.SQSEvents (2.2.0)
- Amazon.Lambda.Core (2.2.0)

### Notes

- This is a pre-release version
- Metadata-driven configuration (IntegrationManager) not yet implemented
- AWS SQS stereotype for message models not yet implemented
- Companion bridge module for Lambda function generation is planned
