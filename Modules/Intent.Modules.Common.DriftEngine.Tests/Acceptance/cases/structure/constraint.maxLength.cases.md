# constraint.maxLength

A string Domain attribute's `Text Limits` Min Length and Max Length (`constraint.minLength`, `constraint.maxLength`), against the length the code validates: .NET's `[MinLength]`, `[MaxLength]`, `[StringLength(max, MinimumLength = min)]` and `[Length(min, max)]`, or Bean Validation's `@Size(min, max)` and Hibernate Validator's `@Length`. `stated: whenSet`, compared as numbers: a Text Limits with neither value states nothing (the Constraints module applies an empty one to string attributes by default). No minimum is `0`; no maximum states none. Where the attribute's Text Constraints MaxLength is set, Intent validates that in place of the Text Limits Max Length, so it is the maximum length claimed, and `optin/unmodelled.value` doesn't report a maximum Text Constraints already states. EF Core reads the same `[MaxLength]` and `[StringLength]` as a column's length (`column.maxLength`), a claim of its own. On a collection, Text Limits limit each item's length, which a length attribute on the member doesn't state: can't verify.

## verified

`Nickname`, `Code` and `Zip` state their limits each a different way, `Code`'s maximum through a constant; `Notes`' Text Limits are empty, and the collection `Tags`' limit its items.

```json case
{ "phase": 4, "kinds": ["constraint.minLength", "constraint.maxLength"], "requirements": ["R2", "R5"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "customer", "type": "Class", "name": "Customer",
            "children": [
              { "id": "customer-nickname", "type": "Attribute", "name": "Nickname", "typeRef": { "type": "string" }, "stereotypes": { "Text Limits": { "Min Length": 2, "Max Length": 50 } } },
              { "id": "customer-code", "type": "Attribute", "name": "Code", "typeRef": { "type": "string" }, "stereotypes": { "Text Limits": { "Min Length": 1, "Max Length": 8 } } },
              { "id": "customer-zip", "type": "Attribute", "name": "Zip", "typeRef": { "type": "string" }, "stereotypes": { "Text Limits": { "Min Length": 3, "Max Length": 10 } } },
              { "id": "customer-notes", "type": "Attribute", "name": "Notes", "typeRef": { "type": "string" }, "stereotypes": { "Text Limits": {} } },
              { "id": "customer-tags", "type": "Attribute", "name": "Tags", "typeRef": { "type": "string", "collection": true }, "stereotypes": { "Text Limits": { "Min Length": 1, "Max Length": 20 } } }
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

```csharp file src/Shop.Domain/Customer.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Customer
{
    [StringLength(50, MinimumLength = 2)]
    public string Nickname { get; set; }

    [MinLength(1)]
    [MaxLength(CustomerLimits.Code)]
    public string Code { get; set; }

    [Length(3, 10)]
    public string Zip { get; set; }

    public string Notes { get; set; }

    [MaxLength(20)]
    public string[] Tags { get; set; }
}

