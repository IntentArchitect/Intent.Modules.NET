# operation.parameters

A modelled operation or constructor has the parameters the model lists, by name and type, in order; the code side is the method's or constructor's parameter list, and the `list` comparer needs the same items in the same order (rules.md, What is compared). Each item's type maps through the language profile as `parameter.type` does. With one method of the operation's name, that method is the operation's; parameter types choose only among overloads (rules.md, Locating an element's code). It is a `stated: always` kind that depends on the member: when the method isn't there, the claim is blocked. The cases don't spell out `expected` and `actual`, since the canonical text of a parameter item isn't defined.

## verified

`Order.AddOrderItem`'s four parameters are the method's four, with the same names and types in the same order.

```json case
{ "phase": 1, "kinds": ["operation.parameters"], "traces": ["ESC-DOM-64"], "requirements": ["R2", "R10", "R41", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-add-order-item", "type": "Operation", "name": "AddOrderItem",
                "children": [
                  { "id": "order-add-order-item-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "int" } },
                  { "id": "order-add-order-item-product-name", "type": "Parameter", "name": "productName", "typeRef": { "type": "string" } },
                  { "id": "order-add-order-item-unit-price", "type": "Parameter", "name": "unitPrice", "typeRef": { "type": "decimal" } },
                  { "id": "order-add-order-item-units", "type": "Parameter", "name": "units", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    private readonly List<OrderItem> _orderItems = [];

    public void AddOrderItem(int productId, string productName, decimal unitPrice, int units)
    {
        _orderItems.Add(new OrderItem(productId, productName, unitPrice, units));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-add-order-item", "outcome": "verified", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "count": 0 }
  ]
}
```

## contradicted-order

The method takes `units` before `unitPrice`. The same parameters in another order are a different signature: contradicted.

```json case
{ "phase": 1, "base": "verified", "kinds": ["operation.parameters"], "traces": ["ESC-DOM-64"], "requirements": ["R1", "R10", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    private readonly List<OrderItem> _orderItems = [];

    public void AddOrderItem(int productId, string productName, int units, decimal unitPrice)
    {
        _orderItems.Add(new OrderItem(productId, productName, unitPrice, units));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-add-order-item", "outcome": "contradicted", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "severity": "error", "subject": "order-add-order-item" }
  ]
}
```

## contradicted-missing

The method has no `units` parameter: contradicted.

```json case
{ "phase": 1, "base": "verified", "kinds": ["operation.parameters"], "traces": ["ESC-DOM-64"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    private readonly List<OrderItem> _orderItems = [];

    public void AddOrderItem(int productId, string productName, decimal unitPrice)
    {
        _orderItems.Add(new OrderItem(productId, productName, unitPrice, 1));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-add-order-item", "outcome": "contradicted", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "severity": "error", "subject": "order-add-order-item" }
  ]
}
```

## blocked

`Order` has no `AddOrderItem`: the operation's `member.exists` claim is contradicted, and its parameters claim is blocked by it.

```json case
{ "phase": 1, "base": "verified", "kinds": ["operation.parameters", "member.exists"], "cases": ["engine/blocked"], "requirements": ["R1", "R5", "R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    private readonly List<OrderItem> _orderItems = [];
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-add-order-item", "outcome": "contradicted" },
    { "kind": "operation.parameters", "element": "order-add-order-item", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "member.exists" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-add-order-item" },
    { "rule": "claim/operation.parameters", "count": 0 }
  ]
}
```

## constructor

A modelled constructor's parameters against the constructor chosen for it. `OrderItem(int productId, int units)` is chosen over the protected parameterless constructor, and its parameters are the model's.

```json case
{ "phase": 1, "kinds": ["operation.parameters"], "traces": ["ESC-DOM-61"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "order-item", "type": "Class", "name": "OrderItem",
            "children": [
              { "id": "order-item-product-id", "type": "Attribute", "name": "ProductId", "typeRef": { "type": "int" } },
              { "id": "order-item-units", "type": "Attribute", "name": "Units", "typeRef": { "type": "int" } },
              { "id": "order-item-ctor", "type": "Class Constructor", "name": "OrderItem",
                "children": [
                  { "id": "order-item-ctor-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "int" } },
                  { "id": "order-item-ctor-units", "type": "Parameter", "name": "units", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderItem.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class OrderItem
{
    protected OrderItem() { }

    public OrderItem(int productId, int units)
    {
        ProductId = productId;
        Units = units;
    }

    public int ProductId { get; private set; }

    public int Units { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-item-ctor", "outcome": "verified", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderItem.cs:7" }
  ]
}
```

## constructor-names

The constructor takes `(int units, int productId)`. The types match position by position, so it is still the constructor chosen, but the names don't: contradicted.

```json case
{ "phase": 1, "base": "constructor", "kinds": ["operation.parameters"], "traces": ["ESC-DOM-61"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderItem.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class OrderItem
{
    protected OrderItem() { }

    public OrderItem(int units, int productId)
    {
        ProductId = productId;
        Units = units;
    }

    public int ProductId { get; private set; }

    public int Units { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-item-ctor", "outcome": "contradicted", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderItem.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "severity": "error", "subject": "order-item-ctor" }
  ]
}
```

