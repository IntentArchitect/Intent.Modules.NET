# column.version

Whether an attribute of a persisted class is its optimistic concurrency token. The Java spike's own module adds a Version stereotype for it, which no stock pack knows, so these cases give its vocabulary in a house pack: `true` for an attribute with Version, `false` for every other attribute of a persisted class. JPA states it with `@Version` (JAV-DOM-19). `column.version` is `stated: always`: JPA never makes a field a version unless it is annotated, so a field without `@Version` really states `false`, and the reading from silence is honest (R3).

## verified

The Java spike's `StockItem` has a `version` field marked `@Version`, and the model gives its `Version` attribute the Version stereotype.

```json case
{ "phase": 4, "kinds": ["column.version"], "traces": ["JAV-DOM-19"], "requirements": ["R2", "R29", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa", "house.eshopjava"], "language": "java" }
```

```json pack
{
  "id": "house.eshopjava",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }, { "id": "intent.jpa", "version": "^1.0.0" }],
  "languages": ["java"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "entries": [
        {
          "kind": "column.version",
          "for": { "element": "Attribute", "where": "parent.hasStereotype('Table')" },
          "value": "hasStereotype('Version')",
          "bindings": { "Entity": "parent.name", "Attribute": "name" },
          "own": ["Attribute"],
          "dependsOn": "member.exists",
          "source": "the Version stereotype"
        }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Catalog",
        "elements": [
          { "id": "stock-item", "type": "Class", "name": "StockItem",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "stock-item-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } },
              { "id": "stock-item-version", "type": "Attribute", "name": "Version", "typeRef": { "type": "int" }, "stereotypes": { "Version": {} } },
              { "id": "stock-item-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } }
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

```java file src/main/java/com/shop/catalog/StockItem.java
package com.shop.catalog;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Version;
import java.util.UUID;

@Entity
public class StockItem {
    @Id
    private UUID id;

    @Version
    private int version;

    private String name;
}
```

```json expect
{
  "claims": [
    { "kind": "column.version", "element": "stock-item-version", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/main/java/com/shop/catalog/StockItem.java:13" }
  ]
}
```

## verified-from-silence

`Name` has no Version stereotype, so the model says it isn't the version, and its field has no `@Version`: verified from silence (R3).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.version"], "cases": ["engine/silence"], "traces": ["JAV-DOM-19"], "requirements": ["R2", "R3", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa", "house.eshopjava"], "language": "java" }
```

```json expect
{
  "claims": [
    { "kind": "column.version", "element": "stock-item-name", "outcome": "verified", "expected": "false", "actual": "false", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/column.version", "subject": "stock-item-name", "count": 0 }
  ]
}
```

## contradicted-not-annotated

The model marks `Version` with the Version stereotype, and the field has no `@Version`. JPA then doesn't check concurrency on `StockItem` at all; the field's silence states `false`, and the claim is contradicted from silence.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.version"], "cases": ["engine/silence"], "traces": ["JAV-DOM-19"], "requirements": ["R1", "R3", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa", "house.eshopjava"], "language": "java" }
```

```java file src/main/java/com/shop/catalog/StockItem.java
package com.shop.catalog;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class StockItem {
    @Id
    private UUID id;

    private int version;

    private String name;
}
```

```json expect
{
  "claims": [
    { "kind": "column.version", "element": "stock-item-version", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/column.version", "severity": "error", "subject": "stock-item-version", "messageContains": "Version stereotype" }
  ]
}
```

## contradicted-unmodelled

The field has `@Version`, and the model's `Version` attribute lacks the stereotype. Because the kind is `always`, the model's silence says the field isn't the version, so the code contradicts it: JAV-DOM-19 checked both ways.

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.version"], "traces": ["JAV-DOM-19"], "requirements": ["R1", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa", "house.eshopjava"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Catalog",
        "elements": [
          { "id": "stock-item", "type": "Class", "name": "StockItem",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "stock-item-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } },
              { "id": "stock-item-version", "type": "Attribute", "name": "Version", "typeRef": { "type": "int" } },
              { "id": "stock-item-name", "type": "Attribute", "name": "Name", "typeRef": { "type": "string" } }
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

```json expect
{
  "claims": [
    { "kind": "column.version", "element": "stock-item-version", "outcome": "contradicted", "expected": "false", "actual": "true",
      "at": "src/main/java/com/shop/catalog/StockItem.java:13" }
  ]
}
```

## blocked-when-member-missing

`StockItem` has no `version` field at all. `member.exists` is contradicted, and the version claim, which depends on it, is can't verify, `blocked`, counted in the missing member's one finding (R5).

```json case
{ "phase": 4, "base": "verified", "kinds": ["column.version", "member.exists"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa", "house.eshopjava"], "language": "java" }
```

```java file src/main/java/com/shop/catalog/StockItem.java
package com.shop.catalog;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class StockItem {
    @Id
    private UUID id;

    private String name;
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "stock-item-version", "outcome": "contradicted" },
    { "kind": "column.version", "element": "stock-item-version", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "member.exists" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "stock-item-version" },
    { "rule": "claim/column.version", "count": 0 }
  ]
}
```
