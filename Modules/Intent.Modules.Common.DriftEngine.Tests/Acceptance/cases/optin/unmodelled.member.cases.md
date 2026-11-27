# optin/unmodelled.member

The opt-in rule that reports a public member of a located type, of a kind its element type can state, that no claim located (rules.md, "Opt-in must be modelled rules"). A Class can state attributes, constructors and operations, so an unmodelled public property, constructor or method of a located class is reported once the rule is on; a member the model has is located by its own `member.exists` claim and never is. By principle 1 the rule is off until the Root Folder's `Drift Check` lists it under `Opt-in Rules`.

A pack's `members` conventions widen the rule beyond an element type's own public members: non-public members, a field EF Core maps by name, an endpoint inside a router, a request-bound parameter. A finding's subject is the file and the line where the member's node starts.

## off-by-default

`Order` is modelled with `Id`, `Description` and `SetPaidStatus`, and the class also has `BuyerId`, which the model doesn't mention. No `Opt-in Rules` row turns the rule on, so nothing is reported (principle 1).

```json case
{ "phase": 1, "cases": ["optin/unmodelled.member"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" } },
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } },
              { "id": "order-set-paid-status", "type": "Operation", "name": "SetPaidStatus" }
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

```csharp file src/Domain/OrderAggregate/Order.cs
namespace Ordering.Domain.OrderAggregate;

public class Order
{
    protected Order()
    {
    }

    public int Id { get; private set; }

    public string Description { get; private set; }

    public int? BuyerId { get; private set; }

    public void SetPaidStatus()
    {
        Description = "Paid";
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.member", "count": 0, "note": "The rule is off" }
  ]
}
```

## on

The same model and code, with the rule on. `BuyerId` is a public instance property of the located `Order`, a kind a Class states as an Attribute, and no claim located it, so it is reported, an error by default. The modelled members are located by their own claims, and the protected parameterless constructor isn't public. eShopCustom's fix also hinted at an association when the property's type is a modelled Class; the spec gives the finding no such hint, so this case shows the property only.

```json case
{ "phase": 1, "base": "off-by-default", "cases": ["optin/unmodelled.member"], "traces": ["ESC-DOM-66"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" } },
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } },
              { "id": "order-set-paid-status", "type": "Operation", "name": "SetPaidStatus" }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-description", "outcome": "verified", "at": "src/Domain/OrderAggregate/Order.cs:11" },
    { "kind": "member.exists", "element": "order-set-paid-status", "outcome": "verified", "at": "src/Domain/OrderAggregate/Order.cs:15" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Domain/OrderAggregate/Order.cs:13", "messageContains": "BuyerId" },
    { "rule": "optin/unmodelled.member", "subject": "src/Domain/OrderAggregate/Order.cs:9", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/Domain/OrderAggregate/Order.cs:11", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/Domain/OrderAggregate/Order.cs:15", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/Domain/OrderAggregate/Order.cs:5", "count": 0, "note": "Protected, so not a public member" }
  ]
}
```

## constructor-and-method

A public constructor with parameters and a public method, neither modelled: both are reported. The protected parameterless constructor EF Core needs isn't public, so it isn't.

```json case
{ "phase": 1, "base": "on", "cases": ["optin/unmodelled.member"], "traces": ["ESC-DOM-62", "ESC-DOM-65"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/OrderAggregate/Order.cs
namespace Ordering.Domain.OrderAggregate;

public class Order
{
    protected Order()
    {
    }

    public Order(string userId, string description) : this()
    {
        Description = description;
    }

    public int Id { get; private set; }

    public string Description { get; private set; }

    public void SetPaidStatus()
    {
        Description = "Paid";
    }

    public void SetCancelledStatus()
    {
        Description = "Cancelled";
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Domain/OrderAggregate/Order.cs:9", "messageContains": "Order" },
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Domain/OrderAggregate/Order.cs:23", "messageContains": "SetCancelledStatus" },
    { "rule": "optin/unmodelled.member", "subject": "src/Domain/OrderAggregate/Order.cs:5", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/Domain/OrderAggregate/Order.cs:18", "count": 0 }
  ]
}
```

## non-public-members

eShopCustom also reported internal and protected members: a non-private constructor with parameters, an internal property and a protected method. The rule's own reach is public members, so the house pack adds `members` conventions for the rest. The constructor convention needs a parameter, so the protected parameterless constructor still isn't reported.

```json case
{ "phase": 2, "base": "on", "cases": ["optin/unmodelled.member"], "traces": ["ESC-DOM-62", "ESC-DOM-65", "ESC-DOM-66"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
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
      "members": [
        { "kind": "constructor", "find": "type > constructor with \\b(?:internal|protected) \\w+\\([^)]", "name": "{Class}", "files": "src/Domain/**/*.cs" },
        { "kind": "property", "find": "type > property with \\b(?:internal|protected) (?!override\\b)", "name": "{Class}.{Member}", "files": "src/Domain/**/*.cs" },
        { "kind": "method", "find": "type > method with \\b(?:internal|protected) (?!override\\b)", "name": "{Class}.{Member}", "files": "src/Domain/**/*.cs", "note": "Overrides are the base type's members" }
      ]
    }
  }
}
```

```csharp file src/Domain/OrderAggregate/Order.cs
namespace Ordering.Domain.OrderAggregate;

public class Order
{
    protected Order()
    {
    }

    protected Order(string description) : this()
    {
        Description = description;
    }

    public int Id { get; private set; }

    public string Description { get; private set; }

    internal string PaymentReference { get; private set; }

    public void SetPaidStatus()
    {
        StampStatusChange("Paid");
    }

    protected void StampStatusChange(string status)
    {
        Description = status;
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Domain/OrderAggregate/Order.cs:9", "messageContains": "Order" },
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Domain/OrderAggregate/Order.cs:18", "messageContains": "Order.PaymentReference" },
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Domain/OrderAggregate/Order.cs:25", "messageContains": "Order.StampStatusChange" },
    { "rule": "optin/unmodelled.member", "subject": "src/Domain/OrderAggregate/Order.cs:5", "count": 0, "note": "No parameters" }
  ]
}
```

## excepted-by-type-member

The rule is on, and an `Exceptions` row names `Order.BuyerId` in the `Type.Member` form, with a reason. It isn't reported, and the exception matched, so it isn't stale.

```json case
{ "phase": 1, "base": "off-by-default", "cases": ["optin/unmodelled.member"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" } },
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } },
              { "id": "order-set-paid-status", "type": "Operation", "name": "SetPaidStatus" }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": {
            "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ],
            "Exceptions": [ { "Rule": "optin/unmodelled.member", "Match": "Order.BuyerId", "Reason": "The buyer link is a foreign key the Buyer association will state once Buyer is modelled" } ]
          }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.member", "subject": "src/Domain/OrderAggregate/Order.cs:13", "count": 0, "note": "Excepted" },
    { "rule": "optin/staleException", "count": 0 }
  ]
}
```

## domain-event-property

A Domain Event's Property is a public property of the event. `UserName` isn't modelled (`member.unmodelled` in eShopCustom).

```json case
{ "phase": 1, "cases": ["optin/unmodelled.member"], "traces": ["ESC-DOM-29"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
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
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/Domain/Events/OrderStartedDomainEvent.cs
namespace Ordering.Domain.Events;

public class OrderStartedDomainEvent : INotification
{
    public string UserId { get; init; }

    public string UserName { get; init; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-started-user-id", "outcome": "verified", "at": "src/Domain/Events/OrderStartedDomainEvent.cs:5" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Domain/Events/OrderStartedDomainEvent.cs:7", "messageContains": "UserName" },
    { "rule": "optin/unmodelled.member", "subject": "src/Domain/Events/OrderStartedDomainEvent.cs:5", "count": 0 }
  ]
}
```

## ef-backing-field

A private field EF Core maps by name, `Property<int?>("_buyerId")`, with no Backing Field attribute in the model (`member.unmodelled` in eShopCustom). The field is private, so only a convention can reach it; the house convention finds the mapping call in the entity's configuration, which `Order`'s `table.name` claim located, and names the field it maps.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.member"], "traces": ["ESC-DOM-43"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": { "Name": "orders" } },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```json pack
{
  "id": "house.ordering",
  "version": "1.0.0",
  "requires": [{ "id": "intent.efcore", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "conventions": {
    "Domain": {
      "members": [
        { "kind": "mapped field", "find": "type with IEntityTypeConfiguration< > chain with \\.Property(?:<[^>]*>)?\\(\"(_\\w+)\"\\)", "name": "$1", "files": "src/Infrastructure/**/*.cs", "note": "A private field EF Core maps by name" }
      ]
    }
  }
}
```

```csharp file src/Domain/OrderAggregate/Order.cs
namespace Ordering.Domain.OrderAggregate;

public class Order
{
    private int? _buyerId;

    public int Id { get; private set; }
}
```

```csharp file src/Infrastructure/EntityConfigurations/OrderEntityTypeConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.OrderAggregate;

namespace Ordering.Infrastructure.EntityConfigurations;

class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> orderConfiguration)
    {
        orderConfiguration.ToTable("orders");

        orderConfiguration.Property<int?>("_buyerId").HasColumnName("BuyerId");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "table.name", "element": "order", "outcome": "verified", "expected": "orders", "actual": "orders",
      "at": "src/Infrastructure/EntityConfigurations/OrderEntityTypeConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Infrastructure/EntityConfigurations/OrderEntityTypeConfiguration.cs:13", "messageContains": "_buyerId" }
  ]
}
```

## ef-unmapped-property

A property EF doesn't map can't be an attribute of a persisted class, since the model persists every attribute of one (`column.mapped`), so it isn't reported as unmodelled either: if the diagram can't reflect it, it isn't drift. `Status` is a `[NotMapped]` enum over the `StatusId` column, as a migrated EF6 application's entities have, and the configuration `Ignore`s `Total`. `Notes` is mapped and unmodelled, so it is still reported.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.member"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-status-id", "type": "Attribute", "name": "StatusId", "typeRef": { "type": "int" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/Domain/OrderAggregate/Order.cs
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.Domain.OrderAggregate;

public class Order
{
    public int Id { get; private set; }

    public int StatusId { get; set; }

    [NotMapped]
    public OrderStatus Status
    {
        get => (OrderStatus)StatusId;
        set => StatusId = (int)value;
    }

    public decimal Total => 0m;

    public string Notes { get; set; }
}

public enum OrderStatus
{
    Submitted = 1,
    Paid = 2
}
```

