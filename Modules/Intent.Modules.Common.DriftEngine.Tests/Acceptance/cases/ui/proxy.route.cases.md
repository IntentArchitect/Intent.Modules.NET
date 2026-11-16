# proxy.route

The path a front end's proxy operation sends its request to, against the route of the backend Operation it calls: the Service's Http Service Settings Route followed by the Operation's Http Settings Route, with `[controller]` standing for the Service's name without its `Service` suffix. In the Java spike's Front End designer, a Proxy Operation has a Calls association to that Operation. The kind is `stated: always` and compares with the `route` comparer, which ignores a leading or trailing `/` and case in literal segments.

No stock pack knows the Front End designer, so these cases' house pack gives the kind and its vocabulary. The claim sits on the Calls association. The proxy method states the route as the path argument of its `request('VERB', path)` call, where each `${encodeURIComponent(name)}` hole is the route parameter `{name}`, which its module's claim reader (`house.readers`) reads.

## verified

The client builds `/api/products/${encodeURIComponent(id)}`, which reads as `/api/products/{id}`. The model's route is `api/[controller]` on `ProductsService` and `{id}` on its Operation, so `api/Products/{id}`; the two differ only in the leading `/` and the literal segment's case.

```json case
{ "phase": "later", "kinds": ["proxy.route"], "traces": ["JAV-FE-11"], "requirements": ["R2", "R10", "R28", "R30"], "packs": ["intent.core", "intent.java", "house.shop", "house.readers"], "language": "typescript" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.java", "version": "^1.0.0" }],
  "languages": ["typescript"],
  "designers": ["Front End"],
  "kinds": [
    { "kind": "proxy.route", "label": "proxy route", "valueType": "route", "comparer": "route", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "Front End": {
      "entries": [
        {
          "kind": "proxy.route",
          "for": { "association": "Calls" },
          "value": "replace(target.element.parent.stereotype('Http Service Settings').Route, '[controller]', trimEnd(target.element.parent.name, 'Service')) + '/' + target.element.stereotype('Http Settings').Route",
          "bindings": {
            "File": "source.element.parent.stereotype('Implementation').Path",
            "Operation": "source.element.name"
          },
          "own": ["Operation"],
          "source": "the called Operation's Service route and Http Settings Route"
        }
      ]
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
          { "id": "api-proxy", "type": "Proxy", "name": "api",
            "stereotypes": { "Implementation": { "Path": "src/api/client.ts" } },
            "children": [
              { "id": "api-get-product-by-id", "type": "Proxy Operation", "name": "getProductById",
                "children": [
                  { "id": "api-get-product-by-id-id", "type": "Parameter", "name": "id", "typeRef": { "type": "string" } }
                ] }
            ] }
        ]
      }],
      "associations": [
        { "id": "get-product-by-id-calls-backend", "type": "Calls",
          "source": { "element": "api-get-product-by-id" }, "target": { "element": "products-get-product-by-id" } }
      ]
    },
    "Services": {
      "packages": [{
        "id": "catalog-services", "name": "Catalog",
        "elements": [
          { "id": "products-service", "type": "Service", "name": "ProductsService",
            "stereotypes": { "Http Service Settings": { "Route": "api/[controller]" } },
            "children": [
              { "id": "products-get-product-by-id", "type": "Operation", "name": "GetProductById",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{id}" } },
                "children": [
                  { "id": "products-get-product-by-id-id", "type": "Parameter", "name": "id", "typeRef": { "type": "guid" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```typescript file src/api/client.ts
import type { ProductDto } from './types';

async function request<T>(method: string, path: string, body?: unknown): Promise<T> {
    const res = await fetch(path, { method, body: body === undefined ? undefined : JSON.stringify(body) });
    return (await res.json()) as T;
}

