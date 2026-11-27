# optin/unmodelled.file

The opt-in rule that reports an in-scope file in which no claim located anything (rules.md, "Opt-in must be modelled rules"). It replaces eShopCustom's Strict scope mode, where every adopted file had to be owned by the model or excluded. By principle 1 the rule is off until the Root Folder's `Drift Check` lists it under `Opt-in Rules`; scope already keeps out what the team never meant to describe, and an `Exceptions` row with a file glob keeps out the rest, with a reason.

A finding's subject is the file, at line 1.

## off-by-default

`Order` and `OrderSummary` are modelled, and their files are located by their `type.exists` claims. `OrderingDomainException.cs` and `Extensions.cs` hold nothing the model describes. No `Opt-in Rules` row turns the rule on, so nothing is reported (principle 1).

```json case
{ "phase": 1, "cases": ["optin/unmodelled.file"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.services"], "language": "csharp" }
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
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "order-summary", "type": "DTO", "name": "OrderSummary",
            "children": [
              { "id": "order-summary-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
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
    public string Description { get; private set; }
}
```

```csharp file src/Domain/Exceptions/OrderingDomainException.cs
namespace Ordering.Domain.Exceptions;

public class OrderingDomainException : Exception
{
    public OrderingDomainException(string message) : base(message)
    {
    }
}
```

```csharp file src/Ordering.API/Application/Queries/OrderSummary.cs
namespace Ordering.API.Application.Queries;

public record OrderSummary(int OrderNumber);
```

```csharp file src/Ordering.API/Extensions/Extensions.cs
namespace Ordering.API.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(Program)));
    }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.file", "count": 0, "note": "The rule is off" }
  ]
}
```

## on

The same model and code, with the rule on. The two files no claim located anything in are reported, one finding each, errors by default: eShopCustom's Domain and Services modules both made each one a Backlog. The two files holding modelled types are located.

```json case
{ "phase": 1, "base": "off-by-default", "cases": ["optin/unmodelled.file"], "traces": ["ESC-DOM-03", "ESC-SVC-04"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.services"], "language": "csharp" }
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
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "order-summary", "type": "DTO", "name": "OrderSummary",
            "children": [
              { "id": "order-summary-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.file" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/OrderAggregate/Order.cs:3" },
    { "kind": "type.exists", "element": "order-summary", "outcome": "verified", "at": "src/Ordering.API/Application/Queries/OrderSummary.cs:3" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.file", "severity": "error", "subject": "src/Domain/Exceptions/OrderingDomainException.cs:1", "messageContains": "OrderingDomainException.cs" },
    { "rule": "optin/unmodelled.file", "severity": "error", "subject": "src/Ordering.API/Extensions/Extensions.cs:1", "messageContains": "Extensions.cs" },
    { "rule": "optin/unmodelled.file", "subject": "src/Domain/OrderAggregate/Order.cs:1", "count": 0 },
    { "rule": "optin/unmodelled.file", "subject": "src/Ordering.API/Application/Queries/OrderSummary.cs:1", "count": 0 }
  ]
}
```

## file-of-types-scoped-out

A scope row may leave types out by name, as test doubles a feature flag swaps in: `CurrencyExchangeFake` is out, so its file, which declares nothing else, is out with it and not asked for. `CurrencyExchange.cs` declares a type still read, so it is reported.

```json case
{ "phase": 1, "cases": ["optin/unmodelled.file"], "requirements": ["R26", "R49"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Bank.Domain",
        "elements": [
          { "id": "account", "type": "Class", "name": "Account" }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Bank", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [
            { "Glob": "src/**", "Scope": "In" },
            { "Glob": "src/**", "Types": ["CurrencyExchangeFake"], "Scope": "Out", "Reason": "Test double a feature flag swaps in" }
          ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.file" } ] }
        } }]
    }
  }
}
```

```csharp file src/Domain/Account.cs
namespace Bank.Domain;

public class Account
{
}
```

```csharp file src/Infrastructure/CurrencyExchangeFake.cs
namespace Bank.Infrastructure;

public sealed class CurrencyExchangeFake
{
}
```

