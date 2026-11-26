# constraint.email

A string Domain attribute's `Email`, `Url` and `Base64` constraints (`constraint.email`, `constraint.url`, `constraint.base64`), each stated by an attribute's presence: .NET's `[EmailAddress]`, `[Url]` and `[Base64String]`, or Bean Validation's `@Email` and Hibernate Validator's `@URL`. Bean Validation has no Base64 constraint, so a Java attribute's can't be verified. `stated: whenSet`: the stereotype states `true`, and silence is `false`. `[DataType(DataType.EmailAddress)]` and `[Phone]` aren't any of them: a data type validates nothing, and a phone number isn't an email address.

## verified

```json case
{ "phase": 4, "kinds": ["constraint.email", "constraint.url", "constraint.base64"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
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
              { "id": "customer-email", "type": "Attribute", "name": "Email", "typeRef": { "type": "string" }, "stereotypes": { "Email": {} } },
              { "id": "customer-website", "type": "Attribute", "name": "Website", "typeRef": { "type": "string" }, "stereotypes": { "Url": {} } },
              { "id": "customer-avatar", "type": "Attribute", "name": "Avatar", "typeRef": { "type": "string" }, "stereotypes": { "Base64": {} } }
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
    [EmailAddress]
    public string Email { get; set; }

    [Url]
    public string Website { get; set; }

    [Base64String]
    public string Avatar { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.email", "element": "customer-email", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Shop.Domain/Customer.cs:7" },
    { "kind": "constraint.url", "element": "customer-website", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Shop.Domain/Customer.cs:10" },
    { "kind": "constraint.base64", "element": "customer-avatar", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Shop.Domain/Customer.cs:13" }
  ]
}
```

## contradicted

`Email` is a `[Phone]`, and `Website` and `Avatar` lose their attributes: three findings, each the code's `false` read from its silence.

```json case
{ "phase": 4, "base": "verified", "kinds": ["constraint.email", "constraint.url", "constraint.base64"], "requirements": ["R1", "R3", "R14"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```csharp file src/Shop.Domain/Customer.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Customer
{
    [Phone]
    public string Email { get; set; }

    [DataType(DataType.Url)]
    public string Website { get; set; }

    public string Avatar { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.email", "element": "customer-email", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true },
    { "kind": "constraint.url", "element": "customer-website", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true },
    { "kind": "constraint.base64", "element": "customer-avatar", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/constraint.email", "severity": "error", "subject": "customer-email",
      "messageContains": "`Customer.Email` Email constraint: model `true`, code `false`, which it states by saying nothing. The model states it in the attribute's Email constraint." }
  ]
}
```

## no-rule-without-data-annotations

No member of `Customer` carries a DataAnnotations validation attribute, so the type validates some other way: can't verify, `noRule`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["constraint.email", "constraint.url"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```csharp file src/Shop.Domain/Customer.cs
namespace Shop.Domain;

public class Customer
{
    public string Email { get; set; }

    public string Website { get; set; }

    public string Avatar { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.email", "element": "customer-email", "outcome": "cantVerify", "reason": "noRule" },
    { "kind": "constraint.url", "element": "customer-website", "outcome": "cantVerify", "reason": "noRule" }
  ]
}
```

## java

Bean Validation's `@Email` and Hibernate Validator's `@URL` state the Email and Url constraints. Bean Validation has no Base64 constraint, so `avatar`'s can't be stated with one: can't verify, never "the code doesn't state it".

```json case
{ "phase": 4, "kinds": ["constraint.email", "constraint.url", "constraint.base64"], "requirements": ["R2", "R5", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.beanvalidation"], "language": "java" }
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
              { "id": "customer-email", "type": "Attribute", "name": "Email", "typeRef": { "type": "string" }, "stereotypes": { "Email": {} } },
              { "id": "customer-website", "type": "Attribute", "name": "Website", "typeRef": { "type": "string" }, "stereotypes": { "Url": {} } },
              { "id": "customer-avatar", "type": "Attribute", "name": "Avatar", "typeRef": { "type": "string" }, "stereotypes": { "Base64": {} } }
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

import jakarta.validation.constraints.Email;
import org.hibernate.validator.constraints.URL;

public class Customer {
    @Email
    private String email;
    @URL
    private String website;
    private String avatar;
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.email", "element": "customer-email", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/main/java/com/shop/domain/Customer.java:7" },
    { "kind": "constraint.url", "element": "customer-website", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/main/java/com/shop/domain/Customer.java:9" },
    { "kind": "constraint.base64", "element": "customer-avatar", "outcome": "cantVerify", "reason": "unresolved" }
  ]
}
```
