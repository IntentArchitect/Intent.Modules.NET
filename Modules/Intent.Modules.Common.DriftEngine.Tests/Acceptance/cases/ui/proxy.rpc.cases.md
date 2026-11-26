# proxy.rpc

The rpc a front end's proxy operation calls through a generated gRPC client, against the backend Operation it calls when that is an Operation of a gRPC Service: the client method is the rpc's name, or its name with `Async`. The kind is `stated: always` and compares exactly; the backend Operation's name is the model's value.

The kind is `later`. eShopCustom's proxies name their backend Operation with its own Backend Operation stereotype, and its gRPC services carry its own gRPC Service stereotype, so these cases' house pack gives the vocabulary; reading the client call is the stock rule's.

## eshop-grpc-rpc

eShopCustom's basket proxy reads the basket with `basketClient.GetBasketAsync`, the rpc `GetBasket` of the Basket gRPC service its Backend Operation names.

```json case
{ "phase": "later", "kinds": ["proxy.rpc"], "traces": ["ESC-FE-64"], "requirements": ["R2", "R28"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "kinds": [
    { "kind": "proxy.rpc", "label": "RPC method", "valueType": "text", "comparer": "exact", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "User Interface": {
      "entries": [
        {
          "kind": "proxy.rpc",
          "for": { "element": "Operation", "where": "parent.specialization == 'Service Proxy' && stereotype('Backend Operation').Operation.parent.hasStereotype('gRPC Service')" },
          "value": "stereotype('Backend Operation').Operation.name",
          "bindings": { "Proxy": "parent.name", "Operation": "name" },
          "own": ["Proxy", "Operation"],
          "source": "the gRPC Service Operation its Backend Operation stereotype names"
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
        "id": "basket-api", "name": "Basket.API",
        "elements": [
          { "id": "basket-grpc-service", "type": "Service", "name": "BasketService",
            "stereotypes": { "gRPC Service": { "Proto File": "Proto/basket.proto", "Proto Service": "Basket" } },
            "children": [
              { "id": "basket-get-basket", "type": "Operation", "name": "GetBasket" }
            ] }
        ]
      }]
    },
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp",
        "elements": [
          { "id": "basket-service", "type": "Service Proxy", "name": "BasketService",
            "children": [
              { "id": "basket-service-get-basket", "type": "Operation", "name": "GetBasketAsync",
                "stereotypes": { "Backend Operation": { "Operation": { "ref": "basket-get-basket" } } } }
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
    { "kind": "proxy.rpc", "element": "basket-service-get-basket", "outcome": "verified",
      "expected": "GetBasket", "actual": "GetBasket", "at": "src/WebApp/Services/BasketService.cs:9" }
  ]
}
```

## csharp-standard-grpc-call

The standard User Interface designer (2.0.0): a Proxy Operation's Call Service Operation Action targets a backend Operation with no Http Settings, a gRPC service's, so the link claims the rpc the proxy calls, the target's name, and no verb or route. `GetBasketAsync` calls the generated client's `GetBasketAsync`; `DeleteBasketAsync` calls `GetBasketAsync` too, where the model says `DeleteBasket`.

```json case
{ "phase": "later", "kinds": ["proxy.rpc", "proxy.verb", "proxy.route"], "requirements": ["R2", "R14", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webapp", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "basket-proxy", "type": "Proxy", "name": "BasketService", "children": [
            { "id": "basket-proxy-get", "type": "Proxy Operation", "name": "GetBasketAsync" },
            { "id": "basket-proxy-delete", "type": "Proxy Operation", "name": "DeleteBasketAsync" }
          ] }
        ]
      }],
      "associations": [
        { "id": "get-basket-calls-backend", "type": "Call Service Operation Action", "source": { "element": "basket-proxy-get" }, "target": { "element": "basket-get-basket" } },
        { "id": "delete-basket-calls-backend", "type": "Call Service Operation Action", "source": { "element": "basket-proxy-delete" }, "target": { "element": "basket-delete-basket" } }
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
        "id": "basket-api", "name": "Basket.API",
        "elements": [
          { "id": "basket-grpc-service", "type": "Service", "name": "BasketService", "children": [
            { "id": "basket-get-basket", "type": "Operation", "name": "GetBasket" },
            { "id": "basket-delete-basket", "type": "Operation", "name": "DeleteBasket" }
          ] }
        ]
      }]
    }
  }
}
```

