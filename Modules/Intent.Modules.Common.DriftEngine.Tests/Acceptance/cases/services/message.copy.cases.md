# message.copy

Another application's Message that a handler here subscribes to, against the subscriber's code: the event bus deserialises the message into a local copy, a class or record of the same name deriving eShopCustom's `IntegrationEvent`, whose own properties must be among the message's modelled properties, with the same types (an enum read as its name or number). The publisher's code is another application's, so the model's message stands for what is sent. The claim is on the subscription, as only its handler names the copy, and only when another application owns the message (a referenced package's): a Message of this application is its own type, which the structure claims check. A copy the model draws itself (Common.Drift's `Integration Event Subscription` from the copy to the published message) is claimed on that link and compared with the published message's code.

The fixture adapts eShopCustom's `Ordering.API` integration events.

## own-message-not-claimed

`OrderPaymentSucceededHandler` subscribes to `OrderPaymentSucceededIntegrationEvent`, which this application's own Eventing Package declares. It is a type of this application, not a copy of another's, so there is no `message.copy` claim on it.

```json case
{ "phase": "later", "kinds": ["message.copy"], "requirements": ["R44"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
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
            { "id": "payment-succeeded", "type": "Message", "name": "OrderPaymentSucceededIntegrationEvent",
              "children": [
                { "id": "payment-succeeded-order-id", "type": "Property", "name": "OrderId", "typeRef": { "type": "int" } }
              ] }
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

```csharp file src/Ordering.API/Application/IntegrationEvents/Events/OrderPaymentSucceededIntegrationEvent.cs
namespace eShop.Ordering.API.Application.IntegrationEvents.Events;

public record OrderPaymentSucceededIntegrationEvent(int OrderId) : IntegrationEvent;
```

```json expect
{
  "claims": [
    { "kind": "message.copy", "element": "payment-succeeded", "absent": true },
    { "kind": "message.copy", "element": "handler-subscribes-payment-succeeded", "absent": true }
  ],
  "findings": [
    { "rule": "claim/message.copy", "count": 0 }
  ]
}
```

## subscriber-copy

Webhooks subscribes to Ordering's `OrderStatusChangedToPaidIntegrationEvent`, which a referenced package of Ordering's models. Its handler takes Webhooks' own record of the name, whose `OrderId` and `OrderStatus` are the message's: `OrderStatus` is an enum the copy reads as its name.

```json case
{ "phase": "later", "kinds": ["message.copy"], "traces": ["ESC-SVC-64"], "requirements": ["R2", "R44"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Webhooks.API",
        "elements": [
          { "id": "paid-handler", "type": "Integration Event Handler", "name": "OrderStatusChangedToPaidIntegrationEventHandler" }
        ]
      }],
      "associations": [
        { "id": "handler-subscribes-paid", "type": "Subscribe Integration Event",
          "source": { "element": "paid-handler" }, "target": { "element": "status-changed-to-paid" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Webhooks", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  },
  "references": {
    "Services": {
      "packages": [{
        "id": "ordering-eventing", "name": "Ordering.Eventing", "type": "Eventing Package",
        "elements": [
          { "id": "order-status", "type": "Enum", "name": "OrderStatus" },
          { "id": "status-changed-to-paid", "type": "Message", "name": "OrderStatusChangedToPaidIntegrationEvent",
            "children": [
              { "id": "status-changed-to-paid-order-id", "type": "Property", "name": "OrderId", "typeRef": { "type": "int" } },
              { "id": "status-changed-to-paid-order-status", "type": "Property", "name": "OrderStatus", "typeRef": { "element": "order-status" } },
              { "id": "status-changed-to-paid-buyer-name", "type": "Property", "name": "BuyerName", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    }
  }
}
```

```csharp file src/Webhooks.API/IntegrationEvents/OrderStatusChangedToPaidIntegrationEvent.cs
namespace Webhooks.API.IntegrationEvents;

public record OrderStatusChangedToPaidIntegrationEvent(int OrderId, string OrderStatus) : IntegrationEvent;
```

```csharp file src/Webhooks.API/IntegrationEvents/OrderStatusChangedToPaidIntegrationEventHandler.cs
namespace Webhooks.API.IntegrationEvents;

public class OrderStatusChangedToPaidIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
{
    public Task Handle(OrderStatusChangedToPaidIntegrationEvent @event) => Task.CompletedTask;
}
```

```json expect
{
  "claims": [
    { "kind": "message.copy", "element": "handler-subscribes-paid", "outcome": "verified",
      "at": "src/Webhooks.API/IntegrationEvents/OrderStatusChangedToPaidIntegrationEvent.cs:3" },
    { "kind": "message.copy", "element": "status-changed-to-paid", "absent": true }
  ]
}
```

## subscriber-copy-extra-property

Webhooks' copy gains a `Note` the message doesn't send, so it is never set when the event arrives.

```json case
{ "phase": "later", "base": "subscriber-copy", "kinds": ["message.copy"], "traces": ["ESC-SVC-64"], "requirements": ["R8", "R44"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Webhooks.API/IntegrationEvents/OrderStatusChangedToPaidIntegrationEvent.cs
namespace Webhooks.API.IntegrationEvents;

public record OrderStatusChangedToPaidIntegrationEvent(int OrderId, string OrderStatus, string Note) : IntegrationEvent;
```

```json expect
{
  "claims": [
    { "kind": "message.copy", "element": "handler-subscribes-paid", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/message.copy", "severity": "error", "subject": "handler-subscribes-paid", "messageContains": "`Note` isn't one of its modelled properties" }
  ]
}
```
