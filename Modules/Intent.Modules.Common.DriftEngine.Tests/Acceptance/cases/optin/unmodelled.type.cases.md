# optin/unmodelled.type

The opt-in rule that reports an in-scope type no claim located (rules.md, "Opt-in must be modelled rules"). By principle 1 such a type is never drift on its own: the rule is off until the Root Folder's `Drift Check` lists it under `Opt-in Rules`, it reads only in-scope code, and each finding can be excepted with a reason (R49).

The row is all the rule needs (user decision, 2026-11-16): its Glob says where to look, as a Common.Drift `own` entry's `files` did, and every type there the model doesn't describe is reported, with the designer whose elements describe the types in its folder, or the nearest folder above it, when there is one. A module that wants more, such as only a base class's subclasses, gives an element type's convention a `find`: within the convention's `files` only the types it finds are reported, as that element type. Most cases here bring a small inline house pack to show that narrowing; `glob-only`, `whole-scope` and `pattern-narrows-its-files` show the rule without one and beside one. A type is located when a claim's reading came from it or from code inside it, so a modelled type is never reported. A type nested in another is part of it, never reported on its own.

A finding's subject is the file and the line where the type's node starts. For an annotated Java type that is its first annotation.

## off-by-default

The house pack's convention says every class deriving `Entity` under `src/Domain` is a Domain Class. `Buyer` follows it and isn't modelled, but no `Opt-in Rules` row turns the rule on, so nothing is reported: code the model doesn't mention isn't drift (principle 1).

```json case
{ "phase": 2, "cases": ["optin/unmodelled.type"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
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
      "types": {
        "Class": { "find": "type with :Entity\\b", "files": "src/Domain/**/*.cs", "note": "Common.Drift's own entry for entities" }
      }
    }
  }
}
```

```csharp file src/Domain/OrderAggregate/Order.cs
namespace Ordering.Domain.OrderAggregate;

public class Order : Entity, IAggregateRoot
{
    public string Description { get; private set; }
}
```

```csharp file src/Domain/BuyerAggregate/Buyer.cs
namespace Ordering.Domain.BuyerAggregate;

public class Buyer : Entity, IAggregateRoot
{
    public string IdentityGuid { get; private set; }

    public string Name { get; private set; }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "count": 0, "note": "The rule is off" }
  ]
}
```

## on

The same model and code, with an `Opt-in Rules` row turning the rule on. `Buyer` follows the Class convention and no claim located it, so it is reported, an error by default. `Order` follows the convention too, but its `type.exists` claim located it, so it isn't. eShopCustom's Domain module reported this as `type.unmodelled` in adopted code; the engine reports it only once the team opts in.

```json case
{ "phase": 2, "base": "off-by-default", "cases": ["optin/unmodelled.type"], "traces": ["ESC-DOM-22"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/OrderAggregate/Order.cs:3" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Domain/BuyerAggregate/Buyer.cs:3", "messageContains": "Buyer" },
    { "rule": "optin/unmodelled.type", "subject": "src/Domain/OrderAggregate/Order.cs:3", "count": 0, "note": "Located by its type.exists claim" }
  ]
}
```

## narrowed-by-glob

The `Opt-in Rules` row has a `Glob`, so the rule reports only under `src/Domain/OrderAggregate`, the area the team has finished modelling. `OrderItem` is there and is reported; `Buyer` is outside the glob and isn't.

```json case
{ "phase": 2, "base": "off-by-default", "cases": ["optin/unmodelled.type"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type", "Glob": "src/Domain/OrderAggregate/**" } ] }
        } }]
    }
  }
}
```

```csharp file src/Domain/OrderAggregate/OrderItem.cs
namespace Ordering.Domain.OrderAggregate;

public class OrderItem : Entity
{
    public int ProductId { get; private set; }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Domain/OrderAggregate/OrderItem.cs:3", "messageContains": "OrderItem" },
    { "rule": "optin/unmodelled.type", "subject": "src/Domain/BuyerAggregate/Buyer.cs:3", "count": 0, "note": "Outside the row's Glob" }
  ]
}
```

## excepted

The rule is on, and an `Exceptions` row names `Buyer` by its type name, with a reason. The team has chosen not to model it, so it isn't reported, and the exception matched something, so it isn't stale either.

```json case
{ "phase": 2, "base": "off-by-default", "cases": ["optin/unmodelled.type"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": {
            "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ],
            "Exceptions": [ { "Rule": "optin/unmodelled.type", "Match": "Buyer", "Reason": "The buyer aggregate is owned by the Identity team and modelled in their application" } ]
          }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "subject": "src/Domain/BuyerAggregate/Buyer.cs:3", "count": 0, "note": "Excepted" },
    { "rule": "optin/staleException", "count": 0 }
  ]
}
```

## excepted-several-in-one-row

One `Exceptions` row lists several types, one per line, for one reason: code skipped for the same reason is one row. `Buyer` is excepted by it; `Ghost`, which the code doesn't have, is the row's one stale entry, and the warning names it alone.

