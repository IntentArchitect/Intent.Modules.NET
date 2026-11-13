# type.base

A base type the model implies without modelling it (R41): eShop's entities derive the SeedWork `Entity`, its value objects `ValueObject`, its domain events implement MediatR's `INotification`. No stock pack knows those conventions, so the codebase's house pack states them in vocabulary, from the element's type and stereotypes (R28, R30). The value is a set and the comparer is `subset`: every base the model implies must be among the type's supertypes, and the code may have more.

The code's supertypes are read from the type node with the C# reader's declared resolution: a base declared in scope resolves to its qualified name, and a library type such as `INotification` stays as written and is compared by name (code-reader.md, Capabilities). So the house pack names the codebase's own SeedWork types by qualified name: a base that resolves to no in-scope declaration doesn't match it. That is how a missing SeedWork type shows (ESC-DOM-07). A library base is named without its generic arguments: `IRequest` matches `IRequest<bool>`.

## verified

An entity, a value object and a domain event, each with the base its element type implies. The SeedWork types are in scope, so `Entity` and `ValueObject` resolve.

```json case
{ "phase": 2, "kinds": ["type.base"], "traces": ["ESC-DOM-07", "ESC-DOM-16", "ESC-DOM-27"], "requirements": ["R2", "R28", "R30", "R41", "R53"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "entries": [
        {
          "kind": "type.base",
          "for": { "element": "Class", "where": "!hasStereotype('Plain Class') && count(where(associations('Generalization'), it.isTarget)) == 0" },
          "value": "'Shop.Domain.SeedWork.Entity'",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "a Class with no Generalization and no Plain Class stereotype",
          "fixHint": "Derive the class from the SeedWork Entity, or mark it Plain Class."
        },
        {
          "kind": "type.base",
          "for": { "element": "Value Object" },
          "value": "'Shop.Domain.SeedWork.ValueObject'",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the element being a Value Object",
          "fixHint": "Derive the value object from the SeedWork ValueObject."
        },
        {
          "kind": "type.base",
          "for": { "element": "Domain Event" },
          "value": "'INotification'",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the element being a Domain Event",
          "fixHint": "Implement MediatR's INotification."
        }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "buyer", "type": "Class", "name": "Buyer" },
          { "id": "address", "type": "Value Object", "name": "Address" },
          { "id": "order-cancelled", "type": "Domain Event", "name": "OrderCancelledDomainEvent" }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/SeedWork/Entity.cs
namespace Shop.Domain.SeedWork;

public abstract class Entity
{
    public int Id { get; protected set; }
}
```

```csharp file src/Domain/SeedWork/ValueObject.cs
namespace Shop.Domain.SeedWork;

public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();
}
```

```csharp file src/Domain/Buyers/Buyer.cs
using Shop.Domain.SeedWork;

namespace Shop.Domain.Buyers;

public class Buyer : Entity
{
    public string Name { get; private set; } = "";
}
```

```csharp file src/Domain/Orders/Address.cs
using Shop.Domain.SeedWork;

namespace Shop.Domain.Orders;

public class Address : ValueObject
{
    public string Street { get; private set; } = "";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
    }
}
```

```csharp file src/Domain/Orders/Events/OrderCancelledDomainEvent.cs
using MediatR;

namespace Shop.Domain.Orders.Events;

public class OrderCancelledDomainEvent : INotification
{
}
```

```json expect
{
  "claims": [
    { "kind": "type.base", "element": "buyer", "outcome": "verified", "expected": "Shop.Domain.SeedWork.Entity",
      "at": "src/Domain/Buyers/Buyer.cs:5" },
    { "kind": "type.base", "element": "address", "outcome": "verified", "expected": "Shop.Domain.SeedWork.ValueObject",
      "at": "src/Domain/Orders/Address.cs:5" },
    { "kind": "type.base", "element": "order-cancelled", "outcome": "verified", "expected": "INotification", "actual": "INotification",
      "at": "src/Domain/Orders/Events/OrderCancelledDomainEvent.cs:5" }
  ]
}
```

## value-object-derives-entity

`Address` is a Value Object in the model and derives `Entity` in the code. The base its element type implies is missing, so the claim is contradicted, naming both (R11, R14).

```json case
{ "phase": 2, "base": "verified", "kinds": ["type.base"], "traces": ["ESC-DOM-16"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```csharp file src/Domain/Orders/Address.cs
using Shop.Domain.SeedWork;

namespace Shop.Domain.Orders;

