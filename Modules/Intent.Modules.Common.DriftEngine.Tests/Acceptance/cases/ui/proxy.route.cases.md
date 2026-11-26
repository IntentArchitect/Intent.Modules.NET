# proxy.route

The path a front end's proxy operation sends its request to, against the route of the backend Operation it calls: the Service's Http Service Settings Route followed by the Operation's Http Settings Route, with `[controller]` standing for the Service's name without its `Service` suffix. In the Java spike's Front End designer, a Proxy Operation has a Calls association to that Operation. The kind is `stated: always` and compares with the `route` comparer, which ignores a leading or trailing `/` and case in literal segments.

No stock pack knows the Front End designer, so these cases' house pack extends `intent.ui`: it claims where the Proxy is (`type.exists`, from its Implementation Path) and the stock `proxy.route` on its Calls association. The stock Vue reader (`intent.vue`) follows the proxy method's `request('VERB', path)` call into the module's own `request` helper, to its `fetch`, and reads the URL with the helper's `path` bound to what the method passes, so each `${encodeURIComponent(name)}` is a value standing for a route parameter. The reading is placed at the method's `request(...)` call.

## verified

The client builds `/api/products/${encodeURIComponent(id)}`, which reads as `/api/products/{id}`. The model's route is `api/[controller]` on `ProductsService` and `{id}` on its Operation, so `api/Products/{id}`; the two differ only in the leading `/` and the literal segment's case.

```json case
{ "phase": "later", "kinds": ["proxy.route"], "traces": ["JAV-FE-11"], "requirements": ["R2", "R10", "R28", "R30"], "packs": ["intent.core", "intent.ui", "house.shop"], "language": "typescript" }
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
          "kind": "proxy.route",
          "for": { "association": "Calls" },
          "value": "replace(target.element.parent.stereotype('Http Service Settings').Route, '[controller]', trimEnd(target.element.parent.name, 'Service')) + '/' + target.element.stereotype('Http Settings').Route",
          "bindings": { "Operation": "source.element.name" },
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
      "expected": "api/Products/{id}", "actual": "api/products/{id}", "at": "src/api/client.ts:11" }
  ]
}
```

## contradicted

The client now sends to `/api/product/…`, a segment the backend's route doesn't have.

```json case
{ "phase": "later", "base": "verified", "kinds": ["proxy.route"], "traces": ["JAV-FE-11"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui", "house.shop"], "language": "typescript" }
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
      "expected": "api/Products/{id}", "actual": "api/product/{id}", "at": "src/api/client.ts:11" }
  ],
  "findings": [
    { "rule": "claim/proxy.route", "severity": "error", "subject": "get-product-by-id-calls-backend" }
  ]
}
```

## vue-standard-http-request

The standard User Interface designer (2.0.0): a Proxy Operation that calls no backend Operation states its request with the HTTP Request stereotype, a Verb and a Route relative to the Base URL. The stock Vue reader reads an axios call's verb method and URL; the code's `/api/legacy/stock` isn't the modelled `api/stock/{sku}`.

```json case
{ "phase": "later", "kinds": ["proxy.route", "proxy.verb"], "requirements": ["R2", "R10", "R14", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "stock-proxy", "type": "Proxy", "name": "StockClient", "children": [
            { "id": "get-stock", "type": "Proxy Operation", "name": "GetStock",
              "stereotypes": { "HTTP Request": { "Verb": "GET", "Route": "api/stock/{sku}" } },
              "children": [ { "id": "get-stock-sku", "type": "Parameter", "name": "sku", "typeRef": { "type": "string" } } ] }
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

```typescript file src/stockClient.ts
import axios from 'axios';

