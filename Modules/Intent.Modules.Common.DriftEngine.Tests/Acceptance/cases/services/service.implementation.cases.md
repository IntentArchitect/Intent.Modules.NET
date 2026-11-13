# service.implementation

An application service's implementation, against eShopCustom's module stereotype `Application Service`: the Service describes an interface `I<Name>`, and its `Implementation` property names the class that implements it (the Service's own name when it is empty). The stereotype is eShopCustom's own, so the house pack brings the vocabulary (a claim on each Service with the stereotype, binding the class it names) and the recipe: that class's declaration implements `I<Name>`. `service.implementation` is an existence kind: a named class missing from the code is contradicted (R8), and one that is there but doesn't implement the interface states `absent`, from silence. Other classes implementing the interface are unmodelled code, never drift by themselves (principle 1). The kind arrives `later`.

eShopCustom also lets an Application Service with no interface describe its class directly; that is a different check, outside this kind.

The fixtures adapt eShopCustom's `Basket.API/Repositories` and `Ordering.API/Application/Queries`.

## implementation-verified

`BasketRepository` is an Application Service whose Implementation is `RedisBasketRepository`, and that class implements `IBasketRepository`. Traces ESC-SVC-65.

```json case
{ "phase": "later", "kinds": ["service.implementation"], "traces": ["ESC-SVC-65"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "basket-repository", "type": "Service", "name": "BasketRepository",
            "stereotypes": { "Application Service": { "Implementation": "RedisBasketRepository" } },
            "children": [
              { "id": "basket-repository-delete-basket", "type": "Operation", "name": "DeleteBasketAsync", "typeRef": { "type": "bool" },
                "children": [
                  { "id": "basket-repository-delete-basket-id", "type": "Parameter", "name": "id", "typeRef": { "type": "string" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Basket", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "vocabulary": {
    "Services": {
      "entries": [
        {
          "kind": "service.implementation",
          "for": { "element": "Service", "where": "hasStereotype('Application Service')" },
          "value": "'present'",
          "bindings": {
            "Service": "name",
            "Implementation": "stereotype('Application Service').Implementation ?? name"
          },
          "own": ["Implementation"],
          "source": "the Application Service stereotype's Implementation"
        }
      ]
    }
  },
  "rules": {
    "Services": {
      "facts": {
        "service.implementation": {
          "find": "type:{Implementation}",
          "read": [{ "match": ":\\s*([\\w.<>]+\\s*,\\s*)*I{Service}\\b", "value": "present", "part": "declaration" }],
          "default": "absent",
          "note": "The named class implements the interface the Service describes"
        }
      }
    }
  }
}
```

```csharp file src/Basket.API/Repositories/IBasketRepository.cs
namespace eShop.Basket.API.Repositories;

public interface IBasketRepository
{
    Task<bool> DeleteBasketAsync(string id);
}
```

```csharp file src/Basket.API/Repositories/RedisBasketRepository.cs
namespace eShop.Basket.API.Repositories;

public class RedisBasketRepository(ILogger<RedisBasketRepository> logger, IConnectionMultiplexer redis) : IBasketRepository
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task<bool> DeleteBasketAsync(string id)
    {
        return await _database.KeyDeleteAsync(id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "service.implementation", "element": "basket-repository", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Basket.API/Repositories/RedisBasketRepository.cs:3" }
  ]
}
```

## implementation-by-name-verified

`OrderQueries` is an Application Service with no Implementation, so its class is `OrderQueries`, which implements `IOrderQueries`. Traces ESC-SVC-65 (the Service's own name).

```json case
{ "phase": "later", "base": "implementation-verified", "without": ["src/Basket.API/Repositories/IBasketRepository.cs", "src/Basket.API/Repositories/RedisBasketRepository.cs"], "kinds": ["service.implementation"], "traces": ["ESC-SVC-65"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "order-queries", "type": "Service", "name": "OrderQueries",
            "stereotypes": { "Application Service": {} },
            "children": [
              { "id": "order-queries-get-card-types", "type": "Operation", "name": "GetCardTypesAsync" }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.API/Application/Queries/IOrderQueries.cs
namespace eShop.Ordering.API.Application.Queries;

public interface IOrderQueries
{
    Task<IEnumerable<CardType>> GetCardTypesAsync();
}
```

```csharp file src/Ordering.API/Application/Queries/OrderQueries.cs
namespace eShop.Ordering.API.Application.Queries;

public class OrderQueries(OrderingContext context) : IOrderQueries
{
    public async Task<IEnumerable<CardType>> GetCardTypesAsync() =>
        await context.CardTypes.Select(c => new CardType { Id = c.Id, Name = c.Name }).ToListAsync();
}
```

```json expect
{
  "claims": [
    { "kind": "service.implementation", "element": "order-queries", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/Queries/OrderQueries.cs:3" }
  ]
}
```

## implementation-missing-contradicted

The Implementation still names `RedisBasketRepository`, but the code's only implementation is `CosmosBasketRepository`. The class the model names isn't in the code. Traces ESC-SVC-65.

```json case
{ "phase": "later", "base": "implementation-verified", "without": ["src/Basket.API/Repositories/RedisBasketRepository.cs"], "kinds": ["service.implementation"], "traces": ["ESC-SVC-65"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Basket.API/Repositories/CosmosBasketRepository.cs
namespace eShop.Basket.API.Repositories;

public class CosmosBasketRepository(Container container) : IBasketRepository
{
    public async Task<bool> DeleteBasketAsync(string id)
    {
        var response = await container.DeleteItemAsync<CustomerBasket>(id, new PartitionKey(id));
        return response.StatusCode == HttpStatusCode.NoContent;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "service.implementation", "element": "basket-repository", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/service.implementation", "severity": "error", "subject": "basket-repository", "messageContains": "isn't in the code" }
  ]
}
```

## not-implementing-contradicted

`RedisBasketRepository` is there, but it no longer declares `IBasketRepository`. The class is found and states no interface, so the claim reads `absent`, from silence. Traces ESC-SVC-65.

```json case
{ "phase": "later", "base": "implementation-verified", "kinds": ["service.implementation"], "traces": ["ESC-SVC-65"], "requirements": ["R1", "R3"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Basket.API/Repositories/RedisBasketRepository.cs
namespace eShop.Basket.API.Repositories;

public class RedisBasketRepository(ILogger<RedisBasketRepository> logger, IConnectionMultiplexer redis)
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task<bool> DeleteBasketAsync(string id)
    {
        return await _database.KeyDeleteAsync(id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "service.implementation", "element": "basket-repository", "outcome": "contradicted", "expected": "present", "actual": "absent", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/service.implementation", "severity": "error", "subject": "basket-repository" }
  ]
}
```

## several-implementations

A second class, `InMemoryBasketRepository`, also implements `IBasketRepository`. eShopCustom warned that only the interface was compared; here the claim is about the class the model names, which implements the interface, so it is verified, and the other implementation is unmodelled code that nothing reports. Traces ESC-SVC-65 (several implementations).

```json case
{ "phase": "later", "base": "implementation-verified", "kinds": ["service.implementation"], "traces": ["ESC-SVC-65"], "requirements": ["R2", "R4"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Basket.API/Repositories/InMemoryBasketRepository.cs
namespace eShop.Basket.API.Repositories;

public class InMemoryBasketRepository : IBasketRepository
{
    private readonly ConcurrentDictionary<string, CustomerBasket> _baskets = new();

    public Task<bool> DeleteBasketAsync(string id) => Task.FromResult(_baskets.TryRemove(id, out _));
}
```

```json expect
{
  "claims": [
    { "kind": "service.implementation", "element": "basket-repository", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Basket.API/Repositories/RedisBasketRepository.cs:3" }
  ],
  "findings": [
    { "rule": "claim/service.implementation", "count": 0 }
  ]
}
```
