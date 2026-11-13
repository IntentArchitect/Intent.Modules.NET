# optin/unmodelled.interaction

The opt-in rule that reports a call the model's interactions can state that no `intent.*` claim located (rules.md, "Opt-in must be modelled rules"): a domain event raised with no Domain Event Origin, a request sent with no Perform Invocation, a component rendered with no Composition. eShopCustom reported these as Backlog. By principle 1 the rule is off until the Root Folder's `Drift Check` lists it under `Opt-in Rules`; a modelled interaction's own claim locates its call, so only the calls the model lacks are reported.

A pack's `interactions` conventions say which calls count. No stock pack has eShop's, so each case brings a house pack. A finding's subject is the file and the line of the call, or of the markup element for a rendered component or a link.

## off-by-default

`Order`'s constructor raises `OrderStartedDomainEvent`, and the model has no Domain Event Origin for it. The house convention finds every `AddDomainEvent(new X(...))`, but no `Opt-in Rules` row turns the rule on, so nothing is reported (principle 1).

```json case
{ "phase": 2, "cases": ["optin/unmodelled.interaction"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-ctor", "type": "Class Constructor", "name": "Order",
                "children": [
                  { "id": "order-ctor-user-id", "type": "Parameter", "name": "userId", "typeRef": { "type": "string" } }
                ] }
            ] },
          { "id": "order-started", "type": "Domain Event", "name": "OrderStartedDomainEvent",
            "children": [
              { "id": "order-started-user-id", "type": "Property", "name": "UserId", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
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
  "id": "house.ordering",
  "version": "1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "conventions": {
    "Domain": {
      "interactions": [
        { "kind": "raise", "find": "type > call with \\bAddDomainEvent\\(new (\\w+)\\(", "name": "{Class} raises $1", "files": "src/Domain/**/*.cs", "note": "SeedWork's AddDomainEvent with a new event" }
      ]
    }
  }
}
```

```csharp file src/Domain/OrderAggregate/Order.cs
namespace Ordering.Domain.OrderAggregate;

public class Order : Entity, IAggregateRoot
{
    public Order(string userId)
    {
        AddDomainEvent(new OrderStartedDomainEvent(userId));
    }
}
```

```csharp file src/Domain/Events/OrderStartedDomainEvent.cs
namespace Ordering.Domain.Events;

public record OrderStartedDomainEvent(string UserId) : INotification;
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.interaction", "count": 0, "note": "The rule is off" }
  ]
}
```

## on

The same model and code, with the rule on. The raise is a call a Domain Event Origin can state, and no `intent.raises` claim located it, so it is reported, an error by default (`event.origin` Backlog in eShopCustom).

```json case
{ "phase": 2, "base": "off-by-default", "cases": ["optin/unmodelled.interaction"], "traces": ["ESC-DOM-31"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-ctor", "type": "Class Constructor", "name": "Order",
                "children": [
                  { "id": "order-ctor-user-id", "type": "Parameter", "name": "userId", "typeRef": { "type": "string" } }
                ] }
            ] },
          { "id": "order-started", "type": "Domain Event", "name": "OrderStartedDomainEvent",
            "children": [
              { "id": "order-started-user-id", "type": "Property", "name": "UserId", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.interaction" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.interaction", "severity": "error", "subject": "src/Domain/OrderAggregate/Order.cs:7", "messageContains": "Order raises OrderStartedDomainEvent" }
  ]
}
```

## raise-modelled

The model links the constructor to the event with a Domain Event Origin. Its `intent.raises` claim reads the raise, which locates the call, so the rule reports nothing.

```json case
{ "phase": 6, "base": "on", "kinds": ["intent.raises"], "cases": ["optin/unmodelled.interaction"], "traces": ["ESC-DOM-31"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-ctor", "type": "Class Constructor", "name": "Order",
                "children": [
                  { "id": "order-ctor-user-id", "type": "Parameter", "name": "userId", "typeRef": { "type": "string" } }
                ] }
            ] },
          { "id": "order-started", "type": "Domain Event", "name": "OrderStartedDomainEvent",
            "children": [
              { "id": "order-started-user-id", "type": "Property", "name": "UserId", "typeRef": { "type": "string" } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-ctor-raises-order-started", "type": "Domain Event Origin Association",
          "source": { "element": "order-ctor" }, "target": { "element": "order-started" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.interaction" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.raises", "element": "order-ctor-raises-order-started", "outcome": "verified", "at": "src/Domain/OrderAggregate/Order.cs:7" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.interaction", "subject": "src/Domain/OrderAggregate/Order.cs:7", "count": 0, "note": "Located by the raise's claim" }
  ]
}
```

