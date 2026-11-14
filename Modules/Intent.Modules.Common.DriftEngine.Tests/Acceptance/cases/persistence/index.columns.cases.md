# index.columns

The columns of each index the Index stereotypes on a class's attributes give, in `Order`, against the index the code declares. Attributes sharing a `UniqueKey` form one named index; an Index with no `UniqueKey` is an unnamed index, and is a claim too (decision 9). One class can have several indexes, so each claim's `key` is its discriminator: the index's name, or for an unnamed one its columns. `index.columns` is `stated: always`: a modelled index the code doesn't declare is contradicted from silence, and the `list` comparer wants the same columns in the same order: column order is part of an index (claim-catalog.md, Notes on kinds), so reordered columns are contradicted, where JAV-DOM-30 compared column sets. A list's canonical text joins its items with ", ".

## verified

An unnamed unique index on `Buyer.IdentityGuid`, and the configuration's `HasIndex("IdentityGuid")`, as eShop's buyer configuration has it (ESC-DOM-48). Common.Drift stated only named indexes, so this claim is new (conversion.md, semantic change 5).

```json case
{ "phase": 4, "kinds": ["index.columns"], "traces": ["ESC-DOM-48"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "buyer-identity-guid", "type": "Attribute", "name": "IdentityGuid", "typeRef": { "type": "string" },
                "stereotypes": { "Index": { "IsUnique": true } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Buyer.cs
namespace Ordering.Domain;

public class Buyer
{
    public int Id { get; private set; }

    public string IdentityGuid { get; private set; } = string.Empty;
}
```

```csharp file src/Infrastructure/Persistence/BuyerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.HasIndex("IdentityGuid").IsUnique(true);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "index.columns", "element": "buyer", "key": "IdentityGuid", "outcome": "verified", "expected": "IdentityGuid", "actual": "IdentityGuid",
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11" }
  ]
}
```

## verified-named

The same index, named: the Index's `UniqueKey` is `IX_Buyers_IdentityGuid`, and the configuration gives the index that name with `HasDatabaseName`. The lambda form `HasIndex(b => b.IdentityGuid)` reads as the string form does (ESC-DOM-48).

```json case
{ "phase": 4, "base": "verified", "kinds": ["index.columns"], "traces": ["ESC-DOM-48"], "requirements": ["R2", "R9"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "buyer-identity-guid", "type": "Attribute", "name": "IdentityGuid", "typeRef": { "type": "string" },
                "stereotypes": { "Index": { "UniqueKey": "IX_Buyers_IdentityGuid", "Order": 1, "IsUnique": true } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Infrastructure/Persistence/BuyerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.HasIndex(b => b.IdentityGuid).HasDatabaseName("IX_Buyers_IdentityGuid").IsUnique();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "index.columns", "element": "buyer", "key": "IX_Buyers_IdentityGuid", "outcome": "verified", "expected": "IdentityGuid", "actual": "IdentityGuid",
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11" }
  ]
}
```

## contradicted-missing-index

The configuration declares no index. Index columns are stated always, so the code's silence states that there's no such index, and the modelled one is contradicted from silence (ESC-DOM-48, model to code).

```json case
{ "phase": 4, "base": "verified", "kinds": ["index.columns"], "traces": ["ESC-DOM-48"], "requirements": ["R1", "R3", "R11"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Infrastructure/Persistence/BuyerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.HasKey(b => b.Id);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "index.columns", "element": "buyer", "key": "IdentityGuid", "outcome": "contradicted", "expected": "IdentityGuid", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/index.columns", "severity": "error", "subject": "buyer" }
  ]
}
```

## contradicted-named-columns

The index named `IX_Buyers_IdentityGuid` is declared on `Name` instead. The name finds the index, and its columns differ, so the claim is contradicted and the finding shows both lists (R14).

```json case
{ "phase": 4, "base": "verified-named", "kinds": ["index.columns"], "traces": ["ESC-DOM-48"], "requirements": ["R1", "R10", "R14"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Domain/Buyer.cs
namespace Ordering.Domain;

public class Buyer
{
    public int Id { get; private set; }

    public string IdentityGuid { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;
}
```

```csharp file src/Infrastructure/Persistence/BuyerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Persistence;

public class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> builder)
    {
        builder.HasIndex(b => b.Name).HasDatabaseName("IX_Buyers_IdentityGuid").IsUnique();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "index.columns", "element": "buyer", "key": "IX_Buyers_IdentityGuid", "outcome": "contradicted", "expected": "IdentityGuid", "actual": "Name",
      "at": "src/Infrastructure/Persistence/BuyerConfiguration.cs:11" }
  ],
  "findings": [
    { "rule": "claim/index.columns", "severity": "error", "subject": "buyer", "messageContains": "code `Name`" }
  ]
}
```

## no-rule

Only the core and Domain packs are active, so nothing reads an index. The modelled index is can't verify, `noRule`, one error, never a silent skip (principle 2).