```csharp file src/Infrastructure/CurrencyExchange.cs
namespace Bank.Infrastructure;

public sealed class CurrencyExchange
{
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "account", "outcome": "verified", "at": "src/Domain/Account.cs:3" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.file", "subject": "src/Infrastructure/CurrencyExchangeFake.cs:1", "count": 0 },
    { "rule": "optin/unmodelled.file", "severity": "error", "subject": "src/Infrastructure/CurrencyExchange.cs:1", "messageContains": "CurrencyExchange.cs" }
  ]
}
```

## narrowed-by-glob

The `Opt-in Rules` row has a `Glob`: the team holds only the Domain project to strict ownership so far. `OrderingDomainException.cs` is under it and is reported; `Extensions.cs` isn't.

```json case
{ "phase": 1, "base": "off-by-default", "cases": ["optin/unmodelled.file"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.services"], "language": "csharp" }
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
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "order-summary", "type": "DTO", "name": "OrderSummary",
            "children": [
              { "id": "order-summary-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.file", "Glob": "src/Domain/**" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.file", "severity": "error", "subject": "src/Domain/Exceptions/OrderingDomainException.cs:1", "messageContains": "OrderingDomainException.cs" },
    { "rule": "optin/unmodelled.file", "subject": "src/Ordering.API/Extensions/Extensions.cs:1", "count": 0, "note": "Outside the row's Glob" }
  ]
}
```

## excepted-by-glob

An `Exceptions` row's `Match` can be a file glob. The team keeps domain exceptions out of the model, and says why; those files aren't reported, and `Extensions.cs` still is. The exception matched, so it isn't stale.

```json case
{ "phase": 1, "base": "off-by-default", "cases": ["optin/unmodelled.file"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.services"], "language": "csharp" }
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
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "order-summary", "type": "DTO", "name": "OrderSummary",
            "children": [
              { "id": "order-summary-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": {
            "Opt-in Rules": [ { "Rule": "optin/unmodelled.file" } ],
            "Exceptions": [ { "Rule": "optin/unmodelled.file", "Match": "src/Domain/Exceptions/**", "Reason": "Domain exceptions are plumbing; the model describes the rules that throw them" } ]
          }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.file", "subject": "src/Domain/Exceptions/OrderingDomainException.cs:1", "count": 0, "note": "Excepted" },
    { "rule": "optin/unmodelled.file", "severity": "error", "subject": "src/Ordering.API/Extensions/Extensions.cs:1", "messageContains": "Extensions.cs" },
    { "rule": "optin/staleException", "count": 0 }
  ]
}
```

## stale-glob-exception

The exception's glob names a folder the code no longer has, so it matches nothing: an `optin/staleException` warning, naming the glob. The findings it was meant to cover are unaffected.

```json case
{ "phase": 1, "base": "off-by-default", "cases": ["optin/unmodelled.file"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.services"], "language": "csharp" }
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
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "order-summary", "type": "DTO", "name": "OrderSummary",
            "children": [
              { "id": "order-summary-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": {
            "Opt-in Rules": [ { "Rule": "optin/unmodelled.file" } ],
            "Exceptions": [ { "Rule": "optin/unmodelled.file", "Match": "src/Ordering.API/Infrastructure/**", "Reason": "Startup wiring isn't modelled" } ]
          }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/staleException", "severity": "warning", "messageContains": "src/Ordering.API/Infrastructure/**" },
    { "rule": "optin/unmodelled.file", "severity": "error", "subject": "src/Ordering.API/Extensions/Extensions.cs:1", "messageContains": "Extensions.cs" }
  ]
}
```

## blazor-files

