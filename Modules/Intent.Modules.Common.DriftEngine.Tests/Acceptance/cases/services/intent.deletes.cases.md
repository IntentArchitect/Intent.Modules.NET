# intent.deletes

A `Delete Entity Action` from an Operation to a Domain class, against the code: the Operation's handler reaches the class's repository (or its owner) and deletes through it, `repo.delete…`. The claim is the association's. `intent.deletes` is an existence kind: a modelled interaction the code lacks is contradicted (R8).

The fixtures adapt the Java spike's `OrdersController`, `Order` and `OrderRepository`.

## verified

`DeleteOrder` deletes an `Order` through `OrderRepository.deleteById`. Traces JAV-SVC-15 (Delete ↔ `repo.delete…`).

```json case
{ "phase": 4, "kinds": ["intent.deletes"], "traces": ["JAV-SVC-15"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java", "intent.springmvc", "intent.jpa"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "eShop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-status", "type": "Attribute", "name": "Status", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Backend",
        "elements": [
          { "id": "orders-controller", "type": "Service", "name": "OrdersController",
            "children": [
              { "id": "delete-order", "type": "Operation", "name": "DeleteOrder",
                "stereotypes": { "Http Settings": { "Verb": "DELETE", "Route": "/api/orders/{orderId}" } },
                "children": [
                  { "id": "delete-order-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "guid" } }
                ] }
            ] }
        ]
      }],
      "associations": [
        { "id": "delete-order-deletes-order", "type": "Delete Entity Action",
          "source": { "element": "delete-order" }, "target": { "element": "order" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/eshop/backend/domain/Order.java
package com.eshop.backend.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;

import java.util.UUID;

@Entity
public class Order {
    @Id
    private UUID id;

    private String status;

    public void cancel() {
        this.status = "Cancelled";
    }
}
```

```java file src/main/java/com/eshop/backend/domain/OrderRepository.java
package com.eshop.backend.domain;

import org.springframework.data.jpa.repository.JpaRepository;

import java.util.UUID;

public interface OrderRepository extends JpaRepository<Order, UUID> {
}
```

```java file src/main/java/com/eshop/backend/api/OrdersController.java
package com.eshop.backend.api;

import com.eshop.backend.domain.OrderRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
public class OrdersController {
    private final OrderRepository orders;

    public OrdersController(OrderRepository orders) {
        this.orders = orders;
    }

    @DeleteMapping("/api/orders/{orderId}")
    public ResponseEntity<Void> deleteOrder(@PathVariable UUID orderId) {
        orders.deleteById(orderId);
        return ResponseEntity.noContent().build();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.deletes", "element": "delete-order-deletes-order", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:21" }
  ]
}
```

## contradicted-soft-delete

The controller cancels the `Order` and saves it instead of deleting it: an update, not a delete, so the modelled delete isn't in the code. Traces JAV-SVC-15.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.deletes"], "traces": ["JAV-SVC-15"], "requirements": ["R8", "R33"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java", "intent.springmvc", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/api/OrdersController.java
package com.eshop.backend.api;

import com.eshop.backend.domain.Order;
import com.eshop.backend.domain.OrderRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
public class OrdersController {
    private final OrderRepository orders;

    public OrdersController(OrderRepository orders) {
        this.orders = orders;
    }

    @DeleteMapping("/api/orders/{orderId}")
    public ResponseEntity<Void> deleteOrder(@PathVariable UUID orderId) {
        Order order = orders.findById(orderId).orElseThrow();
        order.cancel();
        orders.save(order);
        return ResponseEntity.noContent().build();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.deletes", "element": "delete-order-deletes-order", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.deletes", "severity": "error", "subject": "delete-order-deletes-order", "messageContains": "isn't in the code" }
  ]
}
```

## no-rule

Without the Spring MVC and JPA packs, no active pack has a rule for entity actions over Java, so the claim is can't verify, `noRule`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.deletes"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java"], "language": "java" }
```

