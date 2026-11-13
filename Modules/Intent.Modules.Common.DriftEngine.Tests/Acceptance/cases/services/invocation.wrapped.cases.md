# invocation.wrapped

How an endpoint sends a Command it invokes, against the target Command's `Identified Command` stereotype. eShopCustom makes its endpoints idempotent by wrapping an Identified Command in its own `IdentifiedCommand<X, R>` with the request id, and its module's `Identified Command` stereotype says which Commands are sent that way. The stereotype and the wrapper type are eShopCustom's own, so the house pack brings both the vocabulary and the recipe: a claim on each `Perform Invocation` from an Operation to a Command that is an Identified Command, valued `IdentifiedCommand`; the recipe reads `IdentifiedCommand` when the endpoint creates the wrapper around that Command and `bare` when it sends the Command itself. `invocation.wrapped` is `stated: whenSet` (decision 4), so a Command without the stereotype has no claim, and a wrapped send of one is the `unmodelled.value` opt-in rule's. The kind arrives `later`: resolving every way a send's argument is written (locals, casts, conditionals, delegation) needs a plug-in, and the recipe here reads only the shapes eShopCustom's endpoints use.

The fixtures adapt eShopCustom's `Ordering.API/Apis/OrdersApi.cs` and its `UserCheckoutAcceptedIntegrationEventHandler`.

## wrapped-verified

`CancelOrderAsync` invokes `CancelOrderCommand`, an Identified Command, and sends it wrapped in `IdentifiedCommand<CancelOrderCommand, bool>` through a local. Traces ESC-SVC-36.

```json case
{ "phase": "later", "kinds": ["invocation.wrapped"], "traces": ["ESC-SVC-36"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "cancel-order-command", "type": "Command", "name": "CancelOrderCommand", "typeRef": { "type": "bool" },
            "stereotypes": { "Identified Command": { "Validated": false } },
            "children": [
              { "id": "cancel-order-command-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } } }
            ] }
        ]
      }],
      "associations": [
        { "id": "cancel-order-invokes-command", "type": "Perform Invocation",
          "source": { "element": "cancel-order" }, "target": { "element": "cancel-order-command" } }
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
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }, { "id": "intent.aspnetcore", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "vocabulary": {
    "Services": {
      "entries": [
        {
          "kind": "invocation.wrapped",
          "for": { "association": "Perform Invocation", "where": "source.element.specialization == 'Operation' && target.element.specialization == 'Command'" },
          "when": "target.element.hasStereotype('Identified Command')",
          "value": "'IdentifiedCommand'",
          "bindings": {
            "Service": "source.element.parent.name",
            "Operation": "source.element.name",
            "Command": "target.element.name"
          },
          "source": "the target Command's Identified Command stereotype"
        }
      ]
    }
  },
  "rules": {
    "Services": {
      "facts": {
        "invocation.wrapped": {
          "find": "class:{Service} > method:{Operation}",
          "read": [
            { "match": "new\\s+IdentifiedCommand<\\s*{Command}\\s*,", "value": "IdentifiedCommand", "part": "body" },
            { "match": "\\.Send\\(\\s*new\\s+{Command}\\s*\\(", "value": "bare", "part": "body" },
            { "match": "\\b{Command}\\s+(\\w+)\\s*[,)][\\s\\S]*\\.Send\\(\\s*\\1\\s*\\)", "value": "bare" }
          ],
          "note": "The wrapper around the invoked Command, else a send of the Command itself or of the handler's parameter of its type"
        }
      }
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

        api.MapPut("/cancel", CancelOrderAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(
        [FromHeader(Name = "x-requestid")] Guid requestId,
        CancelOrderCommand command,
        [AsParameters] OrderServices services)
    {
        var requestCancelOrder = new IdentifiedCommand<CancelOrderCommand, bool>(command, requestId);
        var commandResult = await services.Mediator.Send(requestCancelOrder);

        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "invocation.wrapped", "element": "cancel-order-invokes-command", "outcome": "verified", "expected": "IdentifiedCommand", "actual": "IdentifiedCommand",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:19" }
  ]
}
```

## sent-bare-contradicted

The endpoint sends the Identified Command it was given straight through MediatR, so a retried request would cancel the order twice. eShopCustom reported a bare send of an Identified Command as Wrong. Traces ESC-SVC-36.

