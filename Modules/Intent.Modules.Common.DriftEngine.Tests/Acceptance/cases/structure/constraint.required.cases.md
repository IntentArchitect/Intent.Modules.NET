# constraint.required

A Domain attribute's `Required` constraint (Intent.Metadata.Domain.Constraints), against the validation the code states on the attribute's member: .NET's `[Required]` (`intent.dataannotations`), or Bean Validation's `@NotNull`, `@NotBlank` or `@NotEmpty` (`intent.beanvalidation`). `stated: whenSet`: the stereotype states `true`, and silence is `false`. EF Core reads the same `[Required]` as a column's nullability (`member.nullable`), a claim of its own. The stereotype's own note says a non-nullable attribute is implicitly required, so `optin/unmodelled.value` reports a `[Required]` the model leaves unset only on a nullable attribute. A reader answers only for a type that validates with its framework, so a type validated another way can't be verified rather than read as stating nothing.

## verified

`Name` and the nullable `Referrer` have the Required stereotype, and their properties `[Required]`.

```json case
{ "phase": 4, "kinds": ["constraint.required"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
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
              { "id": "customer-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" }, "stereotypes": { "Required": {} } },
              { "id": "customer-nickname", "type": "Attribute", "name": "Nickname", "typeRef": { "type": "string", "nullable": true } },
              { "id": "customer-referrer", "type": "Attribute", "name": "Referrer", "typeRef": { "type": "string", "nullable": true }, "stereotypes": { "Required": {} } }
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
    [Required]
    public string Name { get; set; }

    public string? Nickname { get; set; }

    [Required]
    public string? Referrer { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.required", "element": "customer-name", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Shop.Domain/Customer.cs:7" },
    { "kind": "constraint.required", "element": "customer-referrer", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Shop.Domain/Customer.cs:12" },
    { "kind": "constraint.required", "element": "customer-nickname", "absent": true }
  ],
  "findings": [
    { "rule": "claim/constraint.required", "count": 0 }
  ]
}
```

## contradicted

`Name` loses its `[Required]`: one finding, the code's `false` read from its silence (R3, R14).

```json case
{ "phase": 4, "base": "verified", "kinds": ["constraint.required"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```csharp file src/Shop.Domain/Customer.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Customer
{
    public string Name { get; set; }

    public string? Nickname { get; set; }

    [Required]
    public string? Referrer { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.required", "element": "customer-name", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true, "at": "src/Shop.Domain/Customer.cs:7" }
  ],
  "findings": [
    { "rule": "claim/constraint.required", "severity": "error", "subject": "customer-name",
      "messageContains": "`Customer.Name` Required constraint: model `true`, code `false`, which it states by saying nothing. The model states it in the attribute's Required constraint." },
    { "rule": "claim/constraint.required", "count": 1 }
  ]
}
```

## no-rule-without-data-annotations

`Customer` carries no DataAnnotations validation at all, so it validates some other way (FluentValidation, a domain method) and its silence states nothing: can't verify, `noRule`, not contradicted.

```json case
{ "phase": 4, "base": "verified", "kinds": ["constraint.required"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```csharp file src/Shop.Domain/Customer.cs
namespace Shop.Domain;

public class Customer
{
    public string Name { get; set; }

    public string? Nickname { get; set; }

    public string? Referrer { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.required", "element": "customer-name", "outcome": "cantVerify", "reason": "noRule" },
    { "kind": "constraint.required", "element": "customer-referrer", "outcome": "cantVerify", "reason": "noRule" }
  ]
}
```

## unmodelled-value-on-nullable-only

With `optin/unmodelled.value` on, a `[Required]` the model leaves unset is reported on the nullable `Nickname`. `Name` is non-nullable, which the stereotype's note says is implicitly required, so its `[Required]` states what the model already does.

```json case
{ "phase": 4, "kinds": ["constraint.required"], "cases": ["optin/unmodelled.value"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
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
              { "id": "customer-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } },
              { "id": "customer-nickname", "type": "Attribute", "name": "Nickname", "typeRef": { "type": "string", "nullable": true } }
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
    [Required]
    public string Name { get; set; }

    [Required]
    public string? Nickname { get; set; }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Shop.Domain/Customer.cs:10", "messageContains": "`Customer.Nickname` Required constraint: the code states `true`" },
    { "rule": "optin/unmodelled.value", "messageContains": "Required constraint", "count": 1 }
  ]
}
```

## java-verified

Bean Validation's `@NotBlank` states Required, imported with its package.

```json case
{ "phase": 4, "kinds": ["constraint.required"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.beanvalidation"], "language": "java" }
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
              { "id": "customer-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" }, "stereotypes": { "Required": {} } },
              { "id": "customer-referrer", "type": "Attribute", "name": "Referrer", "typeRef": { "type": "string", "nullable": true }, "stereotypes": { "Required": {} } }
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

import jakarta.validation.constraints.*;

public class Customer {
    @NotBlank
    private String name;
    @NotNull
    private String referrer;
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.required", "element": "customer-name", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/main/java/com/shop/domain/Customer.java:6" },
    { "kind": "constraint.required", "element": "customer-referrer", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/main/java/com/shop/domain/Customer.java:8" }
  ]
}
```

## java-other-not-null-isnt-validation

JetBrains' `@NotNull` is a nullability annotation for static analysis, not a Bean Validation constraint: the file imports it from `org.jetbrains.annotations`, so `referrer` states no Required.

```json case
{ "phase": 4, "base": "java-verified", "kinds": ["constraint.required"], "requirements": ["R1", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.beanvalidation"], "language": "java" }
```

```java file src/main/java/com/shop/domain/Customer.java
package com.shop.domain;

import jakarta.validation.constraints.NotBlank;
import org.jetbrains.annotations.NotNull;

public class Customer {
    @NotBlank
    private String name;
    @NotNull
    private String referrer;
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.required", "element": "customer-name", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/main/java/com/shop/domain/Customer.java:7" },
    { "kind": "constraint.required", "element": "customer-referrer", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true }
  ]
}
```
