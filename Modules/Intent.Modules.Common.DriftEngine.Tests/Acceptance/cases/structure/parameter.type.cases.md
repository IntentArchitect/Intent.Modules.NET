# parameter.type

A modelled parameter has the type its type reference states. Inside the operation's method, a parameter is found by name (rules.md, Locating an element's code), so parameters the model doesn't mention, such as a handler's injected ones, don't disturb it. The `type` comparer maps the model's type through the language profile. It is a `stated: always` kind; a parameter the method doesn't have gives no type to compare, so the claim is can't verify, `notFound`. That a parameter is missing is `operation.parameters`' claim.

## verified

eShopCustom models each minimal API router as a Service with one Operation per handler, and its house pack says a Service is the class of its name. `GetItemsByBrandAndTypeId`'s `typeId` is an `int` and its `brandId` an `int?`, as modelled; the `[AsParameters]` holders aren't modelled and aren't compared.

```json case
{ "phase": 2, "kinds": ["parameter.type"], "traces": ["ESC-SVC-30"], "requirements": ["R2", "R41", "R42", "R53"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
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
              { "id": "catalog-api-get-items-by-brand-and-type", "type": "Operation", "name": "GetItemsByBrandAndTypeId",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items/type/{typeId}/brand/{brandId?}" } },
                "children": [
                  { "id": "catalog-api-get-items-by-brand-and-type-type-id", "type": "Parameter", "name": "typeId", "typeRef": { "type": "int" } },
                  { "id": "catalog-api-get-items-by-brand-and-type-brand-id", "type": "Parameter", "name": "brandId", "typeRef": { "type": "int", "nullable": true } }
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
    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByBrandAndTypeId(
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] CatalogServices services,
        int typeId,
        int? brandId)
    {
        return await GetAllItems(paginationRequest, services, null, [typeId], brandId is null ? null : [brandId.Value]);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.type", "element": "catalog-api-get-items-by-brand-and-type-type-id", "outcome": "verified", "expected": "int", "actual": "int", "at": "src/Catalog.API/Apis/CatalogApi.cs:8" },
    { "kind": "parameter.type", "element": "catalog-api-get-items-by-brand-and-type-brand-id", "outcome": "verified", "expected": "int?", "actual": "int?", "at": "src/Catalog.API/Apis/CatalogApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/parameter.type", "count": 0 }
  ]
}
```

## contradicted

`typeId` is a `string` in the handler: contradicted, `services.parameter.type` in eShopCustom's terms.

```json case
{ "phase": 2, "base": "verified", "kinds": ["parameter.type"], "traces": ["ESC-SVC-30"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace eShop.Catalog.API;

public static class CatalogApi
{
    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByBrandAndTypeId(
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] CatalogServices services,
        string typeId,
        int? brandId)
    {
        return await GetAllItems(paginationRequest, services, null, [int.Parse(typeId)], brandId is null ? null : [brandId.Value]);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.type", "element": "catalog-api-get-items-by-brand-and-type-type-id", "outcome": "contradicted", "expected": "int", "actual": "string", "at": "src/Catalog.API/Apis/CatalogApi.cs:8" }
  ],
  "findings": [
    { "rule": "claim/parameter.type", "severity": "error", "subject": "catalog-api-get-items-by-brand-and-type-type-id", "messageContains": "code `string`" }
  ]
}
```

## not-found

The handler's parameter is `catalogTypeId`. The method is found, but it has no parameter named `typeId`, so there is no type to read: can't verify, `notFound`. `parameter.type` isn't an existence kind; the missing parameter is `operation.parameters`' contradiction.

```json case
{ "phase": 2, "base": "verified", "kinds": ["parameter.type"], "cases": ["engine/notFound"], "requirements": ["R1", "R5", "R11"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace eShop.Catalog.API;

public static class CatalogApi
{
    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByBrandAndTypeId(
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] CatalogServices services,
        int catalogTypeId,
        int? brandId)
    {
        return await GetAllItems(paginationRequest, services, null, [catalogTypeId], brandId is null ? null : [brandId.Value]);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.type", "element": "catalog-api-get-items-by-brand-and-type-type-id", "outcome": "cantVerify", "reason": "notFound" },
    { "kind": "parameter.type", "element": "catalog-api-get-items-by-brand-and-type-brand-id", "outcome": "verified", "at": "src/Catalog.API/Apis/CatalogApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/parameter.type", "severity": "error", "subject": "catalog-api-get-items-by-brand-and-type-type-id" }
  ]
}
```

## java-path-variable

The same claim over a Spring MVC handler. With the Spring MVC pack, Service `InventoryService` is its `@RestController`, `InventoryController`. The `@PathVariable UUID productId` is the modelled `guid` through the Java profile. The spike's check also accepts a `String` parameter that the project's own `Http.parseEnum` helper binds to an enum; that reads a method body, and isn't shown here.

```json case
{ "phase": 4, "kinds": ["parameter.type"], "traces": ["JAV-SVC-10"], "requirements": ["R2", "R33", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Backend",
        "elements": [
          { "id": "inventory-service", "type": "Service", "name": "InventoryService",
            "children": [
              { "id": "inventory-service-receive-stock", "type": "Operation", "name": "ReceiveStock",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "api/inventory/{productId}/receive" } },
                "children": [
                  { "id": "inventory-service-receive-stock-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "guid" } },
                  { "id": "inventory-service-receive-stock-quantity", "type": "Parameter", "name": "quantity", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop.Backend", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/eshop/backend/api/InventoryController.java
package com.eshop.backend.api;

import com.eshop.backend.application.StockLevels;
import java.util.UUID;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
public class InventoryController {
    private final StockLevels stockLevels;

    public InventoryController(StockLevels stockLevels) {
        this.stockLevels = stockLevels;
    }

    @PostMapping("/api/inventory/{productId}/receive")
    public ResponseEntity<Void> receiveStock(@PathVariable UUID productId, @RequestParam int quantity) {
        stockLevels.receive(productId, quantity);
        return ResponseEntity.noContent().build();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.type", "element": "inventory-service-receive-stock-product-id", "outcome": "verified", "expected": "guid", "actual": "UUID" },
    { "kind": "parameter.type", "element": "inventory-service-receive-stock-quantity", "outcome": "verified", "expected": "int", "actual": "int" }
  ]
}
```

## java-path-variable-contradicted

`productId` is bound as a `String`, which the Java profile doesn't list for `guid`: contradicted.

```json case
{ "phase": 4, "base": "java-path-variable", "kinds": ["parameter.type"], "traces": ["JAV-SVC-10"], "requirements": ["R1", "R11", "R42"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/api/InventoryController.java
package com.eshop.backend.api;

import com.eshop.backend.application.StockLevels;
import java.util.UUID;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
public class InventoryController {
    private final StockLevels stockLevels;

    public InventoryController(StockLevels stockLevels) {
        this.stockLevels = stockLevels;
    }

    @PostMapping("/api/inventory/{productId}/receive")
    public ResponseEntity<Void> receiveStock(@PathVariable String productId, @RequestParam int quantity) {
        stockLevels.receive(UUID.fromString(productId), quantity);
        return ResponseEntity.noContent().build();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.type", "element": "inventory-service-receive-stock-product-id", "outcome": "contradicted", "expected": "guid", "actual": "String" }
  ],
  "findings": [
    { "rule": "claim/parameter.type", "severity": "error", "subject": "inventory-service-receive-stock-product-id" }
  ]
}
```