## excepted-by-finding-name

An `Exceptions` row can match the name a finding gives, here the convention's `{Class} raises $1`, with a reason. The raise isn't reported, and the exception matched, so it isn't stale.

```json case
{ "phase": 2, "base": "off-by-default", "cases": ["optin/unmodelled.interaction"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-ctor", "type": "Class Constructor", "name": "Order",
                "children": [
                  { "id": "order-ctor-user-id", "type": "Parameter", "name": "userId", "typeRef": { "type": "string" } }
                ] }
            ] },
          { "id": "order-started", "type": "Domain Event", "name": "OrderStartedDomainEvent",
            "children": [
              { "id": "order-started-user-id", "type": "Property", "name": "UserId", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": {
            "Opt-in Rules": [ { "Rule": "optin/unmodelled.interaction" } ],
            "Exceptions": [ { "Rule": "optin/unmodelled.interaction", "Match": "Order raises OrderStartedDomainEvent", "Reason": "Raised for the audit log only; no handler in this application depends on it" } ]
          }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.interaction", "subject": "src/Domain/OrderAggregate/Order.cs:7", "count": 0, "note": "Excepted" },
    { "rule": "optin/staleException", "count": 0 }
  ]
}
```

## handler-calls

Three calls in handlers that the model's interactions could state and don't: an integration event handler sends a command with no Perform Invocation, a query handler calls an application service with no Perform Invocation, and a domain event handler publishes an integration event with no Publish Integration Event. Each is a convention of its own, and each is reported. The handlers and the command are modelled; only the links are missing.

```json case
{ "phase": 2, "cases": ["optin/unmodelled.interaction"], "traces": ["ESC-SVC-34", "ESC-SVC-38", "ESC-SVC-40"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "set-awaiting-validation", "type": "Command", "name": "SetAwaitingValidationOrderStatusCommand",
            "children": [
              { "id": "set-awaiting-validation-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "get-my-orders", "type": "Query", "name": "GetMyOrdersQuery",
            "children": [
              { "id": "get-my-orders-user-id", "type": "DTO-Field", "name": "UserId", "typeRef": { "type": "string" } }
            ] },
          { "id": "order-queries", "type": "Service", "name": "OrderQueries", "stereotypes": { "Application Service": {} } },
          { "id": "grace-period-confirmed-handler", "type": "Integration Event Handler", "name": "GracePeriodConfirmedIntegrationEventHandler" },
          { "id": "order-cancelled-handler", "type": "Domain Event Handler", "name": "OrderCancelledDomainEventHandler" }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.interaction" } ] }
        } }]
    }
  }
}
```

```json pack
{
  "id": "house.ordering",
  "version": "1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "interactions": [
        { "kind": "send", "find": "type > method > call with \\bmediator\\.Send\\(", "name": "{Class}.{Method} sends a request", "files": "src/Ordering.API/Application/**/*.cs" },
        { "kind": "application service call", "find": "type > method > call with \\b\\w*[Qq]ueries\\.(\\w+)\\(", "name": "{Class}.{Method} calls OrderQueries.$1", "files": "src/Ordering.API/Application/**/*.cs" },
        { "kind": "publish", "find": "type > method > call with \\bAddAndSaveEventAsync\\(", "name": "{Class}.{Method} publishes an integration event", "files": "src/Ordering.API/Application/**/*.cs" }
      ]
    }
  }
}
```

```csharp file src/Ordering.API/Application/IntegrationEvents/EventHandling/GracePeriodConfirmedIntegrationEventHandler.cs
namespace Ordering.API.Application.IntegrationEvents.EventHandling;

public class GracePeriodConfirmedIntegrationEventHandler(IMediator mediator) : IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>
{
    public async Task Handle(GracePeriodConfirmedIntegrationEvent @event)
    {
        var command = new SetAwaitingValidationOrderStatusCommand(@event.OrderId);
        await mediator.Send(command);
    }
}
```

