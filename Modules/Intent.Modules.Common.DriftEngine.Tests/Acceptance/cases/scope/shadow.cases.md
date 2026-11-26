# scope/shadow

When a claim's code isn't found in scope, the engine looks for it among the out-of-scope paths, so the finding says "out of scope" rather than "missing" (R25). The lookup repeats the find's named steps against paths only: a `class:Order` step looks for a file whose name, up to its first dot, is `Order`, and a `file:` step matches as written. It never opens an excluded file. A hit makes the claim can't verify, `outOfScope`, naming the file and the row that excludes it; no hit leaves the claim to the rest of the outcome order.

## exposed-dto-out-of-scope

The API exposes `OrderDto`, and the model describes it, but its file is in the shared contracts folder that row 2 excludes. The claim can't be verified, and the finding names `src/Contracts/OrderDto.cs` and the row, so the team can either bring the file into scope or accept that the claim isn't checked here. The excluded file is never read.

eShopCustom's Services module reported exposed types whose files weren't adopted (`services.exposure.not-adopted`); a modelled type outside scope is that report, for any designer.

```json case
{ "phase": 1, "cases": ["scope/shadow", "scope/notRead"], "traces": ["ESC-SVC-05"], "requirements": ["R22", "R25"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Services",
        "elements": [
          { "id": "order-dto", "type": "DTO", "name": "OrderDto",
            "children": [
              { "id": "order-dto-id", "type": "DTO-Field", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Contracts/**", "Scope": "Out", "Reason": "Shared contracts package, checked from its own application" }
        ] } } }]
    }
  }
}
```

```csharp file src/Api/Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;
using Shop.Contracts;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult<OrderDto> GetOrder(Guid id) => Ok(new OrderDto(id));
}
```

```csharp file src/Contracts/OrderDto.cs
namespace Shop.Contracts;

public record OrderDto(Guid Id);
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order-dto", "outcome": "cantVerify", "reason": "outOfScope", "scopeRow": "root#2" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order-dto", "messageContains": "src/Contracts/OrderDto.cs" }
  ],
  "filesRead": { "include": ["src/Api/Controllers/OrdersController.cs"], "exclude": ["src/Contracts/**"] }
}
```

## layer-path-out-of-scope

The architecture model has a `Legacy` Layer whose Layer Settings Path is `src/Legacy`, and row 2 excludes that folder. The Layer's contents claim finds nothing in scope, but its `file:` step matches `src/Legacy/OrderService.cs` among the excluded paths, so the claim is `outOfScope`, naming the file and the row, rather than contradicted from the recipe's `absent`. The `Domain` Layer, in scope, is verified.

eShopCustom's Architecture module reported a Layer whose Path lies outside its Architecture Scope (`architecture.layer.outside-scope`). Its Layers sit on the Codebase Structure designer's Root Folder, through that module's package extension; no stock pack knows them, so a house pack supplies the kind, its vocabulary and its recipe, as phase 2 requires.

```json case
{ "phase": 2, "cases": ["scope/shadow"], "kinds": ["layer.populated"], "traces": ["ESC-ARC-04"], "requirements": ["R22", "R25", "R28"], "packs": ["intent.core", "house.shop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Legacy/**", "Scope": "Out", "Reason": "Being replaced by src/Domain" }
        ] } },
        "elements": [
          { "id": "domain-layer", "type": "Layer", "name": "Domain",
            "stereotypes": { "Layer Settings": { "Path": "src/Domain" } } },
          { "id": "legacy-layer", "type": "Layer", "name": "Legacy",
            "stereotypes": { "Layer Settings": { "Path": "src/Legacy" } } }
        ] }]
    }
  }
}
```

```json pack
{
  "id": "house.shop",
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

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public Guid Id { get; set; }
}
```

```csharp file src/Legacy/OrderService.cs
namespace Shop.Legacy;

public class OrderService
{
}
```

```json expect
{
  "claims": [
    { "kind": "layer.populated", "element": "domain-layer", "outcome": "verified" },
    { "kind": "layer.populated", "element": "legacy-layer", "outcome": "cantVerify", "reason": "outOfScope", "scopeRow": "root#2" }
  ],
  "findings": [
    { "rule": "claim/layer.populated", "severity": "error", "subject": "legacy-layer", "messageContains": "src/Legacy/OrderService.cs" }
  ],
  "filesRead": { "include": ["src/Domain/Order.cs"], "exclude": ["src/Legacy/**"] }
}
```

## paths-only

The excluded `src/Legacy/Order.cs` actually declares `LegacyOrder`, not `Order`. The lookup never opens it: its name matches the `class:Order` step, so the claim is `outOfScope` and the message says the file "may hold it". Opening the file to be sure would be reading out-of-scope code.

```json case
{ "phase": 1, "cases": ["scope/shadow", "scope/notRead"], "requirements": ["R22", "R25"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Legacy/**", "Scope": "Out", "Reason": "Being replaced by src/Domain" }
        ] } } }]
    }
  }
}
```

```csharp file src/Domain/Customer.cs
namespace Shop.Domain;

public class Customer
{
    public Guid Id { get; set; }
}
```

```csharp file src/Legacy/Order.cs
namespace Shop.Legacy;

public class LegacyOrder
{
    public int OrderNumber { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "cantVerify", "reason": "outOfScope", "scopeRow": "root#2" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "may hold it" }
  ],
  "filesRead": { "include": ["src/Domain/Customer.cs"], "exclude": ["src/Legacy/**"] }
}
```

## no-path-match-is-contradicted