```json case
{ "phase": 2, "base": "off-by-default", "cases": ["optin/unmodelled.type"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": {
            "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ],
            "Exceptions": [ { "Rule": "optin/unmodelled.type", "Match": "Buyer\nGhost", "Reason": "Owned by other teams' applications" } ]
          }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "subject": "src/Domain/BuyerAggregate/Buyer.cs:3", "count": 0, "note": "Excepted" },
    { "rule": "optin/staleException", "severity": "warning", "messageContains": "`Ghost`" },
    { "rule": "optin/staleException", "messageContains": "Buyer", "count": 0 }
  ]
}
```

## stale-exception

The exception names `Customer`, which no finding of the rule matches: there is no such type. It is reported as an `optin/staleException` warning, and `Buyer`, which it doesn't cover, is still reported.

```json case
{ "phase": 2, "base": "off-by-default", "cases": ["optin/unmodelled.type"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": {
            "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ],
            "Exceptions": [ { "Rule": "optin/unmodelled.type", "Match": "Customer", "Reason": "Renamed to Buyer in the domain model" } ]
          }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/staleException", "severity": "warning", "messageContains": "Customer" },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Domain/BuyerAggregate/Buyer.cs:3", "messageContains": "Buyer" }
  ]
}
```

## exception-without-reason

The exception names `Buyer` but gives no reason. That is a configuration error, `config/exceptionReason`, on the Root Folder that holds the `Drift Check`: an exception records a decision, and a decision needs its reason.

```json case
{ "phase": 2, "base": "off-by-default", "cases": ["optin/unmodelled.type"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": {
            "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ],
            "Exceptions": [ { "Rule": "optin/unmodelled.type", "Match": "Buyer" } ]
          }
        } }]
    }
  }
}
```

```json expect
{
  "config": [
    { "rule": "config/exceptionReason", "severity": "error", "subject": "root", "messageContains": "Buyer" }
  ]
}
```

## out-of-scope

The rule is on, but `Buyer` sits in a folder a scope row excludes. Opt-in rules report only in-scope code (R49), and the excluded file is never read (R22), so nothing is reported.

```json case
{ "phase": 2, "base": "off-by-default", "cases": ["optin/unmodelled.type", "scope/gate"], "requirements": ["R22", "R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [
            { "Glob": "src/**", "Scope": "In" },
            { "Glob": "src/Domain/BuyerAggregate/**", "Scope": "Out", "Reason": "Buyers move to the Identity service; not adopted here" }
          ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "count": 0 }
  ],
  "filesRead": { "include": ["src/Domain/OrderAggregate/Order.cs"], "exclude": ["src/Domain/BuyerAggregate/**"] }
}
```

## every-domain-convention

eShopCustom's Domain module reported an unmodelled enum, `INotification` type, `Entity` or `ValueObject` subtype, or `IRepository<T>` interface. Each becomes a convention on the Domain element type it stands for: Enum, Domain Event, Class, Value Object and Repository. With the rule on, each unmodelled type is reported once, under its own convention.

```json case
{ "phase": 2, "base": "on", "without": ["src/Domain/BuyerAggregate/Buyer.cs"], "cases": ["optin/unmodelled.type"], "traces": ["ESC-DOM-22"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
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
      "types": {
        "Class": { "find": "type with :Entity\\b", "files": "src/Domain/**/*.cs" },
        "Value Object": { "find": "type with :ValueObject\\b", "files": "src/Domain/**/*.cs" },
        "Enum": { "find": "type with \\benum \\w+", "files": "src/Domain/**/*.cs" },
        "Domain Event": { "find": "type with :INotification\\b", "files": "src/Domain/**/*.cs" },
        "Repository": { "find": "type with :IRepository<", "files": "src/Domain/**/*.cs" }
      }
    }
  }
}
```

```csharp file src/Domain/BuyerAggregate/CardType.cs
namespace Ordering.Domain.BuyerAggregate;

public enum CardType
{
    Amex = 1,
    Visa = 2,
    MasterCard = 3
}
```

```csharp file src/Domain/OrderAggregate/Address.cs
namespace Ordering.Domain.OrderAggregate;

public class Address : ValueObject
{
    public string Street { get; private set; }

    public string City { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
    }
}
```

```csharp file src/Domain/Events/OrderStartedDomainEvent.cs
using MediatR;

namespace Ordering.Domain.Events;

public record class OrderStartedDomainEvent(Order Order, string UserId) : INotification;
```

