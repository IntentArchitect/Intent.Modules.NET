# proxy.request

A Service Proxy's Operation, in the User Interface designer, sends a request to the backend: its method makes an HTTP call or a gRPC call. It is a `presence` kind, an existence kind, `stated: always`, claimed on each Operation of a Service Proxy. An Operation with eShopCustom's Backend Operation stereotype states that it sends a request; one without states that it sends none, so a request the code sends for it is reported, naming the request.

The kind is `later`, so these cases' house pack gives the kind, its vocabulary and its rule. The vocabulary asks only whether the Backend Operation stereotype is applied; the stereotype's Operation property, which names the backend Operation, isn't read here, so the fixtures leave it out. The rule reads the body of the proxy class's method: a call on an injected HTTP or gRPC client, which is how eShopCustom's proxies send their requests.

## verified-http

eShopCustom's catalog proxy reads one item with `GetFromJsonAsync` on its `HttpClient`.

```json case
{ "phase": "later", "kinds": ["proxy.request"], "traces": ["ESC-FE-59"], "requirements": ["R2", "R8", "R28"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "kinds": [
    { "kind": "proxy.request", "label": "proxy request", "valueType": "presence", "comparer": "presence", "stated": "always", "existence": true }
  ],
  "vocabulary": {
    "User Interface": {
      "entries": [
        {
          "kind": "proxy.request",
          "for": { "element": "Operation", "where": "parent.specialization == 'Service Proxy'" },
          "value": "if(hasStereotype('Backend Operation'), 'present', 'absent')",
          "bindings": { "Proxy": "parent.name", "Operation": "name" },
          "own": ["Proxy", "Operation"],
          "source": "the Backend Operation stereotype on the Service Proxy's Operation"
        }
      ]
    }
  },
  "rules": {
    "User Interface": {
      "files": ["src/WebApp/Services/**/*.cs", "src/WebAppComponents/Services/**/*.cs"],
      "facts": {
        "proxy.request": {
          "find": "type:{Proxy} > method:{Operation}",
          "read": [{ "match": "\\b_?\\w*[Cc]lient\\.\\w+Async\\b", "part": "body", "value": "present" }],
          "default": "absent",
          "unstated": "no HTTP or gRPC client call in {Proxy}.{Operation}"
        }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webappcomponents", "name": "WebAppComponents",
        "elements": [
          { "id": "catalog-service", "type": "Service Proxy", "name": "CatalogService",
            "children": [
              { "id": "catalog-service-get-catalog-item", "type": "Operation", "name": "GetCatalogItem",
                "stereotypes": { "Backend Operation": {} },
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

public class CatalogService(HttpClient httpClient) : ICatalogService
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
    { "kind": "proxy.request", "element": "catalog-service-get-catalog-item", "outcome": "verified",
      "expected": "present", "actual": "present", "at": "src/WebAppComponents/Services/CatalogService.cs:13" }
  ]
}
```

## verified-grpc

eShopCustom's basket proxy reads the basket through the generated gRPC client. The rpc call is the request.

```json case
{ "phase": "later", "base": "verified-http", "without": ["src/WebAppComponents/Services/CatalogService.cs"], "kinds": ["proxy.request"], "traces": ["ESC-FE-59"], "requirements": ["R2", "R8"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp",
        "elements": [
          { "id": "basket-service", "type": "Service Proxy", "name": "BasketService",
            "children": [
              { "id": "basket-service-get-basket", "type": "Operation", "name": "GetBasketAsync",
                "stereotypes": { "Backend Operation": {} } }
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

```csharp file src/WebApp/Services/BasketService.cs
using GrpcBasketClient = eShop.Basket.API.Grpc.Basket.BasketClient;

namespace eShop.WebApp.Services;

public class BasketService(GrpcBasketClient basketClient)
{
    public async Task<IReadOnlyCollection<BasketQuantity>> GetBasketAsync()
    {
        var result = await basketClient.GetBasketAsync(new ());
        return MapToBasket(result);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "proxy.request", "element": "basket-service-get-basket", "outcome": "verified",
      "at": "src/WebApp/Services/BasketService.cs:9" }
  ]
}
```

## contradicted-no-request

eShopCustom's `proxy.request`: the catalog proxy's `GetCatalogItem` has been stubbed to return nothing, so it sends no request. The method is found and its body has no client call, so its silence says `absent`.

```json case
{ "phase": "later", "base": "verified-http", "kinds": ["proxy.request"], "cases": ["engine/silence"], "traces": ["ESC-FE-59"], "requirements": ["R3", "R8"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/WebAppComponents/Services/CatalogService.cs
using System.Net.Http.Json;
using eShop.WebAppComponents.Catalog;

namespace eShop.WebAppComponents.Services;

public class CatalogService(HttpClient httpClient) : ICatalogService
{
    private readonly string remoteServiceBaseUrl = "api/catalog/";

    public Task<CatalogItem?> GetCatalogItem(int id)
    {
        return Task.FromResult<CatalogItem?>(null);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "proxy.request", "element": "catalog-service-get-catalog-item", "outcome": "contradicted",
      "expected": "present", "actual": "absent", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/proxy.request", "severity": "error", "subject": "catalog-service-get-catalog-item", "messageContains": "GetCatalogItem" }
  ]
}
```

## contradicted-no-backend-operation

eShopCustom's `proxy.mapping`: the catalog proxy's `GetCatalogItem` has no Backend Operation stereotype, so the model states it sends no request, and the code sends one. The claim is contradicted, and the finding names the request the code sends, so the fix, adding the Backend Operation, starts from it.

```json case
{ "phase": "later", "base": "verified-http", "kinds": ["proxy.request"], "traces": ["ESC-FE-56"], "requirements": ["R2", "R11", "R14"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webappcomponents", "name": "WebAppComponents",
        "elements": [
          { "id": "catalog-service", "type": "Service Proxy", "name": "CatalogService",
            "children": [
              { "id": "catalog-service-get-catalog-item", "type": "Operation", "name": "GetCatalogItem",
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

```json expect
{
  "claims": [
    { "kind": "proxy.request", "element": "catalog-service-get-catalog-item", "outcome": "contradicted",
      "expected": "absent", "actual": "present", "at": "src/WebAppComponents/Services/CatalogService.cs:13" }
  ],
  "findings": [
    { "rule": "claim/proxy.request", "severity": "error", "subject": "catalog-service-get-catalog-item",
      "messageContains": "src/WebAppComponents/Services/CatalogService.cs:13" }
  ]
}
```
