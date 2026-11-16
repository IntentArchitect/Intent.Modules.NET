# type.generalization

The model's own inheritance: a Generalization to another modelled element against the type's base class, compared by name and resolved where the reader can (R41). It runs both ways among modelled classes: a modelled base the code drops is contradicted, and so is a modelled class the code derives from when the model has no Generalization. A base the model implies without modelling it, such as eShop's SeedWork `Entity`, is `type.base`'s, not this kind's.

## verified

`Invoice` generalizes `Document` in the model and derives it in the code.

```json case
{ "phase": 1, "kinds": ["type.generalization"], "traces": ["ESC-DOM-19"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "document", "type": "Class", "name": "Document", "abstract": true },
          { "id": "invoice", "type": "Class", "name": "Invoice", "generalization": "document" }
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
}
```

```csharp file src/Domain/Billing/Invoice.cs
namespace Shop.Domain.Billing;

public class Invoice : Document
{
}
```

```json expect
{
  "claims": [
    { "kind": "type.generalization", "element": "invoice", "outcome": "verified", "expected": "Document", "actual": "Document",
      "at": "src/Domain/Billing/Invoice.cs:3" }
  ]
}
```

## contradicted-base-dropped

The code's `Invoice` no longer derives `Document`. It has no base class, which contradicts the modelled Generalization.

```json case
{ "phase": 1, "base": "verified", "kinds": ["type.generalization"], "traces": ["ESC-DOM-19"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/Billing/Invoice.cs
namespace Shop.Domain.Billing;

public class Invoice
{
}
```

```json expect
{
  "claims": [
    { "kind": "type.generalization", "element": "invoice", "outcome": "contradicted", "expected": "Document", "actual": null,
      "at": "src/Domain/Billing/Invoice.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.generalization", "severity": "error", "subject": "invoice", "messageContains": "Document" }
  ]
}
```

## contradicted-unmodelled-base

The other way, among modelled classes. The model has no Generalization on `Invoice`, but the code derives the modelled class `Document`: contradicted, with no base expected. `Customer` derives the SeedWork `Entity`, which isn't a modelled class, so its claim is verified: that base is `type.base`'s concern.

```json case
{ "phase": 1, "base": "verified", "kinds": ["type.generalization"], "traces": ["ESC-DOM-19"], "requirements": ["R1", "R2", "R41"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "document", "type": "Class", "name": "Document", "abstract": true },
          { "id": "invoice", "type": "Class", "name": "Invoice" },
          { "id": "customer", "type": "Class", "name": "Customer" }
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

```csharp file src/Domain/Customers/Customer.cs
using Shop.Domain.SeedWork;

namespace Shop.Domain.Customers;

public class Customer : Entity
{
}
```

```json expect
{
  "claims": [
    { "kind": "type.generalization", "element": "invoice", "outcome": "contradicted", "expected": null, "actual": "Document",
      "at": "src/Domain/Billing/Invoice.cs:3" },
    { "kind": "type.generalization", "element": "customer", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/type.generalization", "severity": "error", "subject": "invoice" },
    { "rule": "claim/type.generalization", "subject": "customer", "count": 0 }
  ]
}
```

## blocked

`Invoice` isn't in the code. Its base can't be read: `type.generalization` is can't verify, `blocked` behind `type.exists` (R5).

```json case
{ "phase": 1, "base": "verified", "without": ["src/Domain/Billing/Invoice.cs"], "kinds": ["type.exists", "type.generalization"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "invoice", "outcome": "contradicted" },
    { "kind": "type.generalization", "element": "invoice", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "invoice", "messageContains": "isn't in the code" },
    { "rule": "claim/type.generalization", "count": 0, "note": "A blocked claim has no finding of its own" }
  ]
}
```

## services-dto-verified

A Services designer DTO's Generalization is the record's base record: eShop's `CatalogItemsRequest` extends the paginated request.

```json case
{ "phase": 1, "kinds": ["type.generalization"], "traces": ["ESC-SVC-44"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Catalog.API",
        "elements": [
          { "id": "paged-request", "type": "DTO", "name": "PagedRequest", "abstract": true },
          { "id": "catalog-items-request", "type": "DTO", "name": "CatalogItemsRequest", "generalization": "paged-request" }
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

```csharp file src/Catalog.API/Model/CatalogItemsRequest.cs
namespace eShop.Catalog.API.Model;

public record CatalogItemsRequest(int PageSize, int PageIndex, string? Name) : PagedRequest(PageSize, PageIndex);
```

```json expect
{
  "claims": [
    { "kind": "type.generalization", "element": "catalog-items-request", "outcome": "verified", "expected": "PagedRequest", "actual": "PagedRequest",
      "at": "src/Catalog.API/Model/CatalogItemsRequest.cs:3" }
  ]
}
```

## services-dto-contradicted

The record derives nothing; the modelled base is gone.

```json case
{ "phase": 1, "base": "services-dto-verified", "kinds": ["type.generalization"], "traces": ["ESC-SVC-44"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```csharp file src/Catalog.API/Model/CatalogItemsRequest.cs
namespace eShop.Catalog.API.Model;

public record CatalogItemsRequest(int PageSize, int PageIndex, string? Name);
```

```json expect
{
  "claims": [
    { "kind": "type.generalization", "element": "catalog-items-request", "outcome": "contradicted", "expected": "PagedRequest", "actual": null,
      "at": "src/Catalog.API/Model/CatalogItemsRequest.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.generalization", "severity": "error", "subject": "catalog-items-request" }
  ]
}
```

## java-verified

The same claim over Java: the Generalization is the class's `extends` (R33).

```json case
{ "phase": 4, "kinds": ["type.generalization"], "traces": ["JAV-DOM-06"], "requirements": ["R2", "R33", "R42"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "party", "type": "Class", "name": "Party", "abstract": true },
          { "id": "customer", "type": "Class", "name": "Customer", "generalization": "party" }
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

public abstract class Party {
}
```

```java file src/main/java/com/shop/domain/Customer.java
package com.shop.domain;

public class Customer extends Party {
}
```

```json expect
{
  "claims": [
    { "kind": "type.generalization", "element": "customer", "outcome": "verified", "expected": "Party", "actual": "Party",
      "at": "src/main/java/com/shop/domain/Customer.java:3" }
  ]
}
```

## java-unmodelled-extends

The model has no Generalization on `Customer`, and the Java class `extends` the modelled `Party`. The spike reported that as Backlog; here the claim the model makes, no base, is contradicted.

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["type.generalization"], "traces": ["JAV-DOM-06"], "requirements": ["R1", "R33"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "party", "type": "Class", "name": "Party", "abstract": true },
          { "id": "customer", "type": "Class", "name": "Customer" }
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

```json expect
{
  "claims": [
    { "kind": "type.generalization", "element": "customer", "outcome": "contradicted", "expected": null, "actual": "Party",
      "at": "src/main/java/com/shop/domain/Customer.java:3" }
  ],
  "findings": [
    { "rule": "claim/type.generalization", "severity": "error", "subject": "customer" }
  ]
}
```
