# intent.queries

A `Query Entity Action` from an Operation to a Domain class, against the code: the Operation's handler reaches the class's repository (or its owner) and reads from it. The claim is the association's. `intent.queries` is an existence kind: a modelled interaction the code lacks is contradicted (R8).

The fixtures adapt the Java spike's `OrdersController`, `Order` and `OrderRepository`.

## verified

`GetOrderById` queries `Order`: the controller reads it through `OrderRepository.findById`. Traces JAV-SVC-15 (the code reaches the target's repository).

```json case
{ "phase": 4, "kinds": ["intent.queries"], "traces": ["JAV-SVC-15"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java", "intent.springmvc", "intent.jpa"], "language": "java" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "guid" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }]
    },
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Backend",
        "elements": [
          { "id": "order-dto", "type": "DTO", "name": "OrderDto",
            "children": [
              { "id": "order-dto-id", "type": "DTO-Field", "name": "Id", "typeRef": { "type": "guid" } }
            ] },
          { "id": "orders-controller", "type": "Service", "name": "OrdersController",
            "children": [
              { "id": "get-order-by-id", "type": "Operation", "name": "GetOrderById", "typeRef": { "element": "order-dto" },
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "/api/orders/{orderId}" } },
                "children": [
                  { "id": "get-order-by-id-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "guid" } }
                ] }
            ] }
        ]
      }],
      "associations": [
        { "id": "get-order-by-id-queries-order", "type": "Query Entity Action",
          "source": { "element": "get-order-by-id" }, "target": { "element": "order" } }
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

    public UUID getId() {
        return id;
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
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
public class OrdersController {
    private final OrderRepository orders;

    public OrdersController(OrderRepository orders) {
        this.orders = orders;
    }

    @GetMapping("/api/orders/{orderId}")
    public OrderDto getOrderById(@PathVariable UUID orderId) {
        return orders.findById(orderId).map(OrderDto::from).orElseThrow();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.queries", "element": "get-order-by-id-queries-order", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:20" }
  ]
}
```

## contradicted-not-reached

The controller answers from a cache of summaries and never reaches `OrderRepository`, so the modelled query isn't in the code. Traces JAV-SVC-15.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.queries"], "traces": ["JAV-SVC-15"], "requirements": ["R8", "R33"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java", "intent.springmvc", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/api/OrdersController.java
package com.eshop.backend.api;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
public class OrdersController {
    private final OrderSummaryCache cache;

    public OrdersController(OrderSummaryCache cache) {
        this.cache = cache;
    }

    @GetMapping("/api/orders/{orderId}")
    public OrderDto getOrderById(@PathVariable UUID orderId) {
        return cache.get(orderId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.queries", "element": "get-order-by-id-queries-order", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.queries", "severity": "error", "subject": "get-order-by-id-queries-order", "messageContains": "isn't in the code" }
  ]
}
```

## no-rule

Without the Spring MVC and JPA packs, no active pack has a rule for entity actions over Java, so the claim is can't verify, `noRule`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.queries"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java"], "language": "java" }
```

```json expect
{
  "claims": [
    { "kind": "intent.queries", "element": "get-order-by-id-queries-order", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/intent.queries", "severity": "error", "subject": "get-order-by-id-queries-order", "messageContains": "no rule" }
  ]
}
```

## identity-context-users

`_context.Users` on a context deriving from ASP.NET Core Identity's `IdentityDbContext<AppUser>` is the `DbSet<AppUser>` that base class declares, so reading it queries `AppUser`, though the context declares no `DbSet` of its own. From an adopted application's `ProfileReader`.

```json case
{ "phase": 4, "kinds": ["intent.queries"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.interactions", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "app-user", "type": "Class", "name": "AppUser",
            "children": [
              { "id": "app-user-display-name", "type": "Attribute", "name": "DisplayName", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Application",
        "elements": [
          { "id": "profile-reader", "type": "Service", "name": "ProfileReader",
            "children": [
              { "id": "read-profile", "type": "Operation", "name": "ReadProfile", "typeRef": { "type": "string" },
                "children": [
                  { "id": "read-profile-username", "type": "Parameter", "name": "username", "typeRef": { "type": "string" } }
                ] }
            ] }
        ]
      }],
      "associations": [
        { "id": "read-profile-queries-app-user", "type": "Query Entity Action",
          "source": { "element": "read-profile" }, "target": { "element": "app-user" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/AppUser.cs
using Microsoft.AspNetCore.Identity;

namespace Shop.Domain;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; }
}
```

```csharp file src/Persistence/DataContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shop.Domain;

namespace Shop.Persistence;

public class DataContext : IdentityDbContext<AppUser>
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }
}
```

```csharp file src/Application/ProfileReader.cs
using Microsoft.EntityFrameworkCore;
using Shop.Persistence;

namespace Shop.Application;

public class ProfileReader
{
    private readonly DataContext _context;

    public ProfileReader(DataContext context)
    {
        _context = context;
    }

    public async Task<string> ReadProfile(string username)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == username);
        return user.DisplayName;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.queries", "element": "read-profile-queries-app-user", "outcome": "verified", "at": "src/Application/ProfileReader.cs:17" }
  ],
  "findings": [
    { "rule": "claim/intent.queries", "count": 0 }
  ]
}
```

## identity-context-users-not-read

The operation no longer reads the context's users, so the model's Query Entity Action is contradicted.

```json case
{ "phase": 4, "base": "identity-context-users", "kinds": ["intent.queries"], "requirements": ["R14"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.interactions", "intent.efcore"], "language": "csharp" }
```

```csharp file src/Application/ProfileReader.cs
using Shop.Persistence;

namespace Shop.Application;

public class ProfileReader
{
    private readonly DataContext _context;

    public ProfileReader(DataContext context)
    {
        _context = context;
    }

    public Task<string> ReadProfile(string username) => Task.FromResult(username);
}
```

```json expect
{
  "claims": [
    { "kind": "intent.queries", "element": "read-profile-queries-app-user", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/intent.queries", "severity": "error", "subject": "read-profile-queries-app-user", "messageContains": "isn't in the code" }
  ]
}
```
