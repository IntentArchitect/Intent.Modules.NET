# intent.updates

An `Update Entity Action` from an Operation to a Domain class, against the code: the Operation's handler reaches the class's repository (or its owner), loads an instance and writes it back. Reading alone isn't an update. The claim is the association's. `intent.updates` is an existence kind: a modelled interaction the code lacks is contradicted (R8).

The fixtures adapt the Java spike's `OrdersController`, `Order` and `OrderRepository`.

## verified

`CancelOrder` updates an `Order`: the controller loads it from `OrderRepository`, cancels it and saves it. Traces JAV-SVC-15 (read vs write).

```json case
{ "phase": 4, "kinds": ["intent.updates"], "traces": ["JAV-SVC-15"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java", "intent.springmvc", "intent.jpa"], "language": "java" }
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
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "/api/orders/{orderId}/cancel" } },
                "children": [
                  { "id": "cancel-order-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "guid" } }
                ] }
            ] }
        ]
      }],
      "associations": [
        { "id": "cancel-order-updates-order", "type": "Update Entity Action",
          "source": { "element": "cancel-order" }, "target": { "element": "order" } }
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

import com.eshop.backend.domain.Order;
import com.eshop.backend.domain.OrderRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
public class OrdersController {
    private final OrderRepository orders;

    public OrdersController(OrderRepository orders) {
        this.orders = orders;
    }

    @PostMapping("/api/orders/{orderId}/cancel")
    public ResponseEntity<Void> cancelOrder(@PathVariable UUID orderId) {
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
    { "kind": "intent.updates", "element": "cancel-order-updates-order", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:24" }
  ]
}
```

## contradicted-read-only

The controller loads the `Order` and never writes it back. It reaches the repository, but only to read, so the modelled update isn't in the code. Traces JAV-SVC-15.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.updates"], "traces": ["JAV-SVC-15"], "requirements": ["R8", "R33"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java", "intent.springmvc", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/api/OrdersController.java
package com.eshop.backend.api;

import com.eshop.backend.domain.Order;
import com.eshop.backend.domain.OrderRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
public class OrdersController {
    private final OrderRepository orders;

    public OrdersController(OrderRepository orders) {
        this.orders = orders;
    }

    @PostMapping("/api/orders/{orderId}/cancel")
    public ResponseEntity<Void> cancelOrder(@PathVariable UUID orderId) {
        Order order = orders.findById(orderId).orElseThrow();
        return ResponseEntity.noContent().build();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.updates", "element": "cancel-order-updates-order", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.updates", "severity": "error", "subject": "cancel-order-updates-order", "messageContains": "isn't in the code" }
  ]
}
```

## java-transactional-change

The controller loads the `Order` into an untyped local and cancels it, with no save: JPA writes the change back when the `@Transactional` method ends. Nothing types `order`, as `findById` is the library's, but `cancel()` can only be the `Order`'s own method, so the loaded order is changed.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.updates"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java", "intent.springmvc", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/api/OrdersController.java
package com.eshop.backend.api;

import com.eshop.backend.domain.OrderRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
public class OrdersController {
    private final OrderRepository orders;

    public OrdersController(OrderRepository orders) {
        this.orders = orders;
    }

    @Transactional
    @PostMapping("/api/orders/{orderId}/cancel")
    public ResponseEntity<Void> cancelOrder(@PathVariable UUID orderId) {
        var order = orders.findById(orderId).orElseThrow();
        order.cancel();
        return ResponseEntity.noContent().build();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.updates", "element": "cancel-order-updates-order", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:24" }
  ]
}
```

## no-rule