Now the excluded file does declare `Order`, but it is named `LegacyOrders.cs`. No excluded path matches the `class:Order` step, so the lookup has no hit and the claim falls through to the rest of the order: `type.exists` is an existence kind, so a type not found is contradicted (R8). The lookup only ever reads paths; what an excluded file holds never counts.

```json case
{ "phase": 1, "base": "paths-only", "without": ["src/Legacy/Order.cs"], "cases": ["scope/shadow"], "requirements": ["R8", "R25"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Legacy/LegacyOrders.cs
namespace Shop.Legacy;

public class Order
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "isn't in the code" }
  ],
  "filesRead": { "exclude": ["src/Legacy/**"] }
}
```

## member-of-located-type

A member's find has two steps, its type and then the member in it. Here the `class:Order` step finds `Order` in scope, in `src/Ordering/Order.cs`, and only the member step fails: the model's `Test` isn't on it. `Order` isn't partial, so that declaration is all of it and the member is contradicted. Only a partial type's missing member is looked for among out-of-scope files, since only it has other parts (`structure/member.exists#out-of-scope-partial`). `src/ClientApp/Order.cs`, which no row includes, is a different `Order`: several applications over one output folder each keep their own (eShop's Ordering and ClientApp, with the repository root as the output folder for both), and naming it made a planted `Order.Test` "can't be verified" instead of drift.

```json case
{ "phase": 1, "cases": ["scope/shadow", "scope/notRead"], "requirements": ["R8", "R25"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } },
              { "id": "order-test", "type": "Attribute", "name": "Test", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/Ordering/**", "Scope": "In" }
        ] } } }]
    }
  }
}
```

```csharp file src/Ordering/Order.cs
namespace Ordering.Domain;

public class Order
{
    public Guid Id { get; set; }
}
```

```csharp file src/ClientApp/Order.cs
namespace ClientApp.Models;

public class Order
{
    public string Test { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified" },
    { "kind": "member.exists", "element": "order-id", "outcome": "verified" },
    { "kind": "member.exists", "element": "order-test", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-test", "messageContains": "isn't in the code" },
    { "rule": "scope/coverage", "severity": "info", "subject": "root" }
  ],
  "noOtherFindings": true,
  "filesRead": { "include": ["src/Ordering/Order.cs"], "exclude": ["src/ClientApp/**"] }
}
```

## no-row-includes-it

Nothing is in scope until a row says so, so a file no row matches is out of scope as much as one an Out row excludes. ClientA's first real run put only its data project in scope; every DTO in the model lives in the DTO project, which no row matched, and each was reported as missing. Here `src/Contracts/Order.cs` matches no row: the claim is `outOfScope` with no scope row, and the message says no row includes the file.

```json case
{ "phase": 1, "cases": ["scope/shadow", "scope/notRead"], "requirements": ["R22", "R25"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/Domain/**", "Scope": "In" }
        ] } } }]
    }
  }
}
```

```csharp file src/Domain/Customer.cs
namespace Shop.Domain;

public class Customer
{
    public Guid Id { get; set; }
}
```

```csharp file src/Contracts/Order.cs
namespace Shop.Contracts;

public class Order
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "cantVerify", "reason": "outOfScope" },
    { "kind": "member.exists", "element": "order-id", "outcome": "cantVerify", "reason": "blocked" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "no Drift Scope row includes that file" }
  ],
  "filesRead": { "include": ["src/Domain/Customer.cs"], "exclude": ["src/Contracts/**"] }
}
```

## no-row-includes-it-in-a-referenced-project

Where the in-scope code says which projects it builds with, a file no row covers is named only when it is in one of them. The in-scope `Shop.Domain` project references `Shop.Contracts`, so `src/Contracts/Order.cs` is the application's own code that the scope leaves out: `outOfScope`, as in `no-row-includes-it`.

```json case
{ "phase": 1, "base": "no-row-includes-it", "cases": ["scope/shadow", "scope/notRead"], "requirements": ["R22", "R25"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```xml file src/Domain/Shop.Domain.csproj
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\Contracts\Shop.Contracts.csproj" />
  </ItemGroup>
</Project>
```

```xml file src/Contracts/Shop.Contracts.csproj
<Project Sdk="Microsoft.NET.Sdk" />
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "cantVerify", "reason": "outOfScope" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "src/Contracts/Order.cs may hold it, but no Drift Scope row includes that file" }
  ],
  "filesRead": { "include": ["src/Domain/Customer.cs"], "exclude": ["src/Contracts/**"] }
}
```

## another-applications-project

Several applications over one output folder each keep their own code, often with types of the same names: eShop's WebApp and Catalog.API both declare `CatalogBrand`. The in-scope `Shop.Domain` project references no other project, so `src/Catalog/Order.cs`, which no row covers, is in a project the application doesn't build with: another application's code, not a file that may hold this one's. The model's `Order` isn't in the code.

```json case
{ "phase": 1, "cases": ["scope/shadow"], "requirements": ["R22", "R25"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order" }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/Domain/**", "Scope": "In" }
        ] } } }]
    }
  }
}
```

```xml file src/Domain/Shop.Domain.csproj
<Project Sdk="Microsoft.NET.Sdk" />
```

```csharp file src/Domain/Customer.cs
namespace Shop.Domain;

public class Customer
{
    public Guid Id { get; set; }
}
```

```xml file src/Catalog/Shop.Catalog.csproj
<Project Sdk="Microsoft.NET.Sdk" />
```

```csharp file src/Catalog/Order.cs
namespace Shop.Catalog;

public class Order
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order" },
    { "rule": "claim/type.exists", "subject": "order", "messageContains": "may hold it", "count": 0 }
  ],
  "filesRead": { "include": ["src/Domain/Customer.cs"], "exclude": ["src/Catalog/**"] }
}
```
