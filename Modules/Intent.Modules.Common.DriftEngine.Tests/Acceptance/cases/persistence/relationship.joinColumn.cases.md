# relationship.joinColumn

The Foreign Key stereotype's Column Name on an association end, against the `@JoinColumn(name = ...)` on the JPA field that owns the relationship. A JPA relationship may have no foreign key field for `relationship.foreignKey` to name, so the join column is its own kind. The claim is stated by the association, so its `element` is the association. `relationship.joinColumn` is `stated: whenSet`: a modelled column name the code doesn't state is contradicted, never filled in from JPA's `<field>_id` default (decision 4).

## verified

The `Lines` end's Foreign Key names the column `order_id`, and the owning field `lines` in `Order.java` has `@JoinColumn(name = "order_id")` (JAV-DOM-27).

```json case
{ "phase": 4, "kinds": ["relationship.joinColumn"], "traces": ["JAV-DOM-27"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "order-line", "type": "Class", "name": "OrderLine",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-line-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-lines", "type": "Association",
          "source": { "element": "order", "name": "Order", "multiplicity": "1", "navigable": false },
          "target": { "element": "order-line", "name": "Lines", "multiplicity": "*", "navigable": true,
            "stereotypes": { "Foreign Key": { "Column Name": "order_id" } } } }
      ]
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

import jakarta.persistence.CascadeType;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToMany;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    @OneToMany(cascade = CascadeType.ALL, orphanRemoval = true)
    @JoinColumn(name = "order_id", nullable = false)
    private List<OrderLine> lines = new ArrayList<>();
}
```

```java file src/main/java/com/shop/domain/OrderLine.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class OrderLine {
    @Id
    private UUID id;
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.joinColumn", "element": "order-lines", "outcome": "verified", "expected": "order_id", "actual": "order_id",
      "at": "src/main/java/com/shop/domain/Order.java:18" }
  ]
}
```

## contradicted

The model names the column `lines_id`, after the end, and the code's `@JoinColumn` names `order_id`. The finding names the end's Foreign Key Column Name as the model property that states it (R14).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.joinColumn"], "traces": ["JAV-DOM-27"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "order-line", "type": "Class", "name": "OrderLine",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-line-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-lines", "type": "Association",
          "source": { "element": "order", "name": "Order", "multiplicity": "1", "navigable": false },
          "target": { "element": "order-line", "name": "Lines", "multiplicity": "*", "navigable": true,
            "stereotypes": { "Foreign Key": { "Column Name": "lines_id" } } } }
      ]
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
    { "kind": "relationship.joinColumn", "element": "order-lines", "outcome": "contradicted", "expected": "lines_id", "actual": "order_id",
      "at": "src/main/java/com/shop/domain/Order.java:18" }
  ],
  "findings": [
    { "rule": "claim/relationship.joinColumn", "severity": "error", "subject": "order-lines", "messageContains": "code `order_id`" }
  ]
}
```

## contradicted-not-stated

The field has no `@JoinColumn`, so JPA maps the relationship by its own default. The code doesn't state a join column, so the modelled `order_id` is contradicted, with the code side "not stated" (decision 4).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.joinColumn"], "traces": ["JAV-DOM-27"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;

import jakarta.persistence.CascadeType;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.OneToMany;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    @OneToMany(cascade = CascadeType.ALL, orphanRemoval = true)
    private List<OrderLine> lines = new ArrayList<>();
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.joinColumn", "element": "order-lines", "outcome": "contradicted", "expected": "order_id", "actual": null, "fromSilence": false }
  ],
  "findings": [
    { "rule": "claim/relationship.joinColumn", "severity": "error", "subject": "order-lines", "messageContains": "doesn't state" }
  ]
}
```

## not-claimed-without-column-name

The end has no Foreign Key stereotype, though the code names `order_id`. An unset `whenSet` value is no claim, so nothing is checked and nothing is reported (decision 10, principle 1).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.joinColumn"], "requirements": ["R4"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "order-line", "type": "Class", "name": "OrderLine",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "order-line-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-lines", "type": "Association",
          "source": { "element": "order", "name": "Order", "multiplicity": "1", "navigable": false },
          "target": { "element": "order-line", "name": "Lines", "multiplicity": "*", "navigable": true } }
      ]
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
    { "kind": "relationship.joinColumn", "element": "order-lines", "absent": true }
  ],
  "findings": [
    { "rule": "claim/relationship.joinColumn", "count": 0 }
  ]
}
```

## verified-constant

The join column's name is a constant the code declares, `Columns.ORDER_ID`, not a literal. A constant the code declares states its value as a literal would, so the claim is verified.

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.joinColumn"], "traces": ["JAV-DOM-27"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;

import jakarta.persistence.CascadeType;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToMany;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    @OneToMany(cascade = CascadeType.ALL, orphanRemoval = true)
    @JoinColumn(name = Columns.ORDER_ID, nullable = false)
    private List<OrderLine> lines = new ArrayList<>();
}
```

```java file src/main/java/com/shop/domain/Columns.java
package com.shop.domain;

final class Columns {
    static final String ORDER_ID = "order_id";

    private Columns() {
    }
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.joinColumn", "element": "order-lines", "outcome": "verified", "expected": "order_id", "actual": "order_id",
      "at": "src/main/java/com/shop/domain/Order.java:18" }
  ]
}
```

## cant-verify-unreadable

The join column's name is a constant, `Columns.ORDER_ID`, that no in-scope file declares: `Columns` is generated at build time. The code does state a name, so the claim isn't "not stated", but the rule can't read its value without the code that declares it: the claim is can't verify, `unreadable`, at the annotation.

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.joinColumn"], "cases": ["engine/unreadable"], "traces": ["JAV-DOM-27"], "requirements": ["R5", "R11"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;

import jakarta.persistence.CascadeType;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToMany;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    @OneToMany(cascade = CascadeType.ALL, orphanRemoval = true)
    @JoinColumn(name = Columns.ORDER_ID, nullable = false)
    private List<OrderLine> lines = new ArrayList<>();
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.joinColumn", "element": "order-lines", "outcome": "cantVerify", "reason": "unreadable",
      "at": "src/main/java/com/shop/domain/Order.java:18" }
  ],
  "findings": [
    { "rule": "claim/relationship.joinColumn", "severity": "error", "subject": "order-lines", "messageContains": "can't read" }
  ]
}
```
