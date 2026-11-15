# engine/unreadable

A rule can recognise that the code states a value in a form it can't read: a read case marked `unreadable` matched. The claim is can't verify, `unreadable` (outcome step 8), and the finding says where the code states it. It is never compared, so it is neither verified by luck nor contradicted by a misreading. Like every can't verify it is an error (open question 2); eShopCustom's modules reported these as warnings.

## domain-event-not-identified

The model says `SetPaidStatus` raises `OrderStatusChangedToPaidDomainEvent`. The method calls `AddDomainEvent` with an event a helper builds from the order's status, so which event it raises can't be read from its body. The house pack's recipe (eShopCustom's domain-events recipe, as converted) marks a bare `AddDomainEvent(` as unreadable after its two forms that name the event, so the claim is can't verify, `unreadable`, rather than read as `absent` from the recipe's `default` and contradicted.

eShopCustom's Domain module gave an `AddDomainEvent` whose event it couldn't identify a warning.

```json case
{ "phase": 1, "cases": ["engine/unreadable"], "traces": ["ESC-DOM-32"], "requirements": ["R1", "R5"], "packs": ["intent.core", "intent.domain", "intent.messaging", "house.shop"], "language": "csharp" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "rules": {
    "Domain": {
      "facts": {
        "intent.raises": {
          "files": "src/Domain/**/*.cs",
          "find": "class:{Entity} > method:{Member}",
          "read": [
            { "part": "body", "match": "\\bnew {DomainEvent}\\b", "value": "present" },
            { "part": "body", "match": "\\bAdd{DomainEvent}\\(", "value": "present" },
            { "part": "body", "match": "\\bAddDomainEvent\\(", "unreadable": true, "note": "An event built elsewhere: which one it is can't be read here." }
          ],
          "default": "absent"
        }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-set-paid", "type": "Operation", "name": "SetPaidStatus" }
            ] },
          { "id": "order-paid", "type": "Domain Event", "name": "OrderStatusChangedToPaidDomainEvent" }
        ]
      }],
      "associations": [
        { "id": "order-set-paid-raises-paid", "type": "Domain Event Origin Association",
          "source": { "element": "order-set-paid", "multiplicity": "1", "navigable": false },
          "target": { "element": "order-paid", "multiplicity": "1", "navigable": true } }
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

public class Order : Entity
{
    private OrderStatus _status;

    public void SetPaidStatus()
    {
        if (_status != OrderStatus.StockConfirmed)
        {
            return;
        }

        _status = OrderStatus.Paid;
        AddDomainEvent(StatusChangedEvent());
    }

    private INotification StatusChangedEvent() => _status switch
    {
        OrderStatus.Paid => new OrderStatusChangedToPaidDomainEvent(Id),
        _ => new OrderStatusChangedDomainEvent(Id, _status)
    };
}
```

```csharp file src/Domain/Events/OrderStatusChangedToPaidDomainEvent.cs
namespace Shop.Domain.Events;

public record OrderStatusChangedToPaidDomainEvent(int OrderId) : INotification;
```

```json expect
{
  "claims": [
    { "kind": "intent.raises", "element": "order-set-paid-raises-paid", "outcome": "cantVerify", "reason": "unreadable" }
  ],
  "findings": [
    { "rule": "claim/intent.raises", "severity": "error", "subject": "order-set-paid-raises-paid", "messageContains": "can't read" }
  ]
}
```

## route-prefix-not-literal

The model gives the Orders API the route `api/orders`. The router builds its group prefix by interpolating a constant from the company's shared API package, so the code states a prefix, but not as a literal the route rule can read. The claim is can't verify, `unreadable`: not contradicted as unstated, since the code clearly states one, and not compared against text that isn't the route.

eShopCustom's Services module warned on a non-literal prefix or route and turned off its other route checks for it.

```json case
{ "phase": 4, "kinds": ["service.route"], "cases": ["engine/unreadable"], "traces": ["ESC-SVC-10"], "requirements": ["R1", "R5"], "packs": ["intent.core", "intent.services", "intent.aspnetcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Api",
        "elements": [
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Shop.Api/Apis/OrdersApi.cs
namespace Shop.Api.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup($"{ApiRoutes.Prefix}/orders");

        api.MapPut("/cancel", CancelOrderAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var cancelled = await services.Mediator.Send(command);

        return cancelled ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "service.route", "element": "orders-api", "outcome": "cantVerify", "reason": "unreadable" }
  ],
  "findings": [
    { "rule": "claim/service.route", "severity": "error", "subject": "orders-api" }
  ]
}
```

## proxy-verb-built-elsewhere