```json case
{ "phase": "later", "base": "wrapped-verified", "kinds": ["invocation.wrapped"], "traces": ["ESC-SVC-36"], "requirements": ["R1"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(
        CancelOrderCommand command,
        [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);

        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "invocation.wrapped", "element": "cancel-order-invokes-command", "outcome": "contradicted", "expected": "IdentifiedCommand", "actual": "bare" }
  ],
  "findings": [
    { "rule": "claim/invocation.wrapped", "severity": "error", "subject": "cancel-order-invokes-command" }
  ]
}
```

## wrapped-not-identified-reported-when-opted-in

`CancelOrderCommand` has no `Identified Command` stereotype, and the endpoint still wraps it. With nothing set there is no claim (decision 4); eShopCustom reported the wrapped send as Backlog, and here it is the `unmodelled.value` opt-in rule's finding, on the line that creates the wrapper. Traces ESC-SVC-36 (a wrapped send of a non-Identified Command).

```json case
{ "phase": "later", "base": "wrapped-verified", "kinds": ["invocation.wrapped"], "cases": ["optin/unmodelled.value"], "traces": ["ESC-SVC-36"], "requirements": ["R4", "R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "cancel-order-command", "type": "Command", "name": "CancelOrderCommand", "typeRef": { "type": "bool" },
            "children": [
              { "id": "cancel-order-command-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } } }
            ] }
        ]
      }],
      "associations": [
        { "id": "cancel-order-invokes-command", "type": "Perform Invocation",
          "source": { "element": "cancel-order" }, "target": { "element": "cancel-order-command" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "invocation.wrapped", "element": "cancel-order-invokes-command", "absent": true }
  ],
  "findings": [
    { "rule": "claim/invocation.wrapped", "count": 0 },
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Ordering.API/Apis/OrdersApi.cs:19", "messageContains": "IdentifiedCommand" }
  ]
}
```

## handler-send-not-claimed

An Integration Event Handler sends `CreateOrderCommand`, an Identified Command, bare. eShopCustom allowed that, because the event's id already makes the handling idempotent, so the house vocabulary claims only invocations from an Operation, and a subscription's invocation has no `invocation.wrapped` claim. Traces ESC-SVC-36 (the handler half).

```json case
{ "phase": "later", "base": "wrapped-verified", "without": ["src/Ordering.API/Apis/OrdersApi.cs"], "kinds": ["invocation.wrapped"], "traces": ["ESC-SVC-36"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [
        {
          "id": "services", "name": "Ordering.API",
          "elements": [
            { "id": "create-order-command", "type": "Command", "name": "CreateOrderCommand", "typeRef": { "type": "bool" },
              "stereotypes": { "Identified Command": { "Validated": false } },
              "children": [
                { "id": "create-order-command-user-id", "type": "DTO-Field", "name": "UserId", "typeRef": { "type": "string" } }
              ] },
            { "id": "checkout-accepted-handler", "type": "Integration Event Handler", "name": "UserCheckoutAcceptedIntegrationEventHandler" }
          ]
        },
        {
          "id": "eventing", "name": "Basket.Eventing", "type": "Eventing Package",
          "elements": [
            { "id": "checkout-accepted", "type": "Message", "name": "UserCheckoutAcceptedIntegrationEvent" }
          ]
        }
      ],
      "associations": [
        { "id": "handler-subscribes-checkout-accepted", "type": "Subscribe Integration Event",
          "source": { "element": "checkout-accepted-handler" }, "target": { "element": "checkout-accepted" } },
        { "id": "subscription-invokes-create-order", "type": "Perform Invocation",
          "source": { "element": "handler-subscribes-checkout-accepted" }, "target": { "element": "create-order-command" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.API/Application/IntegrationEvents/EventHandling/UserCheckoutAcceptedIntegrationEventHandler.cs
namespace eShop.Ordering.API.Application.IntegrationEvents.EventHandling;

public class UserCheckoutAcceptedIntegrationEventHandler(IMediator mediator) : IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>
{
    public async Task Handle(UserCheckoutAcceptedIntegrationEvent @event)
    {
        var createOrderCommand = new CreateOrderCommand(@event.UserId);
        await mediator.Send(createOrderCommand);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "invocation.wrapped", "element": "subscription-invokes-create-order", "absent": true }
  ],
  "findings": [
    { "rule": "claim/invocation.wrapped", "count": 0 }
  ]
}
```
