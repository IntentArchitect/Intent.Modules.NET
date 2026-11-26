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

The action never calls `UserRoles.GetUserRoles` itself: it passes it to a cache helper, `GetCacheData(key, _roles.GetUserRoles)`, to run on a miss. A method passed on is one that runs, so the code's own methods the action reaches include it, and its call of `IdentityServiceCaller.GetIdentityUserRoles` bears the invocation out.

```json case
{ "phase": 6, "kinds": ["intent.invokes"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Acme.API",
        "elements": [
          { "id": "identity", "type": "Service", "name": "IdentityServiceCaller",
            "children": [
              { "id": "identity-get-user-roles", "type": "Operation", "name": "GetIdentityUserRoles", "typeRef": { "type": "string", "collection": true } }
            ] },
          { "id": "bid-service", "type": "Service", "name": "BidService",
            "stereotypes": { "Http Service Settings": { "Route": "api/bid" } },
            "children": [
              { "id": "bid-get-open-bids", "type": "Operation", "name": "GetOpenBids", "typeRef": { "type": "string", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "open" } } }
            ] }
        ]
      }],
      "associations": [
        { "id": "open-bids-invokes-identity", "type": "Perform Invocation",
          "source": { "element": "bid-get-open-bids" }, "target": { "element": "identity-get-user-roles" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Acme", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Acme.API/Business/UserRoles.cs
namespace Acme.API.Business;

public static class IdentityServiceCaller
{
    public static List<string> GetIdentityUserRoles() => new List<string>();
}

public interface ICache
{
    T GetCacheData<T>(string key, int seconds, Func<T> load);
}

public class UserRoles
{
    public List<string> GetUserRoles() => IdentityServiceCaller.GetIdentityUserRoles();
}
```

