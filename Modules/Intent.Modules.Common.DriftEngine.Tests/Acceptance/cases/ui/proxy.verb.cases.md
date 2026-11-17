# proxy.verb

The HTTP verb a front end's proxy operation sends, against the backend Operation it calls. In the Java spike's Front End designer, a Proxy Operation has a Calls association to a Services designer Operation, whose Http Settings stereotype's Verb is the model's value. The kind is `stated: always` and compares exactly.

No stock pack knows the Front End designer, so these cases' house pack extends `intent.ui`: it claims where the Proxy is (`type.exists`, from its Implementation Path) and the stock `proxy.verb` on its Calls association. The stock Vue reader (`intent.vue`) follows the proxy method's `request('VERB', path)` call into the module's own `request` helper, whose `fetch` sends `{ method }`, the helper's parameter, so the verb is what the method passes. The reading is placed at the method's `request(...)` call.

## verified

The storefront's API client reads one product with a `GET`, and the called backend Operation's Http Settings Verb is `GET`.

```json case
{ "phase": "later", "kinds": ["proxy.verb"], "traces": ["JAV-FE-11"], "requirements": ["R2", "R28", "R30", "R38"], "packs": ["intent.core", "intent.ui", "house.shop"], "language": "typescript" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.ui", "version": "^1.0.0" }],
  "languages": ["typescript"],
  "designers": ["Front End"],
  "vocabulary": {
    "Front End": {
      "entries": [
        {
          "kind": "type.exists",
          "for": { "element": "Proxy" },
          "value": "self",
          "bindings": { "Name": "name", "Proxy": "name", "Path": "stereotype('Implementation').Path" },
          "own": ["Name"],
          "source": "the proxy's Implementation Path"
        },
        {
          "kind": "proxy.verb",
          "for": { "association": "Calls" },
          "value": "target.element.stereotype('Http Settings').Verb",
          "bindings": { "Operation": "source.element.name" },
          "own": ["Operation"],
          "source": "the called Operation's Http Settings Verb"
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
    { "kind": "proxy.verb", "element": "get-product-by-id-calls-backend", "outcome": "verified",
      "expected": "GET", "actual": "GET", "at": "src/api/client.ts:11" }
  ]
}
```

## contradicted

The proxy method now sends a `POST`, and the backend Operation it calls answers `GET`.

```json case
{ "phase": "later", "base": "verified", "kinds": ["proxy.verb"], "traces": ["JAV-FE-11"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui", "house.shop"], "language": "typescript" }
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
        return request<ProductDto>('POST', `/api/products/${encodeURIComponent(id)}`);
    },
};
```

```json expect
{
  "claims": [
    { "kind": "proxy.verb", "element": "get-product-by-id-calls-backend", "outcome": "contradicted",
      "expected": "GET", "actual": "POST", "at": "src/api/client.ts:11" }
  ],
  "findings": [
    { "rule": "claim/proxy.verb", "severity": "error", "subject": "get-product-by-id-calls-backend" }
  ]
}
```

## vue-standard-backend-call

The standard User Interface designer (2.0.0): a Proxy whose Implementation Path names its file, `api/client.ts` from the front end's own root, and a Proxy Operation with a Call Service Operation Action to the backend Operation in another application's Services designer. The verb and route are the called Operation's. The stock Vue reader locates the Proxy as the `api` object in that file, follows each method's `request(...)` into the module's own helper and reads its `fetch` with the helper's parameters bound to what the method passes; the base URL before the path isn't part of the route. `placeOrder` sends a `PUT` where the backend answers `POST`.