```json expect
{
  "claims": [
    { "kind": "intent.deletes", "element": "delete-order-deletes-order", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/intent.deletes", "severity": "error", "subject": "delete-order-deletes-order", "messageContains": "no rule" }
  ]
}
```

## java-owned-item-removed

`RemoveWishlistItem` deletes a `WishlistItem` by removing it from its `Wishlist`'s items, which JPA deletes as an orphan. The wishlist's load isn't typed, but `getItems()` is the code's own method, returning a list of `WishlistItem`, so the removal is of one.

```json case
{ "phase": 4, "kinds": ["intent.deletes"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java", "intent.springmvc", "intent.jpa"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "eShop.Domain",
        "elements": [
          { "id": "wishlist", "type": "Class", "name": "Wishlist",
            "children": [
              { "id": "wishlist-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "wishlist-item", "type": "Class", "name": "WishlistItem",
            "children": [
              { "id": "wishlist-item-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } },
              { "id": "wishlist-item-product-id", "type": "Attribute", "name": "ProductId", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Backend",
        "elements": [
          { "id": "wishlists-controller", "type": "Service", "name": "WishlistsController",
            "children": [
              { "id": "remove-wishlist-item", "type": "Operation", "name": "RemoveWishlistItem",
                "stereotypes": { "Http Settings": { "Verb": "DELETE", "Route": "/api/wishlists/{customerId}/items/{productId}" } },
                "children": [
                  { "id": "remove-wishlist-item-customer-id", "type": "Parameter", "name": "customerId", "typeRef": { "type": "guid" } },
                  { "id": "remove-wishlist-item-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "guid" } }
                ] }
            ] }
        ]
      }],
      "associations": [
        { "id": "remove-wishlist-item-deletes-item", "type": "Delete Entity Action",
          "source": { "element": "remove-wishlist-item" }, "target": { "element": "wishlist-item" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/eshop/backend/domain/Wishlist.java
package com.eshop.backend.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.OneToMany;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

@Entity
public class Wishlist {
    @Id
    private UUID id;

    @OneToMany(orphanRemoval = true)
    private List<WishlistItem> items = new ArrayList<>();

    public List<WishlistItem> getItems() {
        return items;
    }
}
```

```java file src/main/java/com/eshop/backend/domain/WishlistItem.java
package com.eshop.backend.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;

import java.util.UUID;

@Entity
public class WishlistItem {
    @Id
    private UUID id;

    private UUID productId;

    public UUID getProductId() {
        return productId;
    }
}
```

```java file src/main/java/com/eshop/backend/domain/WishlistRepository.java
package com.eshop.backend.domain;

import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;
import java.util.UUID;

public interface WishlistRepository extends JpaRepository<Wishlist, UUID> {
    Optional<Wishlist> findByCustomerId(UUID customerId);
}
```

```java file src/main/java/com/eshop/backend/api/WishlistsController.java
package com.eshop.backend.api;

import com.eshop.backend.domain.WishlistRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
public class WishlistsController {
    private final WishlistRepository wishlists;

    public WishlistsController(WishlistRepository wishlists) {
        this.wishlists = wishlists;
    }

    @Transactional
    @DeleteMapping("/api/wishlists/{customerId}/items/{productId}")
    public ResponseEntity<Void> removeWishlistItem(@PathVariable UUID customerId, @PathVariable UUID productId) {
        var wishlist = wishlists.findByCustomerId(customerId).orElseThrow();
        var item = wishlist.getItems().stream().filter(i -> i.getProductId().equals(productId)).findFirst().orElseThrow();
        wishlist.getItems().remove(item);
        return ResponseEntity.noContent().build();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.deletes", "element": "remove-wishlist-item-deletes-item", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/main/java/com/eshop/backend/api/WishlistsController.java:25" }
  ]
}
```

## generic-repository-loop

`DeleteAd` deletes an `Ad` and its `AdMember`s through a generic repository whose `Delete` takes any entity. Nothing types what each call deletes, so each is read from where its value comes from: `ad` is loaded with `Find<Ad>`, and `member` is each item of the `Find<AdMember>` list the loop runs over.