public static class CustomerLimits
{
    public const int Code = 8;
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.minLength", "element": "customer-nickname", "outcome": "verified", "expected": "2", "actual": "2", "at": "src/Shop.Domain/Customer.cs:7" },
    { "kind": "constraint.maxLength", "element": "customer-nickname", "outcome": "verified", "expected": "50", "actual": "50", "at": "src/Shop.Domain/Customer.cs:7" },
    { "kind": "constraint.minLength", "element": "customer-code", "outcome": "verified", "expected": "1", "actual": "1", "at": "src/Shop.Domain/Customer.cs:10" },
    { "kind": "constraint.maxLength", "element": "customer-code", "outcome": "verified", "expected": "8", "actual": "8", "at": "src/Shop.Domain/Customer.cs:11" },
    { "kind": "constraint.minLength", "element": "customer-zip", "outcome": "verified", "expected": "3", "actual": "3", "at": "src/Shop.Domain/Customer.cs:14" },
    { "kind": "constraint.maxLength", "element": "customer-zip", "outcome": "verified", "expected": "10", "actual": "10", "at": "src/Shop.Domain/Customer.cs:14" },
    { "kind": "constraint.minLength", "element": "customer-notes", "absent": true },
    { "kind": "constraint.maxLength", "element": "customer-notes", "absent": true },
    { "kind": "constraint.minLength", "element": "customer-tags", "outcome": "cantVerify", "reason": "unresolved", "at": "src/Shop.Domain/Customer.cs:19" },
    { "kind": "constraint.maxLength", "element": "customer-tags", "outcome": "cantVerify", "reason": "unresolved", "at": "src/Shop.Domain/Customer.cs:19" }
  ],
  "findings": [
    { "rule": "engine/noVocabulary", "count": 0, "note": "An empty Text Limits is read, and states nothing" }
  ]
}
```

## contradicted

`Nickname` is now `[StringLength(40)]`: its maximum differs, and with no `MinimumLength` its minimum is `0`, by silence. `Zip` states no length at all, so its maximum isn't stated.

```json case
{ "phase": 4, "base": "verified", "kinds": ["constraint.minLength", "constraint.maxLength"], "requirements": ["R1", "R3", "R4", "R14"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```csharp file src/Shop.Domain/Customer.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Customer
{
    [StringLength(40)]
    public string Nickname { get; set; }

    [MinLength(1)]
    [MaxLength(CustomerLimits.Code)]
    public string Code { get; set; }

    public string Zip { get; set; }

    public string Notes { get; set; }

    [MaxLength(20)]
    public string[] Tags { get; set; }
}

public static class CustomerLimits
{
    public const int Code = 8;
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.maxLength", "element": "customer-nickname", "outcome": "contradicted", "expected": "50", "actual": "40", "at": "src/Shop.Domain/Customer.cs:7" },
    { "kind": "constraint.minLength", "element": "customer-nickname", "outcome": "contradicted", "expected": "2", "actual": "0", "fromSilence": true },
    { "kind": "constraint.maxLength", "element": "customer-zip", "outcome": "contradicted", "expected": "10", "actual": null },
    { "kind": "constraint.minLength", "element": "customer-zip", "outcome": "contradicted", "expected": "3", "actual": "0", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/constraint.maxLength", "severity": "error", "subject": "customer-nickname",
      "messageContains": "`Customer.Nickname` maximum length constraint: model `50`, code `40`. The model states it in the attribute's Text Limits Max Length." },
    { "rule": "claim/constraint.maxLength", "severity": "error", "subject": "customer-zip", "messageContains": "the code doesn't state it" }
  ]
}
```

## text-constraints-state-the-validated-maximum

`Nickname`'s Text Constraints MaxLength, 40, is what Intent validates in place of its Text Limits Max Length, 100, so it is the maximum claimed. With `optin/unmodelled.value` on, `Notes`' `[MaxLength(200)]` is the length its Text Constraints already state, so it isn't reported; `Comment`'s `[MaxLength(500)]` is a maximum the model doesn't state anywhere, and is.

```json case
{ "phase": 4, "kinds": ["constraint.maxLength"], "cases": ["optin/unmodelled.value"], "requirements": ["R2", "R49"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "customer", "type": "Class", "name": "Customer",
            "children": [
              { "id": "customer-nickname", "type": "Attribute", "name": "Nickname", "typeRef": { "type": "string" },
                "stereotypes": { "Text Limits": { "Max Length": 100 }, "Text Constraints": { "MaxLength": 40 } } },
              { "id": "customer-notes", "type": "Attribute", "name": "Notes", "typeRef": { "type": "string" }, "stereotypes": { "Text Constraints": { "MaxLength": 200 } } },
              { "id": "customer-comment", "type": "Attribute", "name": "Comment", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ] }
        } }]
    }
  }
}
```

```csharp file src/Shop.Domain/Customer.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Customer
{
    [StringLength(40)]
    public string Nickname { get; set; }

    [MaxLength(200)]
    public string Notes { get; set; }

    [MaxLength(500)]
    public string Comment { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.maxLength", "element": "customer-nickname", "outcome": "verified", "expected": "40", "actual": "40", "at": "src/Shop.Domain/Customer.cs:7" },
    { "kind": "constraint.maxLength", "element": "customer-notes", "absent": true }
  ],
  "findings": [
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Shop.Domain/Customer.cs:13", "messageContains": "`Customer.Comment` maximum length constraint: the code states `500`" },
    { "rule": "optin/unmodelled.value", "messageContains": "maximum length constraint", "count": 1 }
  ]
}
```

## java-verified

Bean Validation's `@Size` and Hibernate Validator's `@Length`.

```json case
{ "phase": 4, "kinds": ["constraint.minLength", "constraint.maxLength"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.beanvalidation"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "customer", "type": "Class", "name": "Customer",
            "children": [
              { "id": "customer-nickname", "type": "Attribute", "name": "Nickname", "typeRef": { "type": "string" }, "stereotypes": { "Text Limits": { "Min Length": 2, "Max Length": 50 } } },
              { "id": "customer-zip", "type": "Attribute", "name": "Zip", "typeRef": { "type": "string" }, "stereotypes": { "Text Limits": { "Min Length": 3, "Max Length": 10 } } }
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

```java file src/main/java/com/shop/domain/Customer.java
package com.shop.domain;

import jakarta.validation.constraints.Size;
import org.hibernate.validator.constraints.Length;

public class Customer {
    @Size(min = 2, max = 50)
    private String nickname;
    @Length(min = 3, max = 12)
    private String zip;
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.minLength", "element": "customer-nickname", "outcome": "verified", "expected": "2", "actual": "2", "at": "src/main/java/com/shop/domain/Customer.java:7" },
    { "kind": "constraint.maxLength", "element": "customer-nickname", "outcome": "verified", "expected": "50", "actual": "50", "at": "src/main/java/com/shop/domain/Customer.java:7" },
    { "kind": "constraint.minLength", "element": "customer-zip", "outcome": "verified", "expected": "3", "actual": "3", "at": "src/main/java/com/shop/domain/Customer.java:9" },
    { "kind": "constraint.maxLength", "element": "customer-zip", "outcome": "contradicted", "expected": "10", "actual": "12", "at": "src/main/java/com/shop/domain/Customer.java:9" }
  ]
}
```
