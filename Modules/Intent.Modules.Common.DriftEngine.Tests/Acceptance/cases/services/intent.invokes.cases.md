# intent.invokes

A `Perform Invocation` from an Operation (or a Command, or a handler subscription) to an application service's Operation or to a Command, and a front end's `Call Service Operation Action` to a Service Proxy's Operation, against the code: the source's handler calls the target, through an injected member of the target's type (`I<Name>` or the class), or sends the target Command through MediatR. The claim is the association's. `intent.invokes` is an existence kind arriving in phase 6: reaching the target means following parameters, holders, locals and handler classes, which needs the later packs and readers, so these cases need no outcome triple.

The fixtures adapt eShopCustom's `Basket.API` and `Ordering.API` endpoints, its `CreateOrderCommandHandler`, and its `WebApp` orders page.

## app-service-call

`GetBasketAsync` invokes the application service's `GetBasketAsync`, and the endpoint calls it through its `[FromServices] IBasketService` parameter. Traces ESC-SVC-37.

```json case
{ "phase": 6, "kinds": ["intent.invokes"], "traces": ["ESC-SVC-37"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "basket-service", "type": "Service", "name": "BasketService",
            "children": [
              { "id": "basket-service-get-basket", "type": "Operation", "name": "GetBasketAsync",
                "children": [
                  { "id": "basket-service-get-basket-buyer-id", "type": "Parameter", "name": "buyerId", "typeRef": { "type": "string" } }
                ] }
            ] },
          { "id": "basket-api", "type": "Service", "name": "BasketApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/basket" } },
            "children": [
              { "id": "basket-api-get-basket", "type": "Operation", "name": "GetBasketAsync",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{buyerId}" } },
                "children": [
                  { "id": "basket-api-get-basket-buyer-id", "type": "Parameter", "name": "buyerId", "typeRef": { "type": "string" } }
                ] }
            ] }
        ]
      }],
      "associations": [
        { "id": "get-basket-invokes-service", "type": "Perform Invocation",
          "source": { "element": "basket-api-get-basket" }, "target": { "element": "basket-service-get-basket" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Basket", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Basket.API/Apis/BasketApi.cs
namespace Basket.API.Apis;

public static class BasketApi
{
    public static RouteGroupBuilder MapBasketApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/basket");

        api.MapGet("{buyerId}", GetBasketAsync);

        return api;
    }

    public static async Task<Ok<CustomerBasket>> GetBasketAsync(string buyerId, [FromServices] IBasketService basketService)
    {
        var basket = await basketService.GetBasketAsync(buyerId);
        return TypedResults.Ok(basket);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "get-basket-invokes-service", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Basket.API/Apis/BasketApi.cs:16" }
  ]
}
```

## reached-through-a-method-passed-on

The action never calls `UserRoles.GetUsersRoles` itself: it passes it to a cache helper, `GetCacheData(key, _members.GetUsersRoles)`, to run on a miss. A method passed on is one that runs, so the code's own methods the action reaches include it, and its call of `KeycloakServiceCaller.GetKeycloakUsersRole` bears the invocation out.

```json case
{ "phase": 6, "kinds": ["intent.invokes"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basislink.API",
        "elements": [
          { "id": "keycloak", "type": "Service", "name": "KeycloakServiceCaller",
            "children": [
              { "id": "keycloak-get-users-role", "type": "Operation", "name": "GetKeycloakUsersRole", "typeRef": { "type": "string", "collection": true } }
            ] },
          { "id": "bid-service", "type": "Service", "name": "BidService",
            "stereotypes": { "Http Service Settings": { "Route": "api/bid" } },
            "children": [
              { "id": "bid-get-open-sell-bids", "type": "Operation", "name": "GetOpenSellBids", "typeRef": { "type": "string", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "sell" } } }
            ] }
        ]
      }],
      "associations": [
        { "id": "open-sell-bids-invokes-keycloak", "type": "Perform Invocation",
          "source": { "element": "bid-get-open-sell-bids" }, "target": { "element": "keycloak-get-users-role" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Basislink", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Basislink.API/Business/UserRoles.cs
namespace Basislink.API.Business;

public static class KeycloakServiceCaller
{
    public static List<string> GetKeycloakUsersRole() => new List<string>();
}

public interface ICache
{
    T GetCacheData<T>(string key, int seconds, Func<T> load);
}

public class UserRoles
{
    public List<string> GetUsersRoles() => KeycloakServiceCaller.GetKeycloakUsersRole();
}
```