```csharp file src/Acme.API/Controllers/BidController.cs
using Acme.API.Business;
using Microsoft.AspNetCore.Mvc;

namespace Acme.API.Controllers;

[ApiController]
[Route("api/bid")]
public class BidController : ControllerBase
{
    private readonly ICache _cache;
    private readonly UserRoles _roles = new UserRoles();

    public BidController(ICache cache)
    {
        _cache = cache;
    }

    [HttpGet("open")]
    public IActionResult GetOpenBids()
    {
        var roles = _cache.GetCacheData<List<string>>("users", 60, _roles.GetUserRoles);
        return Ok(roles);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "open-bids-invokes-identity", "outcome": "verified", "expected": "present", "actual": "present" }
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

## store-class-calls-proxy-operation

A Store whose code is a plain C# class, as a Blazor application's state usually is, calls a Proxy Operation. The User Interface designer gives a Store no member for its methods, so the Store is the whole class: its calls are those of every method and constructor, and of the code's own methods they reach. `BasketState.CheckoutAsync` clears the basket through a private helper, on the `IBasketService` interface the located `BasketService` implements.

```json case
{ "phase": "later", "kinds": ["intent.invokes", "intent.target"], "requirements": ["R2", "R28", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "basket-state", "type": "Store", "name": "BasketState" },
          { "id": "basket-service", "type": "Proxy", "name": "BasketService", "children": [
            { "id": "basket-service-delete-basket", "type": "Proxy Operation", "name": "DeleteBasketAsync" }
          ] }
        ]
      }],
      "associations": [
        { "id": "basket-state-deletes-basket", "type": "Call Service Operation Action", "source": { "element": "basket-state" }, "target": { "element": "basket-service-delete-basket" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/WebApp/Services/BasketState.cs
namespace eShop.WebApp.Services;

public class BasketState(IBasketService basketService)
{
    public async Task CheckoutAsync()
    {
        await ClearAsync();
    }

    private Task ClearAsync() => basketService.DeleteBasketAsync();
}
```

```csharp file src/WebApp/Services/BasketService.cs
namespace eShop.WebApp.Services;

public interface IBasketService
{
    Task DeleteBasketAsync();
}

public class BasketService(HttpClient httpClient) : IBasketService
{
    public Task DeleteBasketAsync() => httpClient.DeleteAsync("api/basket");
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "basket-state-deletes-basket", "outcome": "verified", "at": "src/WebApp/Services/BasketState.cs:10" },
    { "kind": "intent.target", "element": "basket-state-deletes-basket", "outcome": "verified", "at": "src/WebApp/Services/BasketState.cs:10" }
  ],
  "findings": [
    { "rule": "claim/intent.invokes", "count": 0 },
    { "rule": "claim/intent.target", "count": 0 }
  ]
}
```

## store-class-doesnt-call-proxy-operation

The Store no longer clears the basket: nothing in any of its methods, or the code's own methods they call, calls `DeleteBasketAsync`. One finding, on the invocation; the target waits on it.

```json case
{ "phase": "later", "base": "store-class-calls-proxy-operation", "kinds": ["intent.invokes", "intent.target"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/WebApp/Services/BasketState.cs
namespace eShop.WebApp.Services;

public class BasketState(IBasketService basketService)
{
    public Task CheckoutAsync() => Task.CompletedTask;
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "basket-state-deletes-basket", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/intent.invokes", "severity": "error", "subject": "basket-state-deletes-basket" },
    { "rule": "claim/intent.target", "count": 0 }
  ]
}
```

## blazor-form-posts-to-operation

eShop's webhook client logs out with a plain HTML form, which posts to the authentication endpoint with no proxy in between. The User Interface model has a Call Service Operation Action from the `UserMenu` component to the `Logout` Operation of another application's Services designer, whose Http Settings say `POST /logout`. A form with `method="post"` and no `@onsubmit` handler posts to its action, whose path is the operation's route, so it invokes the operation, and its target is the operation's service.

```json case
{ "phase": "later", "kinds": ["intent.invokes", "intent.target"], "requirements": ["R2", "R28", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebhookClient", "type": "User Interface Package",
        "elements": [
          { "id": "user-menu", "type": "Component", "name": "UserMenu" }
        ]
      }],
      "associations": [
        { "id": "user-menu-logs-out", "type": "Call Service Operation Action", "source": { "element": "user-menu" }, "target": { "element": "logout" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  },
  "references": {
    "Services": {
      "packages": [{
        "id": "hooks-services", "name": "WebhookClient.Services",
        "elements": [
          { "id": "authentication-endpoints", "type": "Service", "name": "AuthenticationEndpoints", "children": [
            { "id": "logout", "type": "Operation", "name": "Logout", "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "/logout" } } }
          ] }
        ]
      }]
    }
  }
}
```

```razor file src/WebhookClient/Components/Layout/UserMenu.razor
<AuthorizeView>
    <Authorized>
        <form method="post" action="logout">
            <AntiforgeryToken />
            <button class="action" type="submit">Log out</button>
        </form>
    </Authorized>
</AuthorizeView>
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "user-menu-logs-out", "outcome": "verified", "at": "src/WebhookClient/Components/Layout/UserMenu.razor:3" },
    { "kind": "intent.target", "element": "user-menu-logs-out", "outcome": "verified", "at": "src/WebhookClient/Components/Layout/UserMenu.razor:3" }
  ],
  "noOtherFindings": true
}
```

## blazor-form-posts-elsewhere

A planted drift: the form posts to `signout`. One finding, on the invocation, naming where the forms post; the target waits on it.

```json case
{ "phase": "later", "base": "blazor-form-posts-to-operation", "kinds": ["intent.invokes", "intent.target"], "requirements": ["R3", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebhookClient/Components/Layout/UserMenu.razor
<AuthorizeView>
    <Authorized>
        <form method="post" action="signout">
            <AntiforgeryToken />
            <button class="action" type="submit">Log out</button>
        </form>
    </Authorized>
</AuthorizeView>
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "user-menu-logs-out", "outcome": "contradicted", "actual": "signout", "at": "src/WebhookClient/Components/Layout/UserMenu.razor:3" }
  ],
  "findings": [
    { "rule": "claim/intent.invokes", "severity": "error", "subject": "user-menu-logs-out", "messageContains": "`UserMenu` invokes `Logout`: in the model, not in the code. `Logout` is `POST logout`, and `UserMenu` posts its forms to `signout`." }
  ],
  "noOtherFindings": true
}
```

## blazor-form-posts-to-a-get-operation

The form posts to the operation's route, but the operation answers GET, and a form sends a POST: one finding.

```json case
{ "phase": "later", "base": "blazor-form-posts-to-operation", "kinds": ["intent.invokes"], "requirements": ["R3", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebhookClient", "type": "User Interface Package",
        "elements": [
          { "id": "user-menu", "type": "Component", "name": "UserMenu" }
        ]
      }],
      "associations": [
        { "id": "user-menu-logs-out", "type": "Call Service Operation Action", "source": { "element": "user-menu" }, "target": { "element": "logout" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  },
  "references": {
    "Services": {
      "packages": [{
        "id": "hooks-services", "name": "WebhookClient.Services",
        "elements": [
          { "id": "authentication-endpoints", "type": "Service", "name": "AuthenticationEndpoints", "stereotypes": { "Http Service Settings": { "Route": "account" } }, "children": [
            { "id": "logout", "type": "Operation", "name": "Logout", "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "logout" } } }
          ] }
        ]
      }]
    }
  }
}
```

```razor file src/WebhookClient/Components/Layout/UserMenu.razor
<AuthorizeView>
    <Authorized>
        <form method="post" action="/account/logout">
            <button class="action" type="submit">Log out</button>
        </form>
    </Authorized>
</AuthorizeView>
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "user-menu-logs-out", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/intent.invokes", "severity": "error", "subject": "user-menu-logs-out", "messageContains": "but `Logout` is a `GET` endpoint, and a form sends a POST." }
  ]
}
```

## blazor-form-with-handler-posts-nothing

What isn't a plain post states nothing: a form with an `@onsubmit` handler is handled by the component itself, its action only a fallback, so it doesn't invoke the operation, and with nothing else calling it the component's silence says it doesn't.

```json case
{ "phase": "later", "base": "blazor-form-posts-to-operation", "kinds": ["intent.invokes"], "requirements": ["R3", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebhookClient/Components/Layout/UserMenu.razor
<AuthorizeView>
    <Authorized>
        <form method="post" action="logout" @onsubmit="SignOutAsync" @formname="logout">
            <AntiforgeryToken />
            <button class="action" type="submit">Log out</button>
        </form>
    </Authorized>
</AuthorizeView>

@code {
    private Task SignOutAsync() => Task.CompletedTask;
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "user-menu-logs-out", "outcome": "contradicted", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/intent.invokes", "severity": "error", "subject": "user-menu-logs-out" }
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

## from-handler-subscription-through-a-global-qualified-factory

The handler gets the sender from a factory whose method returns `global::Webhooks.API.Services.IWebhooksSender`, as generated code often writes it. `global::` names the global namespace, so the call is made on `IWebhooksSender`, which `WebhooksSender` implements. A migrated application's v1-clean model invoked each of its file processors this way, and all eight read as missing while the qualifier stayed in the type's name.

```json case
{ "phase": 6, "base": "from-handler-subscription", "kinds": ["intent.invokes"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
```

```csharp file src/Webhooks.API/Services/IWebhooksSenderFactory.cs
namespace Webhooks.API.Services;

public interface IWebhooksSenderFactory
{
    global::Webhooks.API.Services.IWebhooksSender Create(int orderId);
}
```

```csharp file src/Webhooks.API/IntegrationEvents/OrderPaidHandler.cs
using Webhooks.API.Services;

namespace Webhooks.API.IntegrationEvents;

public class OrderPaidHandler(IWebhooksSenderFactory senders) : IHandleMessages<OrderStatusChangedToPaidIntegrationEvent>
{
    public async Task Handle(OrderStatusChangedToPaidIntegrationEvent message, IMessageHandlerContext context)
    {
        var sender = senders.Create(message.OrderId);
        await sender.SendAll(message.OrderId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "subscription-invokes-send-all", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Webhooks.API/IntegrationEvents/OrderPaidHandler.cs:10" }
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

## invoked-through-a-cast-local

`GetOrder` invokes `AcmeDataEntities.spGetOrder`, and the logic class reaches it through its `DbContext` field cast to the code's own EF6 context, `var context = (AcmeDataEntities)_context;`. A downcast is the type it names, so `context.spGetOrder(id)` calls `AcmeDataEntities`' method, not one of `DbContext`'s.

```json case
{ "phase": 6, "kinds": ["intent.invokes"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.interactions"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Acme.Crm",
        "elements": [
          { "id": "acme-data-entities", "type": "Service", "name": "AcmeDataEntities",
            "children": [
              { "id": "acme-data-entities-sp-get-order", "type": "Operation", "name": "spGetOrder",
                "children": [
                  { "id": "acme-data-entities-sp-get-order-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
                ] }
            ] },
          { "id": "order-logic", "type": "Service", "name": "OrderLogic",
            "children": [
              { "id": "order-logic-get-order", "type": "Operation", "name": "GetOrder",
                "children": [
                  { "id": "order-logic-get-order-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }],
      "associations": [
        { "id": "get-order-invokes-sp-get-order", "type": "Perform Invocation",
          "source": { "element": "order-logic-get-order" }, "target": { "element": "acme-data-entities-sp-get-order" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Acme", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Acme.Crm/Data/AcmeDataEntities.cs
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace Acme.Crm.Data;

public partial class AcmeDataEntities : DbContext
{
    public virtual ObjectResult<spGetOrder_Result> spGetOrder(int id) => null!;
}

public class spGetOrder_Result
{
    public decimal Total { get; set; }
}
```

```csharp file src/Acme.Crm/Logic/OrderLogic.cs
using System.Data.Entity;
using System.Linq;
using Acme.Crm.Data;

namespace Acme.Crm.Logic;

public class OrderLogic
{
    private readonly DbContext _context;

    public OrderLogic(DbContext context)
    {
        _context = context;
    }

    public spGetOrder_Result GetOrder(int id)
    {
        var context = (AcmeDataEntities)_context;
        return context.spGetOrder(id).FirstOrDefault();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "get-order-invokes-sp-get-order", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Acme.Crm/Logic/OrderLogic.cs:19" }
  ]
}
```

## mvc-forms-post-to-operations

eShop's Identity site posts its MVC views' forms to the controllers' actions, which the Services model has as Operations with Http Settings. The MVC reader (`intent.mvc`) reads where each form goes: one the form tag helper builds (any `asp-` attribute) posts unless its `method` says otherwise, to `asp-controller`/`asp-action` (each defaulting to the action that rendered the view: the grants page's `asp-action="Revoke"` is `GrantsController.Revoke`), or to the route `asp-route` names, and with no action's route of that name, to the URL the view was rendered at, as the tag helper writes no action of its own (the login form reaches the POST `Login` at `/Account/Login`); a plain form to its `action`'s path. The form must reach the Operation's action with its Http Settings Verb, and its target is then the Operation's service.

```json case
{ "phase": "later", "kinds": ["intent.invokes", "intent.target"], "requirements": ["R2", "R28", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Identity.API", "type": "User Interface Package",
        "elements": [
          { "id": "login", "type": "Page", "name": "Login", "stereotypes": { "Page": { "Route": "/Account/Login" } } },
          { "id": "grants", "type": "Page", "name": "Index", "stereotypes": { "Page": { "Route": "/Grants" } } }
        ]
      }],
      "associations": [
        { "id": "login-posts-login", "type": "Call Service Operation Action", "source": { "element": "login" }, "target": { "element": "account-login" } },
        { "id": "login-posts-register", "type": "Call Service Operation Action", "source": { "element": "login" }, "target": { "element": "account-register" } },
        { "id": "grants-posts-revoke", "type": "Call Service Operation Action", "source": { "element": "grants" }, "target": { "element": "grants-revoke" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  },
  "references": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Identity.API.Services",
        "elements": [
          { "id": "account-controller", "type": "Service", "name": "AccountController", "children": [
            { "id": "account-login", "type": "Operation", "name": "Login", "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "~/Account/Login" } } },
            { "id": "account-register", "type": "Operation", "name": "Register", "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "~/Account/Register" } } }
          ] },
          { "id": "grants-controller", "type": "Service", "name": "GrantsController", "children": [
            { "id": "grants-revoke", "type": "Operation", "name": "Revoke", "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "~/Grants/Revoke" } } }
          ] }
        ]
      }]
    }
  }
}
```

```xml file src/Identity.API/Identity.API.csproj
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
```

```csharp file src/Identity.API/Program.cs
var app = WebApplication.CreateBuilder(args).Build();
app.MapDefaultControllerRoute();
app.Run();
```

```csharp file src/Identity.API/Quickstart/AccountController.cs
namespace Identity.API.Quickstart;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login(string returnUrl) => View();

    [HttpPost]
    public IActionResult Login(LoginInputModel model) => View();

    [HttpPost]
    public IActionResult Register(RegisterInputModel model) => RedirectToAction("Login");
}

public class LoginInputModel { }

public class RegisterInputModel { }
```

```csharp file src/Identity.API/Quickstart/GrantsController.cs
namespace Identity.API.Quickstart;

public class GrantsController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult Revoke(string clientId) => RedirectToAction("Index");
}
```

```cshtml file src/Identity.API/Views/Account/Login.cshtml
<form asp-route="Login">
    <button name="button" value="login">Login</button>
</form>
<form method="post" action="/Account/Register">
    <button>Register</button>
</form>
```

```cshtml file src/Identity.API/Views/Grants/Index.cshtml
<form asp-action="Revoke">
    <input type="hidden" name="clientId" value="@client.ClientId" />
    <button class="btn btn-danger">Revoke Access</button>
</form>
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "login-posts-login", "outcome": "verified", "at": "src/Identity.API/Views/Account/Login.cshtml:1" },
    { "kind": "intent.target", "element": "login-posts-login", "outcome": "verified", "at": "src/Identity.API/Views/Account/Login.cshtml:1" },
    { "kind": "intent.invokes", "element": "login-posts-register", "outcome": "verified", "at": "src/Identity.API/Views/Account/Login.cshtml:4" },
    { "kind": "intent.invokes", "element": "grants-posts-revoke", "outcome": "verified", "at": "src/Identity.API/Views/Grants/Index.cshtml:1" }
  ],
  "noOtherFindings": true
}
```

## mvc-form-posts-elsewhere

A planted drift: the login form now names `asp-action="Logout"`, an action that isn't there. One finding, on the invocation, naming what each form reaches; the target waits on it.

```json case
{ "phase": "later", "base": "mvc-forms-post-to-operations", "kinds": ["intent.invokes", "intent.target"], "requirements": ["R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```cshtml file src/Identity.API/Views/Account/Login.cshtml
<form asp-action="Logout">
    <button name="button" value="login">Login</button>
</form>
<form method="post" action="/Account/Register">
    <button>Register</button>
</form>
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "login-posts-login", "outcome": "contradicted" },
    { "kind": "intent.invokes", "element": "login-posts-register", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/intent.invokes", "severity": "error", "subject": "login-posts-login", "messageContains": "`Login` invokes `AccountController.Login` (POST): in the model, not in the code. `src/Identity.API/Views/Account/Login.cshtml` posts `asp-action=\"Logout\"` (POST no action), `action=\"/Account/Register\"` (POST `AccountController.Register`)." }
  ],
  "noOtherFindings": true
}
```

## mvc-plain-form-gets

A plain form with no `method` sends a GET, whatever its action: the register form no longer says `method="post"`, so it reaches no action that takes the POST the Operation's Http Settings give, and the invocation is contradicted though its action's path is the Operation's route.

```json case
{ "phase": "later", "base": "mvc-forms-post-to-operations", "kinds": ["intent.invokes"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```cshtml file src/Identity.API/Views/Account/Login.cshtml
<form asp-route="Login">
    <button name="button" value="login">Login</button>
</form>
<form action="/Account/Register">
    <button>Register</button>
</form>
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "login-posts-register", "outcome": "contradicted" },
    { "kind": "intent.invokes", "element": "login-posts-login", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/intent.invokes", "severity": "error", "subject": "login-posts-register", "messageContains": "`action=\"/Account/Register\"` (GET no action)" }
  ],
  "noOtherFindings": true
}
```

## mvc-html-helper-forms

The same forms written with the HTML helpers, as ASP.NET MVC views often are: `Html.BeginForm("Login", "Account", FormMethod.Post)` posts to the action and controller it names, `Html.BeginForm("Revoke")` to that action of the controller that rendered the view, and a plain form's action may be the URL helper's (`action="@Url.Action("Register", "Account")"`).

```json case
{ "phase": "later", "base": "mvc-forms-post-to-operations", "kinds": ["intent.invokes"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```cshtml file src/Identity.API/Views/Account/Login.cshtml
@using (Html.BeginForm("Login", "Account", FormMethod.Post))
{
    <button name="button" value="login">Login</button>
}
<form method="post" action="@Url.Action("Register", "Account")">
    <button>Register</button>
</form>
```

```cshtml file src/Identity.API/Views/Grants/Index.cshtml
@using (Html.BeginForm("Revoke"))
{
    <button class="btn btn-danger">Revoke Access</button>
}
```

```json expect
{
  "claims": [
    { "kind": "intent.invokes", "element": "login-posts-login", "outcome": "verified", "at": "src/Identity.API/Views/Account/Login.cshtml:1" },
    { "kind": "intent.invokes", "element": "login-posts-register", "outcome": "verified", "at": "src/Identity.API/Views/Account/Login.cshtml:5" },
    { "kind": "intent.invokes", "element": "grants-posts-revoke", "outcome": "verified", "at": "src/Identity.API/Views/Grants/Index.cshtml:1" }
  ],
  "noOtherFindings": true
}
```