```csharp file src/Domain/BuyerAggregate/IBuyerRepository.cs
namespace Ordering.Domain.BuyerAggregate;

public interface IBuyerRepository : IRepository<Buyer>
{
    Buyer Add(Buyer buyer);
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Domain/BuyerAggregate/CardType.cs:3", "messageContains": "CardType" },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Domain/OrderAggregate/Address.cs:3", "messageContains": "Address" },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Domain/Events/OrderStartedDomainEvent.cs:5", "messageContains": "OrderStartedDomainEvent" },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Domain/BuyerAggregate/IBuyerRepository.cs:3", "messageContains": "IBuyerRepository" },
    { "rule": "optin/unmodelled.type", "subject": "src/Domain/OrderAggregate/Order.cs:3", "count": 0 }
  ]
}
```

## entity-configuration

An `IEntityTypeConfiguration<T>` for a class the model doesn't have. The house convention finds every configuration class under `src/Infrastructure`. `OrderEntityTypeConfiguration` holds the `ToTable` that `Order`'s `table.name` claim read, so it is located; `BuyerEntityTypeConfiguration` configures an unmodelled class, so nothing located it and it is reported. The inline `modelBuilder.Entity<T>(...)` form of the same check is a statement, not a type, so a type convention can't find it.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.type"], "traces": ["ESC-DOM-21"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.efcore", "house.ordering"], "language": "csharp" }
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
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
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
      "types": {
        "Class": { "find": "type with IEntityTypeConfiguration<", "files": "src/Infrastructure/**/*.cs", "note": "Each entity's configuration class" }
      }
    }
  }
}
```

```csharp file src/Domain/OrderAggregate/Order.cs
namespace Ordering.Domain.OrderAggregate;

public class Order : Entity, IAggregateRoot
{
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

        orderConfiguration.HasKey(o => o.Id);
    }
}
```

```csharp file src/Infrastructure/EntityConfigurations/BuyerEntityTypeConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.BuyerAggregate;

namespace Ordering.Infrastructure.EntityConfigurations;

class BuyerEntityTypeConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> buyerConfiguration)
    {
        buyerConfiguration.ToTable("buyers");

        buyerConfiguration.HasKey(b => b.Id);
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
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Infrastructure/EntityConfigurations/BuyerEntityTypeConfiguration.cs:7", "messageContains": "BuyerEntityTypeConfiguration" },
    { "rule": "optin/unmodelled.type", "subject": "src/Infrastructure/EntityConfigurations/OrderEntityTypeConfiguration.cs:7", "count": 0, "note": "Located by Order's table.name claim, whose reading came from inside it" }
  ]
}
```

## services-router

A Minimal API router, a static class that calls `MapGroup`, that no Service describes. `OrdersApi` holds the `MapGroup("api/orders")` the `Orders` Service's route claim read, so it is located. `CatalogApi` follows the same convention and isn't modelled, so it is reported (`services.service.unmodelled` in eShopCustom). An unmodelled endpoint inside a modelled router is a member: [unmodelled.member](unmodelled.member.cases.md) shows it.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.type"], "traces": ["ESC-SVC-25"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "orders", "type": "Service", "name": "Orders",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
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
      "types": {
        "Service": { "find": "type with \\.MapGroup\\(", "files": "src/**/Apis/**/*.cs", "note": "A router: a static class mapping a route group" }
      }
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

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync(CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }
}
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace Catalog.API.Apis;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog");

        api.MapGet("/items", GetAllItems);

        return app;
    }

    public static async Task<Ok<List<CatalogItem>>> GetAllItems([AsParameters] CatalogServices services)
    {
        return TypedResults.Ok(await services.Context.CatalogItems.ToListAsync());
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Catalog.API/Apis/CatalogApi.cs:3", "messageContains": "CatalogApi" },
    { "rule": "optin/unmodelled.type", "subject": "src/Ordering.API/Apis/OrdersApi.cs:3", "count": 0 }
  ]
}
```

## mediatr-request

An `IRequest` type, and the handler and validator of a request, that the model doesn't have (`services.command.unmodelled` and `.handler.unmodelled` in eShopCustom). One Command convention finds all three shapes. `CreateOrderCommand` is modelled and located by its `type.exists` claim. Its handler and validator would be located by `request.handler` and `request.validated` claims, which arrive with a later pack, so this case leaves them out of the code.

```json case
{ "phase": 2, "cases": ["optin/unmodelled.type"], "traces": ["ESC-SVC-56"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "create-order-command", "type": "Command", "name": "CreateOrderCommand",
            "children": [
              { "id": "create-order-command-user-id", "type": "DTO-Field", "name": "UserId", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
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
      "types": {
        "Command": { "find": "type with \\b(?:IRequest\\b|IRequestHandler<|AbstractValidator<)", "files": "src/Ordering.API/Application/**/*.cs", "note": "A request, its handler or its validator" }
      }
    }
  }
}
```

```csharp file src/Ordering.API/Application/Commands/CreateOrderCommand.cs
namespace Ordering.API.Application.Commands;

public record CreateOrderCommand(string UserId) : IRequest<bool>;
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommand.cs
namespace Ordering.API.Application.Commands;

public record CancelOrderCommand(int OrderNumber) : IRequest<bool>;
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs
namespace Ordering.API.Application.Commands;

public class CancelOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<CancelOrderCommand, bool>
{
    public async Task<bool> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetAsync(command.OrderNumber);
        orderToUpdate.SetCancelledStatus();
        return await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
```

