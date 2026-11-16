# repository.entity

The entity a repository is for, against the type argument of the repository's base interface. In C# the model has a Repository element (Intent.Modelers.Domain.Repositories) typed to its entity, located by its conventions' name pattern `I{Name}`, and the code's interface derives `IRepository<T>`. In the Java spike a Domain class carries the spike's own Repository stereotype, and the code's `<Class>Repository` extends `JpaRepository<T, …>`; that stereotype isn't stock, so its vocabulary ships with the spike's module pack, brought inline here (packs.md, "Who ships what"), and its value is `self`, the class itself as a type value. `repository.entity` is `stated: always` and compared by type.

## verified

The `OrderRepository` repository is typed to `Order`, and `IOrderRepository` derives `IRepository<Order>` (ESC-DOM-71).

```json case
{ "phase": 4, "kinds": ["repository.entity"], "traces": ["ESC-DOM-71"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "order-repository", "type": "Repository", "name": "OrderRepository", "typeRef": { "element": "order" } }
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

```csharp file src/Domain/Order.cs
namespace Ordering.Domain;

public class Order
{
    public int Id { get; private set; }
}
```

```csharp file src/Domain/Repositories/IOrderRepository.cs
namespace Ordering.Domain.Repositories;

public interface IOrderRepository : IRepository<Order>
{
}
```

```json expect
{
  "claims": [
    { "kind": "repository.entity", "element": "order-repository", "outcome": "verified", "expected": "Order", "actual": "Order",
      "at": "src/Domain/Repositories/IOrderRepository.cs:3" }
  ]
}
```

## contradicted

`IOrderRepository` derives `IRepository<Buyer>`, a repository for another entity. The model types it to `Order`, so the claim is contradicted (ESC-DOM-71).

```json case
{ "phase": 4, "base": "verified", "kinds": ["repository.entity"], "traces": ["ESC-DOM-71"], "requirements": ["R1", "R10", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Repositories/IOrderRepository.cs
namespace Ordering.Domain.Repositories;

public interface IOrderRepository : IRepository<Buyer>
{
}
```

```json expect
{
  "claims": [
    { "kind": "repository.entity", "element": "order-repository", "outcome": "contradicted", "expected": "Order", "actual": "Buyer",
      "at": "src/Domain/Repositories/IOrderRepository.cs:3" }
  ],
  "findings": [
    { "rule": "claim/repository.entity", "severity": "error", "subject": "order-repository", "messageContains": "code `Buyer`" }
  ]
}
```

## blocked-when-interface-missing

`IOrderRepository` isn't in the code. `type.exists` on the repository is contradicted, and the entity claim, which depends on it, is can't verify, `blocked`, with no finding of its own (R5).

```json case
{ "phase": 4, "base": "verified", "without": ["src/Domain/Repositories/IOrderRepository.cs"], "kinds": ["repository.entity", "type.exists"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order-repository", "outcome": "contradicted" },
    { "kind": "repository.entity", "element": "order-repository", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order-repository", "messageContains": "isn't in the code" },
    { "rule": "claim/repository.entity", "subject": "order-repository", "count": 0, "note": "A blocked claim has no finding of its own" }
  ]
}
```

## java-verified

The Java spike's form: `Order` carries the spike's own Repository stereotype, and `OrderRepository` extends `JpaRepository<Order, UUID>`. The case's pack, `shop.domain`, says what that stereotype states; the JPA pack reads the code (JAV-DOM-09).

```json case
{ "phase": 4, "kinds": ["repository.entity"], "traces": ["JAV-DOM-09"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa", "shop.domain"], "language": "java" }
```

```json pack
{
  "id": "shop.domain",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "description": "The vocabulary for the Java spike's own Repository stereotype on a Domain class.",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }, { "id": "intent.jpa", "version": "^1.0.0" }],
  "languages": ["java"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "entries": [
        {
          "kind": "repository.entity",
          "for": { "element": "Class", "where": "hasStereotype('Repository')" },
          "value": "self",
          "bindings": { "Entity": "name", "Repository": "name + 'Repository'" },
          "own": ["Repository"],
          "dependsOn": "type.exists",
          "source": "the Repository stereotype on the class",
          "fixHint": "Make {Repository} extend JpaRepository<{Entity}, …>, or remove the Repository stereotype."
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
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {}, "Repository": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
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

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;
}
```

```java file src/main/java/com/shop/domain/OrderRepository.java
package com.shop.domain;

import java.util.UUID;
import org.springframework.data.jpa.repository.JpaRepository;

public interface OrderRepository extends JpaRepository<Order, UUID> {
}
```

```json expect
{
  "claims": [
    { "kind": "repository.entity", "element": "order", "outcome": "verified", "expected": "Order", "actual": "Order",
      "at": "src/main/java/com/shop/domain/OrderRepository.java:6" }
  ]
}
```

## java-contradicted

`OrderRepository` extends `JpaRepository<OrderLine, UUID>`, so it isn't the repository of `Order` the stereotype says it is (JAV-DOM-09).

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["repository.entity"], "traces": ["JAV-DOM-09"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa", "shop.domain"], "language": "java" }
```

```java file src/main/java/com/shop/domain/OrderRepository.java
package com.shop.domain;

import java.util.UUID;
import org.springframework.data.jpa.repository.JpaRepository;

public interface OrderRepository extends JpaRepository<OrderLine, UUID> {
}
```

```json expect
{
  "claims": [
    { "kind": "repository.entity", "element": "order", "outcome": "contradicted", "expected": "Order", "actual": "OrderLine",
      "at": "src/main/java/com/shop/domain/OrderRepository.java:6" }
  ],
  "findings": [
    { "rule": "claim/repository.entity", "severity": "error", "subject": "order", "messageContains": "code `OrderLine`" }
  ]
}
```
