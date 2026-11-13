# intent.publishes

A `Publish Integration Event` from an Operation, Command or handler subscription to a Message, against the code: the source's handler publishes a new instance of that message, `Publish(new X(...))`, here through NServiceBus's `IMessageSession` or `IMessageHandlerContext`. The claim is the association's. `intent.publishes` is an existence kind: a modelled publish the code lacks is contradicted (R8). A publish the model lacks is the `unmodelled.interaction` opt-in rule's.

The fixtures adapt eShopCustom's `Ordering.API` endpoints and integration event handlers, moved onto NServiceBus.

## verified

`CreateOrderAsync` publishes `OrderStartedIntegrationEvent` through the injected `IMessageSession`. Traces ESC-SVC-39 (from an operation).

```json case
{ "phase": 4, "kinds": ["intent.publishes"], "traces": ["ESC-SVC-39"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "intent.nservicebus"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [
        {
          "id": "services", "name": "Ordering.API",
          "elements": [
            { "id": "orders-api", "type": "Service", "name": "OrdersApi",
              "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
              "children": [
                { "id": "create-order", "type": "Operation", "name": "CreateOrderAsync",
                  "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "/" } } }
              ] }
          ]
        },
        {
          "id": "eventing", "name": "Ordering.Eventing", "type": "Eventing Package",
          "elements": [
            { "id": "order-started", "type": "Message", "name": "OrderStartedIntegrationEvent" }
          ]
        }
      ],
      "associations": [
        { "id": "create-order-publishes-order-started", "type": "Publish Integration Event",
          "source": { "element": "create-order" }, "target": { "element": "order-started" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPost("/", CreateOrderAsync);

        return api;
    }

    public static async Task<Ok<Guid>> CreateOrderAsync(CreateOrderRequest request, IMessageSession messageSession)
    {
        var orderId = Guid.NewGuid();

        await messageSession.Publish(new OrderStartedIntegrationEvent(orderId, request.UserId));

        return TypedResults.Ok(orderId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.publishes", "element": "create-order-publishes-order-started", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:18" }
  ]
}
```

## contradicted

The endpoint no longer publishes anything. Traces ESC-SVC-39.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.publishes"], "traces": ["ESC-SVC-39"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPost("/", CreateOrderAsync);

        return api;
    }

    public static async Task<Ok<Guid>> CreateOrderAsync(CreateOrderRequest request, IMessageSession messageSession)
    {
        var orderId = Guid.NewGuid();

        return TypedResults.Ok(orderId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.publishes", "element": "create-order-publishes-order-started", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.publishes", "severity": "error", "subject": "create-order-publishes-order-started", "messageContains": "isn't in the code" }
  ]
}
```

## unreadable

The endpoint publishes an event a factory method built. A publish is there, but the rule can't tell which message it is, so the claim is can't verify, `unreadable`: neither verified nor reported missing.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.publishes"], "cases": ["engine/unreadable"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPost("/", CreateOrderAsync);

        return api;
    }

    public static async Task<Ok<Guid>> CreateOrderAsync(CreateOrderRequest request, IMessageSession messageSession)
    {
        var orderId = Guid.NewGuid();
        var integrationEvent = OrderEvents.Started(orderId, request.UserId);

        await messageSession.Publish(integrationEvent);

        return TypedResults.Ok(orderId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.publishes", "element": "create-order-publishes-order-started", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:19" }
  ],
  "findings": [
    { "rule": "claim/intent.publishes", "severity": "error", "subject": "create-order-publishes-order-started", "messageContains": "can't read" }
  ]
}
```

## from-handler-subscription

An Integration Event Handler subscribes to `OrderPaymentSucceededIntegrationEvent`, and its handling of that message publishes `OrderStatusChangedToPaidIntegrationEvent`. In Intent's model a handler's publish is drawn from its subscription, so the `Publish Integration Event`'s source is the `Subscribe Integration Event` association (its subscribed end), and the claim is checked in `Handle(OrderPaymentSucceededIntegrationEvent, ...)`. Traces ESC-SVC-39 (from a handler subscription).

```json case
{ "phase": 4, "kinds": ["intent.publishes"], "traces": ["ESC-SVC-39"], "requirements": ["R2", "R30"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
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
            { "id": "payment-succeeded", "type": "Message", "name": "OrderPaymentSucceededIntegrationEvent" },
            { "id": "status-changed-to-paid", "type": "Message", "name": "OrderStatusChangedToPaidIntegrationEvent" }
          ]
        }
      ],
      "associations": [
        { "id": "handler-subscribes-payment-succeeded", "type": "Subscribe Integration Event",
          "source": { "element": "payment-succeeded-handler" }, "target": { "element": "payment-succeeded" } },
        { "id": "subscription-publishes-status-changed", "type": "Publish Integration Event",
          "source": { "element": "handler-subscribes-payment-succeeded" }, "target": { "element": "status-changed-to-paid" } }
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
    { "kind": "intent.publishes", "element": "subscription-publishes-status-changed", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/IntegrationEvents/EventHandling/OrderPaymentSucceededHandler.cs:7" }
  ]
}
```
