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

eShopCustom's handlers implement its own event bus interface, `IIntegrationEventHandler<M>`, which no framework's extension knows. The stock messaging extension, `intent.messaging`, reads it whatever the framework: the handler class has a method that takes the subscribed message, or declares a base over it. The reading is at `Handle`, the method that takes it. Traces ESC-SVC-61.

```json case
{ "phase": 4, "base": "verified", "without": ["src/Ordering.API/Application/IntegrationEvents/EventHandling/OrderPaymentSucceededHandler.cs"], "kinds": ["subscription.handler"], "traces": ["ESC-SVC-61"], "requirements": ["R2", "R51"], "packs": ["intent.core", "intent.services", "intent.messaging"], "language": "csharp" }
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
      "at": "src/Ordering.API/Application/IntegrationEvents/EventHandling/OrderPaymentSucceededIntegrationEventHandler.cs:5" }
  ]
}
```

## eshop-integration-event-handler-wrong-message

The handler implements `IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>`. The class is found, but no method takes `OrderPaymentSucceededIntegrationEvent` and no base is over it, so it reads `absent`: contradicted. Traces ESC-SVC-61.

```json case
{ "phase": 4, "base": "eshop-integration-event-handler", "kinds": ["subscription.handler"], "traces": ["ESC-SVC-61"], "requirements": ["R1", "R8"], "packs": ["intent.core", "intent.services", "intent.messaging"], "language": "csharp" }
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

## queue-consumer-makes-message-from-body

An Azure Service Bus consumer the framework hands only the raw message (`ProcessMessageEventArgs`): `OrderQueueConsumer`, an `IHostedService`, subscribes to `PlaceOrder`, a contracts package's message, by deserializing the body into it, and relays it as `OrderRelay`. The method that makes the message is the subscription's code, so the subscription is verified there and its send is read from it. A data class elsewhere takes a type of the message's name, a local `PlaceOrder` entity, and sends nothing: it isn't the handler's code. A migrated application's three consumers were contradicted, and the move removed their true sends as missing from its data service.

```json case
{ "phase": 4, "kinds": ["subscription.handler", "intent.sends"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [
        {
          "id": "services", "name": "Shop.Worker",
          "elements": [
            { "id": "order-consumer", "type": "Integration Event Handler", "name": "OrderQueueConsumer" }
          ]
        },
        {
          "id": "eventing", "name": "Shop.Worker.Eventing", "type": "Eventing Package",
          "elements": [
            { "id": "place-order", "type": "Integration Command", "name": "PlaceOrder" },
            { "id": "order-relay", "type": "Integration Command", "name": "OrderRelay" }
          ]
        }
      ],
      "associations": [
        { "id": "consumer-subscribes-place-order", "type": "Subscribe Integration Command",
          "source": { "element": "order-consumer" }, "target": { "element": "place-order" } },
        { "id": "subscription-sends-relay", "type": "Send Integration Command",
          "source": { "element": "consumer-subscribes-place-order" }, "target": { "element": "order-relay" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Shop.Worker/Handlers/OrderQueueConsumer.cs
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Shop.Worker.Messages;

namespace Shop.Worker.Handlers;

public class OrderQueueConsumer : IHostedService
{
    private readonly IRelaySender _sender;
    private ServiceBusProcessor _processor;

    public OrderQueueConsumer(IRelaySender sender) => _sender = sender;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _processor = new ServiceBusClient("connection").CreateProcessor("orders");
        _processor.ProcessMessageAsync += HandleOrder;
        await _processor.StartProcessingAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => _processor.StopProcessingAsync(cancellationToken);

    public async Task HandleOrder(ProcessMessageEventArgs args)
    {
        var order = JsonConvert.DeserializeObject<Shop.Contracts.Pricing.PlaceOrder>(args.Message.Body.ToString());
        await _sender.Send(new OrderRelay { Body = order });
        await args.CompleteMessageAsync(args.Message);
    }
}
```

```csharp file src/Shop.Worker/Messages/OrderRelay.cs
using Shop.Contracts.Pricing;

namespace Shop.Worker.Messages;

public class OrderRelay
{
    public PlaceOrder Body { get; set; }
}

public interface IRelaySender
{
    Task Send(OrderRelay message);
}
```

```csharp file src/Shop.Data/OrderData.cs
namespace Shop.Data;

public class PlaceOrder
{
    public int Id { get; set; }
}

public class OrderData
{
    public void Save(PlaceOrder order)
    {
    }
}
```

```json expect
{
  "claims": [
    { "kind": "subscription.handler", "element": "consumer-subscribes-place-order", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Shop.Worker/Handlers/OrderQueueConsumer.cs:24" },
    { "kind": "intent.sends", "element": "subscription-sends-relay", "outcome": "verified", "actual": "present" }
  ],
  "findings": [
    { "rule": "claim/intent.sends", "count": 0 },
    { "rule": "claim/subscription.handler", "count": 0 }
  ]
}
```

## queue-consumer-not-making-the-message

The same consumer deserializing the body into another message: nothing in its own class takes or makes `PlaceOrder`, so the subscription is contradicted. Its send can't be read, since the handler's code isn't found, rather than read from the data class that takes a local `PlaceOrder`.

```json case
{ "phase": 4, "base": "queue-consumer-makes-message-from-body", "kinds": ["subscription.handler", "intent.sends"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Shop.Worker/Handlers/OrderQueueConsumer.cs
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Shop.Worker.Messages;

namespace Shop.Worker.Handlers;

public class OrderQueueConsumer : IHostedService
{
    private readonly IRelaySender _sender;
    private ServiceBusProcessor _processor;

    public OrderQueueConsumer(IRelaySender sender) => _sender = sender;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _processor = new ServiceBusClient("connection").CreateProcessor("orders");
        _processor.ProcessMessageAsync += HandleOrder;
        await _processor.StartProcessingAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => _processor.StopProcessingAsync(cancellationToken);

    public async Task HandleOrder(ProcessMessageEventArgs args)
    {
        var order = JsonConvert.DeserializeObject<Shop.Contracts.Pricing.CancelOrder>(args.Message.Body.ToString());
        await _sender.Send(new OrderRelay());
        await args.CompleteMessageAsync(args.Message);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "subscription.handler", "element": "consumer-subscribes-place-order", "outcome": "contradicted", "expected": "present", "actual": "absent" },
    { "kind": "intent.sends", "element": "subscription-sends-relay", "outcome": "cantVerify", "reason": "notFound" }
  ],
  "findings": [
    { "rule": "claim/subscription.handler", "severity": "error", "subject": "consumer-subscribes-place-order" }
  ]
}
```
