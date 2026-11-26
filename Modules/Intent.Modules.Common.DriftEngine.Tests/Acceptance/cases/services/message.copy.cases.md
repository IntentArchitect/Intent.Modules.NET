# message.copy

Another application's Message that a handler here subscribes to, against the subscriber's code: the event bus deserialises the message into a local copy, a class or record of the same name deriving eShopCustom's `IntegrationEvent`, whose own properties must be among the message's modelled properties, with the same types (an enum read as its name or number). The publisher's code is another application's, so the model's message stands for what is sent. The claim is on the subscription, as only its handler names the copy, and only when another application owns the message (a referenced package's): a Message of this application is its own type, which the structure claims check. A copy the model draws itself (Common.Drift's `Integration Event Subscription` from the copy to the published message) is claimed on that link and compared with the published message's code. The types such a message carries through its fields, an Eventing DTO and the ones it carries in turn, are deserialised into copies too: each is a `message.copy` claim of its own on the subscription, keyed by the type's name, read from the type of its name that the copy of its carrier names in a property, whose properties must be among the type's fields, with the same types.

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

## subscriber-copy-beside-own-messages

Ordering owns and models `OrderStartedIntegrationEvent`, and subscribes to OrderProcessor's `GracePeriodConfirmedIntegrationEvent`, which a referenced package models, through its own copy of the name. Both derive from `IntegrationEvent`, so the copy is a member of the family Ordering's own message teaches, and it is the referenced message: it isn't reported as a message the diagram doesn't show. The adapted code is eShop's `Ordering.API`.

```json case
{ "phase": "later", "kinds": ["message.copy"], "requirements": ["R2", "R44"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [
        { "id": "eventing", "name": "Ordering.API.Eventing", "type": "Eventing Package",
          "elements": [
            { "id": "order-started", "type": "Message", "name": "OrderStartedIntegrationEvent",
              "children": [{ "id": "order-started-user", "type": "Property", "name": "UserId", "typeRef": { "type": "string" } }] }
          ] },
        { "id": "services", "name": "Ordering.API",
          "elements": [
            { "id": "grace-handler", "type": "Integration Event Handler", "name": "GracePeriodConfirmedIntegrationEventHandler" }
          ] }
      ],
      "associations": [
        { "id": "handler-subscribes-grace", "type": "Subscribe Integration Event",
          "source": { "element": "grace-handler" }, "target": { "element": "grace-confirmed" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  },
  "references": {
    "Services": {
      "packages": [{
        "id": "processor-eventing", "name": "OrderProcessor.Eventing", "type": "Eventing Package",
        "elements": [
          { "id": "grace-confirmed", "type": "Message", "name": "GracePeriodConfirmedIntegrationEvent",
            "children": [{ "id": "grace-confirmed-order", "type": "Property", "name": "OrderId", "typeRef": { "type": "int" } }] }
        ]
      }]
    }
  }
}
```

```csharp file src/EventBus/IntegrationEvent.cs
namespace EventBus;

public record IntegrationEvent
{
    public Guid Id { get; set; }
}
```

```csharp file src/Ordering.API/IntegrationEvents/Events/OrderStartedIntegrationEvent.cs
namespace Ordering.API.IntegrationEvents.Events;

public record OrderStartedIntegrationEvent(string UserId) : IntegrationEvent;
```

```csharp file src/Ordering.API/IntegrationEvents/Events/GracePeriodConfirmedIntegrationEvent.cs
namespace Ordering.API.IntegrationEvents.Events;

public record GracePeriodConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;
```

```csharp file src/Ordering.API/IntegrationEvents/EventHandling/GracePeriodConfirmedIntegrationEventHandler.cs
namespace Ordering.API.IntegrationEvents.EventHandling;

public class GracePeriodConfirmedIntegrationEventHandler : IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>
{
    public Task Handle(GracePeriodConfirmedIntegrationEvent @event) => Task.CompletedTask;
}
```

```json expect
{
  "claims": [
    { "kind": "message.copy", "element": "handler-subscribes-grace", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "messaging/unmodelled.message", "count": 0 }
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

## carried-dto

Catalog subscribes to Ordering's `OrderStatusChangedToAwaitingValidationIntegrationEvent`, whose `OrderStockItems` field carries Ordering's Eventing DTO `OrderStockItem`. Catalog deserialises it into its own copies of both: the copy of the message names Catalog's `OrderStockItem` record in a property, and that record is the copy of the Eventing DTO, whose `ProductId` and `Units` are the DTO's fields. Each type a subscribed message carries is a `message.copy` claim of its own on the subscription, keyed by the type's name, so the copy is checked, and located: the opt-in rules don't report it. The DTO is reached from the message the subscription links to, its fields' types followed, so no referenced package needs to be handed over, as a Software Factory run doesn't for a subscriber's Eventing packages. The fixture adapts eShop's `Catalog.API`.

```json case
{ "phase": "later", "kinds": ["message.copy"], "cases": ["optin/unmodelled.type", "optin/unmodelled.file"], "traces": ["ESC-SVC-64"], "requirements": ["R2", "R44", "R49"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Catalog.API",
        "elements": [
          { "id": "awaiting-handler", "type": "Integration Event Handler", "name": "OrderStatusChangedToAwaitingValidationIntegrationEventHandler" }
        ]
      }],
      "associations": [
        { "id": "handler-subscribes-awaiting", "type": "Subscribe Integration Event",
          "source": { "element": "awaiting-handler" }, "target": { "element": "awaiting-validation" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Catalog", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" }, { "Rule": "optin/unmodelled.file" } ] }
        } }]
    }
  },
  "references": {
    "Services": {
      "packages": [{
        "id": "ordering-eventing", "name": "Ordering.API.Eventing", "type": "Eventing Package",
        "elements": [
          { "id": "order-stock-item", "type": "Eventing DTO", "name": "OrderStockItem",
            "children": [
              { "id": "order-stock-item-product-id", "type": "Eventing DTO-Field", "name": "ProductId", "typeRef": { "type": "int" } },
              { "id": "order-stock-item-units", "type": "Eventing DTO-Field", "name": "Units", "typeRef": { "type": "int" } }
            ] },
          { "id": "awaiting-validation", "type": "Message", "name": "OrderStatusChangedToAwaitingValidationIntegrationEvent",
            "children": [
              { "id": "awaiting-validation-order-id", "type": "Property", "name": "OrderId", "typeRef": { "type": "int" } },
              { "id": "awaiting-validation-items", "type": "Property", "name": "OrderStockItems", "typeRef": { "element": "order-stock-item", "collection": true } }
            ] }
        ]
      }]
    }
  }
}
```

```csharp file src/Catalog.API/IntegrationEvents/Events/OrderStatusChangedToAwaitingValidationIntegrationEvent.cs
namespace eShop.Catalog.API.IntegrationEvents.Events;

