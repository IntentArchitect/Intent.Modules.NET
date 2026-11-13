# message.copy

Another application's Message that a handler here subscribes to, against the subscriber's code: the event bus deserialises the message into a local copy, a class or record of the same name deriving eShopCustom's `IntegrationEvent`, whose properties must be a subset of the publisher's, with the same types. The claim is on the subscribed Message, and only when another application owns it: a Message of this application is its own type, which the structure claims check. `message.copy` is an existence kind arriving `later`; comparing the copy's properties with the publisher's needs a plug-in.

A fixture model can't yet say that a package belongs to another application, and no expression can ask it, so the case that would show the copy check itself isn't written here. Only the boundary is: this application's own Message gives no `message.copy` claim.

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
