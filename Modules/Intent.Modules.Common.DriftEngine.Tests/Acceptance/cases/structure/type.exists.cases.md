# type.exists

A modelled element has a code type: found by name among the in-scope types of the kinds its vocabulary allows (R43). It is an existence kind, so a type the code lacks is contradicted (R8), and every claim that depends on it is blocked behind it (R5).

## verified

```json case
{ "phase": 1, "kinds": ["type.exists"], "traces": ["ESC-DOM-12"], "requirements": ["R41", "R43"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
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

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" },
    { "kind": "member.exists", "element": "order-id", "outcome": "verified", "at": "src/Domain/Order.cs:5" }
  ]
}
```

## contradicted-missing

`Order` isn't in the code. One finding says so and counts the claims it blocks; its members' claims are can't verify, `blocked`, with no finding of their own.

```json case
{ "phase": 1, "base": "verified", "without": ["src/Domain/Order.cs"], "kinds": ["type.exists", "member.exists"], "cases": ["engine/blocked"], "traces": ["ESC-DOM-12"], "requirements": ["R5", "R8", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/Customer.cs
namespace Shop.Domain;

public class Customer
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "contradicted" },
    { "kind": "member.exists", "element": "order-id", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "isn't in the code" },
    { "rule": "claim/member.exists", "count": 0, "note": "A blocked claim is counted in its owner's finding, not reported apart" }
  ]
}
```

## ambiguous

Two in-scope classes are named `Order`, in different folders and namespaces, and nothing narrows the choice. The engine doesn't pick one: the claim is can't verify, `ambiguous`, and the finding names both (R43).

```json case
{ "phase": 1, "base": "verified", "kinds": ["type.exists"], "cases": ["engine/ambiguous"], "requirements": ["R5", "R43"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Reporting/Order.cs
namespace Shop.Reporting;

public class Order
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "cantVerify", "reason": "ambiguous" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "src/Reporting/Order.cs:3" }
  ]
}
```

## out-of-scope

The only `Order` is in a folder a scope row excludes. The file's name matches the type's, so the claim is can't verify, `outOfScope`, naming the file and the row, rather than contradicted (R25). The excluded file is never read.

```json case
{ "phase": 1, "base": "verified", "without": ["src/Domain/Order.cs"], "kinds": ["type.exists"], "cases": ["scope/shadow", "scope/notRead"], "requirements": ["R22", "R25"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Legacy/**", "Scope": "Out", "Reason": "Being replaced by src/Domain" }
        ] } } }]
    }
  }
}
```

```csharp file src/Legacy/Order.cs
namespace Shop.Legacy;

public class Order
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "cantVerify", "reason": "outOfScope", "scopeRow": "root#2" },
    { "kind": "member.exists", "element": "order-id", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "src/Legacy/**" }
  ],
  "filesRead": { "exclude": ["src/Legacy/**"] }
}
```

## services-dto-verified

A Services designer DTO is a record of that name.

```json case
{ "phase": 1, "kinds": ["type.exists"], "traces": ["ESC-SVC-11"], "requirements": ["R41"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Services",
        "elements": [
          { "id": "order-dto", "type": "DTO", "name": "OrderDto",
            "children": [
              { "id": "order-dto-id", "type": "DTO-Field", "name": "Id", "typeRef": { "type": "guid" } }
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

```csharp file src/Application/Orders/OrderDto.cs
namespace Shop.Application.Orders;

public record OrderDto(Guid Id);
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order-dto", "outcome": "verified", "at": "src/Application/Orders/OrderDto.cs:3" }
  ]
}
```

## java-verified

The same claim over Java, through the Java reader and language profile.

```json case
{ "phase": 4, "kinds": ["type.exists"], "traces": ["JAV-DOM-02"], "requirements": ["R33", "R41"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
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

import java.util.UUID;

public class Order {
    private UUID id;

    public UUID getId() {
        return id;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/main/java/com/shop/domain/Order.java:5" }
  ]
}
```
