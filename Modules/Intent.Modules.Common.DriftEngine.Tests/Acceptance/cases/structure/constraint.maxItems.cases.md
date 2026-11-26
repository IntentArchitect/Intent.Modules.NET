# constraint.maxItems

A collection Domain attribute's `Collection Limits` Min Length and Max Length (`constraint.minItems`, `constraint.maxItems`): how many items it may hold, against .NET's `[MinLength]`, `[MaxLength]` and `[Length(min, max)]` on the collection, or Bean Validation's `@Size(min, max)`. Kinds of their own, apart from a string's length (`constraint.minLength`, `constraint.maxLength`), since one string collection can have both, each meaning something else. `stated: whenSet`, compared as numbers; no minimum is `0`.

## verified

`Tags` holds one to five items, and `Codes` at most three.

```json case
{ "phase": 4, "kinds": ["constraint.minItems", "constraint.maxItems"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "product", "type": "Class", "name": "Product",
            "children": [
              { "id": "product-tags", "type": "Attribute", "name": "Tags", "typeRef": { "type": "string", "collection": true }, "stereotypes": { "Collection Limits": { "Min Length": 1, "Max Length": 5 } } },
              { "id": "product-codes", "type": "Attribute", "name": "Codes", "typeRef": { "type": "string", "collection": true }, "stereotypes": { "Collection Limits": { "Max Length": 3 } } }
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

```csharp file src/Shop.Domain/Product.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Product
{
    [MinLength(1)]
    [MaxLength(5)]
    public List<string> Tags { get; set; }

    [Length(0, 3)]
    public List<string> Codes { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.minItems", "element": "product-tags", "outcome": "verified", "expected": "1", "actual": "1", "at": "src/Shop.Domain/Product.cs:7" },
    { "kind": "constraint.maxItems", "element": "product-tags", "outcome": "verified", "expected": "5", "actual": "5", "at": "src/Shop.Domain/Product.cs:8" },
    { "kind": "constraint.maxItems", "element": "product-codes", "outcome": "verified", "expected": "3", "actual": "3", "at": "src/Shop.Domain/Product.cs:11" },
    { "kind": "constraint.minItems", "element": "product-codes", "absent": true }
  ]
}
```

## contradicted

`Tags` loses its minimum, and holds up to six.

```json case
{ "phase": 4, "base": "verified", "kinds": ["constraint.minItems", "constraint.maxItems"], "requirements": ["R1", "R3", "R14"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```csharp file src/Shop.Domain/Product.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Product
{
    [MaxLength(6)]
    public List<string> Tags { get; set; }

    [Length(0, 3)]
    public List<string> Codes { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.minItems", "element": "product-tags", "outcome": "contradicted", "expected": "1", "actual": "0", "fromSilence": true },
    { "kind": "constraint.maxItems", "element": "product-tags", "outcome": "contradicted", "expected": "5", "actual": "6", "at": "src/Shop.Domain/Product.cs:7" }
  ],
  "findings": [
    { "rule": "claim/constraint.maxItems", "severity": "error", "subject": "product-tags",
      "messageContains": "`Product.Tags` maximum items constraint: model `5`, code `6`. The model states it in the attribute's Collection Limits Max Length." }
  ]
}
```

## unreadable

The limits are values the code computes, so they can't be read without running it.

```json case
{ "phase": 4, "base": "verified", "kinds": ["constraint.minItems", "constraint.maxItems"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```csharp file src/Shop.Domain/Product.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Product
{
    [MinLength(Settings.MinimumTags)]
    [MaxLength(Settings.MaximumTags)]
    public List<string> Tags { get; set; }

    [Length(0, 3)]
    public List<string> Codes { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.minItems", "element": "product-tags", "outcome": "cantVerify", "reason": "unreadable", "at": "src/Shop.Domain/Product.cs:7" },
    { "kind": "constraint.maxItems", "element": "product-tags", "outcome": "cantVerify", "reason": "unreadable", "at": "src/Shop.Domain/Product.cs:8" }
  ]
}
```

## java-verified

Bean Validation's `@Size` on a collection limits its count.

```json case
{ "phase": 4, "kinds": ["constraint.minItems", "constraint.maxItems"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.beanvalidation"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "product", "type": "Class", "name": "Product",
            "children": [
              { "id": "product-tags", "type": "Attribute", "name": "Tags", "typeRef": { "type": "string", "collection": true }, "stereotypes": { "Collection Limits": { "Min Length": 1, "Max Length": 5 } } }
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

```java file src/main/java/com/shop/domain/Product.java
package com.shop.domain;

import java.util.List;
import jakarta.validation.constraints.Size;

public class Product {
    @Size(min = 1, max = 5)
    private List<String> tags;
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.minItems", "element": "product-tags", "outcome": "verified", "expected": "1", "actual": "1", "at": "src/main/java/com/shop/domain/Product.java:7" },
    { "kind": "constraint.maxItems", "element": "product-tags", "outcome": "verified", "expected": "5", "actual": "5", "at": "src/main/java/com/shop/domain/Product.java:7" }
  ]
}
```