Without the Spring MVC and JPA packs, no active pack has a rule for entity actions over Java, so the claim is can't verify, `noRule`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.updates"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java"], "language": "java" }
```

```json expect
{
  "claims": [
    { "kind": "intent.updates", "element": "cancel-order-updates-order", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/intent.updates", "severity": "error", "subject": "cancel-order-updates-order", "messageContains": "no rule" }
  ]
}
```

## operation-target-called

`ShipOrderCommand` updates `Order` through its `SetShippedStatus` operation: the action links the operation, not the class. The handler loads the order and calls the operation through `?.`, a call like any other.

```json case
{ "phase": 4, "kinds": ["intent.updates"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } },
              { "id": "order-set-shipped-status", "type": "Operation", "name": "SetShippedStatus" }
            ] }
        ]
      }]
    },
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "ship-order", "type": "Command", "name": "ShipOrderCommand",
            "children": [
              { "id": "ship-order-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] }
        ]
      }],
      "associations": [
        { "id": "ship-order-updates-order", "type": "Update Entity Action",
          "source": { "element": "ship-order" }, "target": { "element": "order-set-shipped-status" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.Domain/Order.cs
namespace Ordering.Domain;

public class Order
{
    public int Id { get; private set; }

    public string Status { get; private set; } = "Submitted";

    public void SetShippedStatus()
    {
        Status = "Shipped";
    }
}
```

```csharp file src/Ordering.Domain/IOrderRepository.cs
namespace Ordering.Domain;

public interface IOrderRepository
{
    Task<Order?> GetAsync(int orderId);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
```

```csharp file src/Ordering.API/Application/Commands/ShipOrderCommand.cs
namespace Ordering.API.Application.Commands;

public record ShipOrderCommand(int OrderNumber) : IRequest<bool>;
```

```csharp file src/Ordering.API/Application/Commands/ShipOrderCommandHandler.cs
using Ordering.Domain;

namespace Ordering.API.Application.Commands;

public class ShipOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<ShipOrderCommand, bool>
{
    public async Task<bool> Handle(ShipOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(command.OrderNumber);
        order?.SetShippedStatus();
        await orderRepository.SaveChangesAsync(cancellationToken);
        return order is not null;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.updates", "element": "ship-order-updates-order", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/Commands/ShipOrderCommandHandler.cs:10" }
  ]
}
```

## operation-target-not-called

The handler still loads and saves the order but no longer calls `SetShippedStatus`. The model says the update is made through that operation, so a load and a save alone aren't it.

```json case
{ "phase": 4, "base": "operation-target-called", "kinds": ["intent.updates"], "requirements": ["R8"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/ShipOrderCommandHandler.cs
using Ordering.Domain;

namespace Ordering.API.Application.Commands;

public class ShipOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<ShipOrderCommand, bool>
{
    public async Task<bool> Handle(ShipOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(command.OrderNumber);
        await orderRepository.SaveChangesAsync(cancellationToken);
        return order is not null;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.updates", "element": "ship-order-updates-order", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.updates", "severity": "error", "subject": "ship-order-updates-order", "messageContains": "no call of `Order.SetShippedStatus` is made" }
  ]
}
```

## ef-load-then-set

`UpdateOrderStatusCommand` updates `Order` the way an EF Core handler does: it loads the order and sets its properties. It calls no update and no save, as the unit of work commits the tracked entity's changes after the handler. The repository's `FindByIdAsync` returns an `Order`, so the local is typed.

```json case
{ "phase": 4, "kinds": ["intent.updates"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
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
        "id": "services", "name": "Ordering.Application",
        "elements": [
          { "id": "update-order-status", "type": "Command", "name": "UpdateOrderStatusCommand",
            "children": [
              { "id": "update-order-status-id", "type": "DTO-Field", "name": "Id", "typeRef": { "type": "guid" } },
              { "id": "update-order-status-status", "type": "DTO-Field", "name": "Status", "typeRef": { "type": "string" } }
            ] }
        ]
      }],
      "associations": [
        { "id": "update-order-status-updates-order", "type": "Update Entity Action",
          "source": { "element": "update-order-status" }, "target": { "element": "order" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.Domain/Order.cs
namespace Ordering.Domain;

public class Order
{
    public Guid Id { get; set; }

    public string Status { get; set; } = "Submitted";
}
```

```csharp file src/Ordering.Domain/IEFRepository.cs
namespace Ordering.Domain;

public interface IEFRepository<TDomain, TPersistence>
{
    void Add(TDomain entity);

    void Remove(TDomain entity);
}
```

```csharp file src/Ordering.Domain/IOrderRepository.cs
namespace Ordering.Domain;

public interface IOrderRepository : IEFRepository<Order, Order>
{
    Task<Order?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
```

```csharp file src/Ordering.Application/Orders/UpdateOrderStatusCommand.cs
namespace Ordering.Application.Orders;

public record UpdateOrderStatusCommand(Guid Id, string Status) : IRequest;
```

```csharp file src/Ordering.Application/Orders/UpdateOrderStatusCommandHandler.cs
using Ordering.Domain;

namespace Ordering.Application.Orders;

public class UpdateOrderStatusCommandHandler(IOrderRepository orderRepository) : IRequestHandler<UpdateOrderStatusCommand>
{
    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var existingOrder = await orderRepository.FindByIdAsync(request.Id, cancellationToken);
        if (existingOrder is null)
        {
            throw new InvalidOperationException($"Could not find Order '{request.Id}'");
        }

        existingOrder.Status = request.Status;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.updates", "element": "update-order-status-updates-order", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.Application/Orders/UpdateOrderStatusCommandHandler.cs:15" }
  ]
}
```

## ef-untyped-load-then-set

The handler loads the order from its `DbContext` with EF Core's `SingleAsync`, a library method, so nothing types `order`. `Status` is a property the `Order` has, so setting it on what the order's load gave changes the order.

```json case
{ "phase": 4, "base": "ef-load-then-set", "kinds": ["intent.updates"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```csharp file src/Ordering.Infrastructure/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using Ordering.Domain;

namespace Ordering.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
}
```

```csharp file src/Ordering.Application/Orders/UpdateOrderStatusCommandHandler.cs
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure;

namespace Ordering.Application.Orders;

public class UpdateOrderStatusCommandHandler(ApplicationDbContext dbContext) : IRequestHandler<UpdateOrderStatusCommand>
{
    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders.SingleAsync(x => x.Id == request.Id, cancellationToken);
        order.Status = request.Status;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.updates", "element": "update-order-status-updates-order", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.Application/Orders/UpdateOrderStatusCommandHandler.cs:11" }
  ]
}
```

## ef-load-without-set

The handler loads the order and checks it exists, but sets nothing on it. A load alone isn't an update.

```json case
{ "phase": 4, "base": "ef-load-then-set", "kinds": ["intent.updates"], "requirements": ["R8"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```csharp file src/Ordering.Application/Orders/UpdateOrderStatusCommandHandler.cs
using Ordering.Domain;

namespace Ordering.Application.Orders;

public class UpdateOrderStatusCommandHandler(IOrderRepository orderRepository) : IRequestHandler<UpdateOrderStatusCommand>
{
    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var existingOrder = await orderRepository.FindByIdAsync(request.Id, cancellationToken);
        if (existingOrder is null)
        {
            throw new InvalidOperationException($"Could not find Order '{request.Id}'");
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.updates", "element": "update-order-status-updates-order", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.updates", "severity": "error", "subject": "update-order-status-updates-order", "messageContains": "`Order` is loaded but never changed" }
  ]
}
```

## ef-load-then-set-another-type

The handler loads the order but sets `Status` on an `OrderSummary` it creates, not on the order. The summary's type is known, so the property's name alone doesn't make it the order's.

```json case
{ "phase": 4, "base": "ef-load-then-set", "kinds": ["intent.updates"], "requirements": ["R8"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```csharp file src/Ordering.Application/Orders/OrderSummary.cs
namespace Ordering.Application.Orders;

public class OrderSummary
{
    public string? Status { get; set; }
}
```

```csharp file src/Ordering.Application/Orders/UpdateOrderStatusCommandHandler.cs
using Ordering.Domain;

namespace Ordering.Application.Orders;

public class UpdateOrderStatusCommandHandler(IOrderRepository orderRepository) : IRequestHandler<UpdateOrderStatusCommand, OrderSummary>
{
    public async Task<OrderSummary> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var existingOrder = await orderRepository.FindByIdAsync(request.Id, cancellationToken);
        var summary = new OrderSummary();
        summary.Status = existingOrder?.Status;
        return summary;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.updates", "element": "update-order-status-updates-order", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.updates", "severity": "error", "subject": "update-order-status-updates-order", "messageContains": "`Order` is loaded but never changed" }
  ]
}
```

## ef-load-then-save-without-change

The handler loads the order and saves the unit of work, but changes nothing on the order. A save writes back only what changed, so a load and a save alone aren't an update.

```json case
{ "phase": 4, "base": "ef-load-then-set", "kinds": ["intent.updates"], "requirements": ["R8"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/IUnitOfWork.cs
namespace Ordering.Domain;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

```csharp file src/Ordering.Application/Orders/UpdateOrderStatusCommandHandler.cs
using Ordering.Domain;

namespace Ordering.Application.Orders;

public class UpdateOrderStatusCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrderStatusCommand>
{
    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var existingOrder = await orderRepository.FindByIdAsync(request.Id, cancellationToken);
        if (existingOrder is null)
        {
            throw new InvalidOperationException($"Could not find Order '{request.Id}'");
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.updates", "element": "update-order-status-updates-order", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.updates", "severity": "error", "subject": "update-order-status-updates-order", "messageContains": "`Order` is loaded but never changed" }
  ]
}
```