```csharp file src/Infrastructure/EntityConfigurations/OrderEntityTypeConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.OrderAggregate;

namespace Ordering.Infrastructure.EntityConfigurations;

class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> orderConfiguration)
    {
        orderConfiguration.Ignore(o => o.Total);
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.member", "count": 1 },
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Domain/OrderAggregate/Order.cs:20", "messageContains": "Order.Notes" }
  ]
}
```

## endpoint-in-router

An endpoint mapped inside a modelled router that no Operation describes (`services.operation.unmodelled` in eShopCustom; the unmodelled router itself is [unmodelled.type](unmodelled.type.cases.md)'s). The house convention finds each `Map<Verb>` call in a router. `/cancel` is located by `CancelOrder`'s verb and route claims; `/ship` isn't modelled.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.member"], "traces": ["ESC-SVC-25"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "cancel-order-command-dto", "type": "DTO", "name": "CancelOrderCommand",
            "children": [
              { "id": "cancel-order-command-dto-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "orders", "type": "Service", "name": "Orders",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } },
                "children": [
                  { "id": "cancel-order-command", "type": "Parameter", "name": "command", "typeRef": { "element": "cancel-order-command-dto" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Body" } } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```json pack
{
  "id": "house.ordering",
  "version": "1.0.0",
  "requires": [{ "id": "intent.aspnetcore", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "members": [
        { "kind": "endpoint", "find": "type with \\.MapGroup\\( > call with \\.Map(Get|Post|Put|Delete|Patch)\\(\"([^\"]*)\"", "name": "{Class} $1 $2", "files": "src/**/Apis/**/*.cs" }
      ]
    }
  }
}
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync);
        api.MapPut("/ship", ShipOrderAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync([FromBody] CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }

    public static async Task<Results<Ok, BadRequest<string>>> ShipOrderAsync([FromBody] ShipOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Ship order failed to process.");
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Ordering.API/Apis/OrdersApi.cs:10", "messageContains": "/ship" },
    { "rule": "optin/unmodelled.member", "subject": "src/Ordering.API/Apis/OrdersApi.cs:9", "count": 0, "note": "Located by CancelOrder's verb and route claims" }
  ]
}
```

## request-parameter

A request-bound handler parameter that isn't modelled (`services.parameter.unmodelled`). The house convention finds every parameter with a binding attribute in a router's handlers. `command` is located by its `parameter.source` claim; `requestId`, bound from a header, isn't modelled. `[AsParameters]` isn't a binding attribute the convention names.

```json case
{ "phase": 4, "base": "endpoint-in-router", "cases": ["optin/unmodelled.member"], "traces": ["ESC-SVC-32"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.ordering"], "language": "csharp" }
```

```json pack
{
  "id": "house.ordering",
  "version": "1.0.0",
  "requires": [{ "id": "intent.aspnetcore", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "members": [
        { "kind": "endpoint", "find": "type with \\.MapGroup\\( > call with \\.Map(Get|Post|Put|Delete|Patch)\\(\"([^\"]*)\"", "name": "{Class} $1 $2", "files": "src/**/Apis/**/*.cs" },
        { "kind": "request parameter", "find": "type with \\.MapGroup\\( > method > parameter with \\[From(?:Body|Form|Header|Query|Route)\\b", "name": "{Method}({Name})", "files": "src/**/Apis/**/*.cs" }
      ]
    }
  }
}
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(
        [FromHeader(Name = "x-requestid")] Guid requestId,
        [FromBody] CancelOrderCommand command,
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
    { "kind": "parameter.source", "element": "cancel-order-command", "outcome": "verified", "at": "src/Ordering.API/Apis/OrdersApi.cs:16" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Ordering.API/Apis/OrdersApi.cs:15", "messageContains": "requestId" },
    { "rule": "optin/unmodelled.member", "subject": "src/Ordering.API/Apis/OrdersApi.cs:16", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/Ordering.API/Apis/OrdersApi.cs:17", "count": 0 }
  ]
}
```

## dto-property

A DTO's DTO-Fields are its public properties. `Total` isn't modelled (`*.field.unmodelled` in eShopCustom).

```json case
{ "phase": 1, "cases": ["optin/unmodelled.member"], "traces": ["ESC-SVC-47"], "requirements": ["R49"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "order-summary", "type": "DTO", "name": "OrderSummary",
            "children": [
              { "id": "order-summary-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } },
              { "id": "order-summary-status", "type": "DTO-Field", "name": "Status", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/Ordering.API/Application/Queries/OrderSummary.cs
namespace Ordering.API.Application.Queries;

public record OrderSummary
{
    public int OrderNumber { get; init; }

    public string Status { get; init; }

    public decimal Total { get; init; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-summary-order-number", "outcome": "verified", "at": "src/Ordering.API/Application/Queries/OrderSummary.cs:5" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Ordering.API/Application/Queries/OrderSummary.cs:9", "messageContains": "Total" },
    { "rule": "optin/unmodelled.member", "subject": "src/Ordering.API/Application/Queries/OrderSummary.cs:5", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/Ordering.API/Application/Queries/OrderSummary.cs:7", "count": 0 }
  ]
}
```

## application-service-method

An application service's public method that no Operation describes (`services.appservice.operation.unmodelled`). eShopCustom's house pack says a Service with no interface is described by the class of its name. eShopCustom also reported an interface's methods when the Service is described by its interface; a convention that locates a Service by `I` plus its name needs a naming placeholder the spec doesn't define for type conventions, so this case shows the class.

```json case
{ "phase": 2, "cases": ["optin/unmodelled.member"], "traces": ["ESC-SVC-68"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "types": {
        "Service": { "typeKinds": ["class"], "locations": ["src/**"], "note": "eShopCustom: a Service with no interface is described by its class" }
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
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "order-dto", "type": "DTO", "name": "Order",
            "children": [
              { "id": "order-dto-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "order-queries", "type": "Service", "name": "OrderQueries",
            "stereotypes": { "Application Service": {} },
            "children": [
              { "id": "order-queries-get-order", "type": "Operation", "name": "GetOrderAsync", "typeRef": { "element": "order-dto" },
                "children": [
                  { "id": "order-queries-get-order-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/Ordering.API/Application/Queries/OrderQueries.cs
namespace eShop.Ordering.API.Application.Queries;

public class OrderQueries(OrderingContext context)
{
    public async Task<Order> GetOrderAsync(int id)
    {
        return await context.Orders.Select(o => new Order { OrderNumber = o.Id }).FirstAsync(o => o.OrderNumber == id);
    }

    public async Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(string userId)
    {
        return await context.Orders.Where(o => o.Buyer.IdentityGuid == userId).Select(o => new OrderSummary { OrderNumber = o.Id }).ToListAsync();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-queries-get-order", "outcome": "verified", "at": "src/Ordering.API/Application/Queries/OrderQueries.cs:5" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Ordering.API/Application/Queries/OrderQueries.cs:10", "messageContains": "GetOrdersFromUserAsync" },
    { "rule": "optin/unmodelled.member", "subject": "src/Ordering.API/Application/Queries/OrderQueries.cs:5", "count": 0 }
  ]
}
```

## hosted-service-override

A hosted service's public override that no Operation describes (`services.hosted-service.operation.unmodelled`). `ExecuteAsync` is modelled and located; `StopAsync` is a public method of the located class, so it is reported.

```json case
{ "phase": 2, "base": "application-service-method", "without": ["src/Ordering.API/Application/Queries/OrderQueries.cs"], "cases": ["optin/unmodelled.member"], "traces": ["ESC-SVC-73"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "OrderProcessor",
        "elements": [
          { "id": "grace-period", "type": "Service", "name": "GracePeriodManagerService",
            "stereotypes": { "Hosted Service": {} },
            "children": [
              { "id": "grace-period-execute", "type": "Operation", "name": "ExecuteAsync" }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/OrderProcessor/Services/GracePeriodManagerService.cs
namespace eShop.OrderProcessor.Services;

public class GracePeriodManagerService(ILogger<GracePeriodManagerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogDebug("GracePeriodManagerService is starting.");
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("GracePeriodManagerService is stopping.");
        await base.StopAsync(cancellationToken);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "grace-period-execute", "outcome": "verified", "at": "src/OrderProcessor/Services/GracePeriodManagerService.cs:5" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/OrderProcessor/Services/GracePeriodManagerService.cs:11", "messageContains": "StopAsync" },
    { "rule": "optin/unmodelled.member", "subject": "src/OrderProcessor/Services/GracePeriodManagerService.cs:5", "count": 0 }
  ]
}
```

## grpc-override

A gRPC service's override that no Operation describes (`services.grpc.operation.unmodelled`). `GetBasket` is modelled; `UpdateBasket` isn't. The `.proto` half, an unmodelled message or field, is `proto-message-and-field`'s.

```json case
{ "phase": 2, "base": "application-service-method", "without": ["src/Ordering.API/Application/Queries/OrderQueries.cs"], "cases": ["optin/unmodelled.member"], "traces": ["ESC-SVC-80"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "get-basket-request", "type": "DTO", "name": "GetBasketRequest", "stereotypes": { "Proto Message": {} } },
          { "id": "customer-basket-response", "type": "DTO", "name": "CustomerBasketResponse", "stereotypes": { "Proto Message": {} } },
          { "id": "basket-service", "type": "Service", "name": "BasketService",
            "stereotypes": { "gRPC Service": { "Proto File": "Proto/basket.proto", "Proto Service": "Basket" } },
            "children": [
              { "id": "basket-service-get-basket", "type": "Operation", "name": "GetBasket", "typeRef": { "element": "customer-basket-response" },
                "children": [
                  { "id": "basket-service-get-basket-request", "type": "Parameter", "name": "request", "typeRef": { "element": "get-basket-request" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/Basket.API/Grpc/BasketService.cs
namespace eShop.Basket.API.Grpc;

public class BasketService(IBasketRepository repository) : Basket.BasketBase
{
    public override async Task<CustomerBasketResponse> GetBasket(GetBasketRequest request, ServerCallContext context)
    {
        var data = await repository.GetBasketAsync(context.GetUserIdentity());
        return data is null ? new() : MapToCustomerBasketResponse(data);
    }

    public override async Task<CustomerBasketResponse> UpdateBasket(UpdateBasketRequest request, ServerCallContext context)
    {
        var response = await repository.UpdateBasketAsync(MapToCustomerBasket(context.GetUserIdentity(), request));
        return MapToCustomerBasketResponse(response);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "basket-service-get-basket", "outcome": "verified", "at": "src/Basket.API/Grpc/BasketService.cs:5" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Basket.API/Grpc/BasketService.cs:11", "messageContains": "UpdateBasket" },
    { "rule": "optin/unmodelled.member", "subject": "src/Basket.API/Grpc/BasketService.cs:5", "count": 0 }
  ]
}
```

## proto-message-and-field

A `.proto` message no Proto Message DTO describes (`services.proto-message.unmodelled`), and a field of a modelled one that no DTO-Field states (`.field.unmodelled`). The house pack says a Proto Message DTO is a message in a `.proto` file; `BasketItem` is located, and its `quantity` isn't modelled. `UpdateBasketRequest` follows the convention and nothing describes it.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.member", "optin/unmodelled.type"], "traces": ["ESC-SVC-84"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.grpc", "house.eshop"], "language": "protobuf" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }, { "id": "intent.grpc", "version": "^1.0.0" }],
  "languages": ["csharp", "protobuf"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "types": {
        "DTO": { "where": "hasStereotype('Proto Message')", "typeKinds": ["message"], "locations": ["src/**/Proto/*.proto"], "find": "type", "files": "src/**/Proto/*.proto", "note": "eShopCustom: a Proto Message DTO is a message in its service's .proto file" }
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
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "basket-item", "type": "DTO", "name": "BasketItem", "stereotypes": { "Proto Message": {} },
            "children": [
              { "id": "basket-item-product-id", "type": "DTO-Field", "name": "ProductId", "typeRef": { "type": "int" } }
            ] },
          { "id": "customer-basket-response", "type": "DTO", "name": "CustomerBasketResponse", "stereotypes": { "Proto Message": {} },
            "children": [
              { "id": "customer-basket-response-items", "type": "DTO-Field", "name": "Items", "typeRef": { "element": "basket-item", "collection": true } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" }, { "Rule": "optin/unmodelled.type" } ] }
        } }]
    }
  }
}
```

```proto file src/Basket.API/Proto/basket.proto
syntax = "proto3";

option csharp_namespace = "eShop.Basket.API.Grpc";

package BasketApi;

message CustomerBasketResponse {
    repeated BasketItem items = 1;
}

message BasketItem {
    int32 product_id = 2;
    int32 quantity = 6;
}

message UpdateBasketRequest {
    repeated BasketItem items = 2;
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Basket.API/Proto/basket.proto:13", "messageContains": "quantity" },
    { "rule": "optin/unmodelled.member", "subject": "src/Basket.API/Proto/basket.proto:12", "count": 0 },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Basket.API/Proto/basket.proto:16", "messageContains": "UpdateBasketRequest" },
    { "rule": "optin/unmodelled.type", "subject": "src/Basket.API/Proto/basket.proto:11", "count": 0 }
  ]
}
```

## component-parameter-and-callback

A component's `[Parameter]` with no Property (`property.unmodelled`) and its `EventCallback` parameter with no Event Emitter (`event.unmodelled`). `Item` is modelled and located. eShopCustom's fixes named Query, Route or Bindable for the property; the spec gives the finding no such fix, so this case shows the findings only.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.member"], "traces": ["ESC-FE-21", "ESC-FE-24"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp",
        "elements": [
          { "id": "catalog-item", "type": "Model Definition", "name": "CatalogItem",
            "children": [
              { "id": "catalog-item-id", "type": "Property", "name": "Id", "typeRef": { "type": "int" } },
              { "id": "catalog-item-name", "type": "Property", "name": "Name", "typeRef": { "type": "string" } }
            ] },
          { "id": "catalog-list-item", "type": "Component", "name": "CatalogListItem",
            "children": [
              { "id": "catalog-list-item-item", "type": "Property", "name": "Item", "typeRef": { "element": "catalog-item" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/WebApp/Catalog/CatalogItem.cs
namespace eShop.WebApp.Catalog;

public record CatalogItem(int Id, string Name);
```

```razor file src/WebApp/Components/Catalog/CatalogListItem.razor
<div class="catalog-item">
    <span class="catalog-product-content">@Item.Name</span>
    @if (IsLoggedIn)
    {
        <button @onclick="() => OnAddToCart.InvokeAsync(Item)">Add to bag</button>
    }
</div>

@code {
    [Parameter, EditorRequired] public required CatalogItem Item { get; set; }

    [Parameter] public bool IsLoggedIn { get; set; }

    [Parameter] public EventCallback<CatalogItem> OnAddToCart { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "catalog-list-item-item", "outcome": "verified", "at": "src/WebApp/Components/Catalog/CatalogListItem.razor:10" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/WebApp/Components/Catalog/CatalogListItem.razor:12", "messageContains": "IsLoggedIn" },
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/WebApp/Components/Catalog/CatalogListItem.razor:14", "messageContains": "OnAddToCart" },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Components/Catalog/CatalogListItem.razor:10", "count": 0 }
  ]
}
```

## component-injections

A component's injected services are its Uses of what it injects, never Properties of it, so none is reported as an unmodelled member, though `JS` is a public property. Nor is a Uses asked for: whether a use is significant enough to draw is the modeller's call, as a lookup's is. `BasketState` is modelled as a UI Service the page uses, so its `@inject` is located and the Uses checked; `CatalogService` is the application's own class with no Uses in the model, and `NavigationManager` and `IJSRuntime` are the framework's.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.member"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "cart-page", "type": "Page", "name": "CartPage", "stereotypes": { "Page": { "Route": "/cart" } } },
          { "id": "basket-state", "type": "UI Service", "name": "BasketState" }
        ]
      }],
      "associations": [
        { "id": "cart-page-uses-basket-state", "type": "Uses", "source": { "element": "cart-page" }, "target": { "element": "basket-state" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```razor file src/WebApp/Components/_Imports.razor
@using eShop.WebApp.Services
```

```razor file src/WebApp/Components/Pages/CartPage.razor
@page "/cart"
@inject BasketState Basket
@inject CatalogService Catalog
@inject NavigationManager Navigation

<h1>Cart</h1>

@code {
    [Inject] public IJSRuntime JS { get; set; } = default!;
}
```

```csharp file src/WebApp/Services/BasketState.cs
namespace eShop.WebApp.Services;

public class BasketState
{
}
```

```csharp file src/WebApp/Services/CatalogService.cs
namespace eShop.WebApp.Services;

public class CatalogService
{
}
```

```json expect
{
  "claims": [
    { "kind": "intent.uses", "element": "cart-page-uses-basket-state", "outcome": "verified", "at": "src/WebApp/Components/Pages/CartPage.razor:2" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Components/Pages/CartPage.razor:2", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Components/Pages/CartPage.razor:3", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Components/Pages/CartPage.razor:4", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Components/Pages/CartPage.razor:9", "count": 0 }
  ]
}
```

## page-properties-that-are-no-parameters

A Page's properties that are no parameter are written in code: the User Interface designer models a Page's Properties only for its route and query parameters. So neither the form-bound `UpdateQuantityId` (`[SupplyParameterFromForm]`) nor the plain public `IsBusy` is asked for, while the unmodelled query parameter `Page` is.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.member"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "cart-page", "type": "Page", "name": "CartPage", "stereotypes": { "Page": { "Route": "/cart" } } }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@page "/cart"

<form method="post" @formname="update-cart">
    <AntiforgeryToken />
</form>

@code {
    [SupplyParameterFromForm]
    public int? UpdateQuantityId { get; set; }

    public bool IsBusy { get; set; }

    [SupplyParameterFromQuery]
    public int? Page { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "cart-page", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/WebApp/Components/Pages/Cart/CartPage.razor:13", "messageContains": "CartPage.Page" },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Components/Pages/Cart/CartPage.razor:8", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Components/Pages/Cart/CartPage.razor:11", "count": 0 }
  ]
}
```

## ui-members-the-designer-cant-state

What the User Interface designer can state of an element's members, as its own rules say: a Store adds Properties only ("Don't model actions or getters; describe them in its comment"), and a UI Service or Utility adds no members at all, so neither asks for its methods. `BasketState`'s unmodelled public property `ItemCount` is still reported; its methods, `ProductImageUrlProvider.GetProductImageUrl` and `Formatting.FormatPrice` aren't.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.member"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "basket-state", "type": "Store", "name": "BasketState", "children": [
            { "id": "basket-state-items", "type": "Property", "name": "Items", "typeRef": { "type": "string", "collection": true } }
          ] },
          { "id": "image-url-provider", "type": "UI Service", "name": "ProductImageUrlProvider" },
          { "id": "formatting", "type": "Utility", "name": "Formatting" }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/WebApp/Services/BasketState.cs
namespace eShop.WebApp.Services;

public class BasketState
{
    public IReadOnlyList<string> Items { get; private set; } = [];

    public int ItemCount { get; private set; }

    public Task AddAsync(string item) => Task.CompletedTask;

    public Task DeleteBasketAsync() => Task.CompletedTask;
}
```

```csharp file src/WebApp/Services/ProductImageUrlProvider.cs
namespace eShop.WebApp.Services;

public class ProductImageUrlProvider
{
    public string GetProductImageUrl(int productId) => $"product-images/{productId}";
}
```

```csharp file src/WebApp/Services/Formatting.cs
namespace eShop.WebApp.Services;

public static class Formatting
{
    public static string FormatPrice(decimal price) => price.ToString("C");
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "basket-state", "outcome": "verified", "at": "src/WebApp/Services/BasketState.cs:3" },
    { "kind": "member.exists", "element": "basket-state-items", "outcome": "verified", "at": "src/WebApp/Services/BasketState.cs:5" },
    { "kind": "type.exists", "element": "image-url-provider", "outcome": "verified" },
    { "kind": "type.exists", "element": "formatting", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/WebApp/Services/BasketState.cs:7", "messageContains": "ItemCount" },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Services/BasketState.cs:9", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Services/BasketState.cs:11", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Services/ProductImageUrlProvider.cs:5", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Services/Formatting.cs:5", "count": 0 }
  ]
}
```

## domain-static-members-the-designer-cant-state

A Domain Class's Attributes are never static (only its Operations may be), so a null object's `static Instance` is no Attribute the designer could add, and isn't asked for. Its static `Create` method is an Operation the designer could state as static, so it is reported, as is the instance property `Label`.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.member"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Bank.Domain",
        "elements": [
          { "id": "account-null", "type": "Class", "name": "AccountNull" }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Bank", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/Domain/AccountNull.cs
namespace Bank.Domain;

public sealed class AccountNull
{
    public static AccountNull Instance { get; } = new AccountNull();

    public static AccountNull Create() => Instance;

    public string Label => "none";
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "account-null", "outcome": "verified", "at": "src/Domain/AccountNull.cs:3" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "subject": "src/Domain/AccountNull.cs:5", "count": 0 },
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Domain/AccountNull.cs:7", "messageContains": "Create" },
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Domain/AccountNull.cs:9", "messageContains": "Label" }
  ]
}
```

## react-class-component-framework-members

What React calls or reads on every class component is React's, never the designer's: `render`, `componentDidMount` and the `static displayName` aren't asked for. Nor are the `match` and `history` props react-router 5 gives a component `withRouter` wraps, or `authService`, which holds a service the component creates (its constructor's `new AuthService()`), its Uses of that service. The component's own `refresh` function is still reported.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.member"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "spa", "name": "Spa", "type": "User Interface Package",
        "elements": [
          { "id": "side-menu", "type": "Component", "name": "SideMenu" },
          { "id": "auth-service", "type": "UI Service", "name": "AuthService" }
        ]
      }],
      "associations": [
        { "id": "side-menu-uses-auth", "type": "Uses", "source": { "element": "side-menu" }, "target": { "element": "auth-service" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "spa", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```js file src/store/AuthService.js
export default class AuthService {
  getUser() {
    return Promise.resolve(null);
  }
}
```

```jsx file src/components/SideMenu.jsx
import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import AuthService from '../store/AuthService';

class SideMenu extends Component {
  static displayName = SideMenu.name;

  authService;

  constructor(props) {
    super(props);
    this.authService = new AuthService();
  }

  componentDidMount() {
    this.refresh();
  }

  refresh() {
    this.authService.getUser().then(() => this.props.history.push(this.props.match.url));
  }

  render() {
    return <nav>Menu</nav>;
  }
}

export default withRouter(SideMenu);
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "side-menu", "outcome": "verified", "at": "src/components/SideMenu.jsx:5" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/components/SideMenu.jsx:19", "messageContains": "refresh" },
    { "rule": "optin/unmodelled.member", "subject": "src/components/SideMenu.jsx:6", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/components/SideMenu.jsx:8", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/components/SideMenu.jsx:15", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/components/SideMenu.jsx:20", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/components/SideMenu.jsx:23", "count": 0 }
  ]
}
```

## view-model-states-properties

A View Model adds Properties only, as its designer says ("Add a Property only for state other elements read ... Don't model its methods; describe them in its comment"), so its methods are never asked for and its Properties get the stock member checks. `ChatState`'s modelled `Messages` verifies; its unmodelled public property `IsBusy` is reported; its method `AddUserMessageAsync` isn't.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.member"], "kinds": ["type.exists", "member.exists"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "chat-state", "type": "View Model", "name": "ChatState", "children": [
            { "id": "chat-state-messages", "type": "Property", "name": "Messages", "typeRef": { "element": "chat-message", "collection": true } }
          ] },
          { "id": "chat-message", "type": "Contract", "name": "ChatMessage", "children": [
            { "id": "chat-message-text", "type": "Property", "name": "Text", "typeRef": { "type": "string" } }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/WebApp/Components/Chatbot/ChatState.cs
namespace eShop.WebApp.Components.Chatbot;

public record ChatMessage(string Text);

public class ChatState
{
    public IList<ChatMessage> Messages { get; } = new List<ChatMessage>();

    public bool IsBusy { get; private set; }

    public Task AddUserMessageAsync(string text) => Task.CompletedTask;
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "chat-state", "outcome": "verified", "at": "src/WebApp/Components/Chatbot/ChatState.cs:5" },
    { "kind": "member.exists", "element": "chat-state-messages", "outcome": "verified", "at": "src/WebApp/Components/Chatbot/ChatState.cs:7" },
    { "kind": "member.type", "element": "chat-state-messages", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/WebApp/Components/Chatbot/ChatState.cs:9", "messageContains": "IsBusy" },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Components/Chatbot/ChatState.cs:11", "count": 0 }
  ]
}
```

## bindable-changed

The exception to `event.unmodelled`: a Bindable Property's `XChanged` callback. `Quantity` has the Bindable stereotype, and its `property.bindable` claim reads `QuantityChanged`, so the callback is located and isn't reported, though no Event Emitter models it. `property.bindable` has no phase yet, so neither has this case.

```json case
{ "phase": "later", "cases": ["optin/unmodelled.member"], "traces": ["ESC-FE-24"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp",
        "elements": [
          { "id": "quantity-input", "type": "Component", "name": "QuantityInput",
            "children": [
              { "id": "quantity-input-quantity", "type": "Property", "name": "Quantity", "typeRef": { "type": "int" }, "stereotypes": { "Bindable": {} } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```razor file src/WebApp/Components/Cart/QuantityInput.razor
<input type="number" min="1" value="@Quantity" @onchange="OnChange" />

@code {
    [Parameter] public int Quantity { get; set; }

    [Parameter] public EventCallback<int> QuantityChanged { get; set; }

    private async Task OnChange(ChangeEventArgs e)
    {
        Quantity = int.Parse((string)e.Value!);
        await QuantityChanged.InvokeAsync(Quantity);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "property.bindable", "element": "quantity-input-quantity", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Components/Cart/QuantityInput.razor:6", "count": 0, "note": "Located by the Bindable claim" },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Components/Cart/QuantityInput.razor:4", "count": 0 }
  ]
}
```

## model-definition-members

A Model Definition's public property with no Property (`definition.property.unmodelled`) and public method with no Operation (`definition.operation.unmodelled`). `Id` and `Name`, the record's positional parameters, are modelled.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.member"], "traces": ["ESC-FE-44", "ESC-FE-47"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp",
        "elements": [
          { "id": "catalog-item", "type": "Model Definition", "name": "CatalogItem",
            "children": [
              { "id": "catalog-item-id", "type": "Property", "name": "Id", "typeRef": { "type": "int" } },
              { "id": "catalog-item-name", "type": "Property", "name": "Name", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/WebApp/Catalog/CatalogItem.cs
namespace eShop.WebApp.Catalog;

public record CatalogItem(int Id, string Name)
{
    public decimal Price { get; init; }

    public string FormatPrice() => Price.ToString("C");
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "catalog-item-name", "outcome": "verified", "at": "src/WebApp/Catalog/CatalogItem.cs:3" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/WebApp/Catalog/CatalogItem.cs:5", "messageContains": "Price" },
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/WebApp/Catalog/CatalogItem.cs:7", "messageContains": "FormatPrice" },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Catalog/CatalogItem.cs:3", "count": 0 }
  ]
}
```

## service-proxy-overload

A Service Proxy's public method with no Operation, overload-aware (`proxy.operation.unmodelled`). The modelled `GetCatalogItems(pageIndex, pageSize)` matches the first overload by its parameters, so the second, with `brand`, is reported. eShopCustom's fix named the request the method sends; the spec gives the finding no such fix.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.member"], "traces": ["ESC-FE-54"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp",
        "elements": [
          { "id": "catalog-result", "type": "Model Definition", "name": "CatalogResult",
            "children": [
              { "id": "catalog-result-count", "type": "Property", "name": "Count", "typeRef": { "type": "long" } }
            ] },
          { "id": "catalog-service", "type": "Service Proxy", "name": "CatalogService",
            "children": [
              { "id": "catalog-service-get-items", "type": "Operation", "name": "GetCatalogItems", "typeRef": { "element": "catalog-result" },
                "children": [
                  { "id": "catalog-service-get-items-page-index", "type": "Parameter", "name": "pageIndex", "typeRef": { "type": "int" } },
                  { "id": "catalog-service-get-items-page-size", "type": "Parameter", "name": "pageSize", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/WebApp/Catalog/CatalogResult.cs
namespace eShop.WebApp.Catalog;

public record CatalogResult(long Count);
```

```csharp file src/WebApp/Services/CatalogService.cs
namespace eShop.WebApp.Services;

public class CatalogService(HttpClient httpClient)
{
    public Task<CatalogResult> GetCatalogItems(int pageIndex, int pageSize)
    {
        return GetCatalogItems(pageIndex, pageSize, brand: null);
    }

    public async Task<CatalogResult> GetCatalogItems(int pageIndex, int pageSize, int? brand)
    {
        var uri = $"api/catalog/items?pageIndex={pageIndex}&pageSize={pageSize}&brand={brand}";
        return (await httpClient.GetFromJsonAsync<CatalogResult>(uri))!;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "catalog-service-get-items", "outcome": "verified", "at": "src/WebApp/Services/CatalogService.cs:5" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/WebApp/Services/CatalogService.cs:10", "messageContains": "GetCatalogItems" },
    { "rule": "optin/unmodelled.member", "subject": "src/WebApp/Services/CatalogService.cs:5", "count": 0 }
  ]
}
```

## proxy-overloads-and-methods-sending-nothing

A Proxy of the standard User Interface designer (2.0.0): the designer names each Proxy Operation uniquely, so of `GetCatalogItems`' two overloads only one can be stated, and the modelled one is located; and its validation requires a Proxy Operation to call a backend operation or describe an HTTP request, so `Dispose` and the static `Describe`, which send none, can't be one. None of them is asked for. `GetBrands` sends a request and isn't modelled, so it is. Unlike the 1.x designer's Service Proxy (`service-proxy-overload`), which reports an overload.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.member"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebAppComponents", "type": "User Interface Package",
        "elements": [
          { "id": "catalog-service", "type": "Proxy", "name": "CatalogService", "children": [
            { "id": "catalog-service-get-items", "type": "Proxy Operation", "name": "GetCatalogItems", "children": [
              { "id": "catalog-service-get-items-page-index", "type": "Parameter", "name": "pageIndex", "typeRef": { "type": "int" } },
              { "id": "catalog-service-get-items-page-size", "type": "Parameter", "name": "pageSize", "typeRef": { "type": "int" } }
            ] }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/WebAppComponents/Services/CatalogService.cs
using System.Net.Http.Json;

namespace eShop.WebAppComponents.Services;

public class CatalogService(HttpClient httpClient) : IDisposable
{
    public Task<string[]?> GetCatalogItems(int pageIndex, int pageSize) =>
        httpClient.GetFromJsonAsync<string[]>($"api/catalog/items?pageIndex={pageIndex}&pageSize={pageSize}");

    public Task<string[]?> GetCatalogItems(IEnumerable<int> ids) =>
        httpClient.GetFromJsonAsync<string[]>($"api/catalog/items/by?ids={string.Join("&ids=", ids)}");

    public Task<string[]?> GetBrands() => httpClient.GetFromJsonAsync<string[]>("api/catalog/catalogbrands");

    public static string Describe(string name) => name.ToUpperInvariant();

    public void Dispose() => httpClient.Dispose();
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "catalog-service-get-items", "outcome": "verified", "at": "src/WebAppComponents/Services/CatalogService.cs:7" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/WebAppComponents/Services/CatalogService.cs:13", "messageContains": "GetBrands" },
    { "rule": "optin/unmodelled.member", "subject": "src/WebAppComponents/Services/CatalogService.cs:10", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/WebAppComponents/Services/CatalogService.cs:15", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/WebAppComponents/Services/CatalogService.cs:17", "count": 0 }
  ]
}
```

## java-entity-methods

The Java spike's check: unmodelled non-private methods and constructors of an entity, except the no-arg constructor JPA needs, field accessors, and `equals`, `hashCode` and `toString`. The public constructor with a parameter and `reassign` are reported. The protected no-arg constructor isn't public, and `getCustomerId` is the accessor of the modelled `CustomerId`, part of the property its `member.exists` claim located. How the rule treats `equals`, `hashCode` and `toString` overrides the spec doesn't say, so the class has none.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.member"], "traces": ["JAV-DOM-21"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop.Backend", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```java file src/main/java/com/eshop/backend/domain/Wishlist.java
package com.eshop.backend.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.time.Instant;
import java.util.UUID;

@Entity
public class Wishlist {
    @Id
    private UUID id;
    private UUID customerId;
    private Instant createdAt;

    protected Wishlist() {
    }

    public Wishlist(UUID customerId) {
        this.id = UUID.randomUUID();
        this.customerId = customerId;
        this.createdAt = Instant.now();
    }

    public UUID getId() { return id; }

    public UUID getCustomerId() { return customerId; }

    public void reassign(UUID customerId) {
        this.customerId = customerId;
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/main/java/com/eshop/backend/domain/Wishlist.java:18", "messageContains": "Wishlist" },
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/main/java/com/eshop/backend/domain/Wishlist.java:28", "messageContains": "reassign" },
    { "rule": "optin/unmodelled.member", "subject": "src/main/java/com/eshop/backend/domain/Wishlist.java:15", "count": 0, "note": "The no-arg constructor, and not public" },
    { "rule": "optin/unmodelled.member", "subject": "src/main/java/com/eshop/backend/domain/Wishlist.java:26", "count": 0, "note": "An accessor of a modelled attribute" }
  ]
}
```

## java-entity-field

The Java spike also reported an unmodelled non-static field of a modelled class, whatever its access. A private field with no accessors is no public member, so the house pack adds a convention for fields of JPA types. `customerId` is located by its attribute's claim; `createdAt` isn't modelled.

```json case
{ "phase": 4, "base": "java-entity-methods", "cases": ["optin/unmodelled.member"], "traces": ["JAV-DOM-32"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa", "house.backend"], "language": "java" }
```

```json pack
{
  "id": "house.backend",
  "version": "1.0.0",
  "requires": [{ "id": "intent.jpa", "version": "^1.0.0" }],
  "languages": ["java"],
  "designers": ["Domain"],
  "conventions": {
    "Domain": {
      "members": [
        { "kind": "field", "find": "type with @(?:Entity|Embeddable|MappedSuperclass)\\b > field with \\bprivate (?!static\\b)", "name": "{Class}.{Member}", "files": "src/main/java/**/domain/**/*.java" }
      ]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/main/java/com/eshop/backend/domain/Wishlist.java:13", "messageContains": "createdAt" },
    { "rule": "optin/unmodelled.member", "subject": "src/main/java/com/eshop/backend/domain/Wishlist.java:12", "count": 0 }
  ]
}
```

## java-handler-method

An unmodelled handler method of a controller. The house convention finds every mapped method of a `@RestController`; `getWishlist` is located by the endpoint claims of the Operation it implements, and `clearWishlist` isn't modelled. An annotated method's node starts at its first annotation.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.member"], "traces": ["JAV-SVC-05"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc", "house.backend"], "language": "java" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Services",
        "elements": [
          { "id": "wishlists", "type": "Service", "name": "WishlistsService",
            "children": [
              { "id": "get-wishlist", "type": "Operation", "name": "getWishlist",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "api/customers/{customerId}/wishlist" } },
                "children": [
                  { "id": "get-wishlist-customer-id", "type": "Parameter", "name": "customerId", "typeRef": { "type": "guid" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Route" } } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop.Backend", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```json pack
{
  "id": "house.backend",
  "version": "1.0.0",
  "requires": [{ "id": "intent.springmvc", "version": "^1.0.0" }],
  "languages": ["java"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "members": [
        { "kind": "handler method", "find": "type with @RestController\\b > method with @(?:Get|Post|Put|Delete|Patch|Request)Mapping\\b", "name": "{Class}.{Method}", "files": "src/main/java/**/api/**/*.java" }
      ]
    }
  }
}
```

```java file src/main/java/com/eshop/backend/api/WishlistsController.java
package com.eshop.backend.api;

import com.eshop.backend.application.WishlistService;
import java.util.List;
import java.util.UUID;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class WishlistsController {
    private final WishlistService wishlists;

    public WishlistsController(WishlistService wishlists) {
        this.wishlists = wishlists;
    }

    @GetMapping("/api/customers/{customerId}/wishlist")
    public List<WishlistItemDto> getWishlist(@PathVariable UUID customerId) {
        return wishlists.get(customerId);
    }

    @DeleteMapping("/api/customers/{customerId}/wishlist")
    public void clearWishlist(@PathVariable UUID customerId) {
        wishlists.clear(customerId);
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/main/java/com/eshop/backend/api/WishlistsController.java:24", "messageContains": "clearWishlist" },
    { "rule": "optin/unmodelled.member", "subject": "src/main/java/com/eshop/backend/api/WishlistsController.java:19", "count": 0 }
  ]
}
```

## java-bound-parameter

An unmodelled bound handler parameter. The convention finds every `@PathVariable`, `@RequestParam`, `@RequestBody` or `@RequestHeader` parameter of a controller's methods. `customerId` is located by its `parameter.source` claim; `sort` isn't modelled.

```json case
{ "phase": 4, "base": "java-handler-method", "cases": ["optin/unmodelled.member"], "traces": ["JAV-SVC-12"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc", "house.backend"], "language": "java" }
```

```json pack
{
  "id": "house.backend",
  "version": "1.0.0",
  "requires": [{ "id": "intent.springmvc", "version": "^1.0.0" }],
  "languages": ["java"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "members": [
        { "kind": "bound parameter", "find": "type with @RestController\\b > method > parameter with @(?:PathVariable|RequestParam|RequestBody|RequestHeader)\\b", "name": "{Method}({Name})", "files": "src/main/java/**/api/**/*.java" }
      ]
    }
  }
}
```

```java file src/main/java/com/eshop/backend/api/WishlistsController.java
package com.eshop.backend.api;

import com.eshop.backend.application.WishlistService;
import java.util.List;
import java.util.UUID;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class WishlistsController {
    private final WishlistService wishlists;

    public WishlistsController(WishlistService wishlists) {
        this.wishlists = wishlists;
    }

    @GetMapping("/api/customers/{customerId}/wishlist")
    public List<WishlistItemDto> getWishlist(
            @PathVariable UUID customerId,
            @RequestParam(required = false) String sort) {
        return wishlists.get(customerId, sort);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.source", "element": "get-wishlist-customer-id", "outcome": "verified", "at": "src/main/java/com/eshop/backend/api/WishlistsController.java:21" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/main/java/com/eshop/backend/api/WishlistsController.java:22", "messageContains": "sort" },
    { "rule": "optin/unmodelled.member", "subject": "src/main/java/com/eshop/backend/api/WishlistsController.java:21", "count": 0 }
  ]
}
```

## java-record-components

A DTO's DTO-Fields are a Java record's components. `price` isn't modelled.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.member"], "traces": ["JAV-SVC-20"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.java"], "language": "java" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Services",
        "elements": [
          { "id": "wishlist-item-dto", "type": "DTO", "name": "WishlistItemDto",
            "children": [
              { "id": "wishlist-item-dto-product-id", "type": "DTO-Field", "name": "ProductId", "typeRef": { "type": "guid" } },
              { "id": "wishlist-item-dto-product-name", "type": "DTO-Field", "name": "ProductName", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop.Backend", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```java file src/main/java/com/eshop/backend/api/WishlistItemDto.java
package com.eshop.backend.api;

import java.math.BigDecimal;
import java.util.UUID;

public record WishlistItemDto(
        UUID productId,
        String productName,
        BigDecimal price) {
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "wishlist-item-dto-product-id", "outcome": "verified", "at": "src/main/java/com/eshop/backend/api/WishlistItemDto.java:7" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/main/java/com/eshop/backend/api/WishlistItemDto.java:9", "messageContains": "price" },
    { "rule": "optin/unmodelled.member", "subject": "src/main/java/com/eshop/backend/api/WishlistItemDto.java:7", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/main/java/com/eshop/backend/api/WishlistItemDto.java:8", "count": 0 }
  ]
}
```

## java-storefront

The Java spike's storefront: a Front End Contract is an exported interface, and a Proxy the object of its methods (`export const api = { ... }`). The house pack gives the spike's own designer vocabulary, and says what each element type can state. `price` isn't a Field of `ProductDto` (unmodelled Contract properties), and `getProducts` isn't an Operation of `Api` (unmodelled Proxy methods). The modelled ones are located: TypeScript names a method in camel case.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.member"], "traces": ["JAV-FE-16", "JAV-FE-18"], "requirements": ["R28", "R49"], "packs": ["intent.core", "house.storefront"], "language": "typescript" }
```

```json pack
{
  "id": "house.storefront",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["typescript"],
  "designers": ["Front End"],
  "vocabulary": {
    "Front End": {
      "entries": [
        { "kind": "type.exists", "for": { "element": "Contract" }, "value": "self", "bindings": { "Name": "name" }, "own": ["Name"], "source": "the Contract" },
        { "kind": "member.exists", "for": { "element": "Field" }, "value": "self", "dependsOn": "parent:type.exists", "source": "the Field" },
        { "kind": "type.exists", "for": { "element": "Proxy" }, "value": "self", "bindings": { "Name": "name" }, "own": ["Name"], "source": "the Proxy" },
        { "kind": "member.exists", "for": { "element": "Proxy Operation" }, "value": "self", "dependsOn": "parent:type.exists", "source": "the Proxy Operation" }
      ]
    }
  },
  "conventions": {
    "Front End": {
      "types": {
        "Contract": { "typeKinds": ["interface", "typeAlias"], "states": ["property"], "note": "The storefront: a Contract is an exported interface" },
        "Proxy": { "name": "{Name|camel}", "typeKinds": ["object"], "states": ["method"], "note": "The storefront: a Proxy is the object of its methods" }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Front End": {
      "packages": [{
        "id": "storefront", "name": "Storefront",
        "elements": [
          { "id": "product-dto", "type": "Contract", "name": "ProductDto",
            "children": [
              { "id": "product-dto-id", "type": "Field", "name": "Id", "typeRef": { "type": "guid" } },
              { "id": "product-dto-name", "type": "Field", "name": "Name", "typeRef": { "type": "string" } }
            ] },
          { "id": "api", "type": "Proxy", "name": "Api",
            "children": [
              { "id": "api-get-product-by-id", "type": "Proxy Operation", "name": "GetProductById", "typeRef": { "element": "product-dto" },
                "children": [
                  { "id": "api-get-product-by-id-id", "type": "Parameter", "name": "id", "typeRef": { "type": "guid" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Storefront", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```typescript file src/api/types.ts
export interface ProductDto {
    Id: string;
    Name: string;
    Price: number;
}
```

```typescript file src/api/client.ts
import type { ProductDto } from './types';

async function request<T>(method: string, path: string): Promise<T> {
    const res = await fetch(path, { method });
    return (await res.json()) as T;
}

export const api = {
    getProductById(id: string): Promise<ProductDto> {
        return request<ProductDto>('GET', `/api/products/${encodeURIComponent(id)}`);
    },
    getProducts(): Promise<ProductDto[]> {
        return request<ProductDto[]>('GET', '/api/products');
    },
};
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "product-dto-name", "outcome": "verified", "at": "src/api/types.ts:3" },
    { "kind": "member.exists", "element": "api-get-product-by-id", "outcome": "verified", "at": "src/api/client.ts:9" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/api/types.ts:4", "messageContains": "ProductDto.Price" },
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/api/client.ts:12", "messageContains": "api.getProducts" },
    { "rule": "optin/unmodelled.member", "subject": "src/api/types.ts:3", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/api/client.ts:9", "count": 0 }
  ]
}
```

## handler-message-method

An Integration Event Handler's `Handle` takes the message its subscription names, so the subscription's reading is there and the method is its code, not an unmodelled member. `Retry` is a public method nothing in the model states: reported.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.member"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.nservicebus"], "language": "csharp" }
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
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```csharp file src/Ordering.API/Application/IntegrationEvents/EventHandling/OrderPaymentSucceededHandler.cs
namespace Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderPaymentSucceededHandler : IHandleMessages<OrderPaymentSucceededIntegrationEvent>
{
    public Task Handle(OrderPaymentSucceededIntegrationEvent message, IMessageHandlerContext context) => Task.CompletedTask;

    public Task Retry(IMessageHandlerContext context) => Task.CompletedTask;
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.member", "subject": "src/Ordering.API/Application/IntegrationEvents/EventHandling/OrderPaymentSucceededHandler.cs:5", "count": 0, "note": "The subscription's code" },
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Ordering.API/Application/IntegrationEvents/EventHandling/OrderPaymentSucceededHandler.cs:7", "messageContains": "OrderPaymentSucceededHandler.Retry" }
  ]
}
```

## mvc-view-members-not-asked

An MVC view takes no parameters, and what a user does on it is a form or a link to a controller's action, so none of its members is a Property or Component Operation the designer can state: its `@inject` is its Uses of what it injects, and its `@functions` field and helper method are written in code. Nothing is asked for.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.member"], "kinds": ["type.exists"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Identity.API", "type": "User Interface Package",
        "elements": [
          { "id": "diagnostics", "type": "Page", "name": "Index", "stereotypes": { "Page": { } } }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```cshtml file src/Identity.API/Views/Diagnostics/Index.cshtml
@inject Microsoft.Extensions.Configuration.IConfiguration Configuration
<h1>Authentication Cookie</h1>
<p>@Describe(Configuration["Issuer"])</p>

@functions {
    private int shown;

    private string Describe(string issuer) => $"Issued by {issuer}";
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "diagnostics", "outcome": "verified", "at": "src/Identity.API/Views/Diagnostics/Index.cshtml:1" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "count": 0 }
  ]
}
```

## maui-view-members

What a .NET MAUI view's members are to the designer (`intent.maui`): a page's Properties are its query parameters (Shell has no route parameters), and a component's are the inputs a parent supplies, which only a `BindableProperty` lets it bind. So a page's property a `[QueryProperty]` binds and the model leaves out is one finding; its other properties, a component's plain property and the static `BindableProperty` backing a modelled one are written in code, and never asked for.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.member"], "kinds": ["member.exists"], "requirements": ["R44", "R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { } } },
          { "id": "badge", "type": "Component", "name": "BadgeView", "children": [
            { "id": "badge-text", "type": "Property", "name": "Text", "typeRef": { "type": "string" }, "stereotypes": { "[Bindable]": { } } }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```xml file src/Shop/Views/CatalogView.xaml
<ContentPage x:Class="Shop.Views.CatalogView"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:views="clr-namespace:Shop.Views">
    <views:BadgeView Text="{Binding Count}" />
</ContentPage>
```

```csharp file src/Shop/Views/CatalogView.xaml.cs
namespace Shop.Views;

[QueryProperty(nameof(BrandId), "brand")]
public partial class CatalogView : ContentPage
{
    public string BrandId { get; set; } = "";

    public bool IsBusy { get; set; }
}
```

```csharp file src/Shop/Views/BadgeView.cs
namespace Shop.Views;

public class BadgeView : Grid
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(BadgeView), "0");

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Caption { get; set; } = "";
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/Shop/Views/CatalogView.xaml.cs:6", "messageContains": "`CatalogView.BrandId` is a public property of `CatalogView`" }
  ],
  "noOtherFindings": true
}
```

## class-store-dependencies

A MobX root store creates each store and hands it itself (`this.activityStore = new ActivityStore(this)`), and each store keeps the root store it is handed (`this.rootStore = rootStore`). Both members hold a class of the code's own the class depends on, which the model states as a Uses, not a Property, so neither is asked for. The store's own state still is. React-router 5's `RouteComponentProps` props of a page a route mounts (`match`, `history`, `location`, `staticContext`) are the router's.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.member"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "client", "name": "client-app", "type": "User Interface Package",
        "elements": [
          { "id": "root-store", "type": "Store", "name": "RootStore" },
          { "id": "activity-store", "type": "Store", "name": "ActivityStore", "children": [
            { "id": "activity-store-activities", "type": "Property", "name": "activities", "typeRef": { "type": "string", "collection": true } }
          ] },
          { "id": "details", "type": "Page", "name": "ActivityDetails", "stereotypes": { "Page": { "Route": "activities/{id}" } } }
        ]
      }],
      "associations": [
        { "id": "root-uses-activities", "type": "Uses", "source": { "element": "root-store" }, "target": { "element": "activity-store" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "client-app", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.member" } ] }
        } }]
    }
  }
}
```

```ts file src/stores/activityStore.ts
import { observable } from 'mobx';
import { RootStore } from './rootStore';

export default class ActivityStore {
    rootStore: RootStore;

    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;
    }

    @observable activities: string[] = [];

    @observable selected: string | null = null;
}
```

```ts file src/stores/rootStore.ts
import ActivityStore from './activityStore';

export class RootStore {
    activityStore: ActivityStore;

    constructor() {
        this.activityStore = new ActivityStore(this);
    }
}
```

```tsx file src/App.tsx
import React from 'react';
import { Route, Switch } from 'react-router-dom';
import ActivityDetails from './ActivityDetails';

const App: React.FC = () => (
    <Switch>
        <Route path='/activities/:id' component={ActivityDetails} />
    </Switch>
);

export default App;
```

```tsx file src/ActivityDetails.tsx
import React from 'react';
import { RouteComponentProps } from 'react-router-dom';

const ActivityDetails: React.FC<RouteComponentProps<{ id: string }>> = ({ match, history }) => (
    <button onClick={() => history.push('/')}>{match.params.id}</button>
);

export default ActivityDetails;
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.member", "subject": "src/stores/activityStore.ts:5", "count": 0, "note": "The root store it is handed: a Uses" },
    { "rule": "optin/unmodelled.member", "subject": "src/stores/rootStore.ts:4", "count": 0, "note": "A store it creates: a Uses" },
    { "rule": "optin/unmodelled.member", "subject": "src/ActivityDetails.tsx:4", "count": 0, "note": "The router's props" },
    { "rule": "optin/unmodelled.member", "severity": "error", "subject": "src/stores/activityStore.ts:13", "messageContains": "`ActivityStore.selected`" }
  ]
}
```
