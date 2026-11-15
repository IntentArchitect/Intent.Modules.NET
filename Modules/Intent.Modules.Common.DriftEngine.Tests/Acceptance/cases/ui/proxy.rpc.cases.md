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
