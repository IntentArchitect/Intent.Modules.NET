# parameter.nullable

A modelled parameter is nullable exactly when its type reference says so. The code side is the parameter's type ref's `nullable`, found by name in the operation's method: always known for a value type, known for a C# reference type only inside a nullable context, and for Java from the profile and the binding (a Spring `@RequestParam` is required unless `required = false`). An `unknown` reading is can't verify, `unresolved` (R35). Values are compared as canonical text, `true` or `false`.

## verified

The handler's `typeId` is an `int` and its `brandId` an `int?`, as modelled. eShopCustom's house pack says a Service is the class of its name.

```json case
{ "phase": 2, "kinds": ["parameter.nullable"], "traces": ["ESC-SVC-30"], "requirements": ["R2", "R41", "R42", "R53"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
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
    { "kind": "parameter.nullable", "element": "catalog-api-get-items-by-brand-and-type-type-id", "outcome": "verified", "expected": "false", "actual": "false", "at": "src/Catalog.API/Apis/CatalogApi.cs:8" },
    { "kind": "parameter.nullable", "element": "catalog-api-get-items-by-brand-and-type-brand-id", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Catalog.API/Apis/CatalogApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/parameter.nullable", "count": 0 }
  ]
}
```

## contradicted

`brandId` is a plain `int`, so the handler always needs a brand, against the model's optional one: contradicted, `services.parameter.nullability` in eShopCustom's terms.

```json case
{ "phase": 2, "base": "verified", "kinds": ["parameter.nullable"], "traces": ["ESC-SVC-30"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Catalog.API/Apis/CatalogApi.cs
namespace eShop.Catalog.API;

public static class CatalogApi
{
    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByBrandAndTypeId(
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] CatalogServices services,
        int typeId,
        int brandId)
    {
        return await GetAllItems(paginationRequest, services, null, [typeId], [brandId]);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.nullable", "element": "catalog-api-get-items-by-brand-and-type-brand-id", "outcome": "contradicted", "expected": "true", "actual": "false", "at": "src/Catalog.API/Apis/CatalogApi.cs:9" }
  ],
  "findings": [
    { "rule": "claim/parameter.nullable", "severity": "error", "subject": "catalog-api-get-items-by-brand-and-type-brand-id" }
  ]
}
```

## unresolved

basislink5's API project turns nullable reference types off, so the action's `string expDate` says nothing about whether it may be null: can't verify, `unresolved`, never a guess either way (R35). Its `string commodity` is the same, so the one note counts both. basislink5's house pack says a Service is the controller class of its name.

```json case
{ "phase": 2, "kinds": ["parameter.nullable"], "cases": ["engine/unresolved"], "traces": ["ESC-SVC-30"], "requirements": ["R1", "R5", "R35"], "packs": ["intent.core", "intent.services", "house.basislink"], "language": "csharp" }
```

```json pack
{
  "id": "house.basislink",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "types": {
        "Service": { "typeKinds": ["class"], "locations": ["Service/**/Controllers/**"], "note": "basislink5: each controller is a Service named as the class is" }
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
        "id": "services", "name": "Basislink.API",
        "elements": [
          { "id": "futures-controller", "type": "Service", "name": "FuturesController",
            "children": [
              { "id": "futures-controller-get-safex", "type": "Operation", "name": "GetSafex",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "Safex/Get/{expDate}/{commodity}" } },
                "children": [
                  { "id": "futures-controller-get-safex-exp-date", "type": "Parameter", "name": "expDate", "typeRef": { "type": "string" } },
                  { "id": "futures-controller-get-safex-commodity", "type": "Parameter", "name": "commodity", "typeRef": { "type": "string" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Basislink", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "Service/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```xml file Service/BasislinkService/Basislink.API/Basislink.API.csproj
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>disable</Nullable>
  </PropertyGroup>

</Project>
```

```csharp file Service/BasislinkService/Basislink.API/Controllers/Grain/FuturesController.cs
using Microsoft.AspNetCore.Mvc;

namespace Basislink.API.Controllers.Grain
{
    [ApiController]
    [Route("v1/[controller]")]
    public class FuturesController : ControllerBase
    {
        private readonly FuturesProcess _futuresProcess;

