# subscription.handler

A handler's subscription to a message, `Subscribe Integration Event` from an Integration Event Handler to a Message or `Domain Event Handler Association` from a Domain Event Handler to a Domain Event, against the handler class: it implements the framework's handler interface for that message, `IHandleMessages<M>` over NServiceBus, eShop's own `IIntegrationEventHandler<M>`, or MediatR's `INotificationHandler<E>`. The claim is the association's. `subscription.handler` is an existence kind: a modelled subscription the handler doesn't implement is contradicted (R8).

The fixtures adapt eShopCustom's `Ordering.API` integration and domain event handlers.

## verified

`OrderPaymentSucceededHandler` subscribes to `OrderPaymentSucceededIntegrationEvent` and implements `IHandleMessages<OrderPaymentSucceededIntegrationEvent>`. The reading is at `Handle`, the method that takes the message, so it is the subscription's code and `optin/unmodelled.member` never reports it.

```json case
{ "phase": 4, "kinds": ["subscription.handler"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [
        {
          "id": "services", "name": "Ordering.API",
          "elements": [
            { "id": "payment-succeeded-handler", "type": "Integration Event Handler", "name": "OrderPaymentSucceededHandler" }
          ]
        },
        {
          "id": "eventing", "name": "Ordering.Eventing", "type": "Eventing Package",
          "elements": [
            { "id": "payment-succeeded", "type": "Message", "name": "OrderPaymentSucceededIntegrationEvent" }
          ]
        }
      ],
      "associations": [
        { "id": "handler-subscribes-payment-succeeded", "type": "Subscribe Integration Event",
          "source": { "element": "payment-succeeded-handler" }, "target": { "element": "payment-succeeded" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.API/Application/IntegrationEvents/EventHandling/OrderPaymentSucceededHandler.cs
namespace Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderPaymentSucceededHandler : IHandleMessages<OrderPaymentSucceededIntegrationEvent>
{
    public async Task Handle(OrderPaymentSucceededIntegrationEvent message, IMessageHandlerContext context)
    {
        await context.Publish(new OrderStatusChangedToPaidIntegrationEvent(message.OrderId));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "subscription.handler", "element": "handler-subscribes-payment-succeeded", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/IntegrationEvents/EventHandling/OrderPaymentSucceededHandler.cs:5" }
  ]
}
```

## contradicted

The handler handles `OrderPaymentFailedIntegrationEvent` instead: the modelled subscription isn't in the code.

```json case
{ "phase": 4, "base": "verified", "kinds": ["subscription.handler"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/IntegrationEvents/EventHandling/OrderPaymentSucceededHandler.cs
namespace Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderPaymentSucceededHandler : IHandleMessages<OrderPaymentFailedIntegrationEvent>
{
    public async Task Handle(OrderPaymentFailedIntegrationEvent message, IMessageHandlerContext context)
    {
        await context.Publish(new OrderStatusChangedToCancelledIntegrationEvent(message.OrderId));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "subscription.handler", "element": "handler-subscribes-payment-succeeded", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/subscription.handler", "severity": "error", "subject": "handler-subscribes-payment-succeeded" }
  ]
}
```

## no-rule

Only the core and Services packs are active, so nothing reads which messages a handler handles. The claim is can't verify, `noRule`.

```json case
{ "phase": 1, "base": "verified", "kinds": ["subscription.handler"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "subscription.handler", "element": "handler-subscribes-payment-succeeded", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/subscription.handler", "severity": "error", "subject": "handler-subscribes-payment-succeeded", "messageContains": "no rule" }
  ]
}
```

## eshop-integration-event-handler