```csharp file src/Ordering.API/Application/Queries/GetMyOrdersQueryHandler.cs
namespace Ordering.API.Application.Queries;

public class GetMyOrdersQueryHandler(IOrderQueries orderQueries) : IRequestHandler<GetMyOrdersQuery, IEnumerable<OrderSummary>>
{
    public async Task<IEnumerable<OrderSummary>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        return await orderQueries.GetOrdersFromUserAsync(request.UserId);
    }
}
```

```csharp file src/Ordering.API/Application/DomainEventHandlers/OrderCancelledDomainEventHandler.cs
namespace Ordering.API.Application.DomainEventHandlers;

public class OrderCancelledDomainEventHandler(IOrderingIntegrationEventService orderingIntegrationEventService) : INotificationHandler<OrderCancelledDomainEvent>
{
    public async Task Handle(OrderCancelledDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var integrationEvent = new OrderStatusChangedToCancelledIntegrationEvent(domainEvent.Order.Id);
        await orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.interaction", "severity": "error",
      "subject": "src/Ordering.API/Application/IntegrationEvents/EventHandling/GracePeriodConfirmedIntegrationEventHandler.cs:8",
      "messageContains": "GracePeriodConfirmedIntegrationEventHandler.Handle sends a request" },
    { "rule": "optin/unmodelled.interaction", "severity": "error",
      "subject": "src/Ordering.API/Application/Queries/GetMyOrdersQueryHandler.cs:7",
      "messageContains": "GetMyOrdersQueryHandler.Handle calls OrderQueries.GetOrdersFromUserAsync" },
    { "rule": "optin/unmodelled.interaction", "severity": "error",
      "subject": "src/Ordering.API/Application/DomainEventHandlers/OrderCancelledDomainEventHandler.cs:8",
      "messageContains": "OrderCancelledDomainEventHandler.Handle publishes an integration event" }
  ]
}
```

## component-renders