```csharp file src/Ordering.API/Application/Validations/CancelOrderCommandValidator.cs
namespace Ordering.API.Application.Validations;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(order => order.OrderNumber).NotEmpty().WithMessage("No orderId found");
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Ordering.API/Application/Commands/CancelOrderCommand.cs:3", "messageContains": "CancelOrderCommand" },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:3", "messageContains": "CancelOrderCommandHandler" },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Ordering.API/Application/Validations/CancelOrderCommandValidator.cs:3", "messageContains": "CancelOrderCommandValidator" },
    { "rule": "optin/unmodelled.type", "subject": "src/Ordering.API/Application/Commands/CreateOrderCommand.cs:3", "count": 0 }
  ]
}
```

## integration-event

An `IntegrationEvent` subclass that no Message describes (`services.message.unmodelled`). The house pack gives the Eventing designer's Message a `type.exists` claim and a convention, so `OrderStartedIntegrationEvent` is located and `OrderStatusChangedToPaidIntegrationEvent` is reported.

```json case
{ "phase": 2, "cases": ["optin/unmodelled.type"], "traces": ["ESC-SVC-59"], "requirements": ["R49"], "packs": ["intent.core", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Eventing": {
      "packages": [{
        "id": "eventing", "name": "Ordering.Eventing",
        "elements": [
          { "id": "order-started", "type": "Message", "name": "OrderStartedIntegrationEvent",
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
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
        } }]
    }
  }
}
```

```json pack
{
  "id": "house.ordering",
  "version": "1.0.0",
  "languages": ["csharp"],
  "designers": ["Eventing"],
  "vocabulary": {
    "Eventing": {
      "entries": [
        { "kind": "type.exists", "for": { "element": "Message" }, "bindings": { "Type": "name" }, "own": ["Type"], "source": "the Message" }
      ],
      "nonClaims": [
        { "for": { "element": "Property" }, "reason": "Message properties aren't checked in this codebase yet" }
      ]
    }
  },
  "conventions": {
    "Eventing": {
      "types": {
        "Message": { "typeKinds": ["class", "record"], "find": "type with :IntegrationEvent\\b", "files": "src/Ordering.API/Application/IntegrationEvents/**/*.cs" }
      }
    }
  }
}
```

```csharp file src/Ordering.API/Application/IntegrationEvents/Events/OrderStartedIntegrationEvent.cs
namespace Ordering.API.Application.IntegrationEvents.Events;

public record OrderStartedIntegrationEvent(string UserId) : IntegrationEvent;
```

```csharp file src/Ordering.API/Application/IntegrationEvents/Events/OrderStatusChangedToPaidIntegrationEvent.cs
namespace Ordering.API.Application.IntegrationEvents.Events;

public record OrderStatusChangedToPaidIntegrationEvent(int OrderId, string OrderStatus) : IntegrationEvent;
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order-started", "outcome": "verified",
      "at": "src/Ordering.API/Application/IntegrationEvents/Events/OrderStartedIntegrationEvent.cs:3" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error",
      "subject": "src/Ordering.API/Application/IntegrationEvents/Events/OrderStatusChangedToPaidIntegrationEvent.cs:3",
      "messageContains": "OrderStatusChangedToPaidIntegrationEvent" },
    { "rule": "optin/unmodelled.type", "subject": "src/Ordering.API/Application/IntegrationEvents/Events/OrderStartedIntegrationEvent.cs:3", "count": 0 }
  ]
}
```

## event-handlers

A domain event handler and an integration event handler that the Services designer doesn't have (`*.unmodelled` in eShopCustom). Each follows its element type's convention: Domain Event Handler for `INotificationHandler<E>`, Integration Event Handler for `IIntegrationEventHandler<M>`. Nothing is modelled yet, so both are reported.

```json case
{ "phase": 2, "cases": ["optin/unmodelled.type"], "traces": ["ESC-SVC-63"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{ "id": "services", "name": "Ordering.Services", "elements": [] }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
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
      "types": {
        "Domain Event Handler": { "find": "type with \\bINotificationHandler<", "files": "src/Ordering.API/Application/**/*.cs" },
        "Integration Event Handler": { "find": "type with \\bIIntegrationEventHandler<", "files": "src/Ordering.API/Application/**/*.cs" }
      }
    }
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

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error",
      "subject": "src/Ordering.API/Application/DomainEventHandlers/OrderCancelledDomainEventHandler.cs:3",
      "messageContains": "OrderCancelledDomainEventHandler" },
    { "rule": "optin/unmodelled.type", "severity": "error",
      "subject": "src/Ordering.API/Application/IntegrationEvents/EventHandling/GracePeriodConfirmedIntegrationEventHandler.cs:3",
      "messageContains": "GracePeriodConfirmedIntegrationEventHandler" }
  ]
}
```

