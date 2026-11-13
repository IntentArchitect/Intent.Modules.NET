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
