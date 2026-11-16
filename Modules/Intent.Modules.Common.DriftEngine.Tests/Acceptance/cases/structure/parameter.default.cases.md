# parameter.default

A modelled parameter's default value, where the model sets one (the Parameter's value). The code side is the parameter's `default`, found by name in the operation's method. It is a `stated: whenSet` kind: a parameter with no modelled default gives no claim, so a default only the code has is never drift (principle 1), and a modelled default the code doesn't state is contradicted as not stated, never filled in from a binder or a framework default (decision 4, R4). Values are compared as canonical text with the `exact` comparer.

## verified

eShopCustom's house pack says a Service is the class of its name. `GetItemsByName`'s `pageSize` defaults to `10`, as modelled. `name` has no modelled default, so it gives no claim.

```json case
{ "phase": 2, "kinds": ["parameter.default"], "traces": ["ESC-SVC-31"], "requirements": ["R2", "R41", "R53"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
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
        "id": "services", "name": "Catalog.API",
        "elements": [
          { "id": "catalog-api", "type": "Service", "name": "CatalogApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/catalog" } },
            "children": [
              { "id": "catalog-api-get-items-by-name", "type": "Operation", "name": "GetItemsByName",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items/by/{name}" } },
                "children": [
                  { "id": "catalog-api-get-items-by-name-name", "type": "Parameter", "name": "name", "typeRef": { "type": "string" } },
                  { "id": "catalog-api-get-items-by-name-page-size", "type": "Parameter", "name": "pageSize", "typeRef": { "type": "int" }, "value": "10" }
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

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace eShop.Catalog.API;

public static class CatalogApi
{
    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByName(
        [AsParameters] CatalogServices services,
        string name,
        int pageSize = 10)
    {
        return await GetAllItems(new PaginationRequest(pageSize, 0), services, name, null, null);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.default", "element": "catalog-api-get-items-by-name-page-size", "outcome": "verified", "expected": "10", "actual": "10", "at": "src/Catalog.API/Apis/CatalogApi.cs:8" },
    { "kind": "parameter.default", "element": "catalog-api-get-items-by-name-name", "absent": true }
  ],
  "findings": [
    { "rule": "claim/parameter.default", "count": 0 }
  ]
}
```

## contradicted

`pageSize` defaults to `20`: contradicted, `services.parameter.default` in eShopCustom's terms.

```json case
{ "phase": 2, "base": "verified", "kinds": ["parameter.default"], "traces": ["ESC-SVC-31"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace eShop.Catalog.API;

public static class CatalogApi
{
    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByName(
        [AsParameters] CatalogServices services,
        string name,
        int pageSize = 20)
    {
        return await GetAllItems(new PaginationRequest(pageSize, 0), services, name, null, null);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.default", "element": "catalog-api-get-items-by-name-page-size", "outcome": "contradicted", "expected": "10", "actual": "20", "at": "src/Catalog.API/Apis/CatalogApi.cs:8" }
  ],
  "findings": [
    { "rule": "claim/parameter.default", "severity": "error", "subject": "catalog-api-get-items-by-name-page-size", "messageContains": "code `20`" }
  ]
}
```

## not-stated

`pageSize` has no default in the code. The method is found and the parameter is there, but nothing states the modelled `10`, so the claim is contradicted with the code side not stated (R4). Whatever the binder or the API's documentation would supply for a missing `pageSize` doesn't count.

```json case
{ "phase": 2, "base": "verified", "kinds": ["parameter.default"], "traces": ["ESC-SVC-31"], "requirements": ["R4", "R11"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace eShop.Catalog.API;

public static class CatalogApi
{
    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByName(
        [AsParameters] CatalogServices services,
        string name,
        int pageSize)
    {
        return await GetAllItems(new PaginationRequest(pageSize, 0), services, name, null, null);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.default", "element": "catalog-api-get-items-by-name-page-size", "outcome": "contradicted", "expected": "10" }
  ],
  "findings": [
    { "rule": "claim/parameter.default", "severity": "error", "subject": "catalog-api-get-items-by-name-page-size", "messageContains": "doesn't state" }
  ]
}
```

## code-default-not-modelled

`name` now defaults to `""` in the code, and the model sets no default for it. The model states nothing, so there is no claim and no finding (principle 1).

```json case
{ "phase": 2, "base": "verified", "kinds": ["parameter.default"], "requirements": ["R2", "R44"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace eShop.Catalog.API;

public static class CatalogApi
{
    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByName(
        [AsParameters] CatalogServices services,
        string name = "",
        int pageSize = 10)
    {
        return await GetAllItems(new PaginationRequest(pageSize, 0), services, name, null, null);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.default", "element": "catalog-api-get-items-by-name-name", "absent": true },
    { "kind": "parameter.default", "element": "catalog-api-get-items-by-name-page-size", "outcome": "verified", "at": "src/Catalog.API/Apis/CatalogApi.cs:8" }
  ],
  "findings": [
    { "rule": "claim/parameter.default", "count": 0 }
  ]
}
```

## domain-constructor

Any operation's or constructor's parameter, not only an HTTP endpoint's: `OrderItem`'s constructor defaults `units` to `1`, as modelled, and its nullable `discount` is a `decimal?`. The `units` default and both parameters' nullability are verified.

```json case
{ "phase": 2, "kinds": ["parameter.default", "parameter.nullable"], "traces": ["ESC-DOM-64"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order-item", "type": "Class", "name": "OrderItem",
            "children": [
              { "id": "order-item-ctor", "type": "Class Constructor", "name": "OrderItem",
                "children": [
                  { "id": "order-item-ctor-units", "type": "Parameter", "name": "units", "typeRef": { "type": "int" }, "value": "1" },
                  { "id": "order-item-ctor-discount", "type": "Parameter", "name": "discount", "typeRef": { "type": "decimal", "nullable": true } }
                ] }
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

```csharp file src/Ordering.Domain/OrderItem.cs
namespace Ordering.Domain;

public class OrderItem
{
    public OrderItem(int units = 1, decimal? discount = null)
    {
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.default", "element": "order-item-ctor-units", "outcome": "verified", "expected": "1", "actual": "1", "at": "src/Ordering.Domain/OrderItem.cs:5" },
    { "kind": "parameter.default", "element": "order-item-ctor-discount", "absent": true },
    { "kind": "parameter.nullable", "element": "order-item-ctor-units", "outcome": "verified", "expected": "false", "actual": "false" },
    { "kind": "parameter.nullable", "element": "order-item-ctor-discount", "outcome": "verified", "expected": "true", "actual": "true" }
  ],
  "findings": [
    { "rule": "claim/parameter.default", "count": 0 },
    { "rule": "claim/parameter.nullable", "count": 0 }
  ]
}
```

## blocked

`CatalogApi` has no `GetItemsByName`. The operation's `member.exists` claim is contradicted, and the default's claim waits on it: can't verify, `blocked`, rather than contradicted as not stated by a method that isn't there.

```json case
{ "phase": 2, "base": "verified", "kinds": ["parameter.default", "member.exists"], "cases": ["engine/blocked"], "requirements": ["R1", "R5", "R8"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace eShop.Catalog.API;

public static class CatalogApi
{
    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetAllItems(
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] CatalogServices services)
    {
        return await GetAllItems(paginationRequest, services, null, null, null);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "catalog-api-get-items-by-name", "outcome": "contradicted" },
    { "kind": "parameter.default", "element": "catalog-api-get-items-by-name-page-size", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "member.exists" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "catalog-api-get-items-by-name" },
    { "rule": "claim/parameter.default", "count": 0 }
  ]
}
```