```json case
{ "phase": 1, "base": "verified", "kinds": ["index.columns"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "index.columns", "element": "buyer", "key": "IdentityGuid", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/index.columns", "severity": "error", "subject": "buyer", "messageContains": "no rule" }
  ]
}
```

## java-verified-unique-column

Over JPA, an unnamed unique index on one attribute is `@Column(unique = true)` on its field (JAV-DOM-30).

```json case
{ "phase": 4, "kinds": ["index.columns"], "traces": ["JAV-DOM-30"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "customer", "type": "Class", "name": "Customer",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "customer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } },
              { "id": "customer-email", "type": "Attribute", "name": "Email", "typeRef": { "type": "string" },
                "stereotypes": { "Index": { "IsUnique": true } } }
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

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class Customer {
    @Id
    private UUID id;

    @Column(nullable = false, unique = true)
    private String email;
}
```

```json expect
{
  "claims": [
    { "kind": "index.columns", "element": "customer", "key": "Email", "outcome": "verified", "expected": "Email", "actual": "Email",
      "at": "src/main/java/com/shop/domain/Customer.java:13" }
  ]
}
```

## java-verified-unique-constraint

A two-column unique index, `UQ_WishlistItem_Product` on `WishlistId` then `ProductId`, as a `@UniqueConstraint` in `@Table`. The constraint lists column names, which the rule maps back to the attributes through each field's `@Column(name)` (JAV-DOM-30).

```json case
{ "phase": 4, "kinds": ["index.columns"], "traces": ["JAV-DOM-30"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "wishlist-item", "type": "Class", "name": "WishlistItem",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "wishlist-item-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } },
              { "id": "wishlist-item-wishlist-id", "type": "Attribute", "name": "WishlistId", "typeRef": { "type": "guid" },
                "stereotypes": { "Index": { "UniqueKey": "UQ_WishlistItem_Product", "Order": 1, "IsUnique": true } } },
              { "id": "wishlist-item-product-id", "type": "Attribute", "name": "ProductId", "typeRef": { "type": "guid" },
                "stereotypes": { "Index": { "UniqueKey": "UQ_WishlistItem_Product", "Order": 2, "IsUnique": true } } }
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

```java file src/main/java/com/shop/domain/WishlistItem.java
package com.shop.domain;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import jakarta.persistence.UniqueConstraint;
import java.util.UUID;

@Entity
@Table(uniqueConstraints = @UniqueConstraint(name = "UQ_WishlistItem_Product", columnNames = {"wishlist_id", "product_id"}))
public class WishlistItem {
    @Id
    private UUID id;

    @Column(name = "wishlist_id", nullable = false)
    private UUID wishlistId;

