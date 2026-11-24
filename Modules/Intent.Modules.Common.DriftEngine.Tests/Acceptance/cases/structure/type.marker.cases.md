# type.marker

A named marker, both ways: modelled means the code has it, not modelled means it doesn't (R41). The marker's name is the claim's discriminator (the `key` below), so one element can carry several. eShop marks an aggregate root by implementing `IAggregateRoot`, and the model says which classes are aggregate roots through its associations: a class no composition owns. A Plain Class is neither an entity nor an aggregate root. The house pack states those markers in vocabulary and reads them with a recipe (R28, R46); the recipe's `default` makes an unmarked type state "no marker", so a marker the model rules out is verified from silence (R3).

## aggregate-root-verified

`Order` owns its `OrderItem`s by composition. No composition owns `Order`, so it is an aggregate root and implements `IAggregateRoot`; `OrderItem` is owned and doesn't, which is verified from silence. The vocabulary reads ownership from the association ends leading away from the class: an end that is its association's source, with multiplicity `1`, is a composite owner (`.isSource`).

```json case
{ "phase": 2, "kinds": ["type.marker"], "traces": ["ESC-DOM-20"], "requirements": ["R2", "R3", "R28", "R30"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
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
          "kind": "type.marker",
          "for": { "element": "Class", "where": "!hasStereotype('Plain Class')" },
          "value": "count(where(associations('Association'), it.isSource && it.multiplicity == '1')) == 0",
          "discriminator": "'IAggregateRoot'",
          "bindings": { "Type": "name", "Marker": "'IAggregateRoot'" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the Class's associations: no composition owns an aggregate root",
          "fixHint": "Implement IAggregateRoot on exactly the classes no composition owns."
        },
        {
          "kind": "type.marker",
          "for": { "element": "Class", "where": "hasStereotype('Plain Class')" },
          "value": "false",
          "discriminator": "'Entity'",
          "bindings": { "Type": "name", "Marker": "'Entity'" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the Plain Class stereotype",
          "fixHint": "A Plain Class doesn't derive Entity: remove the base, or remove the Plain Class stereotype."
        },
        {
          "kind": "type.marker",
          "for": { "element": "Class", "where": "hasStereotype('Plain Class')" },
          "value": "false",
          "discriminator": "'IAggregateRoot'",
          "bindings": { "Type": "name", "Marker": "'IAggregateRoot'" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the Plain Class stereotype",
          "fixHint": "A Plain Class isn't an aggregate root: remove IAggregateRoot, or remove the Plain Class stereotype."
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
          { "id": "order", "type": "Class", "name": "Order" },
          { "id": "order-item", "type": "Class", "name": "OrderItem" }
        ]
      }],
      "associations": [
        { "id": "order-items", "type": "Association",
          "source": { "element": "order", "multiplicity": "1", "navigable": false },
          "target": { "element": "order-item", "name": "OrderItems", "multiplicity": "*", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Orders/Order.cs
using Shop.Domain.SeedWork;

namespace Shop.Domain.Orders;

public class Order : Entity, IAggregateRoot
{
    private readonly List<OrderItem> _orderItems = new();

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
}
```

```csharp file src/Domain/Orders/OrderItem.cs
using Shop.Domain.SeedWork;

namespace Shop.Domain.Orders;

public class OrderItem : Entity
{
    public int Units { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.marker", "element": "order", "key": "IAggregateRoot", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Domain/Orders/Order.cs:5" },
    { "kind": "type.marker", "element": "order-item", "key": "IAggregateRoot", "outcome": "verified", "expected": "false", "actual": "false",
      "fromSilence": true }
  ]
}
```

## aggregate-root-unmarked

`Order` is still an aggregate root in the model, and the code dropped `IAggregateRoot`. The class's silence states "no marker", which contradicts the model.

```json case
{ "phase": 2, "base": "aggregate-root-verified", "kinds": ["type.marker"], "traces": ["ESC-DOM-20"], "requirements": ["R1", "R3", "R11", "R14"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```csharp file src/Domain/Orders/Order.cs
using Shop.Domain.SeedWork;

namespace Shop.Domain.Orders;