export const api = {
    // GET /api/products/{id}
    getProductById(id: string): Promise<ProductDto> {
        return request<ProductDto>('GET', `/api/products/${encodeURIComponent(id)}`);
    },
};
```

```json expect
{
  "claims": [
    { "kind": "proxy.route", "element": "get-product-by-id-calls-backend", "outcome": "verified",
      "expected": "api/Products/{id}", "at": "src/api/client.ts:10" }
  ]
}
```

## contradicted

The client now sends to `/api/product/…`, a segment the backend's route doesn't have.

```json case
{ "phase": "later", "base": "verified", "kinds": ["proxy.route"], "traces": ["JAV-FE-11"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.java", "house.shop", "house.readers"], "language": "typescript" }
```

```typescript file src/api/client.ts
import type { ProductDto } from './types';

async function request<T>(method: string, path: string, body?: unknown): Promise<T> {
    const res = await fetch(path, { method, body: body === undefined ? undefined : JSON.stringify(body) });
    return (await res.json()) as T;
}

export const api = {
    // GET /api/products/{id}
    getProductById(id: string): Promise<ProductDto> {
        return request<ProductDto>('GET', `/api/product/${encodeURIComponent(id)}`);
    },
};
```

```json expect
{
  "claims": [
    { "kind": "proxy.route", "element": "get-product-by-id-calls-backend", "outcome": "contradicted",
      "expected": "api/Products/{id}", "at": "src/api/client.ts:10" }
  ],
  "findings": [
    { "rule": "claim/proxy.route", "severity": "error", "subject": "get-product-by-id-calls-backend" }
  ]
}
```

## eshop-http-route

eShopCustom's Blazor catalog proxy builds its URI from a field and an interpolation: `api/catalog/` and `items/{id}`, the string the engine's value evaluation gives. The backend Operation its Backend Operation stereotype names is at `api/catalog` and `items/{id}`, so the routes match segment by segment, the hole against the parameter. The house pack gives the vocabulary for eShopCustom's own stereotype; reading the path from the proxy's `HttpClient` call is the stock proxy reader's.

```json case
{ "phase": "later", "kinds": ["proxy.route"], "traces": ["ESC-FE-61"], "requirements": ["R2", "R10", "R28"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "kinds": [
    { "kind": "proxy.route", "label": "proxy route", "valueType": "text", "comparer": "route", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "User Interface": {
      "entries": [
        {
          "kind": "proxy.route",
          "for": { "element": "Operation", "where": "parent.specialization == 'Service Proxy' && hasStereotype('Backend Operation')" },
          "value": "stereotype('Backend Operation').Operation.parent.stereotype('Http Service Settings').Route + '/' + stereotype('Backend Operation').Operation.stereotype('Http Settings').Route",
          "bindings": { "Proxy": "parent.name", "Operation": "name" },
          "own": ["Proxy", "Operation"],
          "source": "the Service and Operation routes of the backend Operation its Backend Operation stereotype names"
        }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "catalog-api", "name": "Catalog.API",
        "elements": [
          { "id": "catalog-api-service", "type": "Service", "name": "CatalogApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/catalog" } },
            "children": [
              { "id": "get-item-by-id", "type": "Operation", "name": "GetItemById",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "items/{id}" } },
                "children": [
                  { "id": "get-item-by-id-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "User Interface": {
      "packages": [{
        "id": "webappcomponents", "name": "WebAppComponents",
        "elements": [
          { "id": "catalog-service", "type": "Service Proxy", "name": "CatalogService",
            "children": [
              { "id": "catalog-service-get-catalog-item", "type": "Operation", "name": "GetCatalogItem",
                "stereotypes": { "Backend Operation": { "Operation": { "ref": "get-item-by-id" } } },
                "children": [
                  { "id": "catalog-service-get-catalog-item-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
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

```csharp file src/WebAppComponents/Services/CatalogService.cs
using System.Net.Http.Json;
using eShop.WebAppComponents.Catalog;

namespace eShop.WebAppComponents.Services;

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
  "claims": [
    { "kind": "proxy.route", "element": "catalog-service-get-catalog-item", "outcome": "verified",
      "expected": "api/catalog/items/{id}", "at": "src/WebAppComponents/Services/CatalogService.cs:13" }
  ]
}
```

## eshop-http-route-contradicted

The proxy asks for `items/by/{id}`, a path the backend Operation doesn't serve.

```json case
{ "phase": "later", "base": "eshop-http-route", "kinds": ["proxy.route"], "traces": ["ESC-FE-61"], "requirements": ["R2", "R14"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/WebAppComponents/Services/CatalogService.cs
using System.Net.Http.Json;
using eShop.WebAppComponents.Catalog;

namespace eShop.WebAppComponents.Services;

public class CatalogService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "api/catalog/";

    public Task<CatalogItem?> GetCatalogItem(int id)
    {
        var uri = $"{remoteServiceBaseUrl}items/by/{id}";
        return httpClient.GetFromJsonAsync<CatalogItem>(uri);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "proxy.route", "element": "catalog-service-get-catalog-item", "outcome": "contradicted",
      "expected": "api/catalog/items/{id}", "at": "src/WebAppComponents/Services/CatalogService.cs:13" }
  ],
  "findings": [
    { "rule": "claim/proxy.route", "severity": "error", "subject": "catalog-service-get-catalog-item" }
  ]
}
```