```json case
{ "phase": "later", "kinds": ["proxy.verb", "proxy.route"], "requirements": ["R2", "R10", "R14", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "api-folder", "type": "Folder", "name": "api", "children": [
            { "id": "api", "type": "Proxy", "name": "Api", "stereotypes": { "Implementation": { "Path": "api/client.ts" } }, "children": [
              { "id": "get-order", "type": "Proxy Operation", "name": "GetOrderById", "children": [
                { "id": "get-order-id", "type": "Parameter", "name": "id", "typeRef": { "type": "string" } }
              ] },
              { "id": "place-order", "type": "Proxy Operation", "name": "PlaceOrder", "children": [
                { "id": "place-order-data", "type": "Parameter", "name": "data", "typeRef": { "type": "object" } }
              ] }
            ] }
          ] }
        ]
      }],
      "associations": [
        { "id": "get-order-calls-backend", "type": "Call Service Operation Action", "source": { "element": "get-order" }, "target": { "element": "orders-get-by-id" } },
        { "id": "place-order-calls-backend", "type": "Call Service Operation Action", "source": { "element": "place-order" }, "target": { "element": "orders-place" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  },
  "references": {
    "Services": {
      "packages": [{
        "id": "backend", "name": "Backend",
        "elements": [
          { "id": "orders-service", "type": "Service", "name": "OrdersService", "stereotypes": { "Http Service Settings": { "Route": "api/orders" } }, "children": [
            { "id": "orders-get-by-id", "type": "Operation", "name": "GetOrderById", "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{id}" } } },
            { "id": "orders-place", "type": "Operation", "name": "PlaceOrder", "stereotypes": { "Http Settings": { "Verb": "POST" } } }
          ] }
        ]
      }]
    }
  }
}
```

```typescript file src/api/client.ts
const baseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:8080';

async function request<T>(method: string, path: string, body?: unknown): Promise<T> {
    const res = await fetch(`${baseUrl}${path}`, { method, body: body === undefined ? undefined : JSON.stringify(body) });
    return (await res.json()) as T;
}

export const api = {
    getOrderById(id: string): Promise<unknown> {
        return request<unknown>('GET', `/api/orders/${encodeURIComponent(id)}`);
    },

    placeOrder(data: object): Promise<string> {
        return request<string>('PUT', '/api/orders', data);
    },
};
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "api", "outcome": "verified", "at": "src/api/client.ts:8" },
    { "kind": "proxy.verb", "element": "get-order-calls-backend", "outcome": "verified", "expected": "GET", "actual": "GET", "at": "src/api/client.ts:10" },
    { "kind": "proxy.route", "element": "get-order-calls-backend", "outcome": "verified", "expected": "api/orders/{id}", "actual": "api/orders/{id}", "at": "src/api/client.ts:10" },
    { "kind": "proxy.verb", "element": "place-order-calls-backend", "outcome": "contradicted", "expected": "POST", "actual": "PUT", "at": "src/api/client.ts:14" },
    { "kind": "proxy.route", "element": "place-order-calls-backend", "outcome": "verified", "expected": "api/orders", "at": "src/api/client.ts:14" }
  ],
  "findings": [
    { "rule": "claim/proxy.verb", "severity": "error", "subject": "place-order-calls-backend" },
    { "rule": "claim/proxy.route", "count": 0 }
  ]
}
```

## eshop-http-verb

eShopCustom's Blazor catalog proxy: a Service Proxy Operation's Backend Operation stereotype names the backend Operation, whose Http Settings Verb is the model's value. `GetFromJsonAsync` sends a GET. The house pack gives the vocabulary for eShopCustom's own stereotype; reading the verb from the proxy method's `HttpClient` call is the stock proxy reader's.

```json case
{ "phase": "later", "kinds": ["proxy.verb"], "traces": ["ESC-FE-60"], "requirements": ["R2", "R28"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
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
    { "kind": "proxy.verb", "element": "catalog-service-get-catalog-item", "outcome": "verified",
      "expected": "GET", "actual": "GET", "at": "src/WebAppComponents/Services/CatalogService.cs:13" }
  ]
}
```

## eshop-http-verb-contradicted

The proxy now posts the item's id with `PostAsJsonAsync`, and the backend Operation answers GET.

```json case
{ "phase": "later", "base": "eshop-http-verb", "kinds": ["proxy.verb"], "traces": ["ESC-FE-60"], "requirements": ["R2", "R14"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/WebAppComponents/Services/CatalogService.cs
using System.Net.Http.Json;
using eShop.WebAppComponents.Catalog;

namespace eShop.WebAppComponents.Services;

public class CatalogService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "api/catalog/";

    public async Task<CatalogItem?> GetCatalogItem(int id)
    {
        var response = await httpClient.PostAsJsonAsync($"{remoteServiceBaseUrl}items", id);
        return await response.Content.ReadFromJsonAsync<CatalogItem>();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "proxy.verb", "element": "catalog-service-get-catalog-item", "outcome": "contradicted",
      "expected": "GET", "actual": "POST", "at": "src/WebAppComponents/Services/CatalogService.cs:12" }
  ],
  "findings": [
    { "rule": "claim/proxy.verb", "severity": "error", "subject": "catalog-service-get-catalog-item" }
  ]
}
```
