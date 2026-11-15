# relationship.navigable

Whether an association's source end is navigable, against a back-reference field on the target class. A navigable end is a member, which `member.exists` finds in any language, so where the source end is navigable the claim waits on that end's `member.exists` and reads only how the framework maps the field it found: a missing back-reference is the member's one finding. Where the source end isn't navigable, the claim waits on the target end's, the navigation a back-reference would pair with, and reads that the target class has none: over JPA from `{Target}.java`, the navigation's `mappedBy` or a field typed as the source class. The claim is stated by the association, so its `element` is the association. `relationship.navigable` is `stated: always`: a target class with no field back to the source states that the source end isn't navigable.

## verified

`Order` owns its lines and each line refers back to its order: the source end is navigable. `OrderLine.java` has the back-reference field `order`, mapped by the order's `@OneToMany(mappedBy = "order")` (JAV-DOM-26).

```json case
{ "phase": 4, "kinds": ["relationship.navigable"], "traces": ["JAV-DOM-26"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
          "source": { "element": "order", "name": "Order", "multiplicity": "1", "navigable": true },
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

    @OneToMany(mappedBy = "order", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<OrderLine> lines = new ArrayList<>();
}
```

```java file src/main/java/com/shop/domain/OrderLine.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import java.util.UUID;

@Entity
public class OrderLine {
    @Id
    private UUID id;

    @ManyToOne(optional = false)
    @JoinColumn(name = "order_id")
    private Order order;
}
```

```json expect
{
  "claims": [
    { "kind": "relationship.navigable", "element": "order-lines", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/main/java/com/shop/domain/OrderLine.java:14" }
  ]
}
```

## contradicted-missing-back-reference

The model says a line navigates to its order, but `OrderLine` has no `Order` field: the relationship is mapped from the order's side only, with a join column. The navigable source end is a member, so `member.exists` finds it missing, and that is the one finding (JAV-DOM-26): the navigability claim waits on it and is blocked, counted in the member's finding, rather than a second finding for the same missing field.

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.navigable", "member.exists"], "cases": ["engine/blocked"], "traces": ["JAV-DOM-26"], "requirements": ["R1", "R5", "R8"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
    { "kind": "relationship.navigable", "element": "order-lines", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "member.exists" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "messageContains": "isn't in the code" },
    { "rule": "claim/relationship.navigable", "count": 0, "note": "A blocked claim is counted in the missing member's finding, not reported apart" }
  ]
}
```

## verified-not-navigable

The model's source end isn't navigable, and `OrderLine` has no field back to its order. The class saying nothing states it, so the claim is verified from silence (R3).

```json case
{ "phase": 4, "base": "contradicted-missing-back-reference", "kinds": ["relationship.navigable"], "traces": ["JAV-DOM-26"], "requirements": ["R3"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
    { "kind": "relationship.navigable", "element": "order-lines", "outcome": "verified", "expected": "false", "actual": "false", "fromSilence": true,
      "at": "src/main/java/com/shop/domain/OrderLine.java:7" }
  ]
}
```

## contradicted-unmodelled-back-reference

The other way round: the model's source end isn't navigable, but `OrderLine` has a `@ManyToOne` field back to its order. The code states the end is navigable, so the claim is contradicted (JAV-DOM-26, both ways).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.navigable"], "traces": ["JAV-DOM-26"], "requirements": ["R1", "R14"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
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
    { "kind": "relationship.navigable", "element": "order-lines", "outcome": "contradicted", "expected": "false", "actual": "true",
      "at": "src/main/java/com/shop/domain/OrderLine.java:14" }
  ],
  "findings": [
    { "rule": "claim/relationship.navigable", "severity": "error", "subject": "order-lines", "messageContains": "code `true`" }
  ]
}
```

## cant-verify-no-rule

The Java reader is active but the JPA pack isn't, so no rule reads a back-reference. The claim is can't verify, `noRule`, one error, never a silent skip (principle 2).

```json case
{ "phase": 4, "base": "verified", "kinds": ["relationship.navigable"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```json expect
{
  "claims": [
    { "kind": "relationship.navigable", "element": "order-lines", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/relationship.navigable", "severity": "error", "subject": "order-lines", "messageContains": "no rule" }
  ]
}
```