public record OrderStatusChangedToAwaitingValidationIntegrationEvent(int OrderId, IEnumerable<OrderStockItem> OrderStockItems) : IntegrationEvent;
```

```csharp file src/Catalog.API/IntegrationEvents/Events/OrderStockItem.cs
namespace eShop.Catalog.API.IntegrationEvents.Events;

public record OrderStockItem(int ProductId, int Units);
```

```csharp file src/Catalog.API/IntegrationEvents/EventHandling/OrderStatusChangedToAwaitingValidationIntegrationEventHandler.cs
using eShop.Catalog.API.IntegrationEvents.Events;

namespace eShop.Catalog.API.IntegrationEvents.EventHandling;

public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent>
{
    public Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent @event) => Task.CompletedTask;
}
```

```json expect
{
  "claims": [
    { "kind": "message.copy", "element": "handler-subscribes-awaiting", "outcome": "verified",
      "at": "src/Catalog.API/IntegrationEvents/Events/OrderStatusChangedToAwaitingValidationIntegrationEvent.cs:3" },
    { "kind": "message.copy", "element": "handler-subscribes-awaiting", "key": "OrderStockItem", "outcome": "verified",
      "at": "src/Catalog.API/IntegrationEvents/Events/OrderStockItem.cs:3" }
  ],
  "findings": [
    { "rule": "claim/message.copy", "count": 0 },
    { "rule": "optin/unmodelled.type", "subject": "src/Catalog.API/IntegrationEvents/Events/OrderStockItem.cs:3", "count": 0 },
    { "rule": "optin/unmodelled.file", "count": 0 }
  ]
}
```

## carried-dto-other-type

Catalog's copy of `OrderStockItem` makes `Units` a `long`, a planted drift: the DTO's `Units` is an `int`. It is one finding, on the subscription, naming the copy, the DTO and the message that carries it. The copy of the message is still right.

```json case
{ "phase": "later", "base": "carried-dto", "kinds": ["message.copy"], "traces": ["ESC-SVC-64"], "requirements": ["R8", "R11", "R44"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Catalog.API/IntegrationEvents/Events/OrderStockItem.cs
namespace eShop.Catalog.API.IntegrationEvents.Events;

public record OrderStockItem(int ProductId, long Units);
```

```json expect
{
  "claims": [
    { "kind": "message.copy", "element": "handler-subscribes-awaiting", "outcome": "verified" },
    { "kind": "message.copy", "element": "handler-subscribes-awaiting", "key": "OrderStockItem", "outcome": "contradicted", "expected": "present", "actual": "absent",
      "at": "src/Catalog.API/IntegrationEvents/Events/OrderStockItem.cs:3" }
  ],
  "findings": [
    { "rule": "claim/message.copy", "severity": "error", "subject": "handler-subscribes-awaiting",
      "messageContains": "`OrderStockItem` (src/Catalog.API/IntegrationEvents/Events/OrderStockItem.cs) is the copy of `OrderStockItem`, the Eventing DTO of `Ordering.API.Eventing` that `OrderStatusChangedToAwaitingValidationIntegrationEvent` carries, but `Units` is `long` in the copy and `int` in the model." },
    { "rule": "claim/message.copy", "count": 1 }
  ]
}
```

## carried-dto-extra-field

Catalog's copy of `OrderStockItem` has a `Quantity` the Eventing DTO doesn't have, so it is never set when the event arrives. A copy may leave a field out, as the message's copy may, but every property it has must be one of the DTO's.

```json case
{ "phase": "later", "base": "carried-dto", "kinds": ["message.copy"], "traces": ["ESC-SVC-64"], "requirements": ["R8", "R44"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Catalog.API/IntegrationEvents/Events/OrderStockItem.cs
namespace eShop.Catalog.API.IntegrationEvents.Events;

public record OrderStockItem(int ProductId, int Quantity);
```

```json expect
{
  "claims": [
    { "kind": "message.copy", "element": "handler-subscribes-awaiting", "key": "OrderStockItem", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/message.copy", "severity": "error", "subject": "handler-subscribes-awaiting", "messageContains": "but `Quantity` isn't one of its fields." },
    { "rule": "claim/message.copy", "count": 1 }
  ]
}
```

## carried-dto-not-named-by-the-copy

Catalog's copy of the message keeps only `OrderId`, as a copy may, so it names no `OrderStockItem`. A record of that name elsewhere in the code isn't the DTO's copy for its name alone: the carried type's claim is `blocked`, with no finding, and the record is code the model doesn't describe, which `optin/unmodelled.type` reports.

```json case
{ "phase": "later", "base": "carried-dto", "kinds": ["message.copy"], "cases": ["optin/unmodelled.type"], "requirements": ["R5", "R49"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Catalog.API/IntegrationEvents/Events/OrderStatusChangedToAwaitingValidationIntegrationEvent.cs
namespace eShop.Catalog.API.IntegrationEvents.Events;

public record OrderStatusChangedToAwaitingValidationIntegrationEvent(int OrderId) : IntegrationEvent;
```

```json expect
{
  "claims": [
    { "kind": "message.copy", "element": "handler-subscribes-awaiting", "outcome": "verified" },
    { "kind": "message.copy", "element": "handler-subscribes-awaiting", "key": "OrderStockItem", "outcome": "cantVerify", "reason": "blocked" }
  ],
  "findings": [
    { "rule": "claim/message.copy", "count": 0 },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Catalog.API/IntegrationEvents/Events/OrderStockItem.cs:3", "messageContains": "OrderStockItem" }
  ]
}
```

## carried-dto-nested

`OrderStockItem` carries an Eventing DTO of its own, `StockProduct`, so the message carries it too, through it. The copy of `OrderStockItem` names Catalog's `StockProduct` class, which is that DTO's copy, checked and located like the first: a type the message carries through another carried type's fields is followed down as far as the fields go.

```json case
{ "phase": "later", "kinds": ["message.copy"], "cases": ["optin/unmodelled.type"], "traces": ["ESC-SVC-64"], "requirements": ["R2", "R44"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Catalog.API",
        "elements": [
          { "id": "awaiting-handler", "type": "Integration Event Handler", "name": "OrderStatusChangedToAwaitingValidationIntegrationEventHandler" }
        ]
      }],
      "associations": [
        { "id": "handler-subscribes-awaiting", "type": "Subscribe Integration Event",
          "source": { "element": "awaiting-handler" }, "target": { "element": "awaiting-validation" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Catalog", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
        } }]
    }
  },
  "references": {
    "Services": {
      "packages": [{
        "id": "ordering-eventing", "name": "Ordering.API.Eventing", "type": "Eventing Package",
        "elements": [
          { "id": "stock-product", "type": "Eventing DTO", "name": "StockProduct",
            "children": [
              { "id": "stock-product-id", "type": "Eventing DTO-Field", "name": "Id", "typeRef": { "type": "int" } },
              { "id": "stock-product-name", "type": "Eventing DTO-Field", "name": "Name", "typeRef": { "type": "string" } }
            ] },
          { "id": "order-stock-item", "type": "Eventing DTO", "name": "OrderStockItem",
            "children": [
              { "id": "order-stock-item-product", "type": "Eventing DTO-Field", "name": "Product", "typeRef": { "element": "stock-product" } },
              { "id": "order-stock-item-units", "type": "Eventing DTO-Field", "name": "Units", "typeRef": { "type": "int" } }
            ] },
          { "id": "awaiting-validation", "type": "Message", "name": "OrderStatusChangedToAwaitingValidationIntegrationEvent",
            "children": [
              { "id": "awaiting-validation-order-id", "type": "Property", "name": "OrderId", "typeRef": { "type": "int" } },
              { "id": "awaiting-validation-items", "type": "Property", "name": "OrderStockItems", "typeRef": { "element": "order-stock-item", "collection": true } }
            ] }
        ]
      }]
    }
  }
}
```

```csharp file src/Catalog.API/IntegrationEvents/Events/OrderStatusChangedToAwaitingValidationIntegrationEvent.cs
namespace eShop.Catalog.API.IntegrationEvents.Events;

public record OrderStatusChangedToAwaitingValidationIntegrationEvent(int OrderId, IEnumerable<OrderStockItem> OrderStockItems) : IntegrationEvent;
```

```csharp file src/Catalog.API/IntegrationEvents/Events/OrderStockItem.cs
namespace eShop.Catalog.API.IntegrationEvents.Events;

public record OrderStockItem(StockProduct Product, int Units);

public class StockProduct
{
    public int Id { get; set; }

    public string Name { get; set; }
}
```

```csharp file src/Catalog.API/IntegrationEvents/EventHandling/OrderStatusChangedToAwaitingValidationIntegrationEventHandler.cs
using eShop.Catalog.API.IntegrationEvents.Events;

namespace eShop.Catalog.API.IntegrationEvents.EventHandling;

public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent>
{
    public Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent @event) => Task.CompletedTask;
}
```

```json expect
{
  "claims": [
    { "kind": "message.copy", "element": "handler-subscribes-awaiting", "key": "OrderStockItem", "outcome": "verified",
      "at": "src/Catalog.API/IntegrationEvents/Events/OrderStockItem.cs:3" },
    { "kind": "message.copy", "element": "handler-subscribes-awaiting", "key": "StockProduct", "outcome": "verified",
      "at": "src/Catalog.API/IntegrationEvents/Events/OrderStockItem.cs:5" }
  ],
  "findings": [
    { "rule": "claim/message.copy", "count": 0 },
    { "rule": "optin/unmodelled.type", "count": 0 }
  ]
}
```