## application-service

An application service that isn't modelled (`services.appservice.unmodelled`). The convention finds a class that implements an interface named `I` plus its own name, so `OrderQueries` is reported and its interface isn't. eShopCustom also required exactly one implementation, and accepted a class registered as itself; both need a look across files that a convention's `find` can't make, so they would take a plug-in rule.

```json case
{ "phase": 2, "base": "event-handlers", "without": ["src/Ordering.API/Application/DomainEventHandlers/OrderCancelledDomainEventHandler.cs", "src/Ordering.API/Application/IntegrationEvents/EventHandling/GracePeriodConfirmedIntegrationEventHandler.cs"], "cases": ["optin/unmodelled.type"], "traces": ["ESC-SVC-69"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "house.ordering"], "language": "csharp" }
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
      "types": {
        "Service": { "find": "type with \\bclass (\\w+)(?:\\([^)]*\\))?:I\\1\\b", "files": "src/Ordering.API/Application/Queries/**/*.cs", "note": "An application service: X implementing IX" }
      }
    }
  }
}
```

```csharp file src/Ordering.API/Application/Queries/IOrderQueries.cs
namespace Ordering.API.Application.Queries;

public interface IOrderQueries
{
    Task<Order> GetOrderAsync(int id);
}
```

```csharp file src/Ordering.API/Application/Queries/OrderQueries.cs
namespace Ordering.API.Application.Queries;

public class OrderQueries(OrderingContext context) : IOrderQueries
{
    public async Task<Order> GetOrderAsync(int id)
    {
        return await context.Orders.FirstOrDefaultAsync(o => o.Id == id);
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Ordering.API/Application/Queries/OrderQueries.cs:3", "messageContains": "OrderQueries" },
    { "rule": "optin/unmodelled.type", "subject": "src/Ordering.API/Application/Queries/IOrderQueries.cs:3", "count": 0 }
  ]
}
```

## hosted-service

A `BackgroundService` that no Service describes (`services.hosted-service.unmodelled`).

```json case
{ "phase": 2, "base": "event-handlers", "without": ["src/Ordering.API/Application/DomainEventHandlers/OrderCancelledDomainEventHandler.cs", "src/Ordering.API/Application/IntegrationEvents/EventHandling/GracePeriodConfirmedIntegrationEventHandler.cs"], "cases": ["optin/unmodelled.type"], "traces": ["ESC-SVC-74"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "house.ordering"], "language": "csharp" }
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
      "types": {
        "Service": { "find": "type with :BackgroundService\\b", "files": "src/OrderProcessor/Services/**/*.cs", "note": "A hosted service" }
      }
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
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogDebug("GracePeriodManagerService background task is doing background work.");
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/OrderProcessor/Services/GracePeriodManagerService.cs:3", "messageContains": "GracePeriodManagerService" }
  ]
}
```

## grpc-service

A gRPC service class, one deriving the generated `<Service>Base`, that no Service describes (`services.grpc.unmodelled`). An unmodelled `.proto` message is `unmodelled.member`'s `proto-message-and-field`.

```json case
{ "phase": 2, "base": "event-handlers", "without": ["src/Ordering.API/Application/DomainEventHandlers/OrderCancelledDomainEventHandler.cs", "src/Ordering.API/Application/IntegrationEvents/EventHandling/GracePeriodConfirmedIntegrationEventHandler.cs"], "cases": ["optin/unmodelled.type"], "traces": ["ESC-SVC-80"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "house.ordering"], "language": "csharp" }
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
      "types": {
        "Service": { "find": "type with :\\w+\\.\\w+Base\\b", "files": "src/Basket.API/Grpc/**/*.cs", "note": "A gRPC service: derives the generated <Service>Base" }
      }
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
        var data = await repository.GetBasketAsync(request.BuyerId);
        return MapToCustomerBasketResponse(data);
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Basket.API/Grpc/BasketService.cs:3", "messageContains": "BasketService" }
  ]
}
```

## ui-located

The User Interface designer has a Page, a Model Definition and a Service Proxy, and the house pack gives each element type a convention: every `.razor` component, every record, and every class holding an `HttpClient` or a generated gRPC client. With the rule on, each modelled one is located by its own claims, so nothing is reported. The next three cases add one unmodelled type each.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.type"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui", "house.webapp"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp",
        "elements": [
          { "id": "catalog", "type": "Component", "name": "Catalog", "stereotypes": { "Page": { "Route": "/" } } },
          { "id": "catalog-item", "type": "Model Definition", "name": "CatalogItem",
            "children": [
              { "id": "catalog-item-id", "type": "Property", "name": "Id", "typeRef": { "type": "int" } },
              { "id": "catalog-item-name", "type": "Property", "name": "Name", "typeRef": { "type": "string" } }
            ] },
          { "id": "catalog-service", "type": "Service Proxy", "name": "CatalogService" }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
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
      "types": {
        "Component": { "find": "type", "files": "src/WebApp/Components/**/*.razor", "note": "Every Razor component" },
        "Model Definition": { "find": "type with \\brecord \\w+", "files": "src/WebApp/Catalog/**/*.cs", "note": "Records; a class with only properties would need a plug-in" },
        "Service Proxy": { "find": "type with \\b(?:HttpClient|\\w+\\.\\w+Client)\\b", "files": "src/WebApp/Services/**/*.cs", "note": "An API client: an HttpClient or a generated gRPC client member" }
      }
    }
  }
}
```

```razor file src/WebApp/Components/Pages/Catalog/Catalog.razor
@page "/"