```csharp file src/Basislink.API/Controllers/BidController.cs
using Basislink.API.Business;
using Microsoft.AspNetCore.Mvc;

namespace Basislink.API.Controllers;

[ApiController]
[Route("api/bid")]
public class BidController : ControllerBase
{
    private readonly ICache _cache;
    private readonly UserRoles _members = new UserRoles();

    public BidController(ICache cache)
    {
        _cache = cache;
    }

    [HttpGet("sell")]
    public IActionResult GetOpenSellBids()
    {
        var roles = _cache.GetCacheData<List<string>>("users", 60, _members.GetUsersRoles);
        return Ok(roles);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "open-sell-bids-invokes-keycloak", "outcome": "verified", "expected": "present", "actual": "present" }
  ]
}
```

## app-service-not-called

The endpoint reads the basket from the repository itself and never calls the application service, so the modelled invocation is contradicted. Traces ESC-SVC-37.

```json case
{ "phase": 6, "base": "app-service-call", "kinds": ["intent.invokes"], "traces": ["ESC-SVC-37"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```csharp file src/Basket.API/Apis/BasketApi.cs
namespace Basket.API.Apis;

public static class BasketApi
{
    public static RouteGroupBuilder MapBasketApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/basket");

        api.MapGet("{buyerId}", GetBasketAsync);

