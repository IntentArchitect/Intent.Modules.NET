# type.abstract

Whether the type is abstract: `abstract` on the element against the `abstract` modifier (R41). It is stated `always`, so a class with no `abstract` modifier states that it isn't abstract and the claim is compared from silence (R3). The check runs both ways: a modelled abstract class the code makes concrete, and an abstract code type the model doesn't mark abstract.

## verified

The Class `Document` is abstract in the model and in the code.

```json case
{ "phase": 1, "kinds": ["type.abstract"], "traces": ["ESC-DOM-18"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "document", "type": "Class", "name": "Document", "abstract": true,
            "children": [
              { "id": "document-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
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

```csharp file src/Domain/Billing/Document.cs
namespace Shop.Domain.Billing;

public abstract class Document
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.abstract", "element": "document", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Domain/Billing/Document.cs:3" }
  ]
}
```

## verified-from-silence

The model doesn't mark `Document` abstract, and the class has no `abstract` modifier. Saying nothing states "not abstract", so the claim is verified from silence (R3).

```json case
{ "phase": 1, "base": "verified", "kinds": ["type.abstract"], "traces": ["ESC-DOM-18"], "requirements": ["R2", "R3"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "document", "type": "Class", "name": "Document",
            "children": [
              { "id": "document-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
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

```csharp file src/Domain/Billing/Document.cs
namespace Shop.Domain.Billing;

public class Document
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.abstract", "element": "document", "outcome": "verified", "expected": "false", "actual": "false",
      "fromSilence": true, "at": "src/Domain/Billing/Document.cs:3" }
  ]
}
```

## contradicted-not-abstract

The model marks `Document` abstract, and the code made it concrete. The class's silence states "not abstract", which contradicts the model (R1, R3).

```json case
{ "phase": 1, "base": "verified", "kinds": ["type.abstract"], "traces": ["ESC-DOM-18"], "requirements": ["R1", "R3", "R11", "R14"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/Billing/Document.cs
namespace Shop.Domain.Billing;

public class Document
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.abstract", "element": "document", "outcome": "contradicted", "expected": "true", "actual": "false",
      "fromSilence": true, "at": "src/Domain/Billing/Document.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.abstract", "severity": "error", "subject": "document", "messageContains": "Document" }
  ]
}
```

## contradicted-abstract-in-code

The other way: the code declares `Document` abstract, and the model doesn't. The claim is still the model's, so it is contradicted, with `false` expected.

```json case
{ "phase": 1, "base": "verified-from-silence", "kinds": ["type.abstract"], "traces": ["ESC-DOM-18"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/Billing/Document.cs
namespace Shop.Domain.Billing;

public abstract class Document
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.abstract", "element": "document", "outcome": "contradicted", "expected": "false", "actual": "true",
      "at": "src/Domain/Billing/Document.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.abstract", "severity": "error", "subject": "document" }
  ]
}
```

## blocked

`Document` isn't in the code, so there is no modifier to read. `type.abstract` is can't verify, `blocked` behind the contradicted `type.exists`, with no finding of its own (R5).

```json case
{ "phase": 1, "base": "verified", "without": ["src/Domain/Billing/Document.cs"], "kinds": ["type.exists", "type.abstract"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/Billing/Invoice.cs
namespace Shop.Domain.Billing;

public class Invoice
{
    public Guid Id { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "document", "outcome": "contradicted" },
    { "kind": "type.abstract", "element": "document", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "document", "messageContains": "isn't in the code" },
    { "rule": "claim/type.abstract", "count": 0, "note": "A blocked claim has no finding of its own" }
  ]
}
```

## services-dto-verified

A Services designer DTO marked Abstract is an abstract record, as eShop's paginated requests share a base.

```json case
{ "phase": 1, "kinds": ["type.abstract"], "traces": ["ESC-SVC-42"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Catalog.API",
        "elements": [
          { "id": "paged-request", "type": "DTO", "name": "PagedRequest", "abstract": true }
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

```csharp file src/Catalog.API/Model/PagedRequest.cs
namespace eShop.Catalog.API.Model;

public abstract record PagedRequest(int PageSize = 10, int PageIndex = 0);
```

```json expect
{
  "claims": [
    { "kind": "type.abstract", "element": "paged-request", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Catalog.API/Model/PagedRequest.cs:3" }
  ]
}
```

## services-dto-contradicted

The DTO is Abstract in the model, and the record isn't.

```json case
{ "phase": 1, "base": "services-dto-verified", "kinds": ["type.abstract"], "traces": ["ESC-SVC-42"], "requirements": ["R1", "R3"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```csharp file src/Catalog.API/Model/PagedRequest.cs
namespace eShop.Catalog.API.Model;

public record PagedRequest(int PageSize = 10, int PageIndex = 0);
```

```json expect
{
  "claims": [
    { "kind": "type.abstract", "element": "paged-request", "outcome": "contradicted", "expected": "true", "actual": "false",
      "fromSilence": true, "at": "src/Catalog.API/Model/PagedRequest.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.abstract", "severity": "error", "subject": "paged-request" }
  ]
}
```

## java-verified

The same claim over Java, through the Java reader's modifiers (R33, R42).

```json case
{ "phase": 4, "kinds": ["type.abstract"], "traces": ["JAV-DOM-05"], "requirements": ["R2", "R33", "R42"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "party", "type": "Class", "name": "Party", "abstract": true,
            "children": [
              { "id": "party-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" } }
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

```java file src/main/java/com/shop/domain/Party.java
package com.shop.domain;

import java.util.UUID;

public abstract class Party {
    private UUID id;
}
```

```json expect
{
  "claims": [
    { "kind": "type.abstract", "element": "party", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/main/java/com/shop/domain/Party.java:5" }
  ]
}
```

## java-contradicted

The Java class lost its `abstract` modifier.

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["type.abstract"], "traces": ["JAV-DOM-05"], "requirements": ["R1", "R3", "R33"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```java file src/main/java/com/shop/domain/Party.java
package com.shop.domain;

import java.util.UUID;

public class Party {
    private UUID id;
}
```

```json expect
{
  "claims": [
    { "kind": "type.abstract", "element": "party", "outcome": "contradicted", "expected": "true", "actual": "false",
      "fromSilence": true, "at": "src/main/java/com/shop/domain/Party.java:5" }
  ],
  "findings": [
    { "rule": "claim/type.abstract", "severity": "error", "subject": "party" }
  ]
}
```