<PageTitle>Northern Mountains</PageTitle>

<h1>Ready for a new adventure?</h1>
```

```csharp file src/WebApp/Catalog/CatalogItem.cs
namespace eShop.WebApp.Catalog;

public record CatalogItem(int Id, string Name);
```

```csharp file src/WebApp/Services/CatalogService.cs
namespace eShop.WebApp.Services;

public class CatalogService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "api/catalog/";

    public Task<CatalogItem?> GetCatalogItem(int id)
    {
        var uri = $"{remoteServiceBaseUrl}items/{id}";
        return httpClient.GetFromJsonAsync<CatalogItem>(uri);
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "count": 0, "note": "Every type that follows a convention is modelled" }
  ]
}
```

## blazor-page

An adopted `.razor` file with no Component, Layout or Page (`component.unmodelled`). eShopCustom's fix named the kind and the Route to model; here the finding names the component and the convention it follows.

```json case
{ "phase": 6, "base": "ui-located", "cases": ["optin/unmodelled.type"], "traces": ["ESC-FE-15"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui", "house.webapp"], "language": "csharp" }
```

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@page "/cart"

<PageTitle>Shopping Bag | Northern Mountains</PageTitle>

<h1>Shopping bag</h1>
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/WebApp/Components/Pages/Cart/CartPage.razor:1", "messageContains": "CartPage" },
    { "rule": "optin/unmodelled.type", "subject": "src/WebApp/Components/Pages/Catalog/Catalog.razor:1", "count": 0 }
  ]
}
```

## model-definition

An adopted record with no Model Definition (`definition.unmodelled`). eShopCustom also counted a class or struct with only properties; telling that apart from a class with behaviour needs a plug-in, so the convention keeps to records.

```json case
{ "phase": 6, "base": "ui-located", "cases": ["optin/unmodelled.type"], "traces": ["ESC-FE-49"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui", "house.webapp"], "language": "csharp" }
```

```csharp file src/WebApp/Catalog/CatalogBrand.cs
namespace eShop.WebApp.Catalog;

public record CatalogBrand(int Id, string Brand);
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/WebApp/Catalog/CatalogBrand.cs:3", "messageContains": "CatalogBrand" },
    { "rule": "optin/unmodelled.type", "subject": "src/WebApp/Catalog/CatalogItem.cs:3", "count": 0 }
  ]
}
```

## service-proxy

Adopted API clients with no Service Proxy (`proxy.unmodelled`): one holds an `HttpClient`, the other the generated gRPC client `Basket.BasketClient`. A gRPC client known only from an `AddGrpcClient<T>` registration in another file needs a plug-in, so both clients here take theirs as a constructor parameter.

```json case
{ "phase": 6, "base": "ui-located", "cases": ["optin/unmodelled.type"], "traces": ["ESC-FE-55"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui", "house.webapp"], "language": "csharp" }
```

```csharp file src/WebApp/Services/OrderingService.cs
namespace eShop.WebApp.Services;

public class OrderingService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "/api/Orders/";

    public Task<OrderRecord[]> GetOrders()
    {
        return httpClient.GetFromJsonAsync<OrderRecord[]>(remoteServiceBaseUrl)!;
    }
}
```

```csharp file src/WebApp/Services/BasketService.cs
namespace eShop.WebApp.Services;

public class BasketService(Basket.BasketClient basketClient)
{
    public async Task DeleteBasketAsync()
    {
        await basketClient.DeleteBasketAsync(new DeleteBasketRequest());
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/WebApp/Services/OrderingService.cs:3", "messageContains": "OrderingService" },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/WebApp/Services/BasketService.cs:3", "messageContains": "BasketService" },
    { "rule": "optin/unmodelled.type", "subject": "src/WebApp/Services/CatalogService.cs:3", "count": 0 }
  ]
}
```

## java-jpa

The Java spike's check: an unmodelled `@Entity`, `@Embeddable`, `@MappedSuperclass` or enum in the domain package. Two conventions, Class by its JPA annotation and Enum by its kind, through the Java reader. `Wishlist` is located by its `type.exists` claim; the other three are reported, each at the line its node starts.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.type"], "traces": ["JAV-DOM-31"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa", "house.backend"], "language": "java" }
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
              { "id": "wishlist-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop.Backend", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
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
  "designers": ["Domain"],
  "conventions": {
    "Domain": {
      "types": {
        "Class": { "find": "type with @(?:Entity|Embeddable|MappedSuperclass)\\b", "files": "src/main/java/**/domain/**/*.java" },
        "Enum": { "find": "type with \\benum \\w+", "files": "src/main/java/**/domain/**/*.java" }
      }
    }
  }
}
```

```java file src/main/java/com/eshop/backend/domain/Wishlist.java
package com.eshop.backend.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Wishlist {
    @Id
    private UUID id;

    public UUID getId() { return id; }
}
```

```java file src/main/java/com/eshop/backend/domain/WishlistItem.java
package com.eshop.backend.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.time.LocalDateTime;
import java.util.UUID;

@Entity
public class WishlistItem {
    @Id
    private UUID id;
    private UUID productId;
    private LocalDateTime dateAdded;
}
```

```java file src/main/java/com/eshop/backend/domain/Address.java
package com.eshop.backend.domain;

import jakarta.persistence.Embeddable;

@Embeddable
public class Address {
    private String street;
    private String city;
}
```

```java file src/main/java/com/eshop/backend/domain/OrderStatus.java
package com.eshop.backend.domain;

public enum OrderStatus {
    PENDING,
    PAID,
    SHIPPED,
    CANCELLED
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/main/java/com/eshop/backend/domain/WishlistItem.java:8", "messageContains": "WishlistItem" },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/main/java/com/eshop/backend/domain/Address.java:5", "messageContains": "Address" },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/main/java/com/eshop/backend/domain/OrderStatus.java:3", "messageContains": "OrderStatus" },
    { "rule": "optin/unmodelled.type", "subject": "src/main/java/com/eshop/backend/domain/Wishlist.java:7", "count": 0 }
  ]
}
```

## java-controller

An unmodelled `@RestController`. `WishlistsController` holds the handler the `WishlistsService`'s endpoint claims read, so it is located; `ReturnsController` follows the convention and no Service describes it.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.type"], "traces": ["JAV-SVC-03"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc", "house.backend"], "language": "java" }
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
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
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
      "types": {
        "Service": { "find": "type with @RestController\\b", "files": "src/main/java/**/api/**/*.java" }
      }
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
}
```