eShopCustom's Front End module held every adopted `.razor` file to the same strict rule. `Catalog.razor` is located by its page's claims, and `_Imports.razor`, which only holds `@using` lines, is excepted by a glob for every folder. `MainLayout.razor` has no Layout in the model, so it is reported.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.file"], "traces": ["ESC-FE-04"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp",
        "elements": [
          { "id": "catalog", "type": "Component", "name": "Catalog", "stereotypes": { "Page": { "Route": "/" } } }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/WebApp/**", "Scope": "In" } ] },
          "Drift Check": {
            "Opt-in Rules": [ { "Rule": "optin/unmodelled.file" } ],
            "Exceptions": [ { "Rule": "optin/unmodelled.file", "Match": "**/_Imports.razor", "Reason": "Namespace imports only; nothing to model" } ]
          }
        } }]
    }
  }
}
```

```razor file src/WebApp/Components/Pages/Catalog/Catalog.razor
@page "/"

<h1>Ready for a new adventure?</h1>
```

```razor file src/WebApp/Components/Layout/MainLayout.razor
@inherits LayoutComponentBase

<HeaderBar />
@Body
<FooterBar />
```

```razor file src/WebApp/Components/_Imports.razor
@using System.Net.Http
@using Microsoft.AspNetCore.Components.Web
@using eShop.WebApp.Components.Layout
```

```json expect
{
  "claims": [
    { "kind": "page.route", "element": "catalog", "outcome": "verified", "at": "src/WebApp/Components/Pages/Catalog/Catalog.razor:1" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.file", "severity": "error", "subject": "src/WebApp/Components/Layout/MainLayout.razor:1", "messageContains": "MainLayout.razor" },
    { "rule": "optin/unmodelled.file", "subject": "src/WebApp/Components/_Imports.razor:1", "count": 0, "note": "Excepted" },
    { "rule": "optin/unmodelled.file", "subject": "src/WebApp/Components/Pages/Catalog/Catalog.razor:1", "count": 0 },
    { "rule": "optin/staleException", "count": 0 }
  ]
}
```

## unlayered

eShopCustom's Architecture module warned about files under its Sources that no Layer covers. Its Layers sit on the Root Folder with a Layer Settings Path; the house module's `layer.populated` reader gives each in-scope file the Layer whose Path is its longest prefix and reads every such file, so every file inside a Layer is located by its Layer's claim. No reader for it is built yet, so the case waits. With `unmodelled.file` on, `Program.cs`, in no Layer, is reported, and a `Severity Overrides` row makes it the warning eShopCustom gave (R13). eShopCustom gave one warning per Source with a count of files; the engine reports each file, and has no finding that groups an opt-in rule's findings.

```json case
{ "phase": 3, "kinds": ["layer.populated"], "cases": ["optin/unmodelled.file"], "traces": ["ESC-ARC-07"], "requirements": ["R13", "R46", "R49"], "packs": ["intent.core", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": {
            "Opt-in Rules": [ { "Rule": "optin/unmodelled.file" } ],
            "Severity Overrides": [ { "Rule": "optin/unmodelled.file", "Severity": "Warning", "Reason": "Unlayered files are listed for the architecture review, not failed" } ]
          }
        },
        "elements": [
          { "id": "domain-layer", "type": "Layer", "name": "Domain",
            "stereotypes": { "Layer Settings": { "Path": "src/Ordering.Domain" } } },
          { "id": "infrastructure-layer", "type": "Layer", "name": "Infrastructure",
            "stereotypes": { "Layer Settings": { "Path": "src/Ordering.Infrastructure" } } }
        ] }]
    }
  }
}
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "languages": ["csharp"],
  "designers": ["Codebase Structure"],
  "kinds": [
    { "kind": "layer.populated", "label": "layer contents", "valueType": "bool", "comparer": "exact", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "Codebase Structure": {
      "entries": [
        {
          "kind": "layer.populated",
          "for": { "element": "Layer" },
          "value": "true",
          "bindings": { "Layer": "name", "Path": "stereotype('Layer Settings').Path" },
          "own": ["Layer"],
          "source": "the Layer and its Layer Settings Path"
        }
      ]
    }
  }
}
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    public int Id { get; private set; }
}
```

```csharp file src/Ordering.Infrastructure/OrderingContext.cs
namespace eShop.Ordering.Infrastructure;