    @Column(name = "product_id", nullable = false)
    private UUID productId;
}
```

```json expect
{
  "claims": [
    { "kind": "index.columns", "element": "wishlist-item", "key": "UQ_WishlistItem_Product", "outcome": "verified",
      "expected": "WishlistId, ProductId", "actual": "WishlistId, ProductId",
      "at": "src/main/java/com/shop/domain/WishlistItem.java:11" }
  ]
}
```

## java-contradicted-columns

The constraint covers only `wishlist_id`, so a customer could not add two products to one wishlist. The model's index has two columns, so the claim is contradicted (JAV-DOM-30, both ways).

```json case
{ "phase": 4, "base": "java-verified-unique-constraint", "kinds": ["index.columns"], "traces": ["JAV-DOM-30"], "requirements": ["R1", "R10", "R14"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/WishlistItem.java
package com.shop.domain;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import jakarta.persistence.UniqueConstraint;
import java.util.UUID;

@Entity
@Table(uniqueConstraints = @UniqueConstraint(name = "UQ_WishlistItem_Product", columnNames = {"wishlist_id"}))
public class WishlistItem {
    @Id
    private UUID id;

    @Column(name = "wishlist_id", nullable = false)
    private UUID wishlistId;

    @Column(name = "product_id", nullable = false)
    private UUID productId;
}
```

```json expect
{
  "claims": [
    { "kind": "index.columns", "element": "wishlist-item", "key": "UQ_WishlistItem_Product", "outcome": "contradicted",
      "expected": "WishlistId, ProductId", "actual": "WishlistId",
      "at": "src/main/java/com/shop/domain/WishlistItem.java:11" }
  ],
  "findings": [
    { "rule": "claim/index.columns", "severity": "error", "subject": "wishlist-item", "messageContains": "code `WishlistId`" }
  ]
}
```

## java-contradicted-column-order

The constraint lists both columns, but `product_id` first. Column order is part of an index, so the same columns in another order are contradicted (claim-catalog.md, Notes on kinds). JAV-DOM-30 compared the columns as a set and would have passed this; the ordered comparison is a deliberate tightening (JAV-DOM-30).

```json case
{ "phase": 4, "base": "java-verified-unique-constraint", "kinds": ["index.columns"], "traces": ["JAV-DOM-30"], "requirements": ["R1", "R10", "R14"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/WishlistItem.java
package com.shop.domain;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import jakarta.persistence.UniqueConstraint;
import java.util.UUID;

@Entity
@Table(uniqueConstraints = @UniqueConstraint(name = "UQ_WishlistItem_Product", columnNames = {"product_id", "wishlist_id"}))
public class WishlistItem {
    @Id
    private UUID id;

    @Column(name = "wishlist_id", nullable = false)
    private UUID wishlistId;

    @Column(name = "product_id", nullable = false)
    private UUID productId;
}
```

```json expect
{
  "claims": [
    { "kind": "index.columns", "element": "wishlist-item", "key": "UQ_WishlistItem_Product", "outcome": "contradicted",
      "expected": "WishlistId, ProductId", "actual": "ProductId, WishlistId",
      "at": "src/main/java/com/shop/domain/WishlistItem.java:11" }
  ],
  "findings": [
    { "rule": "claim/index.columns", "severity": "error", "subject": "wishlist-item", "messageContains": "code `ProductId, WishlistId`" }
  ]
}
```

## java-verified-association-column

The index's first column is a one-to-many's join column: `Wishlist.items` owns `wishlist_id`, in WishlistItem's table, and WishlistItem has no field for it. Intent's RDBMS stereotypes put that column in the index with the Index stereotype on the association's target end, `Items`, so the index is on the class whose table holds the column, and the end's column is its Foreign Key Column Name. The constraint has no name, so the rule finds it by its columns (the Java eShop's `WishlistItem`).

```json case
{ "phase": 4, "kinds": ["index.columns"], "traces": ["JAV-DOM-30"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "wishlist", "type": "Class", "name": "Wishlist",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "wishlist-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "wishlist-item", "type": "Class", "name": "WishlistItem",
            "stereotypes": { "Table": {} },
            "children": [
              { "id": "wishlist-item-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } },
              { "id": "wishlist-item-product-id", "type": "Attribute", "name": "ProductId", "typeRef": { "type": "guid" },
                "stereotypes": { "Column": { "Name": "product_id" }, "Index": { "UniqueKey": "UX_WishlistItem_Wishlist_Product", "Order": 2, "IsUnique": true } } }
            ] }
        ]
      }],
      "associations": [
        { "id": "wishlist-items", "type": "Association",
          "source": { "element": "wishlist", "name": "Wishlist", "multiplicity": "1", "navigable": false },
          "target": { "element": "wishlist-item", "name": "Items", "multiplicity": "*", "navigable": true,
            "stereotypes": { "Foreign Key": { "Column Name": "wishlist_id" }, "Index": { "UniqueKey": "UX_WishlistItem_Wishlist_Product", "Order": 1, "IsUnique": true } } } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/shop/domain/Wishlist.java
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
public class Wishlist {
    @Id
    private UUID id;

    @OneToMany(cascade = CascadeType.ALL, orphanRemoval = true)
    @JoinColumn(name = "wishlist_id", nullable = false)
    private List<WishlistItem> items = new ArrayList<>();
}
```

```java file src/main/java/com/shop/domain/WishlistItem.java
package com.shop.domain;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import jakarta.persistence.UniqueConstraint;
import java.util.UUID;

@Entity
@Table(uniqueConstraints = @UniqueConstraint(columnNames = {"wishlist_id", "product_id"}))
public class WishlistItem {
    @Id
    private UUID id;

    @Column(name = "product_id", nullable = false)
    private UUID productId;
}
```

```json expect
{
  "claims": [
    { "kind": "index.columns", "element": "wishlist-item", "key": "UX_WishlistItem_Wishlist_Product", "outcome": "verified",
      "expected": "wishlist_id, ProductId", "actual": "wishlist_id, ProductId",
      "at": "src/main/java/com/shop/domain/WishlistItem.java:11" }
  ]
}
```

## java-contradicted-association-column

The constraint covers only `product_id`, so a product could be in only one wishlist. The model's index starts with the `Items` end's `wishlist_id`, so the claim is contradicted from silence: no declared index has the model's columns (JAV-DOM-30).

```json case
{ "phase": 4, "base": "java-verified-association-column", "kinds": ["index.columns"], "traces": ["JAV-DOM-30"], "requirements": ["R1", "R3", "R10"], "packs": ["intent.core", "intent.domain", "intent.java", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/shop/domain/WishlistItem.java
package com.shop.domain;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import jakarta.persistence.UniqueConstraint;
import java.util.UUID;

@Entity
@Table(uniqueConstraints = @UniqueConstraint(columnNames = {"product_id"}))
public class WishlistItem {
    @Id
    private UUID id;

    @Column(name = "product_id", nullable = false)
    private UUID productId;
}
```

```json expect
{
  "claims": [
    { "kind": "index.columns", "element": "wishlist-item", "key": "UX_WishlistItem_Wishlist_Product", "outcome": "contradicted",
      "expected": "wishlist_id, ProductId", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/index.columns", "severity": "error", "subject": "wishlist-item" }
  ]
}
```
