# constraint.pattern

A string Domain attribute's `Regular Expression` Pattern and Message (`constraint.pattern`, `constraint.patternMessage`), against .NET's `[RegularExpression(pattern, ErrorMessage = message)]` or Bean Validation's `@Pattern(regexp, message)`. Both are `stated: whenSet` and compared exactly, the pattern as the literal's value (a verbatim string's or a Java string's escapes resolved). The message waits on the pattern, so a missing `[RegularExpression]` is one finding. A message the framework looks up (`ErrorMessageResourceName`, Bean Validation's `{key}`) isn't code the rule reads: can't verify.

## verified

`Expiration` is modelled with the pattern and message eShop's Identity `ApplicationUser` gives it.

```json case
{ "phase": 4, "kinds": ["constraint.pattern", "constraint.patternMessage"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Identity.Domain",
        "elements": [
          { "id": "user", "type": "Class", "name": "ApplicationUser",
            "children": [
              { "id": "user-expiration", "type": "Attribute", "name": "Expiration", "typeRef": { "type": "string" },
                "stereotypes": { "Regular Expression": { "Pattern": "(0[1-9]|1[0-2])\\/[0-9]{2}", "Message": "Expiration should match a valid MM/YY value" } } },
              { "id": "user-zip", "type": "Attribute", "name": "ZipCode", "typeRef": { "type": "string" },
                "stereotypes": { "Regular Expression": { "Pattern": "^[0-9]{5}$" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Identity", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Identity.API/Models/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models;

public class ApplicationUser
{
    [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "Expiration should match a valid MM/YY value")]
    public string Expiration { get; set; }

    [RegularExpression(Patterns.Zip)]
    public string ZipCode { get; set; }
}

public static class Patterns
{
    public const string Zip = "^[0-9]{5}$";
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.pattern", "element": "user-expiration", "outcome": "verified", "expected": "(0[1-9]|1[0-2])\\/[0-9]{2}", "actual": "(0[1-9]|1[0-2])\\/[0-9]{2}", "at": "src/Identity.API/Models/ApplicationUser.cs:7" },
    { "kind": "constraint.patternMessage", "element": "user-expiration", "outcome": "verified", "expected": "Expiration should match a valid MM/YY value", "actual": "Expiration should match a valid MM/YY value" },
    { "kind": "constraint.pattern", "element": "user-zip", "outcome": "verified", "expected": "^[0-9]{5}$", "actual": "^[0-9]{5}$", "at": "src/Identity.API/Models/ApplicationUser.cs:10" },
    { "kind": "constraint.patternMessage", "element": "user-zip", "absent": true }
  ]
}
```

## contradicted

The pattern takes a four-digit year: one finding, the one the eShop house module's proof planted (R14). The message waits on the pattern, so it adds none.

```json case
{ "phase": 4, "base": "verified", "kinds": ["constraint.pattern", "constraint.patternMessage"], "requirements": ["R1", "R5", "R14"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```csharp file src/Identity.API/Models/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models;

public class ApplicationUser
{
    [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{4}", ErrorMessage = "Bad expiry")]
    public string Expiration { get; set; }

    [RegularExpression(Patterns.Zip)]
    public string ZipCode { get; set; }
}

public static class Patterns
{
    public const string Zip = "^[0-9]{5}$";
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.pattern", "element": "user-expiration", "outcome": "contradicted", "expected": "(0[1-9]|1[0-2])\\/[0-9]{2}", "actual": "(0[1-9]|1[0-2])\\/[0-9]{4}" },
    { "kind": "constraint.patternMessage", "element": "user-expiration", "outcome": "cantVerify", "reason": "blocked" }
  ],
  "findings": [
    { "rule": "claim/constraint.pattern", "severity": "error", "subject": "user-expiration",
      "messageContains": "`ApplicationUser.Expiration` Regular Expression pattern: model `(0[1-9]|1[0-2])\\/[0-9]{2}`, code `(0[1-9]|1[0-2])\\/[0-9]{4}`. The model states it in the attribute's Regular Expression Pattern." },
    { "rule": "claim/constraint.patternMessage", "count": 0 }
  ]
}
```

## contradicted-message

The pattern agrees, and the message differs.

```json case
{ "phase": 4, "base": "verified", "kinds": ["constraint.patternMessage"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```csharp file src/Identity.API/Models/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models;

public class ApplicationUser
{
    [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "Bad expiry")]
    public string Expiration { get; set; }

    [RegularExpression(Patterns.Zip)]
    public string ZipCode { get; set; }
}

public static class Patterns
{
    public const string Zip = "^[0-9]{5}$";
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.patternMessage", "element": "user-expiration", "outcome": "contradicted", "expected": "Expiration should match a valid MM/YY value", "actual": "Bad expiry",
      "at": "src/Identity.API/Models/ApplicationUser.cs:7" }
  ],
  "findings": [
    { "rule": "claim/constraint.patternMessage", "severity": "error", "subject": "user-expiration",
      "messageContains": "Regular Expression message: model `Expiration should match a valid MM/YY value`, code `Bad expiry`. The model states it in the attribute's Regular Expression Message." }
  ]
}
```

## missing-is-one-finding

`ZipCode` has no `[RegularExpression]`: its pattern isn't stated, one finding. `Expiration`'s message comes from resources, which can't be read.

```json case
{ "phase": 4, "base": "verified", "kinds": ["constraint.pattern", "constraint.patternMessage"], "requirements": ["R4", "R5"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Identity.Domain",
        "elements": [
          { "id": "user", "type": "Class", "name": "ApplicationUser",
            "children": [
              { "id": "user-expiration", "type": "Attribute", "name": "Expiration", "typeRef": { "type": "string" },
                "stereotypes": { "Regular Expression": { "Pattern": "(0[1-9]|1[0-2])\\/[0-9]{2}", "Message": "Expiration should match a valid MM/YY value" } } },
              { "id": "user-zip", "type": "Attribute", "name": "ZipCode", "typeRef": { "type": "string" },
                "stereotypes": { "Regular Expression": { "Pattern": "^[0-9]{5}$", "Message": "Five digits" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Identity", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Identity.API/Models/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models;

public class ApplicationUser
{
    [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessageResourceName = "Expiration", ErrorMessageResourceType = typeof(Messages))]
    public string Expiration { get; set; }

    public string ZipCode { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.pattern", "element": "user-zip", "outcome": "contradicted", "expected": "^[0-9]{5}$", "actual": null },
    { "kind": "constraint.patternMessage", "element": "user-zip", "outcome": "cantVerify", "reason": "blocked" },
    { "kind": "constraint.patternMessage", "element": "user-expiration", "outcome": "cantVerify", "reason": "unreadable", "at": "src/Identity.API/Models/ApplicationUser.cs:7" }
  ],
  "findings": [
    { "rule": "claim/constraint.pattern", "severity": "error", "subject": "user-zip", "messageContains": "the code doesn't state it" },
    { "rule": "claim/constraint.patternMessage", "subject": "user-zip", "count": 0 }
  ]
}
```

## unreadable

The pattern is a value the code builds at run time.

```json case
{ "phase": 4, "base": "verified", "kinds": ["constraint.pattern"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```csharp file src/Identity.API/Models/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Models;

public class ApplicationUser
{
    [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "Expiration should match a valid MM/YY value")]
    public string Expiration { get; set; }

    [RegularExpression(Patterns.Zip)]
    public string ZipCode { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.pattern", "element": "user-zip", "outcome": "cantVerify", "reason": "unreadable", "at": "src/Identity.API/Models/ApplicationUser.cs:10" }
  ]
}
```

## java-verified

Bean Validation's `@Pattern`, its `regexp` a Java string whose escapes resolve to the model's pattern; a `{key}` message is looked up in `ValidationMessages.properties`, which isn't code.

```json case
{ "phase": 4, "kinds": ["constraint.pattern", "constraint.patternMessage"], "requirements": ["R2", "R5", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.beanvalidation"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "card", "type": "Class", "name": "Card",
            "children": [
              { "id": "card-expiration", "type": "Attribute", "name": "Expiration", "typeRef": { "type": "string" },
                "stereotypes": { "Regular Expression": { "Pattern": "\\d{2}/\\d{2}", "Message": "MM/YY" } } },
              { "id": "card-number", "type": "Attribute", "name": "Number", "typeRef": { "type": "string" },
                "stereotypes": { "Regular Expression": { "Pattern": "\\d{16}", "Message": "Sixteen digits" } } }
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

```java file src/main/java/com/shop/domain/Card.java
package com.shop.domain;

import jakarta.validation.constraints.Pattern;

public class Card {
    @Pattern(regexp = "\\d{2}/\\d{2}", message = "MM/YY")
    private String expiration;
    @Pattern(regexp = "\\d{16}", message = "{card.number.invalid}")
    private String number;
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.pattern", "element": "card-expiration", "outcome": "verified", "expected": "\\d{2}/\\d{2}", "actual": "\\d{2}/\\d{2}", "at": "src/main/java/com/shop/domain/Card.java:6" },
    { "kind": "constraint.patternMessage", "element": "card-expiration", "outcome": "verified", "expected": "MM/YY", "actual": "MM/YY" },
    { "kind": "constraint.pattern", "element": "card-number", "outcome": "verified", "expected": "\\d{16}", "actual": "\\d{16}" },
    { "kind": "constraint.patternMessage", "element": "card-number", "outcome": "cantVerify", "reason": "unreadable", "at": "src/main/java/com/shop/domain/Card.java:8" }
  ]
}
```