        public FuturesController(FuturesProcess futuresProcess)
        {
            _futuresProcess = futuresProcess;
        }

        [HttpGet("Safex/Get/{expDate}/{commodity}")]
        public IActionResult GetSafex(string expDate, string commodity)
        {
            var data = _futuresProcess.GetSafex(expDate, commodity);
            return Ok(data);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.nullable", "element": "futures-controller-get-safex-exp-date", "outcome": "cantVerify", "reason": "unresolved" }
  ],
  "findings": [
    { "rule": "claim/parameter.nullable", "subject": "futures-controller-get-safex-exp-date", "count": 0 },
    { "rule": "engine/notStated", "severity": "info", "messageContains": "Not checked: 2 nullability claims (Services 2)" }
  ]
}
```

## java-required-false

With the Spring MVC pack, Service `InventoryService` is its `@RestController`, `InventoryController`. `@RequestParam(required = false) Integer maxResults` is an optional, boxed parameter: nullable, as the model's `maxResults` is.

```json case
{ "phase": 4, "kinds": ["parameter.nullable"], "traces": ["JAV-SVC-11"], "requirements": ["R2", "R33", "R41", "R42"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Backend",
        "elements": [
          { "id": "stock-movement-dto", "type": "DTO", "name": "StockMovementDto",
            "children": [
              { "id": "stock-movement-dto-quantity", "type": "DTO-Field", "name": "Quantity", "typeRef": { "type": "int" } }
            ] },
          { "id": "inventory-service", "type": "Service", "name": "InventoryService",
            "children": [
              { "id": "inventory-service-get-stock-movements", "type": "Operation", "name": "GetStockMovements", "typeRef": { "element": "stock-movement-dto", "collection": true },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "api/inventory/{productId}/movements" } },
                "children": [
                  { "id": "inventory-service-get-stock-movements-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "guid" } },
                  { "id": "inventory-service-get-stock-movements-max-results", "type": "Parameter", "name": "maxResults", "typeRef": { "type": "int", "nullable": true } }
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

```java file src/main/java/com/eshop/backend/application/StockMovementDto.java
package com.eshop.backend.application;

public record StockMovementDto(int quantity) {
}
```

```java file src/main/java/com/eshop/backend/api/InventoryController.java
package com.eshop.backend.api;

import com.eshop.backend.application.StockLevels;
import com.eshop.backend.application.StockMovementDto;
import java.util.List;
import java.util.UUID;
import org.springframework.web.bind.annotation.*;

@RestController
public class InventoryController {
    private final StockLevels stockLevels;

    public InventoryController(StockLevels stockLevels) {
        this.stockLevels = stockLevels;
    }

    @GetMapping("/api/inventory/{productId}/movements")
    public List<StockMovementDto> getStockMovements(@PathVariable UUID productId, @RequestParam(required = false) Integer maxResults) {
        return stockLevels.movements(productId, maxResults);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.nullable", "element": "inventory-service-get-stock-movements-max-results", "outcome": "verified", "expected": "true", "actual": "true" }
  ]
}
```

## java-required-contradicted

`@RequestParam int maxResults` is required, and a primitive besides: not nullable, against the model's optional `maxResults`. Contradicted.

```json case
{ "phase": 4, "base": "java-required-false", "kinds": ["parameter.nullable"], "traces": ["JAV-SVC-11"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/api/InventoryController.java
package com.eshop.backend.api;

import com.eshop.backend.application.StockLevels;
import com.eshop.backend.application.StockMovementDto;
import java.util.List;
import java.util.UUID;
import org.springframework.web.bind.annotation.*;

@RestController
public class InventoryController {
    private final StockLevels stockLevels;

    public InventoryController(StockLevels stockLevels) {
        this.stockLevels = stockLevels;
    }

    @GetMapping("/api/inventory/{productId}/movements")
    public List<StockMovementDto> getStockMovements(@PathVariable UUID productId, @RequestParam int maxResults) {
        return stockLevels.movements(productId, maxResults);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "parameter.nullable", "element": "inventory-service-get-stock-movements-max-results", "outcome": "contradicted", "expected": "true", "actual": "false" }
  ],
  "findings": [
    { "rule": "claim/parameter.nullable", "severity": "error", "subject": "inventory-service-get-stock-movements-max-results" }
  ]
}
```