```java file src/main/java/com/eshop/backend/api/ReturnsController.java
package com.eshop.backend.api;

import com.eshop.backend.application.ReturnService;
import java.util.UUID;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class ReturnsController {
    private final ReturnService returns;

    public ReturnsController(ReturnService returns) {
        this.returns = returns;
    }

    @GetMapping("/api/returns/{returnId}")
    public ReturnDto getReturn(@PathVariable UUID returnId) {
        return returns.get(returnId);
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/main/java/com/eshop/backend/api/ReturnsController.java:9", "messageContains": "ReturnsController" },
    { "rule": "optin/unmodelled.type", "subject": "src/main/java/com/eshop/backend/api/WishlistsController.java:10", "count": 0 }
  ]
}
```

## java-storefront-contract

The Java spike's storefront keeps its Contracts in `src/api/types.ts`. An exported interface or type there that no Contract describes is reported (unmodelled exported interfaces or types in files that hold Contracts). `ProductDto` is modelled; `BasketDto` isn't.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.type"], "traces": ["JAV-FE-19"], "requirements": ["R28", "R49"], "packs": ["intent.core", "house.storefront"], "language": "typescript" }
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
        { "kind": "type.exists", "for": { "element": "Contract" }, "value": "self", "bindings": { "Name": "name" }, "own": ["Name"], "source": "the Contract" }
      ]
    }
  },
  "conventions": {
    "Front End": {
      "types": {
        "Contract": { "typeKinds": ["interface", "typeAlias"], "find": "type", "files": "src/api/types.ts", "note": "The storefront: a Contract is an exported interface or type in src/api/types.ts" }
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
              { "id": "product-dto-id", "type": "Field", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Storefront", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
        } }]
    }
  }
}
```

```typescript file src/api/types.ts
export interface ProductDto {
    Id: string;
}

