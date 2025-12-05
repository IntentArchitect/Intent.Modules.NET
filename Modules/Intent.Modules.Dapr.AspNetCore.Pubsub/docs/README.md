---
uid: modules-dotnet.intent-dapr-aspnetcore-pubsub
---
# Intent.Dapr.AspNetCore.Pubsub

This module provides publish/subscribe capabilities for Dapr.

## What is Dapr Publish/Subscribe?

Publish/subscribe (often shortened to "pub/sub") is a messaging pattern where senders of messages, called publishers, do not program the messages to be sent directly to specific receivers, called subscribers. Instead, published messages are categorized into topics, without knowledge of what subscribers there might be. Similarly, subscribers express interest in one or more topics and only receive messages that are of interest, without knowledge of what publishers there are.

Dapr provides a pluggable API for pub/sub that allows your application to send and receive messages without being coupled to a specific message broker.

For more information, see the [official Dapr documentation on Publish & Subscribe](https://docs.dapr.io/developing-applications/building-blocks/pubsub/pubsub-overview/).

## What this module generates

This module generates:
- A `DaprEventHandlerController` to receive and handle events from Dapr pub/sub.
- HTTP endpoints that Dapr can call to deliver messages to your application.
- Integration code to wire up your Intent-modeled message handlers with Dapr's pub/sub system.

### Example: Generated Controller

```csharp
[ApiController]
[Route("api/dapr/events")]
public class DaprEventHandlerController : ControllerBase
{
    private readonly IMediator _mediator;

    public DaprEventHandlerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("order-created")]
    [Topic("pubsub", "order-created")]
    public async Task<IActionResult> HandleOrderCreated([FromBody] OrderCreatedEvent @event)
    {
        await _mediator.Send(@event);
        return Ok();
    }
}
```

### Example: Dapr Component Configuration

```yaml
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: pubsub
spec:
  type: pubsub.redis
  version: v1
  metadata:
  - name: redisHost
    value: localhost:6379
```

## How to model messages

To define messages that can be published and subscribed to, use Intent Architect's **Eventing Designer**. This allows you to:
- Model message contracts (events/commands)
- Define publishers and subscribers
- Automatically generate strongly-typed handlers

For detailed guidance on modeling eventing patterns, see the [Message-Based Integration Modeling documentation](https://docs.intentarchitect.com/articles/application-development/modelling/services-designer/message-based-integration-modeling/message-based-integration-modeling.html).

## Working with Multiple Message Bus Providers

This module can coexist with other message bus implementations in the same application. When multiple providers are installed, Intent Architect automatically generates a **Composite Message Bus** that intelligently routes messages based on configuration.

### Designating Messages for Dapr

When you have only this provider installed, all messages automatically use it—no configuration needed.

When you have **multiple providers** installed, you must designate which messages should be handled by Dapr using the **`Message Bus`** stereotype:

1. **Right-click** on a **Package** or **Folder** in the Services designer
2. Select **Add Stereotype** → **Message Bus**
3. In the stereotype properties, select `Dapr Pub Sub` from the **Providers** list

The stereotype can be applied at multiple levels:
- **Package level**: All messages in the package use the selected provider(s)
- **Folder level**: All messages in the folder inherit the designation
- **Message level**: Individual message-level control (rarely needed)

**Stereotype Inheritance**: Child elements inherit their parent's `Message Bus` stereotype automatically, so you typically only need to set it at the package or folder level. Intent handles all the routing transparently.

### Generated Code Filtering

When multiple providers are installed:
- Dapr **only generates** handlers, consumers, and configuration for messages marked with its provider designation
- Messages designated for other providers are ignored by this module
- Messages can be marked for multiple providers and will be handled by each

### Additional Resources

For comprehensive details on the Composite Message Bus architecture and design, see the [Intent.Eventing.Contracts documentation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-eventing-contracts/intent-eventing-contracts.html).

## When to use this module

Use this module when you want to leverage Dapr's pub/sub capabilities in your ASP.NET Core application. Dapr acts as the message broker abstraction, allowing you to switch between different backing implementations (Redis, Azure Service Bus, Kafka, etc.) without changing your application code.

This is particularly useful in:
- **Microservices architectures** where services need to communicate asynchronously
- **Event-driven systems** where services react to domain events
- **Cloud-native applications** where you want portability across different messaging infrastructures

## Related Modules

- **Intent.Eventing.Contracts**: For modeling message contracts in the Eventing Designer.
- **Intent.Dapr.AspNetCore.StateManagement**: For managing state in your Dapr application.
- **Intent.Dapr.AspNetCore.ServiceInvocation**: For direct service-to-service communication.

## External Resources

- [Dapr Publish & Subscribe building block](https://docs.dapr.io/developing-applications/building-blocks/pubsub/pubsub-overview/)
- [Dapr Pub/Sub API reference](https://docs.dapr.io/reference/api/pubsub_api/)
- [Intent Architect Message-Based Integration Modeling](https://docs.intentarchitect.com/articles/application-development/modelling/services-designer/message-based-integration-modeling/message-based-integration-modeling.html)