A page renders two components, neither through a Composition. `CatalogListItem` is modelled, so the model could state the Composition (`composition.unmodelled`, Backlog); `CatalogSearch` isn't modelled yet (`composition.not-modelled`, a note in eShopCustom). Both are reported, as errors by default: the engine's opt-in findings have one default severity, and a team that wants either as a note says so with a `Severity Overrides` row. The convention matches a markup element whose tag is PascalCase, a component.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.interaction"], "traces": ["ESC-FE-28", "ESC-FE-29"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui", "house.webapp"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp",
        "elements": [
          { "id": "catalog", "type": "Component", "name": "Catalog", "stereotypes": { "Page": { "Route": "/" } } },
          { "id": "catalog-list-item", "type": "Component", "name": "CatalogListItem" },
          { "id": "cart", "type": "Component", "name": "CartPage", "stereotypes": { "Page": { "Route": "/cart" } } },
          { "id": "catalog-service", "type": "Service Proxy", "name": "CatalogService",
            "children": [
              { "id": "catalog-service-get-items", "type": "Operation", "name": "GetCatalogItems" }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.interaction" } ] }
        } }]
    }
  }
}
```

```json pack
{
  "id": "house.webapp",
  "version": "1.0.0",
  "requires": [{ "id": "intent.ui", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "conventions": {
    "User Interface": {
      "interactions": [
        { "kind": "render", "find": "type > markup with ^<([A-Z]\\w*)", "name": "{Class} renders $1", "files": "src/WebApp/Components/**/*.razor" }
      ]
    }
  }
}
```

```razor file src/WebApp/Components/Pages/Catalog/Catalog.razor
@page "/"
@inject CatalogService CatalogService
@inject BasketState BasketState
@inject NavigationManager Nav

<div class="catalog">
    <CatalogSearch BrandId="@BrandId" />
    <div class="catalog-items">
        @foreach (var item in catalogResult.Data)
        {
            <CatalogListItem Item="@item" />
        }
    </div>
    <a class="catalog-cart-link" href="cart">View bag (@basketCount)</a>
</div>

@code {
    [SupplyParameterFromQuery] public int? BrandId { get; set; }

    CatalogResult catalogResult = new([]);
    int basketCount;

    protected override async Task OnInitializedAsync()
    {
        catalogResult = await CatalogService.GetCatalogItems(0, 9, BrandId);
        basketCount = (await BasketState.GetBasketItemsAsync()).Count;
    }

    private void ShowReturns() => Nav.NavigateTo("returns");
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.interaction", "severity": "error", "subject": "src/WebApp/Components/Pages/Catalog/Catalog.razor:11", "messageContains": "Catalog renders CatalogListItem" },
    { "rule": "optin/unmodelled.interaction", "severity": "error", "subject": "src/WebApp/Components/Pages/Catalog/Catalog.razor:7", "messageContains": "Catalog renders CatalogSearch" },
    { "rule": "optin/unmodelled.interaction", "subject": "src/WebApp/Components/Pages/Catalog/Catalog.razor:6", "count": 0, "note": "A lowercase tag is HTML, not a component" }
  ]
}
```

## composition-modelled

The model adds the Composition from `Catalog` to `CatalogListItem`. Its `intent.uses` claim reads the `<CatalogListItem>` element, which locates it, so only `CatalogSearch` is still reported.

```json case
{ "phase": 6, "base": "component-renders", "kinds": ["intent.uses"], "cases": ["optin/unmodelled.interaction"], "traces": ["ESC-FE-28"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui", "house.webapp"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp",
        "elements": [
          { "id": "catalog", "type": "Component", "name": "Catalog", "stereotypes": { "Page": { "Route": "/" } } },
          { "id": "catalog-list-item", "type": "Component", "name": "CatalogListItem" },
          { "id": "cart", "type": "Component", "name": "CartPage", "stereotypes": { "Page": { "Route": "/cart" } } },
          { "id": "catalog-service", "type": "Service Proxy", "name": "CatalogService",
            "children": [
              { "id": "catalog-service-get-items", "type": "Operation", "name": "GetCatalogItems" }
            ] }
        ]
      }],
      "associations": [
        { "id": "catalog-renders-list-item", "type": "Composition",
          "source": { "element": "catalog" }, "target": { "element": "catalog-list-item" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.interaction" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "catalog-renders-list-item", "outcome": "verified", "at": "src/WebApp/Components/Pages/Catalog/Catalog.razor:11" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.interaction", "subject": "src/WebApp/Components/Pages/Catalog/Catalog.razor:11", "count": 0, "note": "Located by the Composition's claim" },
    { "rule": "optin/unmodelled.interaction", "severity": "error", "subject": "src/WebApp/Components/Pages/Catalog/Catalog.razor:7", "messageContains": "Catalog renders CatalogSearch" }
  ]
}
```

## component-navigates

The page links to `cart`, a modelled page's route, with no Navigation (`navigation.unmodelled`), and navigates to `returns`, which no modelled page serves (`navigation.not-modelled`). The convention finds an `href` on a link and a `NavigateTo` call; both are reported.

```json case
{ "phase": 6, "base": "component-renders", "cases": ["optin/unmodelled.interaction"], "traces": ["ESC-FE-33", "ESC-FE-34"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui", "house.webapp"], "language": "csharp" }
```

```json pack
{
  "id": "house.webapp",
  "version": "1.0.0",
  "requires": [{ "id": "intent.ui", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "conventions": {
    "User Interface": {
      "interactions": [
        { "kind": "navigate", "find": ["type > markup with ^<a\\b[^>]*\\bhref=\"([^\"]*)\"", "type > call with \\bNavigateTo\\(\"([^\"]*)\""], "name": "{Class} navigates to $1", "files": "src/WebApp/Components/**/*.razor" }
      ]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.interaction", "severity": "error", "subject": "src/WebApp/Components/Pages/Catalog/Catalog.razor:14", "messageContains": "Catalog navigates to cart" },
    { "rule": "optin/unmodelled.interaction", "severity": "error", "subject": "src/WebApp/Components/Pages/Catalog/Catalog.razor:29", "messageContains": "Catalog navigates to returns" }
  ]
}
```

## component-calls

The page calls `CatalogService.GetCatalogItems`, an Operation of a modelled Service Proxy, with no Call (`call.unmodelled`), and `BasketState.GetBasketItemsAsync`, through a client the model doesn't have (`call.not-modelled`). The convention finds a call through an injected service or state member; both are reported.

```json case
{ "phase": 6, "base": "component-renders", "cases": ["optin/unmodelled.interaction"], "traces": ["ESC-FE-36", "ESC-FE-37"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui", "house.webapp"], "language": "csharp" }
```

```json pack
{
  "id": "house.webapp",
  "version": "1.0.0",
  "requires": [{ "id": "intent.ui", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "conventions": {
    "User Interface": {
      "interactions": [
        { "kind": "call", "find": "type > call with \\b(\\w+(?:Service|State))\\.(\\w+)\\(", "name": "{Class} calls $1.$2", "files": "src/WebApp/Components/**/*.razor" }
      ]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.interaction", "severity": "error", "subject": "src/WebApp/Components/Pages/Catalog/Catalog.razor:25", "messageContains": "Catalog calls CatalogService.GetCatalogItems" },
    { "rule": "optin/unmodelled.interaction", "severity": "error", "subject": "src/WebApp/Components/Pages/Catalog/Catalog.razor:26", "messageContains": "Catalog calls BasketState.GetBasketItemsAsync" },
    { "rule": "optin/unmodelled.interaction", "subject": "src/WebApp/Components/Pages/Catalog/Catalog.razor:29", "count": 0, "note": "NavigationManager isn't a service or state client" }
  ]
}
```

## java-repository-reach

The Java spike's check: an aggregate the code reaches through a repository with no Entity Action. The house convention finds a write through a repository in an `@Service` method. `addItem` saves a `Wishlist`, and the model's `addItem` Operation has no Update Entity Action, so the call is reported. Reaching the repository through a helper in another class needs a call graph, which only a plug-in can follow.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.interaction"], "traces": ["JAV-SVC-16"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java", "intent.jpa", "house.backend"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "eShop.Domain",
        "elements": [
          { "id": "wishlist", "type": "Class", "name": "Wishlist",
            "children": [
              { "id": "wishlist-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } },
              { "id": "wishlist-customer-id", "type": "Attribute", "name": "CustomerId", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Services",
        "elements": [
          { "id": "wishlist-service", "type": "Service", "name": "WishlistService",
            "children": [
              { "id": "add-item", "type": "Operation", "name": "addItem",
                "children": [
                  { "id": "add-item-customer-id", "type": "Parameter", "name": "customerId", "typeRef": { "type": "guid" } },
                  { "id": "add-item-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "guid" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop.Backend", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.interaction" } ] }
        } }]
    }
  }
}
```

```json pack
{
  "id": "house.backend",
  "version": "1.0.0",
  "requires": [{ "id": "intent.jpa", "version": "^1.0.0" }],
  "languages": ["java"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "interactions": [
        { "kind": "repository write", "find": "type with @Service\\b > method > call with \\b\\w+Repository\\.(save|saveAll|delete|deleteAll)\\(", "name": "{Class}.{Method} calls $1", "files": "src/main/java/**/application/**/*.java" }
      ]
    }
  }
}
```

```java file src/main/java/com/eshop/backend/application/WishlistService.java
package com.eshop.backend.application;

import com.eshop.backend.domain.Wishlist;
import com.eshop.backend.domain.WishlistRepository;
import java.util.UUID;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
public class WishlistService {
    private final WishlistRepository wishlistRepository;

    public WishlistService(WishlistRepository wishlistRepository) {
        this.wishlistRepository = wishlistRepository;
    }

    @Transactional
    public void addItem(UUID customerId, UUID productId) {
        Wishlist wishlist = wishlistRepository.findByCustomerId(customerId).orElseGet(() -> new Wishlist(customerId));
        wishlist.addItem(productId);
        wishlistRepository.save(wishlist);
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.interaction", "severity": "error", "subject": "src/main/java/com/eshop/backend/application/WishlistService.java:21", "messageContains": "WishlistService.addItem calls save" },
    { "rule": "optin/unmodelled.interaction", "subject": "src/main/java/com/eshop/backend/application/WishlistService.java:19", "count": 0, "note": "A read; the convention counts writes" }
  ]
}
```
