# type.kind

The kind of code type the model means: an Enum is an `enum`, not a class (R41). `type.exists` finds the type by name among the kinds the Domain pack's conventions locate an element type by; `type.kind` then compares the kind it found with the one the model implies. The Domain pack locates an Enum among enums and classes, so a class written in eShop's older `Enumeration` style is found and reported as the wrong kind, not as missing. These cases are about Enums, where exactly one kind is right; a Class may be a class or a record.

## verified

The Enum `OrderStatus` is a C# `enum`.

```json case
{ "phase": 1, "kinds": ["type.kind"], "traces": ["ESC-DOM-13"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-status", "type": "Enum", "name": "OrderStatus",
            "children": [
              { "id": "order-status-submitted", "type": "Literal", "name": "Submitted" },
              { "id": "order-status-paid", "type": "Literal", "name": "Paid" }
            ] }
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

```csharp file src/Domain/Orders/OrderStatus.cs
namespace Shop.Domain.Orders;

public enum OrderStatus
{
    Submitted,
    Paid
}
```

```json expect
{
  "claims": [
    { "kind": "type.kind", "element": "order-status", "outcome": "verified", "expected": "enum", "actual": "enum",
      "at": "src/Domain/Orders/OrderStatus.cs:3" }
  ]
}
```

## contradicted

The code keeps eShop's older `Enumeration` pattern: `OrderStatus` is a class with static instances. The type is there, so `type.exists` is verified; its kind isn't the one the model states, so `type.kind` is contradicted, naming both kinds and the line (R11, R14).

```json case
{ "phase": 1, "base": "verified", "kinds": ["type.exists", "type.kind"], "traces": ["ESC-DOM-13"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/Orders/OrderStatus.cs
namespace Shop.Domain.Orders;

public class OrderStatus : Enumeration
{
    public static OrderStatus Submitted = new(1, nameof(Submitted));
    public static OrderStatus Paid = new(2, nameof(Paid));

    public OrderStatus(int id, string name) : base(id, name) { }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order-status", "outcome": "verified", "at": "src/Domain/Orders/OrderStatus.cs:3" },
    { "kind": "type.kind", "element": "order-status", "outcome": "contradicted", "expected": "enum", "actual": "class",
      "at": "src/Domain/Orders/OrderStatus.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.kind", "severity": "error", "subject": "order-status", "messageContains": "OrderStatus" }
  ]
}
```

## blocked

`OrderStatus` isn't in the code at all. Its kind has nothing to be read from: `type.kind` is can't verify, `blocked` behind the contradicted `type.exists`, and only the missing type is reported (R5).

```json case
{ "phase": 1, "base": "verified", "without": ["src/Domain/Orders/OrderStatus.cs"], "kinds": ["type.exists", "type.kind"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/Orders/Order.cs
namespace Shop.Domain.Orders;

public class Order
{
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order-status", "outcome": "contradicted" },
    { "kind": "type.kind", "element": "order-status", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order-status", "messageContains": "isn't in the code" },
    { "rule": "claim/type.kind", "count": 0, "note": "Counted in the missing type's finding" }
  ]
}
```

## java-verified

The same claim over Java: the Enum is a Java `enum`, read through the Java reader and language profile (R33, R42).

```json case
{ "phase": 4, "kinds": ["type.kind"], "traces": ["JAV-DOM-04"], "requirements": ["R2", "R33", "R42"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-status", "type": "Enum", "name": "OrderStatus",
            "children": [
              { "id": "order-status-placed", "type": "Literal", "name": "Placed" },
              { "id": "order-status-shipped", "type": "Literal", "name": "Shipped" }
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

```java file src/main/java/com/shop/domain/OrderStatus.java
package com.shop.domain;

public enum OrderStatus { Placed, Shipped }
```

```json expect
{
  "claims": [
    { "kind": "type.kind", "element": "order-status", "outcome": "verified", "expected": "enum", "actual": "enum",
      "at": "src/main/java/com/shop/domain/OrderStatus.java:3" }
  ]
}
```

## java-contradicted

The Java code holds the statuses as string constants on a helper class. The spike reported it as "not a Java `enum`"; here `type.kind` is contradicted, `enum` against `class`.

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["type.kind"], "traces": ["JAV-DOM-04"], "requirements": ["R1", "R11", "R33"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```java file src/main/java/com/shop/domain/OrderStatus.java
package com.shop.domain;

public final class OrderStatus {
    public static final String PLACED = "Placed";
    public static final String SHIPPED = "Shipped";

    private OrderStatus() {
    }
}
```

```json expect
{
  "claims": [
    { "kind": "type.kind", "element": "order-status", "outcome": "contradicted", "expected": "enum", "actual": "class",
      "at": "src/main/java/com/shop/domain/OrderStatus.java:3" }
  ],
  "findings": [
    { "rule": "claim/type.kind", "severity": "error", "subject": "order-status" }
  ]
}
```