```json case
{ "phase": 4, "kinds": ["intent.deletes"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ads.Domain",
        "elements": [
          { "id": "ad", "type": "Class", "name": "Ad",
            "children": [
              { "id": "ad-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] },
          { "id": "ad-member", "type": "Class", "name": "AdMember",
            "children": [
              { "id": "ad-member-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } },
              { "id": "ad-member-ad-id", "type": "Attribute", "name": "AdId", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Services": {
      "packages": [{
        "id": "services", "name": "Ads.Services",
        "elements": [
          { "id": "ads-service", "type": "Service", "name": "AdsService",
            "children": [
              { "id": "delete-ad", "type": "Operation", "name": "DeleteAd", "typeRef": { "type": "guid" },
                "children": [
                  { "id": "delete-ad-id", "type": "Parameter", "name": "id", "typeRef": { "type": "guid" } }
                ] }
            ] }
        ]
      }],
      "associations": [
        { "id": "delete-ad-deletes-ad", "type": "Delete Entity Action",
          "source": { "element": "delete-ad" }, "target": { "element": "ad" } },
        { "id": "delete-ad-deletes-members", "type": "Delete Entity Action",
          "source": { "element": "delete-ad" }, "target": { "element": "ad-member", "multiplicity": "*" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ads", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ads/Domain/Ad.cs
namespace Ads.Domain;

public class Ad
{
    public Guid Id { get; set; }
}
```

```csharp file src/Ads/Domain/AdMember.cs
namespace Ads.Domain;

public class AdMember
{
    public Guid Id { get; set; }

    public Guid AdId { get; set; }
}
```

```csharp file src/Ads/Data/IGenericRepository.cs
using System.Linq.Expressions;

namespace Ads.Data;

public interface IGenericRepository
{
    IQueryable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : class;

    void Delete<T>(T entity) where T : class;

    void Save();
}
```

```csharp file src/Ads/Services/AdsService.cs
using Ads.Data;
using Ads.Domain;

namespace Ads.Services;

public class AdsService(IGenericRepository repo)
{
    public Guid DeleteAd(Guid id)
    {
        var ad = repo.Find<Ad>(x => x.Id == id).FirstOrDefault();
        if (ad != null)
        {
            var members = repo.Find<AdMember>(x => x.AdId == id).ToList();
            foreach (var member in members)
            {
                repo.Delete(member);
            }

            repo.Delete(ad);
            repo.Save();
        }

        return id;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.deletes", "element": "delete-ad-deletes-ad", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ads/Services/AdsService.cs:19" },
    { "kind": "intent.deletes", "element": "delete-ad-deletes-members", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ads/Services/AdsService.cs:16" }
  ]
}
```

## generic-repository-loop-parent-not-deleted

The `Ad` itself is no longer deleted, only its members. The loop's deletes are of `AdMember`s, so they don't stand in for the `Ad`'s: one claim is contradicted, the other still verified.

```json case
{ "phase": 4, "base": "generic-repository-loop", "kinds": ["intent.deletes"], "requirements": ["R8"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Ads/Services/AdsService.cs
using Ads.Data;
using Ads.Domain;

namespace Ads.Services;

public class AdsService(IGenericRepository repo)
{
    public Guid DeleteAd(Guid id)
    {
        var ad = repo.Find<Ad>(x => x.Id == id).FirstOrDefault();
        if (ad != null)
        {
            var members = repo.Find<AdMember>(x => x.AdId == id).ToList();
            foreach (var member in members)
            {
                repo.Delete(member);
            }

            repo.Save();
        }

        return id;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.deletes", "element": "delete-ad-deletes-ad", "outcome": "contradicted", "expected": "present", "actual": "absent" },
    { "kind": "intent.deletes", "element": "delete-ad-deletes-members", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ads/Services/AdsService.cs:16" }
  ],
  "findings": [
    { "rule": "claim/intent.deletes", "severity": "error", "subject": "delete-ad-deletes-ad", "messageContains": "no delete or remove of `Ad` is made" }
  ]
}
```
