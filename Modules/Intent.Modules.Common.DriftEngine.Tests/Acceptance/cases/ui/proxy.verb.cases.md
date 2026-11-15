# proxy.verb

The HTTP verb a front end's proxy operation sends, against the backend Operation it calls. In the Java spike's Front End designer, a Proxy Operation has a Calls association to a Services designer Operation, whose Http Settings stereotype's Verb is the model's value. The kind is `stated: always` and compares exactly.

The kind is `later`, so these cases' house pack gives the kind, the Front End designer's vocabulary and the rule. The claim sits on the Calls association. The rule reads the first argument of the `request('VERB', path)` call in the proxy method, in the Proxy's file, as text.

## verified

The storefront's API client reads one product with a `GET`, and the called backend Operation's Http Settings Verb is `GET`.

```json case
{ "phase": "later", "kinds": ["proxy.verb"], "traces": ["JAV-FE-11"], "requirements": ["R2", "R28", "R30", "R38"], "packs": ["intent.core", "intent.java", "house.shop"], "language": "typescript" }
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
    { "kind": "proxy.verb", "label": "proxy verb", "valueType": "text", "comparer": "exact", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "Front End": {
      "entries": [
        {
          "kind": "proxy.verb",
          "for": { "association": "Calls" },
          "value": "target.element.stereotype('Http Settings').Verb",
          "bindings": {
            "File": "source.element.parent.stereotype('Implementation').Path",
            "Operation": "source.element.name"
          },
          "own": ["Operation"],
          "source": "the called Operation's Http Settings Verb"
        }
      ]
    }
  },
  "rules": {
    "Front End": {
      "files": ["src/**/*.ts"],
      "facts": {
        "proxy.verb": {
          "find": "file:{File}",
          "read": [{ "match": "\\b{Operation}\\([^)]*\\):[^{]*\\{return request<[^>]*>\\('(\\w+)'" }],
          "unstated": "no `request('VERB', …)` in `{Operation}` in `{File}`"
        }
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
      "expected": "GET", "actual": "GET", "at": "src/api/client.ts:10" }
  ]
}
```

## contradicted

The proxy method now sends a `POST`, and the backend Operation it calls answers `GET`.

```json case
{ "phase": "later", "base": "verified", "kinds": ["proxy.verb"], "traces": ["JAV-FE-11"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.java", "house.shop"], "language": "typescript" }
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
      "expected": "GET", "actual": "POST", "at": "src/api/client.ts:10" }
  ],
  "findings": [
    { "rule": "claim/proxy.verb", "severity": "error", "subject": "get-product-by-id-calls-backend" }
  ]
}
```

## eshop-http-verb

eShopCustom's Blazor catalog proxy: a Service Proxy Operation's Backend Operation stereotype names the backend Operation, whose Http Settings Verb is the model's value. `GetFromJsonAsync` sends a GET. The house pack gives the vocabulary for eShopCustom's own stereotype; reading the verb from the proxy method's `HttpClient` call is the stock rule's, as `engine/unreadable`'s house recipe does for a few calls.

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
