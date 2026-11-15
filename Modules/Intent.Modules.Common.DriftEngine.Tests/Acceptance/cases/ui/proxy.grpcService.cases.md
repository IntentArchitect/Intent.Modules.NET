# proxy.grpcService

The `.proto` service whose generated client a front end's proxy calls through (`Basket.BasketClient` is the client of `service Basket`), against the Proto Service of the gRPC Service its backend Operation belongs to. The kind is `stated: always` and compares exactly.

The kind is `later`. eShopCustom's proxies name their backend Operation with its own Backend Operation stereotype, and its gRPC services carry its own gRPC Service stereotype, so these cases' house pack gives the vocabulary; reading the client's type is the stock rule's.

## eshop-grpc-service

eShopCustom's basket proxy holds a `Basket.BasketClient`, the client of the `Basket` service the backend's gRPC Service implements.

```json case
{ "phase": "later", "kinds": ["proxy.grpcService"], "traces": ["ESC-FE-65"], "requirements": ["R2", "R28"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "kinds": [
    { "kind": "proxy.grpcService", "label": "gRPC service", "valueType": "text", "comparer": "exact", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "User Interface": {
      "entries": [
        {
          "kind": "proxy.grpcService",
          "for": { "element": "Operation", "where": "parent.specialization == 'Service Proxy' && stereotype('Backend Operation').Operation.parent.hasStereotype('gRPC Service')" },
          "value": "stereotype('Backend Operation').Operation.parent.stereotype('gRPC Service')['Proto Service']",
          "bindings": { "Proxy": "parent.name", "Operation": "name" },
          "own": ["Proxy", "Operation"],
          "source": "the Proto Service of the gRPC Service its Backend Operation stereotype names"
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
    { "kind": "proxy.grpcService", "element": "basket-service-get-basket", "outcome": "verified",
      "expected": "Basket", "actual": "Basket", "at": "src/WebApp/Services/BasketService.cs:5" }
  ]
}
```

## eshop-grpc-service-other-client

The proxy holds the ordering service's client, `Ordering.OrderingClient`, so its calls go to `service Ordering`, not the basket's.

```json case
{ "phase": "later", "base": "eshop-grpc-service", "kinds": ["proxy.grpcService"], "traces": ["ESC-FE-65"], "requirements": ["R2", "R14"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/WebApp/Services/BasketService.cs
using GrpcOrderingClient = eShop.Ordering.API.Grpc.Ordering.OrderingClient;

namespace eShop.WebApp.Services;

public class BasketService(GrpcOrderingClient basketClient)
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
    { "kind": "proxy.grpcService", "element": "basket-service-get-basket", "outcome": "contradicted",
      "expected": "Basket", "actual": "Ordering", "at": "src/WebApp/Services/BasketService.cs:5" }
  ],
  "findings": [
    { "rule": "claim/proxy.grpcService", "severity": "error", "subject": "basket-service-get-basket" }
  ]
}
```
