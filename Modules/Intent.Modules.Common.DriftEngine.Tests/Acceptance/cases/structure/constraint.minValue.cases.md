# constraint.minValue

A numeric Domain attribute's `Numeric Limits` (`constraint.minValue`, `constraint.maxValue`, and for a `decimal`, `float` or `double`, whose boundary types the designer offers, `constraint.minExclusive` and `constraint.maxExclusive`), against .NET's `[Range(min, max)]` or `[Range(typeof(T), "min", "max")]` with `MinimumIsExclusive` and `MaximumIsExclusive`, or Bean Validation's `@Min`, `@Max`, `@DecimalMin`, `@DecimalMax` (with `inclusive`), `@Positive`, `@PositiveOrZero`, `@Negative`, `@NegativeOrZero` and Hibernate Validator's `@Range`. `stated: whenSet`, compared as numbers (`120` is `120.0`); a bound written as its type's extreme (`int.MaxValue`, `Long.MAX_VALUE`) is no bound. A boundary is inclusive unless the code says otherwise, and waits on its value.

## verified

`Age` is between 1 and 120; `Discount` from 0 to under 1, the code's maximum exclusive; `Score` at least 10, as `[Range(10, int.MaxValue)]` says, with no maximum.

```json case
{ "phase": 4, "kinds": ["constraint.minValue", "constraint.maxValue", "constraint.minExclusive", "constraint.maxExclusive"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
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
              { "id": "customer-age", "type": "Attribute", "name": "Age", "typeRef": { "type": "int" }, "stereotypes": { "Numeric Limits": { "Min Value": "1", "Max Value": "120" } } },
              { "id": "customer-discount", "type": "Attribute", "name": "Discount", "typeRef": { "type": "decimal" },
                "stereotypes": { "Numeric Limits": { "Min Value": "0", "Min Boundary Type": "Inclusive", "Max Value": "1", "Max Boundary Type": "Exclusive" } } },
              { "id": "customer-score", "type": "Attribute", "name": "Score", "typeRef": { "type": "int" }, "stereotypes": { "Numeric Limits": { "Min Value": "10" } } }
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
    [Range(1, 120)]
    public int Age { get; set; }

    [Range(typeof(decimal), "0", "1.0", MaximumIsExclusive = true)]
    public decimal Discount { get; set; }

    [Range(10, int.MaxValue)]
    public int Score { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.minValue", "element": "customer-age", "outcome": "verified", "expected": "1", "actual": "1", "at": "src/Shop.Domain/Customer.cs:7" },
    { "kind": "constraint.maxValue", "element": "customer-age", "outcome": "verified", "expected": "120", "actual": "120", "at": "src/Shop.Domain/Customer.cs:7" },
    { "kind": "constraint.minExclusive", "element": "customer-age", "absent": true },
    { "kind": "constraint.minValue", "element": "customer-discount", "outcome": "verified", "expected": "0", "actual": "0", "at": "src/Shop.Domain/Customer.cs:10" },
    { "kind": "constraint.maxValue", "element": "customer-discount", "outcome": "verified", "expected": "1", "actual": "1.0", "at": "src/Shop.Domain/Customer.cs:10" },
    { "kind": "constraint.minExclusive", "element": "customer-discount", "outcome": "verified", "expected": "false", "actual": "false", "fromSilence": true },
    { "kind": "constraint.maxExclusive", "element": "customer-discount", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Shop.Domain/Customer.cs:10" },
    { "kind": "constraint.minValue", "element": "customer-score", "outcome": "verified", "expected": "10", "actual": "10", "at": "src/Shop.Domain/Customer.cs:13" },
    { "kind": "constraint.maxValue", "element": "customer-score", "absent": true }
  ]
}
```

## contradicted

`Age` runs from 0 to 100, `Discount`'s minimum is exclusive and its maximum inclusive, and `Score` has no minimum, as `int.MinValue` says.