        return api;
    }

    public static async Task<Ok<CustomerBasket>> GetBasketAsync(string buyerId, [FromServices] IBasketRepository repository)
    {
        var basket = await repository.GetBasketAsync(buyerId);
        return TypedResults.Ok(basket);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "get-basket-invokes-service", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.invokes", "severity": "error", "subject": "get-basket-invokes-service", "messageContains": "isn't in the code" }
  ]
}
```

## command-sent-wrapped-in-a-local

As eShopCustom's `CancelOrderAsync` does, the endpoint wraps the Command in `IdentifiedCommand<CancelOrderCommand, bool>`, keeps it in a local, and sends the local through MediatR. Resolving the send's argument unwraps the identified command and follows the local to its assignment, so the invoked Command is `CancelOrderCommand`. MediatR's pack arrives `later`. Traces ESC-SVC-33 and ESC-SVC-35.

```json case
{ "phase": "later", "kinds": ["intent.invokes"], "traces": ["ESC-SVC-33", "ESC-SVC-35"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "intent.mediatr"], "language": "csharp" }
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
    { "kind": "intent.invokes", "element": "cancel-order-invokes-command", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Apis/OrdersApi.cs:20" }
  ]
}
```

## command-not-sent

The endpoint sends another Command, so the modelled invocation of `CancelOrderCommand` is contradicted. Traces ESC-SVC-33.

```json case
{ "phase": "later", "base": "command-sent-wrapped-in-a-local", "kinds": ["intent.invokes"], "traces": ["ESC-SVC-33"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "intent.mediatr"], "language": "csharp" }
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
        var requestShipOrder = new IdentifiedCommand<ShipOrderCommand, bool>(new ShipOrderCommand(command.OrderNumber), requestId);
        var commandResult = await services.Mediator.Send(requestShipOrder);

        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "cancel-order-invokes-command", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.invokes", "severity": "error", "subject": "cancel-order-invokes-command", "messageContains": "isn't in the code" }
  ]
}
```

## command-interactions-are-its-handlers

A Command has no code of its own to call anything: its interactions are those of its `IRequestHandler<CreateOrderCommand, bool>.Handle(CreateOrderCommand, ...)`. The Command invokes the integration event service's `AddAndSaveEventAsync`, and the handler calls it through its injected `IOrderingIntegrationEventService`. Traces ESC-SVC-41.

```json case
{ "phase": "later", "kinds": ["intent.invokes"], "traces": ["ESC-SVC-41"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "integration-event-service", "type": "Service", "name": "OrderingIntegrationEventService",
            "children": [
              { "id": "add-and-save-event", "type": "Operation", "name": "AddAndSaveEventAsync" }
            ] },
          { "id": "create-order-command", "type": "Command", "name": "CreateOrderCommand", "typeRef": { "type": "bool" },
            "children": [
              { "id": "create-order-command-user-id", "type": "DTO-Field", "name": "UserId", "typeRef": { "type": "string" } }
            ] }
        ]
      }],
      "associations": [
        { "id": "create-order-invokes-add-and-save", "type": "Perform Invocation",
          "source": { "element": "create-order-command" }, "target": { "element": "add-and-save-event" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.API/Application/Commands/CreateOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public class CreateOrderCommandHandler(
    IOrderingIntegrationEventService orderingIntegrationEventService,
    IOrderRepository orderRepository) : IRequestHandler<CreateOrderCommand, bool>
{
    public async Task<bool> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
    {
        var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(message.UserId);
        await orderingIntegrationEventService.AddAndSaveEventAsync(orderStartedIntegrationEvent);

        var order = new Order(message.UserId);
        orderRepository.Add(order);

        return await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "create-order-invokes-add-and-save", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/Commands/CreateOrderCommandHandler.cs:10" }
  ]
}
```

## blazor-page-calls-service-proxy

eShopCustom's orders page calls the ordering service proxy's `GetOrders` from `OnInitializedAsync`, through the member `@inject` gives it. The `intent.ui` pack gives the vocabulary for a `Call Service Operation Action`, and the Blazor reader reads only the calling Component Operation's method, where it finds the Operation called on the proxy's type (by name or `I<Name>`). Traces ESC-FE-35.

```json case
{ "phase": 6, "kinds": ["intent.invokes"], "traces": ["ESC-FE-35"], "requirements": ["R2", "R28", "R38"], "packs": ["intent.core", "intent.services", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp",
        "elements": [
          { "id": "ordering-service", "type": "Service Proxy", "name": "OrderingService",
            "children": [
              { "id": "ordering-service-get-orders", "type": "Operation", "name": "GetOrders" }
            ] },
          { "id": "pages", "type": "Folder", "name": "Pages",
            "children": [
              { "id": "orders-page", "type": "Component", "name": "Orders",
                "stereotypes": { "Page": { "Route": "/user/orders" } },
                "children": [
                  { "id": "orders-page-on-initialized", "type": "Component Operation", "name": "OnInitializedAsync" }
                ] }
            ] }
        ]
      }],
      "associations": [
        { "id": "orders-page-calls-get-orders", "type": "Call Service Operation Action",
          "source": { "element": "orders-page-on-initialized" }, "target": { "element": "ordering-service-get-orders" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "WebApp", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```razor file src/WebApp/Components/Pages/Orders.razor
@page "/user/orders"
@attribute [Authorize]
@inject OrderingService OrderingService

<PageTitle>Orders | AdventureWorks</PageTitle>

@code {
    private OrderRecord[]? orders;

    protected override async Task OnInitializedAsync()
    {
        orders = await OrderingService.GetOrders();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "orders-page-calls-get-orders", "outcome": "verified", "expected": "present", "actual": "present" }
  ]
}
```

## from-handler-subscription

An Integration Event Handler's handling of `OrderStatusChangedToPaidIntegrationEvent` invokes `WebhooksSender.SendAll`. The invocation is drawn from the subscription, an association end, not from an element, so it is only claimed when the engine collects the links that start at a link's end. The handler calls it through the injected interface.

```json case
{ "phase": 6, "kinds": ["intent.invokes"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [
        {
          "id": "services", "name": "Webhooks.API",
          "elements": [
            { "id": "paid-handler", "type": "Integration Event Handler", "name": "OrderPaidHandler" },
            { "id": "webhooks-sender", "type": "Service", "name": "WebhooksSender",
              "children": [
                { "id": "webhooks-sender-send-all", "type": "Operation", "name": "SendAll",
                  "children": [
                    { "id": "webhooks-sender-send-all-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "int" } }
                  ] }
              ] }
          ]
        },
        {
          "id": "eventing", "name": "Webhooks.Eventing", "type": "Eventing Package",
          "elements": [
            { "id": "status-changed-to-paid", "type": "Message", "name": "OrderStatusChangedToPaidIntegrationEvent" }
          ]
        }
      ],
      "associations": [
        { "id": "handler-subscribes-paid", "type": "Subscribe Integration Event",
          "source": { "element": "paid-handler" }, "target": { "element": "status-changed-to-paid" } },
        { "id": "subscription-invokes-send-all", "type": "Perform Invocation",
          "source": { "element": "handler-subscribes-paid" }, "target": { "element": "webhooks-sender-send-all" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Webhooks", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Webhooks.API/IntegrationEvents/OrderStatusChangedToPaidIntegrationEvent.cs
namespace Webhooks.API.IntegrationEvents;

public record OrderStatusChangedToPaidIntegrationEvent(int OrderId);
```

```csharp file src/Webhooks.API/Services/WebhooksSender.cs
namespace Webhooks.API.Services;

public interface IWebhooksSender
{
    Task SendAll(int orderId);
}

public class WebhooksSender : IWebhooksSender
{
    public Task SendAll(int orderId) => Task.CompletedTask;
}
```

```csharp file src/Webhooks.API/IntegrationEvents/OrderPaidHandler.cs
using Webhooks.API.Services;

namespace Webhooks.API.IntegrationEvents;

public class OrderPaidHandler(IWebhooksSender sender) : IHandleMessages<OrderStatusChangedToPaidIntegrationEvent>
{
    public async Task Handle(OrderStatusChangedToPaidIntegrationEvent message, IMessageHandlerContext context)
    {
        await sender.SendAll(message.OrderId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "subscription-invokes-send-all", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Webhooks.API/IntegrationEvents/OrderPaidHandler.cs:9" }
  ]
}
```

## from-handler-subscription-not-called

The handler no longer calls `SendAll`, so the invocation drawn from its subscription isn't in the code. The finding names the handler, not the subscription's end.

```json case
{ "phase": 6, "base": "from-handler-subscription", "kinds": ["intent.invokes"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Webhooks.API/IntegrationEvents/OrderPaidHandler.cs
using Webhooks.API.Services;

namespace Webhooks.API.IntegrationEvents;

public class OrderPaidHandler(IWebhooksSender sender) : IHandleMessages<OrderStatusChangedToPaidIntegrationEvent>
{
    public async Task Handle(OrderStatusChangedToPaidIntegrationEvent message, IMessageHandlerContext context)
    {
        await Task.CompletedTask;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "subscription-invokes-send-all", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.invokes", "severity": "error", "subject": "subscription-invokes-send-all", "messageContains": "`OrderPaidHandler`'s handling of `OrderStatusChangedToPaidIntegrationEvent` invokes `WebhooksSender.SendAll` in the model, but it isn't in the code" }
  ]
}
```

## minimal-api-lambda

`CreateSubscription` is a Minimal API endpoint whose handler is a lambda, so it is no method of the router class. Its invocation of `GrantUrlTesterService.TestGrantUrl` is read in the lambda's body.

```json case
{ "phase": 6, "kinds": ["intent.invokes"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Webhooks.API",
        "elements": [
          { "id": "grant-url-tester", "type": "Service", "name": "GrantUrlTesterService",
            "children": [
              { "id": "grant-url-tester-test", "type": "Operation", "name": "TestGrantUrl",
                "children": [
                  { "id": "grant-url-tester-test-url", "type": "Parameter", "name": "url", "typeRef": { "type": "string" } }
                ] }
            ] },
          { "id": "webhooks-api", "type": "Service", "name": "WebHooksApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/webhooks" } },
            "children": [
              { "id": "create-subscription", "type": "Operation", "name": "CreateSubscription",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "/" } } }
            ] }
        ]
      }],
      "associations": [
        { "id": "create-subscription-invokes-test", "type": "Perform Invocation",
          "source": { "element": "create-subscription" }, "target": { "element": "grant-url-tester-test" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Webhooks", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Webhooks.API/Services/GrantUrlTesterService.cs
namespace Webhooks.API.Services;

public interface IGrantUrlTesterService
{
    Task<bool> TestGrantUrl(string url);
}

public class GrantUrlTesterService : IGrantUrlTesterService
{
    public Task<bool> TestGrantUrl(string url) => Task.FromResult(true);
}
```

```csharp file src/Webhooks.API/Apis/WebHooksApi.cs
using Webhooks.API.Services;

namespace Webhooks.API.Apis;

public static class WebHooksApi
{
    public static RouteGroupBuilder MapWebHooksApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/webhooks");

        api.MapPost("/", async (string url, IGrantUrlTesterService grantUrlTester) =>
        {
            var granted = await grantUrlTester.TestGrantUrl(url);
            return granted ? Results.Ok() : Results.BadRequest();
        });

        return api;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "create-subscription-invokes-test", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Webhooks.API/Apis/WebHooksApi.cs:13" }
  ]
}
```

## vue-page-calls-proxy-operation

The standard User Interface designer (2.0.0): a Call Service Operation Action from a page, and one from a utility, to a Proxy's operations. The stock Vue reader follows the caller's code, a `<script setup>`'s top-level code included, and the code's own functions it calls, to the call of the proxy's method on the object the Proxy is located as: the catalog page calls `api.getProducts` itself, and the checkout utility `api.placeOrder`. The catalog page doesn't call `api.placeOrder`, so that link is contradicted.

```json case
{ "phase": "later", "kinds": ["intent.invokes", "intent.target"], "requirements": ["R2", "R3", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "pages", "children": [
            { "id": "catalog-page", "type": "Page", "name": "CatalogPage", "stereotypes": { "Page": { "Route": "/" } } }
          ] },
          { "id": "checkout-folder", "type": "Folder", "name": "checkout", "children": [
            { "id": "submit-checkout", "type": "Utility", "name": "SubmitCheckout" }
          ] },
          { "id": "api-folder", "type": "Folder", "name": "api", "children": [
            { "id": "api", "type": "Proxy", "name": "Api", "stereotypes": { "Implementation": { "Path": "api/client.ts" } }, "children": [
              { "id": "get-products", "type": "Proxy Operation", "name": "GetProducts" },
              { "id": "place-order", "type": "Proxy Operation", "name": "PlaceOrder" }
            ] }
          ] }
        ]
      }],
      "associations": [
        { "id": "catalog-calls-get-products", "type": "Call Service Operation Action", "source": { "element": "catalog-page" }, "target": { "element": "get-products" } },
        { "id": "submit-calls-place-order", "type": "Call Service Operation Action", "source": { "element": "submit-checkout" }, "target": { "element": "place-order" } },
        { "id": "catalog-calls-place-order", "type": "Call Service Operation Action", "source": { "element": "catalog-page" }, "target": { "element": "place-order" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```typescript file src/api/client.ts
export const api = {
    getProducts(): Promise<string[]> {
        return fetch('/api/products').then((x) => x.json());
    },

    placeOrder(): Promise<string> {
        return fetch('/api/orders', { method: 'POST' }).then((x) => x.text());
    },
};
```

```typescript file src/checkout/submitCheckout.ts
import { api } from '../api/client';

export async function submitCheckout(): Promise<string> {
    return await api.placeOrder();
}
```

```vue file src/pages/CatalogPage.vue
<script setup lang="ts">
import { ref } from 'vue';
import { api } from '../api/client';

const products = ref<string[]>([]);
api.getProducts().then((x) => { products.value = x; });
</script>
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "catalog-calls-get-products", "outcome": "verified", "at": "src/pages/CatalogPage.vue:6" },
    { "kind": "intent.target", "element": "catalog-calls-get-products", "outcome": "verified", "actual": "api" },
    { "kind": "intent.invokes", "element": "submit-calls-place-order", "outcome": "verified", "at": "src/checkout/submitCheckout.ts:4" },
    { "kind": "intent.invokes", "element": "catalog-calls-place-order", "outcome": "contradicted", "expected": "present" }
  ],
  "findings": [
    { "rule": "claim/intent.invokes", "severity": "error", "subject": "catalog-calls-place-order" }
  ]
}
```

## react-page-calls-proxy-operation

The same Call Service Operation Actions read against React by the stock React reader. It follows the component's code, its hooks and handlers included, and the code's own functions it calls, to the call of the proxy's method: the catalog page calls a custom hook, `useProducts`, whose effect calls `catalogApi.getProducts`; the product card calls `catalogApi.addToCart` from a click handler in its JSX. Two components in one file are each their own code, so the card's call isn't the page's, and the catalog page's Call Service Operation Action to `addToCart` is contradicted.

```json case
{ "phase": "later", "kinds": ["intent.invokes", "intent.target"], "requirements": ["R2", "R3", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "pages", "children": [
            { "id": "catalog-page", "type": "Page", "name": "CatalogPage", "stereotypes": { "Page": { "Route": "/" } } },
            { "id": "product-card", "type": "Component", "name": "ProductCard" }
          ] },
          { "id": "api-folder", "type": "Folder", "name": "api", "children": [
            { "id": "catalog-api", "type": "Proxy", "name": "CatalogApi", "children": [
              { "id": "get-products", "type": "Proxy Operation", "name": "GetProducts" },
              { "id": "add-to-cart", "type": "Proxy Operation", "name": "AddToCart" }
            ] }
          ] }
        ]
      }],
      "associations": [
        { "id": "catalog-calls-get-products", "type": "Call Service Operation Action", "source": { "element": "catalog-page" }, "target": { "element": "get-products" } },
        { "id": "card-calls-add-to-cart", "type": "Call Service Operation Action", "source": { "element": "product-card" }, "target": { "element": "add-to-cart" } },
        { "id": "catalog-calls-add-to-cart", "type": "Call Service Operation Action", "source": { "element": "catalog-page" }, "target": { "element": "add-to-cart" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```ts file src/api/catalogApi.ts
export const catalogApi = {
    getProducts(): Promise<string[]> {
        return fetch('/api/products').then((x) => x.json());
    },

    addToCart(id: string): Promise<void> {
        return fetch(`/api/cart/${id}`, { method: 'POST' }).then(() => undefined);
    },
};
```

```ts file src/hooks/useProducts.ts
import { useEffect, useState } from 'react';
import { catalogApi } from '../api/catalogApi';

export function useProducts() {
    const [products, setProducts] = useState<string[]>([]);
    useEffect(() => {
        catalogApi.getProducts().then(setProducts);
    }, []);
    return products;
}
```

```tsx file src/pages/CatalogPage.tsx
import { catalogApi } from '../api/catalogApi';
import { useProducts } from '../hooks/useProducts';

export function ProductCard({ id }: { id: string }) {
    return <button onClick={() => catalogApi.addToCart(id)}>Add {id}</button>;
}

export default function CatalogPage() {
    const products = useProducts();
    return <ul>{products.map((id) => <ProductCard key={id} id={id} />)}</ul>;
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "catalog-api", "outcome": "verified", "at": "src/api/catalogApi.ts:1" },
    { "kind": "intent.invokes", "element": "catalog-calls-get-products", "outcome": "verified", "at": "src/hooks/useProducts.ts:7" },
    { "kind": "intent.target", "element": "catalog-calls-get-products", "outcome": "verified", "actual": "catalogApi" },
    { "kind": "intent.invokes", "element": "card-calls-add-to-cart", "outcome": "verified", "at": "src/pages/CatalogPage.tsx:5" },
    { "kind": "intent.invokes", "element": "catalog-calls-add-to-cart", "outcome": "contradicted", "expected": "present" }
  ],
  "findings": [
    { "rule": "claim/intent.invokes", "severity": "error", "subject": "catalog-calls-add-to-cart" }
  ]
}
```

## domain-service-call

`PlaceOrder` invokes the Domain Service operation `PricingService.PriceOf`, and the application service calls it through its injected `IPricingService`, the interface beside the Domain Service's class.

```json case
{ "phase": 6, "kinds": ["intent.invokes"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.interactions"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order" },
          { "id": "pricing-service", "type": "Domain Service", "name": "PricingService",
            "children": [
              { "id": "pricing-service-price-of", "type": "Operation", "name": "PriceOf", "typeRef": { "type": "decimal" },
                "children": [
                  { "id": "pricing-service-price-of-order", "type": "Parameter", "name": "order", "typeRef": { "element": "order" } }
                ] }
            ] }
        ]
      }]
    },
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Application",
        "elements": [
          { "id": "order-service", "type": "Service", "name": "OrderService",
            "children": [
              { "id": "order-service-place-order", "type": "Operation", "name": "PlaceOrder", "typeRef": { "type": "decimal" } }
            ] }
        ]
      }],
      "associations": [
        { "id": "place-order-invokes-price-of", "type": "Perform Invocation",
          "source": { "element": "order-service-place-order" }, "target": { "element": "pricing-service-price-of" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
}
```

```csharp file src/Domain/Services/IPricingService.cs
namespace Shop.Domain.Services;

public interface IPricingService
{
    decimal PriceOf(Order order);
}
```

```csharp file src/Domain/Services/PricingService.cs
namespace Shop.Domain.Services;

public class PricingService : IPricingService
{
    public decimal PriceOf(Order order) => 0m;
}
```

```csharp file src/Application/OrderService.cs
using Shop.Domain;
using Shop.Domain.Services;

namespace Shop.Application;

public class OrderService
{
    private readonly IPricingService _pricing;

    public OrderService(IPricingService pricing)
    {
        _pricing = pricing;
    }

    public decimal PlaceOrder()
    {
        return _pricing.PriceOf(new Order());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "place-order-invokes-price-of", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Application/OrderService.cs:17" }
  ]
}
```

## domain-service-not-called

`PlaceOrder` prices the order itself and never calls the Domain Service, so the modelled invocation is contradicted.

```json case
{ "phase": 6, "base": "domain-service-call", "kinds": ["intent.invokes"], "requirements": ["R8"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.interactions"], "language": "csharp" }
```

```csharp file src/Application/OrderService.cs
using Shop.Domain;
using Shop.Domain.Services;

namespace Shop.Application;

public class OrderService
{
    public decimal PlaceOrder()
    {
        return 0m;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "place-order-invokes-price-of", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.invokes", "severity": "error", "subject": "place-order-invokes-price-of", "messageContains": "`PricingService.PriceOf`" }
  ]
}
```