export const stockClient = {
    getStock(sku: string): Promise<number> {
        return axios.get(`/api/legacy/stock?sku=${sku}`).then((x) => x.data);
    },
};
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "stock-proxy", "outcome": "verified", "at": "src/stockClient.ts:3" },
    { "kind": "proxy.verb", "element": "get-stock", "outcome": "verified", "expected": "GET", "actual": "GET", "at": "src/stockClient.ts:5" },
    { "kind": "proxy.route", "element": "get-stock", "outcome": "contradicted", "expected": "api/stock/{sku}", "actual": "api/legacy/stock", "at": "src/stockClient.ts:5" }
  ],
  "findings": [
    { "rule": "claim/proxy.route", "severity": "error", "subject": "get-stock" }
  ]
}
```

## react-axios-instance-request

A React front end's proxy usually sends through an axios instance its code creates, named as the code likes (`http = axios.create({ baseURL })`). The proxy reader every TypeScript front end shares reads a call on a variable given `axios.create(...)` as it does one on `axios`: `getProduct`'s `http.get(`/products/${id}`)` is `GET products/{id}`, and `removeLine` sends a `DELETE` to `cart/lines/{id}` where the model has `cart/{id}`.

```json case
{ "phase": "later", "kinds": ["proxy.route", "proxy.verb"], "requirements": ["R2", "R10", "R14", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "shop-api", "type": "Proxy", "name": "ShopApi", "children": [
            { "id": "get-product", "type": "Proxy Operation", "name": "GetProduct",
              "stereotypes": { "HTTP Request": { "Verb": "GET", "Route": "products/{id}" } } },
            { "id": "remove-line", "type": "Proxy Operation", "name": "RemoveLine",
              "stereotypes": { "HTTP Request": { "Verb": "DELETE", "Route": "cart/{id}" } } }
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

```js file src/http.js
import axios from 'axios';

export const http = axios.create({ baseURL: import.meta.env.VITE_API_URL });
```

```js file src/shopApi.js
import { http } from './http';

export const shopApi = {
    getProduct(id) {
        return http.get(`/products/${id}`).then((x) => x.data);
    },

    removeLine(id) {
        return http.delete(`/cart/lines/${id}`);
    },
};
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "shop-api", "outcome": "verified", "at": "src/shopApi.js:3" },
    { "kind": "proxy.verb", "element": "get-product", "outcome": "verified", "actual": "GET", "at": "src/shopApi.js:5" },
    { "kind": "proxy.route", "element": "get-product", "outcome": "verified", "actual": "products/{id}", "at": "src/shopApi.js:5" },
    { "kind": "proxy.verb", "element": "remove-line", "outcome": "verified", "actual": "DELETE", "at": "src/shopApi.js:9" },
    { "kind": "proxy.route", "element": "remove-line", "outcome": "contradicted", "expected": "cart/{id}", "actual": "cart/lines/{id}", "at": "src/shopApi.js:9" }
  ],
  "findings": [
    { "rule": "claim/proxy.route", "severity": "error", "subject": "remove-line" }
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

## csharp-request-wrapper

A C# proxy that sends its requests through the application's own wrapper, `IRequestProvider`, as eShop's MAUI client does: the wrapper's implementation sends an `HttpClient` request to the URI it is given, so a call of it is the proxy's request, read with the wrapper's `uri` bound to what the proxy passes. No wrapper name is known to the engine. The URI starts with the gateway's base address, a setting, which is dropped; the backend service's route carries its API version as a query string, which isn't part of the path. `CancelOrderAsync` posts where the backend answers PUT, and its route holds; the route of `GetOrdersAsync` names `order` where the backend serves `orders`.

```json case
{ "phase": "later", "kinds": ["proxy.route", "proxy.verb"], "requirements": ["R2", "R10", "R14", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "clientapp", "name": "ClientApp", "type": "User Interface Package",
        "elements": [
          { "id": "order-service", "type": "Proxy", "name": "OrderService", "children": [
            { "id": "get-orders", "type": "Proxy Operation", "name": "GetOrdersAsync" },
            { "id": "cancel-order", "type": "Proxy Operation", "name": "CancelOrderAsync", "children": [
              { "id": "cancel-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "int" } }
            ] }
          ] }
        ]
      }],
      "associations": [
        { "id": "get-orders-calls-backend", "type": "Call Service Operation Action", "source": { "element": "get-orders" }, "target": { "element": "ordering-get-orders" } },
        { "id": "cancel-order-calls-backend", "type": "Call Service Operation Action", "source": { "element": "cancel-order" }, "target": { "element": "ordering-cancel-order" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  },
  "references": {
    "Services": {
      "packages": [{
        "id": "ordering-api", "name": "Ordering.API",
        "elements": [
          { "id": "ordering-api-service", "type": "Service", "name": "OrdersApi", "stereotypes": { "Http Service Settings": { "Route": "api/orders?api-version={version}" } }, "children": [
            { "id": "ordering-get-orders", "type": "Operation", "name": "GetOrdersByUser", "stereotypes": { "Http Settings": { "Verb": "GET" } } },
            { "id": "ordering-cancel-order", "type": "Operation", "name": "CancelOrder", "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } } }
          ] }
        ]
      }]
    }
  }
}
```

```csharp file src/ClientApp/Services/RequestProvider/RequestProvider.cs
using System.Net.Http.Json;

namespace eShop.ClientApp.Services.RequestProvider;

public interface IRequestProvider
{
    Task<TResult> GetAsync<TResult>(string uri, string token = "");

    Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data, string token = "");

    Task<bool> PutAsync(string uri, string token = "");
}

public class RequestProvider(HttpClient httpClient) : IRequestProvider
{
    public async Task<TResult> GetAsync<TResult>(string uri, string token = "")
    {
        var response = await httpClient.GetAsync(uri);
        return (await response.Content.ReadFromJsonAsync<TResult>())!;
    }

    public async Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data, string token = "")
    {
        var response = await httpClient.PostAsJsonAsync(uri, data);
        return (await response.Content.ReadFromJsonAsync<TResult>())!;
    }

    public async Task<bool> PutAsync(string uri, string token = "")
    {
        var response = await httpClient.PutAsync(uri, null);
        return response.IsSuccessStatusCode;
    }
}
```

```csharp file src/ClientApp/Services/Order/OrderService.cs
using eShop.ClientApp.Services.RequestProvider;

namespace eShop.ClientApp.Services.Order;

public class OrderService(IRequestProvider requestProvider, ISettingsService settings)
{
    public Task<List<OrderSummary>> GetOrdersAsync() =>
        requestProvider.GetAsync<List<OrderSummary>>($"{settings.GatewayOrdersEndpointBase}api/order?api-version=1.0");

    public Task<bool> CancelOrderAsync(int orderId) =>
        requestProvider.PostAsync<object, bool>($"{settings.GatewayOrdersEndpointBase}api/orders/cancel?api-version=1.0", new { orderId });
}

public record OrderSummary(int OrderNumber);
```

```json expect
{
  "claims": [
    { "kind": "proxy.route", "element": "get-orders-calls-backend", "outcome": "contradicted", "expected": "api/orders", "actual": "api/order", "at": "src/ClientApp/Services/Order/OrderService.cs:8" },
    { "kind": "proxy.verb", "element": "get-orders-calls-backend", "outcome": "verified", "expected": "GET", "actual": "GET", "at": "src/ClientApp/Services/Order/OrderService.cs:8" },
    { "kind": "proxy.route", "element": "cancel-order-calls-backend", "outcome": "verified", "expected": "api/orders/cancel", "actual": "api/orders/cancel", "at": "src/ClientApp/Services/Order/OrderService.cs:11" },
    { "kind": "proxy.verb", "element": "cancel-order-calls-backend", "outcome": "contradicted", "expected": "PUT", "actual": "POST", "at": "src/ClientApp/Services/Order/OrderService.cs:11" }
  ],
  "findings": [
    { "rule": "claim/proxy.route", "severity": "error", "subject": "get-orders-calls-backend" },
    { "rule": "claim/proxy.verb", "severity": "error", "subject": "cancel-order-calls-backend" }
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
