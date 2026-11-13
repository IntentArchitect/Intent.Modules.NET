# intent.creates

A `Create Entity Action` from an Operation to a Domain class, against the code: the Operation's handler reaches the class's repository (or its owner) and constructs it, `new X(`. The claim is the association's, so its element is the association. `intent.creates` is an existence kind: a modelled interaction the code lacks is contradicted (R8). Interactions the code has and the model lacks are the `unmodelled.interaction` opt-in rule's, never drift by default (principle 1).

The fixtures adapt the Java spike's `OrdersController` and `OrderRepository`; the controller calls the repository itself so each case shows one interaction.

## verified

`PlaceOrder` creates an `Order`: the controller constructs one and saves it through `OrderRepository`, a Spring Data repository of `Order`. Traces JAV-SVC-15 (Create ↔ `new X(`).

```json case
{ "phase": 4, "kinds": ["intent.creates"], "traces": ["JAV-SVC-15"], "requirements": ["R2", "R33"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java", "intent.springmvc", "intent.jpa"], "language": "java" }
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
          { "id": "orders-controller", "type": "Service", "name": "OrdersController",
            "children": [
              { "id": "place-order", "type": "Operation", "name": "PlaceOrder",
                "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "/api/orders" } } }
            ] }
        ]
      }],
      "associations": [
        { "id": "place-order-creates-order", "type": "Create Entity Action",
          "source": { "element": "place-order" }, "target": { "element": "order" } }
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

    protected Order() {
    }

    public Order(UUID buyerId) {
        this.id = UUID.randomUUID();
    }

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

import com.eshop.backend.domain.Order;
import com.eshop.backend.domain.OrderRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
public class OrdersController {
    private final OrderRepository orders;

    public OrdersController(OrderRepository orders) {
        this.orders = orders;
    }

    @PostMapping("/api/orders")
    public ResponseEntity<UUID> placeOrder(@RequestBody PlaceOrderInput body) {
        Order order = new Order(body.buyerId());
        orders.save(order);
        return ResponseEntity.ok(order.getId());
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.creates", "element": "place-order-creates-order", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/main/java/com/eshop/backend/api/OrdersController.java:22" }
  ]
}
```

## contradicted

`placeOrder` only accepts the request; nothing constructs an `Order`. The modelled interaction isn't in the code. Traces JAV-SVC-15.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.creates"], "traces": ["JAV-SVC-15"], "requirements": ["R8", "R33"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java", "intent.springmvc", "intent.jpa"], "language": "java" }
```

```java file src/main/java/com/eshop/backend/api/OrdersController.java
package com.eshop.backend.api;

import com.eshop.backend.domain.OrderRepository;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import java.util.UUID;

@RestController
public class OrdersController {
    private final OrderRepository orders;

    public OrdersController(OrderRepository orders) {
        this.orders = orders;
    }

    @PostMapping("/api/orders")
    public ResponseEntity<UUID> placeOrder(@RequestBody PlaceOrderInput body) {
        return ResponseEntity.accepted().build();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "intent.creates", "element": "place-order-creates-order", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/intent.creates", "severity": "error", "subject": "place-order-creates-order", "messageContains": "isn't in the code" }
  ]
}
```

## no-rule

Only the core, Domain, Services and Java packs are active. The Java reader reads the controller, but no active pack has a rule for entity actions, so the claim is can't verify, `noRule`.

```json case
{ "phase": 4, "base": "verified", "kinds": ["intent.creates"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.domain", "intent.services", "intent.java"], "language": "java" }
```

```json expect
{
  "claims": [
    { "kind": "intent.creates", "element": "place-order-creates-order", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/intent.creates", "severity": "error", "subject": "place-order-creates-order", "messageContains": "no rule" }
  ]
}
```