```json case
{ "phase": 4, "base": "verified", "kinds": ["constraint.minValue", "constraint.maxValue", "constraint.minExclusive", "constraint.maxExclusive"], "requirements": ["R1", "R3", "R4", "R14"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```csharp file src/Shop.Domain/Customer.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Customer
{
    [Range(0, 100)]
    public int Age { get; set; }

    [Range(typeof(decimal), "0", "1.0", MinimumIsExclusive = true)]
    public decimal Discount { get; set; }

    [Range(int.MinValue, 100)]
    public int Score { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.minValue", "element": "customer-age", "outcome": "contradicted", "expected": "1", "actual": "0", "at": "src/Shop.Domain/Customer.cs:7" },
    { "kind": "constraint.maxValue", "element": "customer-age", "outcome": "contradicted", "expected": "120", "actual": "100", "at": "src/Shop.Domain/Customer.cs:7" },
    { "kind": "constraint.minExclusive", "element": "customer-discount", "outcome": "contradicted", "expected": "false", "actual": "true", "at": "src/Shop.Domain/Customer.cs:10" },
    { "kind": "constraint.maxExclusive", "element": "customer-discount", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true },
    { "kind": "constraint.minValue", "element": "customer-score", "outcome": "contradicted", "expected": "10", "actual": null }
  ],
  "findings": [
    { "rule": "claim/constraint.minValue", "severity": "error", "subject": "customer-age",
      "messageContains": "`Customer.Age` minimum value constraint: model `1`, code `0`. The model states it in the attribute's Numeric Limits Min Value." },
    { "rule": "claim/constraint.maxExclusive", "severity": "error", "subject": "customer-discount", "messageContains": "Max Boundary Type" }
  ]
}
```

## unreadable

The bounds and a boundary are values the code computes.

```json case
{ "phase": 4, "base": "verified", "kinds": ["constraint.minValue", "constraint.maxValue", "constraint.minExclusive", "constraint.maxExclusive"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```csharp file src/Shop.Domain/Customer.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class Customer
{
    [Range(Limits.MinimumAge, Limits.MaximumAge)]
    public int Age { get; set; }

    [Range(typeof(decimal), "0", "1.0", MinimumIsExclusive = Limits.Open, MaximumIsExclusive = Limits.Exclusive)]
    public decimal Discount { get; set; }

    [Range(10, int.MaxValue)]
    public int Score { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.minValue", "element": "customer-age", "outcome": "cantVerify", "reason": "unreadable", "at": "src/Shop.Domain/Customer.cs:7" },
    { "kind": "constraint.maxValue", "element": "customer-age", "outcome": "cantVerify", "reason": "unreadable", "at": "src/Shop.Domain/Customer.cs:7" },
    { "kind": "constraint.minExclusive", "element": "customer-discount", "outcome": "cantVerify", "reason": "unreadable", "at": "src/Shop.Domain/Customer.cs:10" },
    { "kind": "constraint.maxExclusive", "element": "customer-discount", "outcome": "cantVerify", "reason": "unreadable", "at": "src/Shop.Domain/Customer.cs:10" }
  ]
}
```

## unmodelled-value

With `optin/unmodelled.value` on, a range the model leaves unset is reported: `Quantity`'s minimum and maximum, each where the code states it.

```json case
{ "phase": 4, "kinds": ["constraint.minValue", "constraint.maxValue"], "cases": ["optin/unmodelled.value"], "requirements": ["R49"], "packs": ["intent.core", "intent.domain", "intent.dataannotations"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-line", "type": "Class", "name": "OrderLine",
            "children": [
              { "id": "order-line-quantity", "type": "Attribute", "name": "Quantity", "typeRef": { "type": "int" } }
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

```csharp file src/Shop.Domain/OrderLine.cs
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain;

public class OrderLine
{
    [Range(1, 99)]
    public int Quantity { get; set; }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Shop.Domain/OrderLine.cs:7", "messageContains": "minimum value constraint: the code states `1`" },
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Shop.Domain/OrderLine.cs:7", "messageContains": "maximum value constraint: the code states `99`" }
  ]
}
```

## java

`@DecimalMin(value = "0.0", inclusive = false)` is an exclusive minimum, `@Max(100)` an inclusive maximum, `@PositiveOrZero` a minimum of 0, and Hibernate's `@Range(max = 50)` a minimum of 0 by default.

```json case
{ "phase": 4, "kinds": ["constraint.minValue", "constraint.maxValue", "constraint.minExclusive", "constraint.maxExclusive"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.beanvalidation"], "language": "java" }
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
              { "id": "product-price", "type": "Attribute", "name": "Price", "typeRef": { "type": "decimal" },
                "stereotypes": { "Numeric Limits": { "Min Value": "0", "Min Boundary Type": "Exclusive", "Max Value": "100", "Max Boundary Type": "Inclusive" } } },
              { "id": "product-stock", "type": "Attribute", "name": "Stock", "typeRef": { "type": "int" }, "stereotypes": { "Numeric Limits": { "Min Value": "0" } } },
              { "id": "product-rating", "type": "Attribute", "name": "Rating", "typeRef": { "type": "int" }, "stereotypes": { "Numeric Limits": { "Min Value": "1", "Max Value": "50" } } }
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

import java.math.BigDecimal;
import jakarta.validation.constraints.*;
import org.hibernate.validator.constraints.Range;

public class Product {
    @DecimalMin(value = "0.0", inclusive = false)
    @Max(100)
    private BigDecimal price;
    @PositiveOrZero
    private int stock;
    @Range(max = 50)
    private int rating;
}
```

```json expect
{
  "claims": [
    { "kind": "constraint.minValue", "element": "product-price", "outcome": "verified", "expected": "0", "actual": "0.0", "at": "src/main/java/com/shop/domain/Product.java:8" },
    { "kind": "constraint.minExclusive", "element": "product-price", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/main/java/com/shop/domain/Product.java:8" },
    { "kind": "constraint.maxValue", "element": "product-price", "outcome": "verified", "expected": "100", "actual": "100", "at": "src/main/java/com/shop/domain/Product.java:9" },
    { "kind": "constraint.maxExclusive", "element": "product-price", "outcome": "verified", "expected": "false", "actual": "false", "at": "src/main/java/com/shop/domain/Product.java:9" },
    { "kind": "constraint.minValue", "element": "product-stock", "outcome": "verified", "expected": "0", "actual": "0", "at": "src/main/java/com/shop/domain/Product.java:11" },
    { "kind": "constraint.minValue", "element": "product-rating", "outcome": "contradicted", "expected": "1", "actual": "0", "fromSilence": true },
    { "kind": "constraint.maxValue", "element": "product-rating", "outcome": "verified", "expected": "50", "actual": "50", "at": "src/main/java/com/shop/domain/Product.java:13" }
  ]
}
```