public class Address : Entity
{
    public string Street { get; private set; } = "";
}
```

```json expect
{
  "claims": [
    { "kind": "type.base", "element": "address", "outcome": "contradicted", "expected": "Shop.Domain.SeedWork.ValueObject", "actual": "Shop.Domain.SeedWork.Entity",
      "at": "src/Domain/Orders/Address.cs:5" }
  ],
  "findings": [
    { "rule": "claim/type.base", "severity": "error", "subject": "address", "messageContains": "ValueObject" }
  ]
}
```

## seedwork-missing

The model relies on the SeedWork `ValueObject`, and the codebase no longer declares it. `Address` still names `ValueObject` as its base, but the name resolves to nothing in scope, so it stays as written and isn't the SeedWork type the model implies: contradicted. eShopCustom reported this as a missing SeedWork type.

```json case
{ "phase": 2, "base": "verified", "without": ["src/Domain/SeedWork/ValueObject.cs"], "kinds": ["type.base"], "traces": ["ESC-DOM-07"], "requirements": ["R1", "R33", "R35"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "type.base", "element": "address", "outcome": "contradicted", "expected": "Shop.Domain.SeedWork.ValueObject", "actual": "ValueObject",
      "at": "src/Domain/Orders/Address.cs:5" },
    { "kind": "type.base", "element": "buyer", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/type.base", "severity": "error", "subject": "address" }
  ]
}
```

## domain-event-not-notification

The domain event no longer implements `INotification`.

```json case
{ "phase": 2, "base": "verified", "kinds": ["type.base"], "traces": ["ESC-DOM-27"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```csharp file src/Domain/Orders/Events/OrderCancelledDomainEvent.cs
namespace Shop.Domain.Orders.Events;

public class OrderCancelledDomainEvent
{
}
```

```json expect
{
  "claims": [
    { "kind": "type.base", "element": "order-cancelled", "outcome": "contradicted", "expected": "INotification",
      "at": "src/Domain/Orders/Events/OrderCancelledDomainEvent.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.base", "severity": "error", "subject": "order-cancelled", "messageContains": "INotification" }
  ]
}
```

## blocked

`Address` isn't in the code. Its supertypes have nothing to be read from, so `type.base` is can't verify, `blocked` behind the contradicted `type.exists`, and only the missing type is reported (R5). The other two elements are unaffected.

```json case
{ "phase": 2, "base": "verified", "without": ["src/Domain/Orders/Address.cs"], "kinds": ["type.exists", "type.base"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "address", "outcome": "contradicted" },
    { "kind": "type.base", "element": "address", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" },
    { "kind": "type.base", "element": "buyer", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "address", "messageContains": "isn't in the code" },
    { "rule": "claim/type.base", "count": 0, "note": "Counted in the missing type's finding" }
  ]
}
```

## services-verified

The Services designer's element types and eShopCustom's Services stereotypes imply bases too: a Command implements MediatR's `IRequest` (or `IRequest<T>`), a Message derives `IntegrationEvent`, a Hosted Service derives `BackgroundService`, and a gRPC Service derives the base the `.proto` generates for its Proto Service, `<Proto Service>Base`.

```json case
{ "phase": 2, "kinds": ["type.base"], "traces": ["ESC-SVC-50", "ESC-SVC-58", "ESC-SVC-70", "ESC-SVC-76"], "requirements": ["R2", "R28", "R30", "R41"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "vocabulary": {
    "Services": {
      "entries": [
        {
          "kind": "type.base",
          "for": { "element": "Command" },
          "value": "'IRequest'",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the element being a Command",
          "fixHint": "Implement IRequest, or IRequest<T> for a Command that returns a value."
        },
        {
          "kind": "type.base",
          "for": { "element": "Message" },
          "value": "'IntegrationEvent'",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the element being a Message"
        },
        {
          "kind": "type.base",
          "for": { "element": "Service", "where": "hasStereotype('Hosted Service')" },
          "value": "'BackgroundService'",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the Hosted Service stereotype"
        },
        {
          "kind": "type.base",
          "for": { "element": "Service", "where": "hasStereotype('gRPC Service')" },
          "value": "stereotype('gRPC Service')['Proto Service'] + 'Base'",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the gRPC Service stereotype's Proto Service"
        }
      ]
    }
  },
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
        "id": "services", "name": "eShop.Services",
        "elements": [
          { "id": "cancel-order", "type": "Command", "name": "CancelOrderCommand", "typeRef": { "type": "bool" } },
          { "id": "order-started", "type": "Message", "name": "OrderStartedIntegrationEvent" },
          { "id": "grace-period", "type": "Service", "name": "GracePeriodManagerService",
            "stereotypes": { "Hosted Service": {} } },
          { "id": "basket-grpc", "type": "Service", "name": "BasketService",
            "stereotypes": { "gRPC Service": { "Proto File": "Basket.API/Proto/basket.proto", "Proto Service": "Basket" } } }
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

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommand.cs
namespace eShop.Ordering.API.Application.Commands;

public record CancelOrderCommand(int OrderNumber) : IRequest<bool>;
```

```csharp file src/Ordering.API/Application/IntegrationEvents/Events/OrderStartedIntegrationEvent.cs
namespace eShop.Ordering.API.Application.IntegrationEvents.Events;

public record OrderStartedIntegrationEvent(string UserId) : IntegrationEvent;
```

```csharp file src/OrderProcessor/Services/GracePeriodManagerService.cs
namespace eShop.OrderProcessor.Services;

public class GracePeriodManagerService(ILogger<GracePeriodManagerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("GracePeriodManagerService is starting.");
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
    }
}
```

```csharp file src/Basket.API/Grpc/BasketService.cs
using eShop.Basket.API.Repositories;

namespace eShop.Basket.API.Grpc;

public class BasketService(IBasketRepository repository) : Basket.BasketBase
{
    private readonly IBasketRepository _repository = repository;
}
```

```json expect
{
  "claims": [
    { "kind": "type.base", "element": "cancel-order", "outcome": "verified", "expected": "IRequest",
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommand.cs:3" },
    { "kind": "type.base", "element": "order-started", "outcome": "verified", "expected": "IntegrationEvent",
      "at": "src/Ordering.API/Application/IntegrationEvents/Events/OrderStartedIntegrationEvent.cs:3" },
    { "kind": "type.base", "element": "grace-period", "outcome": "verified", "expected": "BackgroundService",
      "at": "src/OrderProcessor/Services/GracePeriodManagerService.cs:3" },
    { "kind": "type.base", "element": "basket-grpc", "outcome": "verified", "expected": "BasketBase",
      "at": "src/Basket.API/Grpc/BasketService.cs:5" }
  ]
}
```

## grpc-base-missing

The gRPC class lost its generated base. Nothing in the code serves the Proto Service, so the claim is contradicted.

```json case
{ "phase": 2, "base": "services-verified", "kinds": ["type.base"], "traces": ["ESC-SVC-76"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Basket.API/Grpc/BasketService.cs
using eShop.Basket.API.Repositories;

namespace eShop.Basket.API.Grpc;

public class BasketService(IBasketRepository repository)
{
    private readonly IBasketRepository _repository = repository;
}
```

```json expect
{
  "claims": [
    { "kind": "type.base", "element": "basket-grpc", "outcome": "contradicted", "expected": "BasketBase",
      "at": "src/Basket.API/Grpc/BasketService.cs:5" }
  ],
  "findings": [
    { "rule": "claim/type.base", "severity": "error", "subject": "basket-grpc", "messageContains": "BasketBase" }
  ]
}
```

## state-store-verified

A phase 2 demonstration: eShopCustom's Front End module puts a `State Store` stereotype on a User Interface Model Definition, naming the interface the store implements. No stock pack knows that stereotype, and intent.ui arrives only in phase 6, so the house pack gives the Model Definition its type and its required base with no engine change (R28, R53).

```json case
{ "phase": 2, "kinds": ["type.exists", "type.base"], "traces": ["ESC-FE-48"], "requirements": ["R2", "R28", "R30", "R51", "R53"], "packs": ["intent.core", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "vocabulary": {
    "User Interface": {
      "entries": [
        {
          "kind": "type.exists",
          "for": { "element": "Model Definition" },
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "source": "the Model Definition"
        },
        {
          "kind": "type.base",
          "for": { "element": "Model Definition", "where": "hasStereotype('State Store')" },
          "value": "stereotype('State Store').Interface",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the State Store stereotype's Interface",
          "fixHint": "Implement the interface, or change the State Store stereotype's Interface."
        }
      ]
    }
  },
  "conventions": {
    "User Interface": {
      "types": {
        "Model Definition": { "typeKinds": ["record", "class", "struct"] }
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
        "id": "web-app", "name": "WebApp",
        "elements": [
          { "id": "basket-state", "type": "Model Definition", "name": "BasketState",
            "stereotypes": { "State Store": { "Interface": "IBasketState" } } }
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

```csharp file src/WebApp/Services/IBasketState.cs
namespace eShop.WebApp.Services;

public interface IBasketState
{
    Task<IReadOnlyCollection<BasketItem>> GetBasketItemsAsync();
}
```

```csharp file src/WebApp/Services/BasketState.cs
namespace eShop.WebApp.Services;

public class BasketState(BasketService basketService) : IBasketState
{
    public Task<IReadOnlyCollection<BasketItem>> GetBasketItemsAsync()
        => basketService.GetBasketItemsAsync();
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "basket-state", "outcome": "verified", "at": "src/WebApp/Services/BasketState.cs:3" },
    { "kind": "type.base", "element": "basket-state", "outcome": "verified", "expected": "IBasketState",
      "at": "src/WebApp/Services/BasketState.cs:3" }
  ]
}
```

## state-store-contradicted

The store no longer implements `IBasketState`; the interface is still in the code, but nothing the model names implements it.

```json case
{ "phase": 2, "base": "state-store-verified", "kinds": ["type.base"], "traces": ["ESC-FE-48"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "house.eshop"], "language": "csharp" }
```

```csharp file src/WebApp/Services/BasketState.cs
namespace eShop.WebApp.Services;

public class BasketState(BasketService basketService)
{
    public Task<IReadOnlyCollection<BasketItem>> GetBasketItemsAsync()
        => basketService.GetBasketItemsAsync();
}
```

```json expect
{
  "claims": [
    { "kind": "type.base", "element": "basket-state", "outcome": "contradicted", "expected": "IBasketState",
      "at": "src/WebApp/Services/BasketState.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.base", "severity": "error", "subject": "basket-state", "messageContains": "IBasketState" }
  ]
}
```