```csharp file src/WebApp/Services/BasketService.cs
using GrpcBasketClient = eShop.Basket.API.Grpc.Basket.BasketClient;

namespace eShop.WebApp.Services;

public class BasketService(GrpcBasketClient basketClient)
{
    public async Task GetBasketAsync()
    {
        await basketClient.GetBasketAsync(new ());
    }

    public async Task DeleteBasketAsync()
    {
        await basketClient.GetBasketAsync(new ());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "proxy.rpc", "element": "get-basket-calls-backend", "outcome": "verified", "expected": "GetBasket", "actual": "GetBasket", "at": "src/WebApp/Services/BasketService.cs:9" },
    { "kind": "proxy.rpc", "element": "delete-basket-calls-backend", "outcome": "contradicted", "expected": "DeleteBasket", "actual": "GetBasket", "at": "src/WebApp/Services/BasketService.cs:14" },
    { "kind": "proxy.verb", "element": "get-basket-calls-backend", "absent": true },
    { "kind": "proxy.route", "element": "get-basket-calls-backend", "absent": true }
  ],
  "findings": [
    { "rule": "claim/proxy.rpc", "severity": "error", "subject": "delete-basket-calls-backend" }
  ]
}
```

## eshop-grpc-rpc-other

The proxy calls `UpdateBasketAsync`, another rpc of the same service.

```json case
{ "phase": "later", "base": "eshop-grpc-rpc", "kinds": ["proxy.rpc"], "traces": ["ESC-FE-64"], "requirements": ["R2", "R14"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/WebApp/Services/BasketService.cs
using GrpcBasketClient = eShop.Basket.API.Grpc.Basket.BasketClient;

namespace eShop.WebApp.Services;

public class BasketService(GrpcBasketClient basketClient)
{
    public async Task<IReadOnlyCollection<BasketQuantity>> GetBasketAsync()
    {
        var result = await basketClient.UpdateBasketAsync(new ());
        return MapToBasket(result);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "proxy.rpc", "element": "basket-service-get-basket", "outcome": "contradicted",
      "expected": "GetBasket", "actual": "UpdateBasket", "at": "src/WebApp/Services/BasketService.cs:9" }
  ],
  "findings": [
    { "rule": "claim/proxy.rpc", "severity": "error", "subject": "basket-service-get-basket" }
  ]
}
```

## eshop-http-to-grpc-operation

The transport doesn't match (`proxy.target`): the proxy's Backend Operation is an rpc of a gRPC Service, but the proxy sends an HTTP request through an `HttpClient`, so it calls no rpc.

```json case
{ "phase": "later", "base": "eshop-grpc-rpc", "kinds": ["proxy.rpc"], "traces": ["ESC-FE-58"], "requirements": ["R2", "R8"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/WebApp/Services/BasketService.cs
using System.Net.Http.Json;

namespace eShop.WebApp.Services;

public class BasketService(HttpClient httpClient)
{
    public async Task<IReadOnlyCollection<BasketQuantity>> GetBasketAsync()
    {
        var result = await httpClient.GetFromJsonAsync<CustomerBasketResponse>("api/basket");
        return MapToBasket(result!);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "proxy.rpc", "element": "basket-service-get-basket", "outcome": "contradicted", "expected": "GetBasket" }
  ],
  "findings": [
    { "rule": "claim/proxy.rpc", "severity": "error", "subject": "basket-service-get-basket", "messageContains": "GetBasket" }
  ]
}
```
