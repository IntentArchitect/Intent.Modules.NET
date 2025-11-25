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