eShopCustom's handlers implement its own event bus interface, `IIntegrationEventHandler<M>`, which no stock pack knows, so the house pack brings the recipe: the handler class (`{Service}`, the association's source) implements it for the subscribed message (`{Message}`, its target); a handler that doesn't reads `absent`. Traces ESC-SVC-61.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Ordering.API/Application/IntegrationEvents/EventHandling/OrderPaymentSucceededHandler.cs"], "kinds": ["subscription.handler"], "traces": ["ESC-SVC-61"], "requirements": ["R2", "R46", "R51"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [
        {
          "id": "services", "name": "Ordering.API",
          "elements": [
            { "id": "payment-succeeded-handler", "type": "Integration Event Handler", "name": "OrderPaymentSucceededIntegrationEventHandler" }
          ]
        },
        {
          "id": "eventing", "name": "Ordering.Eventing", "type": "Eventing Package",
          "elements": [
            { "id": "payment-succeeded", "type": "Message", "name": "OrderPaymentSucceededIntegrationEvent" }
          ]
        }
      ],
      "associations": [
        { "id": "handler-subscribes-payment-succeeded", "type": "Subscribe Integration Event",
          "source": { "element": "payment-succeeded-handler" }, "target": { "element": "payment-succeeded" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "rules": {
    "Services": {
      "files": "src/**/*.cs",
      "facts": {
        "subscription.handler": {
          "for": { "association": "Subscribe Integration Event" },
          "find": "class:{Service}",
          "read": [
            { "match": "[:,]\\s*IIntegrationEventHandler<{Message}>", "value": "present", "part": "declaration" }
          ],
          "default": "absent",
          "note": "eShop's own event bus interface. {Service} is the handler and {Message} the subscribed message, as the Services vocabulary binds them"
        }
      }
    }
  }
}
```

```csharp file src/Ordering.API/Application/IntegrationEvents/EventHandling/OrderPaymentSucceededIntegrationEventHandler.cs
namespace eShop.Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderPaymentSucceededIntegrationEventHandler(IMediator mediator) : IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
{
    public async Task Handle(OrderPaymentSucceededIntegrationEvent @event)
    {
        var command = new SetPaidOrderStatusCommand(@event.OrderId);
        await mediator.Send(command);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "subscription.handler", "element": "handler-subscribes-payment-succeeded", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/IntegrationEvents/EventHandling/OrderPaymentSucceededIntegrationEventHandler.cs:3" }
  ]
}
```

## eshop-integration-event-handler-wrong-message

The handler implements `IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>`. The class is found and the house recipe's read doesn't match, so it reads `absent`: contradicted. Traces ESC-SVC-61.

```json case
{ "phase": 4, "base": "eshop-integration-event-handler", "kinds": ["subscription.handler"], "traces": ["ESC-SVC-61"], "requirements": ["R1", "R8"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/IntegrationEvents/EventHandling/OrderPaymentSucceededIntegrationEventHandler.cs
namespace eShop.Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderPaymentSucceededIntegrationEventHandler(IMediator mediator) : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
{
    public async Task Handle(OrderPaymentFailedIntegrationEvent @event)
    {
        var command = new CancelOrderCommand(@event.OrderId);
        await mediator.Send(command);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "subscription.handler", "element": "handler-subscribes-payment-succeeded", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/subscription.handler", "severity": "error", "subject": "handler-subscribes-payment-succeeded" }
  ]
}
```

## domain-event-handler-across-partial-parts

A Domain Event Handler subscribed to `OrderStartedDomainEvent`, whose class is split in two partial parts: one handles the event, the other declares `INotificationHandler<OrderStartedDomainEvent>`. The parts of a partial type are read as one type (R37), so the subscription is found on the second part, and read at `Handle` in the first. MediatR's pack arrives `later`. Traces ESC-SVC-60.

```json case
{ "phase": "later", "kinds": ["subscription.handler"], "traces": ["ESC-SVC-60"], "requirements": ["R2", "R37"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order-started", "type": "Domain Event", "name": "OrderStartedDomainEvent" }
        ]
      }]
    },
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "validate-buyer-handler", "type": "Domain Event Handler", "name": "ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler" }
        ]
      }],
      "associations": [
        { "id": "handler-handles-order-started", "type": "Domain Event Handler Association",
          "source": { "element": "validate-buyer-handler" }, "target": { "element": "order-started" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.API/Application/DomainEventHandlers/ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler.cs
namespace eShop.Ordering.API.Application.DomainEventHandlers;

public partial class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(IBuyerRepository buyerRepository)
{
    public async Task Handle(OrderStartedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var buyer = await buyerRepository.FindAsync(domainEvent.UserId) ?? new Buyer(domainEvent.UserId, domainEvent.UserName);
        buyerRepository.Add(buyer);
        await buyerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
```

```csharp file src/Ordering.API/Application/DomainEventHandlers/ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler.Notification.cs
namespace eShop.Ordering.API.Application.DomainEventHandlers;

public partial class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
{
}
```

```json expect
{
  "claims": [
    { "kind": "subscription.handler", "element": "handler-handles-order-started", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/DomainEventHandlers/ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler.cs:5" }
  ]
}
```