public class Order : Entity
{
    private readonly List<OrderItem> _orderItems = new();

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
}
```

```json expect
{
  "claims": [
    { "kind": "type.marker", "element": "order", "key": "IAggregateRoot", "outcome": "contradicted", "expected": "true", "actual": "false",
      "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/type.marker", "severity": "error", "subject": "order", "messageContains": "IAggregateRoot" }
  ]
}
```

## owned-entity-marked

The other way: `OrderItem` is owned by `Order`'s composition, and the code marks it an aggregate root.

```json case
{ "phase": 2, "base": "aggregate-root-verified", "kinds": ["type.marker"], "traces": ["ESC-DOM-20"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```csharp file src/Domain/Orders/OrderItem.cs
using Shop.Domain.SeedWork;

namespace Shop.Domain.Orders;

public class OrderItem : Entity, IAggregateRoot
{
    public int Units { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.marker", "element": "order-item", "key": "IAggregateRoot", "outcome": "contradicted", "expected": "false", "actual": "true",
      "at": "src/Domain/Orders/OrderItem.cs:5" }
  ],
  "findings": [
    { "rule": "claim/type.marker", "severity": "error", "subject": "order-item" }
  ]
}
```

## blocked

A second `Order` class in a legacy folder of the domain project makes `Order`'s `type.exists` ambiguous: both namespaces name the package `Shop.Domain` equally. With no single type to read, its marker claim is can't verify, `blocked` behind it (R5).

```json case
{ "phase": 2, "base": "aggregate-root-verified", "kinds": ["type.exists", "type.marker"], "cases": ["engine/blocked"], "requirements": ["R5", "R43"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```csharp file src/Domain/Legacy/Order.cs
namespace Shop.Domain.Legacy;

public class Order
{
    public decimal Total { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "cantVerify", "reason": "ambiguous" },
    { "kind": "type.marker", "element": "order", "key": "IAggregateRoot", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "src/Domain/Legacy/Order.cs:3" },
    { "rule": "claim/type.marker", "subject": "order", "count": 0, "note": "A blocked claim has no finding of its own" }
  ]
}
```

## plain-class-verified

`CardType` is a Plain Class: a lookup type that is neither an entity nor an aggregate root. The code's sealed class has no base, so both markers the model rules out are verified from silence. The aggregate root entry doesn't apply to a Plain Class, so `IAggregateRoot` is claimed once.

```json case
{ "phase": 2, "base": "aggregate-root-verified", "without": ["src/Domain/Orders/Order.cs", "src/Domain/Orders/OrderItem.cs"], "kinds": ["type.marker"], "traces": ["ESC-DOM-17"], "requirements": ["R2", "R3", "R30"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "card-type", "type": "Class", "name": "CardType",
            "stereotypes": { "Plain Class": {} } }
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

```csharp file src/Domain/Buyers/CardType.cs
namespace Shop.Domain.Buyers;

public sealed class CardType
{
    public int Id { get; init; }
    public required string Name { get; init; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.marker", "element": "card-type", "key": "Entity", "outcome": "verified", "expected": "false", "actual": "false", "fromSilence": true },
    { "kind": "type.marker", "element": "card-type", "key": "IAggregateRoot", "outcome": "verified", "expected": "false", "actual": "false", "fromSilence": true }
  ]
}
```

## plain-class-derives-entity

The Plain Class derives `Entity` in the code: its `Entity` marker is contradicted, and its `IAggregateRoot` marker is still verified.

```json case
{ "phase": 2, "base": "plain-class-verified", "kinds": ["type.marker"], "traces": ["ESC-DOM-17"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```csharp file src/Domain/Buyers/CardType.cs
using Shop.Domain.SeedWork;

namespace Shop.Domain.Buyers;

public class CardType : Entity
{
    public required string Name { get; init; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.marker", "element": "card-type", "key": "Entity", "outcome": "contradicted", "expected": "false", "actual": "true",
      "at": "src/Domain/Buyers/CardType.cs:5" },
    { "kind": "type.marker", "element": "card-type", "key": "IAggregateRoot", "outcome": "verified", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/type.marker", "severity": "error", "subject": "card-type", "messageContains": "Entity" }
  ]
}
```

## dto-implements-request

A type that implements MediatR's `IRequest` is a request, which the Services designer models as a Command. The house pack claims "no `IRequest`" for every DTO. `OrderDraftDto` is a plain record and verifies from silence; `CreateOrderDraftCommand` was modelled as a DTO but implements `IRequest<OrderDraftDto>`, so its claim is contradicted.

```json case
{ "phase": 2, "kinds": ["type.marker"], "traces": ["ESC-SVC-43"], "requirements": ["R1", "R2", "R3", "R28"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
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
          "kind": "type.marker",
          "for": { "element": "DTO" },
          "value": "false",
          "discriminator": "'IRequest'",
          "bindings": { "Type": "name", "Marker": "'IRequest'" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the element being a DTO, not a Command",
          "fixHint": "Model a request as a Command, or remove IRequest from the DTO."
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
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "order-draft-dto", "type": "DTO", "name": "OrderDraftDto" },
          { "id": "create-order-draft", "type": "DTO", "name": "CreateOrderDraftCommand" }
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

```csharp file src/Ordering.API/Application/Commands/OrderDraftDto.cs
namespace eShop.Ordering.API.Application.Commands;

public record OrderDraftDto(decimal Total);
```

```csharp file src/Ordering.API/Application/Commands/CreateOrderDraftCommand.cs
namespace eShop.Ordering.API.Application.Commands;

public record CreateOrderDraftCommand(string BuyerId) : IRequest<OrderDraftDto>;
```

```json expect
{
  "claims": [
    { "kind": "type.marker", "element": "order-draft-dto", "key": "IRequest", "outcome": "verified", "expected": "false", "actual": "false", "fromSilence": true },
    { "kind": "type.marker", "element": "create-order-draft", "key": "IRequest", "outcome": "contradicted", "expected": "false", "actual": "true",
      "at": "src/Ordering.API/Application/Commands/CreateOrderDraftCommand.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.marker", "severity": "error", "subject": "create-order-draft", "messageContains": "IRequest" }
  ]
}
```

## java-entity-verified

The spike checked that a Class is an entity type, not an enum or a helper. That `@Entity` maps the class is `entity.persisted`'s fact, which JPA reads from the annotation, so it isn't a marker too: one missing `@Entity` is one finding (user decision, 2026-11-15: "flag it once"). The model states that a class is persisted with its Table stereotype, not with a Primary Key, so a stock `Entity` marker on every Class with a Primary Key would claim what the model doesn't say, and on C# code nothing would read it. `Customer` shows JPA is in the code.

```json case
{ "phase": 4, "kinds": ["entity.persisted", "type.marker"], "traces": ["JAV-DOM-04"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-line-item", "type": "Class", "name": "OrderLineItem",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-line-item-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" },
                "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/shop/domain/OrderLineItem.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class OrderLineItem {
    @Id
    private UUID id;
}
```

```java file src/main/java/com/shop/domain/Customer.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Customer {
    @Id
    private UUID id;
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order-line-item", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/main/java/com/shop/domain/OrderLineItem.java:7" },
    { "kind": "type.marker", "element": "order-line-item", "absent": true, "note": "Mapping the class is entity.persisted's fact, not a marker" }
  ]
}
```

## java-entity-unannotated

The class kept its `@Id` field and lost `@Entity`: a helper class, not an entity. JPA maps nothing it doesn't annotate, so `entity.persisted` is contradicted from silence, one finding.

```json case
{ "phase": 4, "base": "java-entity-verified", "kinds": ["entity.persisted"], "traces": ["JAV-DOM-04"], "requirements": ["R1", "R3", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/OrderLineItem.java
package com.shop.domain;

import jakarta.persistence.Id;
import java.util.UUID;

public class OrderLineItem {
    @Id
    private UUID id;
}
```

```json expect
{
  "claims": [
    { "kind": "entity.persisted", "element": "order-line-item", "outcome": "contradicted", "expected": "true", "actual": "false",
      "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/entity.persisted", "severity": "error", "subject": "order-line-item", "messageContains": "OrderLineItem" }
  ]
}
```