export interface BasketDto {
    Items: ProductDto[];
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "product-dto", "outcome": "verified", "at": "src/api/types.ts:1" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/api/types.ts:5", "messageContains": "BasketDto" },
    { "rule": "optin/unmodelled.type", "subject": "src/api/types.ts:1", "count": 0 }
  ]
}
```

## glob-only

No pack says what the code's types look like: the row's Glob, `src/Domain/**`, is the whole setup, as a Common.Drift `own` entry's `files` was. `Buyer` and the SeedWork's `Entity` are under it and no claim located them, so each is reported, with the Domain designer, whose `Order` is in `Buyer`'s folder and in the folder above `Entity`'s. `Order` is located; its nested `Snapshot` is part of it; `OrderService` is outside the Glob.

```json case
{ "phase": 2, "cases": ["optin/unmodelled.type"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type", "Glob": "src/Domain/**" } ] }
        } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order
{
    public string Description { get; private set; }

    private sealed class Snapshot
    {
    }
}
```

```csharp file src/Domain/Buyer.cs
namespace Ordering.Domain;

public class Buyer
{
    public string Name { get; private set; }
}
```

```csharp file src/Domain/SeedWork/Entity.cs
namespace Ordering.Domain.SeedWork;

public abstract class Entity
{
}
```

```csharp file src/Application/OrderService.cs
namespace Ordering.Application;

public class OrderService
{
    public void Ship()
    {
    }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Domain/Buyer.cs:3",
      "messageContains": "`Buyer` isn't in the model. Model it in the Domain designer, or add an exception with a reason to the Root Folder's Drift Check." },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Domain/SeedWork/Entity.cs:3", "messageContains": "`Entity` isn't in the model. Model it in the Domain designer" },
    { "rule": "optin/unmodelled.type", "subject": "src/Domain/Order.cs:3", "count": 0, "note": "Located by its type.exists claim" },
    { "rule": "optin/unmodelled.type", "messageContains": "Snapshot", "count": 0, "note": "Nested in Order, so part of it" },
    { "rule": "optin/unmodelled.type", "messageContains": "OrderService", "count": 0, "note": "Outside the row's Glob" }
  ]
}
```

## whole-scope

The same code, with a row that has no Glob: the rule looks at every type in scope. `OrderService` is reported too. No located type is in its folder or a folder above it, so no designer is named.

```json case
{ "phase": 2, "base": "glob-only", "cases": ["optin/unmodelled.type"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Application/OrderService.cs:3",
      "messageContains": "`OrderService` isn't in the model. Model it, or add an exception with a reason to the Root Folder's Drift Check." },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Domain/Buyer.cs:3", "messageContains": "Buyer" },
    { "rule": "optin/unmodelled.type", "subject": "src/Domain/Order.cs:3", "count": 0 }
  ]
}
```

## pattern-narrows-its-files

The house pack's Class convention finds the types deriving `Entity` in `src/Domain/**/*.cs`, and the row has no Glob. Within the convention's files only what it finds is reported: `Buyer`, as a Class, and not the SeedWork's `Utc`. Outside them the row alone decides, so `OrderService` is reported as any type is.

```json case
{ "phase": 2, "base": "off-by-default", "cases": ["optin/unmodelled.type"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "house.ordering"], "language": "csharp" }
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
              { "id": "order-description", "type": "Attribute", "name": "Description", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" } ] }
        } }]
    }
  }
}
```

```csharp file src/Domain/SeedWork/Utc.cs
namespace Ordering.Domain.SeedWork;

public static class Utc
{
    public static DateTime Now() => DateTime.UtcNow;
}
```

```csharp file src/Application/OrderService.cs
namespace Ordering.Application;

public class OrderService
{
    public void Ship()
    {
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Domain/BuyerAggregate/Buyer.cs:3", "messageContains": "`Buyer` isn't in the model: no Class in the Domain designer describes it." },
    { "rule": "optin/unmodelled.type", "messageContains": "Utc", "count": 0, "note": "In the convention's files, and not what it finds" },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Application/OrderService.cs:3", "messageContains": "`OrderService` isn't in the model." }
  ]
}
```

## interface-of-a-modelled-class

A located class's interfaces are its code: Intent's own Domain Service, Repository and Service output is an interface beside its class, and no element describes the interface on its own. `BuyerLookup` is the `BuyerLookup` Domain Service, so `IBuyerLookup`, which it implements, isn't reported. `IClock` is implemented by nothing the model describes: reported.

```json case
{ "phase": 2, "cases": ["optin/unmodelled.type"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "buyer-lookup", "type": "Domain Service", "name": "BuyerLookup" }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type", "Glob": "src/Domain/**" } ] }
        } }]
    }
  }
}
```

```csharp file src/Domain/Services/IBuyerLookup.cs
namespace Ordering.Domain.Services;

public interface IBuyerLookup
{
    bool Exists(string email);
}
```

```csharp file src/Domain/Services/BuyerLookup.cs
namespace Ordering.Domain.Services;

public class BuyerLookup : IBuyerLookup
{
    public bool Exists(string email) => false;
}
```

```csharp file src/Domain/Services/IClock.cs
namespace Ordering.Domain.Services;

public interface IClock
{
    DateTime Now { get; }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.type", "subject": "src/Domain/Services/IBuyerLookup.cs:3", "count": 0, "note": "The interface of a located class" },
    { "rule": "optin/unmodelled.type", "subject": "src/Domain/Services/BuyerLookup.cs:3", "count": 0 },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Domain/Services/IClock.cs:3", "messageContains": "`IClock` isn't in the model." }
  ]
}
```