The front end's ordering proxy has two operations, each with eShopCustom's Backend Operation stereotype naming the backend Operation it calls. The house pack's `proxy.verb` vocabulary follows that stereotype's Operation property to the backend Operation and states its Http Settings Verb. `GetOrders` calls `GetFromJsonAsync`, which reads as `GET`, and is verified. `CancelOrder` sends an `HttpRequestMessage` that an injected request factory builds, so the code states a verb, but somewhere this method doesn't show. The rule's last read case marks a bare `SendAsync(` as unreadable, so the claim is can't verify, `unreadable`: not contradicted as unstated, and not read as some other verb.

eShopCustom's Front End module warned on request readings it couldn't verify: several requests or rpc calls in one method, a verb it couldn't read, an absolute or computed URI, computed or unknown headers, or a computed `api-version`. This case shows the verb; each of the others is the same outcome from a read case marked `unreadable` in the kind's rule.

```json case
{ "phase": "later", "kinds": ["proxy.verb"], "cases": ["engine/unreadable"], "traces": ["ESC-FE-72"], "requirements": ["R1", "R5", "R30"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "kinds": [
    { "kind": "proxy.verb", "label": "proxy verb", "valueType": "text", "comparer": "exact", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "User Interface": {
      "entries": [
        {
          "kind": "proxy.verb",
          "for": { "element": "Operation", "where": "parent.specialization == 'Service Proxy' && hasStereotype('Backend Operation')" },
          "value": "stereotype('Backend Operation').Operation.stereotype('Http Settings').Verb",
          "bindings": { "Proxy": "parent.name", "Operation": "name" },
          "own": ["Proxy", "Operation"],
          "source": "the Http Settings Verb of the backend Operation its Backend Operation stereotype names"
        }
      ]
    }
  },
  "rules": {
    "User Interface": {
      "files": ["src/WebAppComponents/Services/**/*.cs"],
      "facts": {
        "proxy.verb": {
          "find": "type:{Proxy} > method:{Operation}",
          "read": [
            { "part": "body", "match": "\\.GetFromJsonAsync\\b", "value": "GET" },
            { "part": "body", "match": "\\.PostAsJsonAsync\\b", "value": "POST" },
            { "part": "body", "match": "\\.PutAsJsonAsync\\b", "value": "PUT" },
            { "part": "body", "match": "\\.DeleteAsync\\b", "value": "DELETE" },
            { "part": "body", "match": "\\.SendAsync\\(", "unreadable": true, "note": "A request message built elsewhere: its verb can't be read here." }
          ],
          "unstated": "no HTTP client call in {Proxy}.{Operation}"
        }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "ordering-api", "name": "Ordering.API",
        "elements": [
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "get-orders", "type": "Operation", "name": "GetOrdersByUserAsync",
                "stereotypes": { "Http Settings": { "Verb": "GET" } } },
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } } }
            ] }
        ]
      }]
    },
    "User Interface": {
      "packages": [{
        "id": "webappcomponents", "name": "WebAppComponents",
        "elements": [
          { "id": "ordering-service", "type": "Service Proxy", "name": "OrderingService",
            "children": [
              { "id": "ordering-service-get-orders", "type": "Operation", "name": "GetOrders",
                "stereotypes": { "Backend Operation": { "Operation": { "ref": "get-orders" } } } },
              { "id": "ordering-service-cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Backend Operation": { "Operation": { "ref": "cancel-order" } } },
                "children": [
                  { "id": "ordering-service-cancel-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/WebAppComponents/Services/OrderingService.cs
using System.Net.Http.Json;
using eShop.WebAppComponents.Ordering;

namespace eShop.WebAppComponents.Services;

public class OrderingService(HttpClient httpClient, IOrderingRequests requests)
{
    private readonly string remoteServiceBaseUrl = "api/orders/";

    public async Task<OrderRecord[]> GetOrders()
    {
        var orders = await httpClient.GetFromJsonAsync<OrderRecord[]>(remoteServiceBaseUrl);
        return orders ?? [];
    }

    public async Task CancelOrder(int orderId)
    {
        using var request = requests.Cancel(orderId);
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "proxy.verb", "element": "ordering-service-get-orders", "outcome": "verified",
      "expected": "GET", "actual": "GET", "at": "src/WebAppComponents/Services/OrderingService.cs:12" },
    { "kind": "proxy.verb", "element": "ordering-service-cancel-order", "outcome": "cantVerify", "reason": "unreadable" }
  ],
  "findings": [
    { "rule": "claim/proxy.verb", "severity": "error", "subject": "ordering-service-cancel-order", "messageContains": "can't read" }
  ]
}
```