public class OrderingContext(DbContextOptions<OrderingContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
}
```

```csharp file src/Ordering.API/Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();

var app = builder.Build();
app.Run();
```

```json expect
{
  "claims": [
    { "kind": "layer.populated", "element": "domain-layer", "outcome": "verified" },
    { "kind": "layer.populated", "element": "infrastructure-layer", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.file", "severity": "warning", "subject": "src/Ordering.API/Program.cs:1", "messageContains": "Program.cs" },
    { "rule": "optin/unmodelled.file", "subject": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:1", "count": 0 },
    { "rule": "optin/unmodelled.file", "subject": "src/Ordering.Infrastructure/OrderingContext.cs:1", "count": 0 }
  ]
}
```

## java-layer-files

The Java spike's check: every source file in a Layer is some convention's file or matches a File Exception glob. The `Opt-in Rules` row's `Glob` narrows the rule to the domain package, the spike's Domain Layer, and an `Exceptions` row stands for the spike's File Exception glob. `Wishlist.java` is located by its class's claim and `WishlistNotFoundException.java` is excepted; `Utc.java` is reported. `JsonConfig.java`, outside the domain package, isn't.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.file"], "traces": ["JAV-ARC-06"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
          "Drift Check": {
            "Opt-in Rules": [ { "Rule": "optin/unmodelled.file", "Glob": "src/main/java/com/eshop/backend/domain/**" } ],
            "Exceptions": [ { "Rule": "optin/unmodelled.file", "Match": "**/*Exception.java", "Reason": "Domain exceptions aren't modelled" } ]
          }
        } }]
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

```java file src/main/java/com/eshop/backend/domain/WishlistNotFoundException.java
package com.eshop.backend.domain;

import java.util.UUID;

public class WishlistNotFoundException extends RuntimeException {
    public WishlistNotFoundException(UUID customerId) {
        super("No wishlist for customer " + customerId);
    }
}
```

```java file src/main/java/com/eshop/backend/domain/Utc.java
package com.eshop.backend.domain;

import java.time.Clock;

public final class Utc {
    public static final Clock CLOCK = Clock.systemUTC();

    private Utc() {
    }
}
```

```java file src/main/java/com/eshop/backend/config/JsonConfig.java
package com.eshop.backend.config;

import org.springframework.context.annotation.Configuration;

@Configuration
public class JsonConfig {
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "wishlist", "outcome": "verified", "at": "src/main/java/com/eshop/backend/domain/Wishlist.java:7" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.file", "severity": "error", "subject": "src/main/java/com/eshop/backend/domain/Utc.java:1", "messageContains": "Utc.java" },
    { "rule": "optin/unmodelled.file", "subject": "src/main/java/com/eshop/backend/domain/WishlistNotFoundException.java:1", "count": 0, "note": "Excepted" },
    { "rule": "optin/unmodelled.file", "subject": "src/main/java/com/eshop/backend/domain/Wishlist.java:1", "count": 0 },
    { "rule": "optin/unmodelled.file", "subject": "src/main/java/com/eshop/backend/config/JsonConfig.java:1", "count": 0, "note": "Outside the row's Glob" }
  ]
}
```

## vue-files

The Java spike's storefront: every `.ts` and `.vue` file under `src` is owned by an element, except tests, declaration files, `main.ts` and `router.ts`. A house module's `file.exists` reader (`house.readers`) locates each Component's `.vue` file, which is all this rule needs. Tests and declaration files are out of scope; `main.ts` and `router.ts` are excepted. `format.ts` is owned by nothing, so it is reported.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.file", "scope/gate"], "traces": ["JAV-FE-12"], "requirements": ["R22", "R49"], "packs": ["intent.core", "intent.java", "house.storefront", "house.readers"], "language": "typescript" }
```

```json model
{
  "designers": {
    "Front End": {
      "packages": [{
        "id": "front-end", "name": "eShop.Storefront",
        "elements": [
          { "id": "stock-pill", "type": "Component", "name": "StockPill" }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop.Storefront", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [
            { "Glob": "src/**", "Scope": "In" },
            { "Glob": "src/**/*.test.ts", "Scope": "Out", "Reason": "Unit tests describe no element" },
            { "Glob": "src/**/*.d.ts", "Scope": "Out", "Reason": "Type declarations for tooling" }
          ] },
          "Drift Check": {
            "Opt-in Rules": [ { "Rule": "optin/unmodelled.file" } ],
            "Exceptions": [
              { "Rule": "optin/unmodelled.file", "Match": "src/main.ts", "Reason": "The app's entry point wires plugins; nothing to model" },
              { "Rule": "optin/unmodelled.file", "Match": "src/router.ts", "Reason": "Routes are each Page's own; the router file only lists them" }
            ]
          }
        } }]
    }
  }
}
```

```json pack
{
  "id": "house.storefront",
  "version": "1.0.0",
  "languages": ["typescript"],
  "designers": ["Front End"],
  "vocabulary": {
    "Front End": {
      "entries": [
        { "kind": "file.exists", "for": { "element": "Component" }, "bindings": { "Component": "name", "File": "'src/components/' + name + '.vue'" }, "own": ["Component"], "source": "the Component" }
      ]
    }
  }
}
```

```vue file src/components/StockPill.vue
<template>
  <span class="stock-pill" :class="{ out: available === 0 }">{{ label }}</span>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{ available: number }>()
const label = computed(() => (props.available === 0 ? 'Out of stock' : `${props.available} in stock`))
</script>
```

```ts file src/components/StockPill.test.ts
import { mount } from '@vue/test-utils'
import StockPill from './StockPill.vue'

test('shows out of stock', () => {
  expect(mount(StockPill, { props: { available: 0 } }).text()).toBe('Out of stock')
})
```

```ts file src/utils/format.ts
export function formatPrice(value: number): string {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value)
}
```

```ts file src/main.ts
import { createApp } from 'vue'
import App from './App.vue'
import { router } from './router'

createApp(App).use(router).mount('#app')
```

```ts file src/router.ts
import { createRouter, createWebHistory } from 'vue-router'

export const router = createRouter({ history: createWebHistory(), routes: [] })
```

```json expect
{
  "claims": [
    { "kind": "file.exists", "element": "stock-pill", "outcome": "verified", "at": "src/components/StockPill.vue:1" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.file", "severity": "error", "subject": "src/utils/format.ts:1", "messageContains": "format.ts" },
    { "rule": "optin/unmodelled.file", "subject": "src/components/StockPill.vue:1", "count": 0 },
    { "rule": "optin/unmodelled.file", "subject": "src/main.ts:1", "count": 0, "note": "Excepted" },
    { "rule": "optin/unmodelled.file", "subject": "src/router.ts:1", "count": 0, "note": "Excepted" },
    { "rule": "optin/unmodelled.file", "subject": "src/components/StockPill.test.ts:1", "count": 0, "note": "Out of scope" },
    { "rule": "optin/staleException", "count": 0 }
  ],
  "filesRead": { "include": ["src/utils/format.ts"], "exclude": ["src/**/*.test.ts"] }
}
```

## xaml-views-are-their-classs-and-resources-add-no-file

A XAML view (`intent.xaml`) is the markup half of the class its `x:Class` names, so a located view's XAML is located with its code-behind. The application's own XAML (`App.xaml`, an `Application`) and a resource dictionary (`Colors.xaml`, no `x:Class`) hold resources for the whole app, which no designer has a construct for: they add no file, so the rules for unmodelled files and types report nothing.

```json case
{ "phase": 6, "cases": ["optin/unmodelled.file", "optin/unmodelled.type"], "kinds": ["type.exists"], "requirements": ["R38", "R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [ { "id": "catalog", "type": "Page", "name": "CatalogView", "stereotypes": { "Page": { } } } ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.file" }, { "Rule": "optin/unmodelled.type" } ] }
        } }]
    }
  }
}
```

```xml file src/Shop/App.xaml
<Application x:Class="Shop.App" xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <Application.Resources>
        <ResourceDictionary Source="Resources/Colors.xaml" />
    </Application.Resources>
</Application>
```

```xml file src/Shop/Resources/Colors.xaml
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <Color x:Key="Accent">#512BD4</Color>
</ResourceDictionary>
```

```xml file src/Shop/Views/CatalogView.xaml
<ContentPage x:Class="Shop.Views.CatalogView" xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    <Label Text="Catalog" TextColor="{StaticResource Accent}" />
</ContentPage>
```

```csharp file src/Shop/Views/CatalogView.xaml.cs
namespace Shop.Views;

public partial class CatalogView : ContentPage
{
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "catalog", "outcome": "verified", "at": "src/Shop/Views/CatalogView.xaml.cs:3" }
  ],
  "noOtherFindings": true
}
```

## angular-lazy-parent-routes-file

An Angular app's root routes file holds only lazy parents and a redirect: `{ path: USER_PATHS.base, loadChildren: () => import('./features/user/user.routes')... }` and `{ path: '**', redirectTo: ... }`. Each lazy entry's path is the start of the Route of every Page under it, so a Page's route is read through it: the file is code the model describes, and isn't reported.

```json case
{ "phase": 6, "kinds": ["page.route"], "cases": ["optin/unmodelled.file"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "app", "name": "App", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "App" },
          { "id": "my-pokemon", "type": "Page", "name": "MyPokemonComponent", "stereotypes": { "Page": { "Route": "/user/my-pokemon" } } }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "app", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" }, { "Glob": "src/main.ts", "Scope": "Context" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.file", "Glob": "src/app/**" } ] }
        } }]
    }
  }
}
```

```typescript file src/main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { App } from './app/app';
import { appRoutes } from './app/app.routes';

bootstrapApplication(App, { providers: [provideRouter(appRoutes)] });
```

```typescript file src/app/app.ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({ selector: 'app-root', imports: [RouterOutlet], template: `<router-outlet />` })
export class App {}
```

```typescript file src/app/app.routes.ts
import type { Route } from '@angular/router';

export const appRoutes: Route[] = [
  {
    path: 'user',
    loadChildren: async () => import('./features/user/user.routes').then((module) => module.USER_ROUTES),
  },
  { path: '**', redirectTo: '/user/my-pokemon' },
];
```

```typescript file src/app/features/user/user.routes.ts
import { MyPokemonComponent } from './my-pokemon.component';

export const USER_ROUTES = [{ path: 'my-pokemon', component: MyPokemonComponent }];
```

```typescript file src/app/features/user/my-pokemon.component.ts
import { Component } from '@angular/core';

@Component({ selector: 'app-my-pokemon', template: `<h1>My Pokemon</h1>` })
export class MyPokemonComponent {}
```

```json expect
{
  "claims": [
    { "kind": "page.route", "element": "my-pokemon", "outcome": "verified", "at": "src/app/features/user/user.routes.ts:3" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.file", "subject": "src/app/app.routes.ts:1", "count": 0 },
    { "rule": "optin/unmodelled.file", "subject": "src/app/features/user/user.routes.ts:1", "count": 0 }
  ]
}
```

## angular-lazy-parent-routes-file-of-no-page

With no Page modelled under it, the root routes file and the child routes file describe nothing the model states, and both are reported.

```json case
{ "phase": 6, "base": "angular-lazy-parent-routes-file", "kinds": ["page.route"], "cases": ["optin/unmodelled.file"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "app", "name": "App", "type": "User Interface Package",
        "elements": [
          { "id": "shell", "type": "Layout", "name": "App" }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "app", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" }, { "Glob": "src/main.ts", "Scope": "Context" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.file", "Glob": "src/app/**" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.file", "severity": "error", "subject": "src/app/app.routes.ts:1", "messageContains": "app.routes.ts" },
    { "rule": "optin/unmodelled.file", "severity": "error", "subject": "src/app/features/user/user.routes.ts:1", "messageContains": "user.routes.ts" }
  ]
}
```