## repository-operation

A Repository Operation's parameters against the interface method's: `GetAsync(int orderId)`.

```json case
{ "phase": 1, "kinds": ["operation.parameters"], "traces": ["ESC-DOM-73"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
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
              { "id": "order-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" } }
            ] },
          { "id": "order-repository", "type": "Repository", "name": "OrderRepository", "typeRef": { "element": "order" },
            "children": [
              { "id": "order-repository-get", "type": "Operation", "name": "GetAsync", "typeRef": { "element": "order" },
                "children": [
                  { "id": "order-repository-get-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    public int Id { get; private set; }
}
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public interface IOrderRepository
{
    Order Add(Order order);

    Task<Order> GetAsync(int orderId);
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-repository-get", "outcome": "verified", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs:7" }
  ]
}
```

## repository-operation-signature

`GetAsync` takes a `Guid`, not the modelled `int`: the Repository Operation's signature is contradicted.

```json case
{ "phase": 1, "base": "repository-operation", "kinds": ["operation.parameters"], "traces": ["ESC-DOM-73"], "requirements": ["R1", "R11", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public interface IOrderRepository
{
    Order Add(Order order);

    Task<Order> GetAsync(Guid orderId);
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "order-repository-get", "outcome": "contradicted", "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs:7" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "severity": "error", "subject": "order-repository-get" }
  ]
}
```

## services-endpoint

basislink5 models each controller as a Service named as the class is, with one Operation per action; its house pack says a Service is the class of its name. The action `GetSafex(string expDate, string commodity)` has the modelled Parameters. A controller takes its dependencies through its constructor, so the action's parameters are all bound from the request. A minimal API handler's injected parameters (`[AsParameters]` holders, services, a `CancellationToken`) would be compared too: nothing in the language profile or conventions leaves them out.

```json case
{ "phase": 2, "kinds": ["operation.parameters"], "traces": ["ESC-SVC-26"], "requirements": ["R2", "R41", "R53"], "packs": ["intent.core", "intent.services", "house.basislink"], "language": "csharp" }
```

```json pack
{
  "id": "house.basislink",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "types": {
        "Service": { "typeKinds": ["class"], "locations": ["Service/**/Controllers/**"], "note": "basislink5: each controller is a Service named as the class is" }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basislink.API",
        "elements": [
          { "id": "futures-controller", "type": "Service", "name": "FuturesController",
            "children": [
              { "id": "futures-controller-get-safex", "type": "Operation", "name": "GetSafex",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "Safex/Get/{expDate}/{commodity}" } },
                "children": [
                  { "id": "futures-controller-get-safex-exp-date", "type": "Parameter", "name": "expDate", "typeRef": { "type": "string" } },
                  { "id": "futures-controller-get-safex-commodity", "type": "Parameter", "name": "commodity", "typeRef": { "type": "string" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Basislink", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "Service/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file Service/BasislinkService/Basislink.API/Controllers/Grain/FuturesController.cs
using Microsoft.AspNetCore.Mvc;

namespace Basislink.API.Controllers.Grain
{
    [ApiController]
    [Route("v1/[controller]")]
    public class FuturesController : ControllerBase
    {
        private readonly FuturesProcess _futuresProcess;

        public FuturesController(FuturesProcess futuresProcess)
        {
            _futuresProcess = futuresProcess;
        }

        [HttpGet("Safex/Get/{expDate}/{commodity}")]
        public IActionResult GetSafex(string expDate, string commodity)
        {
            var data = _futuresProcess.GetSafex(expDate, commodity);
            return Ok(data);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "futures-controller-get-safex", "outcome": "verified" }
  ]
}
```

## services-endpoint-parameter-missing

The model adds a `month` Parameter that the action doesn't take: contradicted, `services.parameter.missing` in eShopCustom's terms.

```json case
{ "phase": 2, "base": "services-endpoint", "kinds": ["operation.parameters"], "traces": ["ESC-SVC-26"], "requirements": ["R1", "R11"], "packs": ["intent.core", "intent.services", "house.basislink"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basislink.API",
        "elements": [
          { "id": "futures-controller", "type": "Service", "name": "FuturesController",
            "children": [
              { "id": "futures-controller-get-safex", "type": "Operation", "name": "GetSafex",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "Safex/Get/{expDate}/{commodity}" } },
                "children": [
                  { "id": "futures-controller-get-safex-exp-date", "type": "Parameter", "name": "expDate", "typeRef": { "type": "string" } },
                  { "id": "futures-controller-get-safex-commodity", "type": "Parameter", "name": "commodity", "typeRef": { "type": "string" } },
                  { "id": "futures-controller-get-safex-month", "type": "Parameter", "name": "month", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Basislink", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "Service/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "operation.parameters", "element": "futures-controller-get-safex", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/operation.parameters", "severity": "error", "subject": "futures-controller-get-safex" }
  ]
}
```
