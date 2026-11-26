# member.exists

A modelled attribute, constructor, operation, Domain Event property, DTO field, component property, event emitter or proxy operation has a member in its owner's code. Inside the located type, an attribute is a property (in Java, a field), an operation is a method chosen among overloads by its parameters, and a constructor is a constructor (rules.md, Locating an element's code). It is an existence kind, so a member the code lacks is contradicted, never a pending change (R8). A member the model doesn't mention is never a finding unless the `unmodelled.member` opt-in rule is on (R44).

## verified

`Order.OrderDate` is a property of the located `Order` class. The class also has `Description`, which the model doesn't mention: no claim and no finding, because the `unmodelled.member` rule is off (principle 1).

```json case
{ "phase": 1, "kinds": ["member.exists"], "traces": ["ESC-DOM-33"], "requirements": ["R2", "R41", "R44"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-order-date", "type": "Attribute", "name": "OrderDate", "typeRef": { "type": "datetime" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public DateTime OrderDate { get; private set; }

    public string? Description { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-order-date", "outcome": "verified", "at": "src/Domain/Order.cs:5" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "count": 0 },
    { "rule": "optin/unmodelled.member", "count": 0, "note": "Description isn't modelled, and the rule is off" }
  ]
}
```

## contradicted-missing

`Order` is in the code but has no `OrderDate`. The type is found, so the member's claim isn't blocked: it is contradicted, and the finding says the member isn't in the code and links `Order`'s declaration, where it belongs (R8, R11). It says nothing of how the engine searched.

```json case
{ "phase": 1, "base": "verified", "kinds": ["member.exists"], "traces": ["ESC-DOM-33"], "requirements": ["R8", "R11"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public string? Description { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" },
    { "kind": "member.exists", "element": "order-order-date", "outcome": "contradicted", "at": "src/Domain/Order.cs:3" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-order-date", "messageContains": "`Order.OrderDate` isn't in the code." },
    { "rule": "claim/member.exists", "subject": "order-order-date", "messageContains": "Searched", "count": 0 }
  ]
}
```

## out-of-scope-partial

`Order` is a partial class. The part in scope is found, so `type.exists` is verified, but `OrderDate` is declared in the scaffolded part under `src/Domain/Generated`, which a scope row excludes. The engine never opens that file; its name matches the type's, so the member's claim is can't verify, `outOfScope`, naming the file and the row, rather than contradicted (R25).

```json case
{ "phase": 1, "kinds": ["member.exists", "type.exists"], "cases": ["scope/shadow", "scope/notRead"], "requirements": ["R5", "R22", "R25", "R37"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-order-date", "type": "Attribute", "name": "OrderDate", "typeRef": { "type": "datetime" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/Domain/Generated/**", "Scope": "Out", "Reason": "Scaffolded from the database and regenerated" }
        ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public partial class Order
{
    public string? Description { get; private set; }
}
```

```csharp file src/Domain/Generated/Order.Generated.cs
namespace Shop.Domain;

public partial class Order
{
    public DateTime OrderDate { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Domain/Order.cs:3" },
    { "kind": "member.exists", "element": "order-order-date", "outcome": "cantVerify", "reason": "outOfScope", "scopeRow": "root#2" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-order-date", "messageContains": "src/Domain/Generated/Order.Generated.cs" }
  ],
  "filesRead": { "include": ["src/Domain/Order.cs"], "exclude": ["src/Domain/Generated/**"] }
}
```

## shadow-foreign-key

`BuyerId` is a foreign key with no property on `Order`: EF Core maps it as a shadow property, `Property<int?>("BuyerId")`. With the EF Core pack, a Foreign Key attribute is found as a property or as a shadow property, so the claim is verified at the configuration's statement.

```json case
{ "phase": 4, "kinds": ["member.exists"], "traces": ["ESC-DOM-33"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.domain", "intent.efcore"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order", "type": "Class", "name": "Order",
            "children": [
              { "id": "order-buyer-id", "type": "Attribute", "name": "BuyerId", "typeRef": { "type": "int", "nullable": true },
                "stereotypes": { "Foreign Key": { "Association": "order-buyer" } } }
            ] },
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" }, "stereotypes": { "Primary Key": {} } }
            ] }
        ]
      }],
      "associations": [
        { "id": "order-buyer", "type": "Association",
          "source": { "element": "order", "multiplicity": "*", "navigable": false },
          "target": { "element": "buyer", "name": "Buyer", "multiplicity": "0..1", "navigable": true } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public Buyer? Buyer { get; private set; }
}
```

```csharp file src/Domain/Buyer.cs
namespace Shop.Domain;

public class Buyer
{
    public int Id { get; private set; }
}
```

```csharp file src/Infrastructure/Persistence/OrderConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain;

namespace Shop.Infrastructure.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property<int?>("BuyerId");

        builder.HasOne(o => o.Buyer)
            .WithMany()
            .HasForeignKey("BuyerId");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-buyer-id", "outcome": "verified", "at": "src/Infrastructure/Persistence/OrderConfiguration.cs:11" }
  ]
}
```

## constructor-and-operation

`OrderItem`'s modelled constructor is the code's two-parameter constructor, chosen over the protected parameterless one by its parameters. Its modelled operation `AddUnits` is a method of that name.

```json case
{ "phase": 1, "kinds": ["member.exists"], "traces": ["ESC-DOM-60", "ESC-DOM-63"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-item", "type": "Class", "name": "OrderItem",
            "children": [
              { "id": "order-item-product-id", "type": "Attribute", "name": "ProductId", "typeRef": { "type": "int" } },
              { "id": "order-item-units", "type": "Attribute", "name": "Units", "typeRef": { "type": "int" } },
              { "id": "order-item-ctor", "type": "Class Constructor", "name": "OrderItem",
                "children": [
                  { "id": "order-item-ctor-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "int" } },
                  { "id": "order-item-ctor-units", "type": "Parameter", "name": "units", "typeRef": { "type": "int" } }
                ] },
              { "id": "order-item-add-units", "type": "Operation", "name": "AddUnits",
                "children": [
                  { "id": "order-item-add-units-units", "type": "Parameter", "name": "units", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/OrderItem.cs
namespace Shop.Domain;

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

    public void AddUnits(int units)
    {
        Units += units;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-item-ctor", "outcome": "verified", "at": "src/Domain/OrderItem.cs:7" },
    { "kind": "member.exists", "element": "order-item-add-units", "outcome": "verified", "at": "src/Domain/OrderItem.cs:17" }
  ]
}
```

## constructor-missing

`OrderItem` declares no constructor, so the modelled one isn't there: contradicted, `constructor.missing` in eShopCustom's terms.

```json case
{ "phase": 1, "base": "constructor-and-operation", "kinds": ["member.exists"], "traces": ["ESC-DOM-60"], "requirements": ["R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/OrderItem.cs
namespace Shop.Domain;

public class OrderItem
{
    public int ProductId { get; private set; }

    public int Units { get; private set; }

    public void AddUnits(int units)
    {
        Units += units;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-item-ctor", "outcome": "contradicted" },
    { "kind": "member.exists", "element": "order-item-add-units", "outcome": "verified", "at": "src/Domain/OrderItem.cs:9" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-item-ctor", "messageContains": "isn't in the code" }
  ]
}
```

## operation-missing

`OrderItem` has no `AddUnits` method: the modelled operation is contradicted, `operation.missing` in eShopCustom's terms.

```json case
{ "phase": 1, "base": "constructor-and-operation", "kinds": ["member.exists"], "traces": ["ESC-DOM-63"], "requirements": ["R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/OrderItem.cs
namespace Shop.Domain;

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
    { "kind": "member.exists", "element": "order-item-add-units", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-item-add-units", "messageContains": "isn't in the code" }
  ]
}
```

## domain-event-positional

A Domain Event's properties are the record's positional parameters. Each exists, `OrderItems` is a collection (`IEnumerable<OrderItem>` against the model's `OrderItem[*]`), and `OrderId`, a value type, isn't nullable whatever the nullable context. Traces ESC-DOM-28's three parts: exists, type with its collection flag, and nullability.

```json case
{ "phase": 1, "kinds": ["member.exists", "member.type", "member.nullable"], "traces": ["ESC-DOM-28"], "requirements": ["R2", "R41", "R42"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "order-item", "type": "Class", "name": "OrderItem",
            "children": [
              { "id": "order-item-units", "type": "Attribute", "name": "Units", "typeRef": { "type": "int" } }
            ] },
          { "id": "paid-event", "type": "Domain Event", "name": "OrderStatusChangedToPaidDomainEvent",
            "children": [
              { "id": "paid-event-order-id", "type": "Property", "name": "OrderId", "typeRef": { "type": "int" } },
              { "id": "paid-event-order-items", "type": "Property", "name": "OrderItems", "typeRef": { "element": "order-item", "collection": true } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/OrderItem.cs
namespace Shop.Domain;

public class OrderItem
{
    public int Units { get; private set; }
}
```

```csharp file src/Domain/Events/OrderStatusChangedToPaidDomainEvent.cs
namespace Shop.Domain.Events;

public record OrderStatusChangedToPaidDomainEvent(int OrderId, IEnumerable<OrderItem> OrderItems) : INotification;
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "paid-event-order-id", "outcome": "verified", "at": "src/Domain/Events/OrderStatusChangedToPaidDomainEvent.cs:3" },
    { "kind": "member.exists", "element": "paid-event-order-items", "outcome": "verified", "at": "src/Domain/Events/OrderStatusChangedToPaidDomainEvent.cs:3" },
    { "kind": "member.type", "element": "paid-event-order-items", "outcome": "verified", "expected": "OrderItem[]", "actual": "IEnumerable<OrderItem>" },
    { "kind": "member.nullable", "element": "paid-event-order-id", "outcome": "verified", "expected": "false", "actual": "false" }
  ]
}
```

## repository-operation

A Repository is described by its interface `IOrderRepository`. Its modelled `GetAsync` is a method of the interface; `Add`, which the model doesn't mention, is no finding while the `unmodelled.member` rule is off.

```json case
{ "phase": 1, "kinds": ["member.exists"], "traces": ["ESC-DOM-73"], "requirements": ["R2", "R41", "R44"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
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
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Domain/Order.cs
namespace Shop.Domain;

public class Order
{
    public int Id { get; private set; }
}
```

```csharp file src/Domain/IOrderRepository.cs
namespace Shop.Domain;

public interface IOrderRepository : IRepository<Order>
{
    Order Add(Order order);

    Task<Order> GetAsync(int orderId);
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-repository-get", "outcome": "verified", "at": "src/Domain/IOrderRepository.cs:7" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "count": 0, "note": "Add isn't modelled, and the rule is off" }
  ]
}
```

## repository-operation-missing

`IOrderRepository` has no `GetAsync`: the modelled Repository Operation is contradicted.

```json case
{ "phase": 1, "base": "repository-operation", "kinds": ["member.exists"], "traces": ["ESC-DOM-73"], "requirements": ["R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/IOrderRepository.cs
namespace Shop.Domain;

public interface IOrderRepository : IRepository<Order>
{
    Order Add(Order order);
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-repository-get", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-repository-get", "messageContains": "isn't in the code" }
  ]
}
```

## services-dto-field

A Services DTO's fields are the record's positional parameters.

```json case
{ "phase": 1, "kinds": ["member.exists"], "traces": ["ESC-SVC-46"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Shop.Services",
        "elements": [
          { "id": "order-dto", "type": "DTO", "name": "OrderDto",
            "children": [
              { "id": "order-dto-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } },
              { "id": "order-dto-status", "type": "DTO-Field", "name": "Status", "typeRef": { "type": "string" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Application/Orders/OrderDto.cs
namespace Shop.Application.Orders;

public record OrderDto(int OrderNumber, string Status);
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-dto-order-number", "outcome": "verified", "at": "src/Application/Orders/OrderDto.cs:3" },
    { "kind": "member.exists", "element": "order-dto-status", "outcome": "verified", "at": "src/Application/Orders/OrderDto.cs:3" }
  ]
}
```

## services-dto-field-missing

The record has no `Status`: the DTO-Field is contradicted, `*.field.missing` in eShopCustom's terms.

```json case
{ "phase": 1, "base": "services-dto-field", "kinds": ["member.exists"], "traces": ["ESC-SVC-46"], "requirements": ["R8"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```csharp file src/Application/Orders/OrderDto.cs
namespace Shop.Application.Orders;

public record OrderDto(int OrderNumber);
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-dto-order-number", "outcome": "verified", "at": "src/Application/Orders/OrderDto.cs:3" },
    { "kind": "member.exists", "element": "order-dto-status", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "order-dto-status", "messageContains": "isn't in the code" }
  ]
}
```

## application-service-partial

eShopCustom's house pack says where its Services are: a Service with no interface is described by the class of its name. `OrderQueries` is a partial class in two files, read as one type (R37), so each Operation is found in whichever part declares it.

```json case
{ "phase": 2, "kinds": ["member.exists"], "traces": ["ESC-SVC-66", "ESC-SVC-67"], "requirements": ["R2", "R28", "R37", "R41", "R53"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "types": {
        "Service": { "typeKinds": ["class"], "locations": ["src/**"], "note": "eShopCustom: a Service with no interface is described by its class" }
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
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "order-dto", "type": "DTO", "name": "Order",
            "children": [
              { "id": "order-dto-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "order-summary", "type": "DTO", "name": "OrderSummary",
            "children": [
              { "id": "order-summary-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "order-queries", "type": "Service", "name": "OrderQueries",
            "stereotypes": { "Application Service": {} },
            "children": [
              { "id": "order-queries-get-order", "type": "Operation", "name": "GetOrderAsync", "typeRef": { "element": "order-dto" },
                "children": [
                  { "id": "order-queries-get-order-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
                ] },
              { "id": "order-queries-get-orders", "type": "Operation", "name": "GetOrdersFromUserAsync", "typeRef": { "element": "order-summary", "collection": true },
                "children": [
                  { "id": "order-queries-get-orders-user-id", "type": "Parameter", "name": "userId", "typeRef": { "type": "string" } }
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

```csharp file src/Ordering.API/Application/Queries/OrderQueries.cs
namespace eShop.Ordering.API.Application.Queries;

public partial class OrderQueries(OrderingContext context)
{
    public async Task<Order> GetOrderAsync(int id)
    {
        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        return order is null ? throw new KeyNotFoundException() : new Order { OrderNumber = order.Id };
    }
}
```

```csharp file src/Ordering.API/Application/Queries/OrderQueries.Summaries.cs
namespace eShop.Ordering.API.Application.Queries;

public partial class OrderQueries
{
    public async Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(string userId)
    {
        return await context.Orders
            .Where(o => o.Buyer.IdentityGuid == userId)
            .Select(o => new OrderSummary { OrderNumber = o.Id })
            .ToListAsync();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-queries-get-order", "outcome": "verified", "at": "src/Ordering.API/Application/Queries/OrderQueries.cs:5" },
    { "kind": "member.exists", "element": "order-queries-get-orders", "outcome": "verified", "at": "src/Ordering.API/Application/Queries/OrderQueries.Summaries.cs:5" }
  ]
}
```

## application-service-overload

`GetOrdersFromUserAsync` has two overloads. The modelled Operation's one parameter, `userId: string`, matches the first by name and by type, so that is the method it describes, and the second, unmodelled, is no finding.

```json case
{ "phase": 2, "base": "application-service-partial", "kinds": ["member.exists"], "traces": ["ESC-SVC-67"], "requirements": ["R2", "R41", "R44"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Queries/OrderQueries.Summaries.cs
namespace eShop.Ordering.API.Application.Queries;

public partial class OrderQueries
{
    public Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(string userId)
    {
        return GetOrdersFromUserAsync(userId, pageSize: 50);
    }

    public async Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(string userId, int pageSize)
    {
        return await context.Orders
            .Where(o => o.Buyer.IdentityGuid == userId)
            .Take(pageSize)
            .Select(o => new OrderSummary { OrderNumber = o.Id })
            .ToListAsync();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-queries-get-orders", "outcome": "verified", "at": "src/Ordering.API/Application/Queries/OrderQueries.Summaries.cs:5" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.member", "count": 0 }
  ]
}
```

## application-service-overload-renamed-parameters

`GetOrdersFromUserAsync` has two overloads of one parameter, and the class renames the parameter its interface declares, `userId`, to `user`, so neither overload has the modelled Operation's parameter name. Of the two, the one whose parameter type matches, `string`, is the method it describes, not the first of that count, `Guid buyerId`.

```json case
{ "phase": 2, "base": "application-service-partial", "kinds": ["member.exists"], "requirements": ["R2", "R41", "R44"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Queries/OrderQueries.Summaries.cs
namespace eShop.Ordering.API.Application.Queries;

public partial class OrderQueries : IOrderQueries
{
    public Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(Guid buyerId)
    {
        return GetOrdersFromUserAsync(buyerId.ToString());
    }

    public async Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(string user)
    {
        return await context.Orders
            .Where(o => o.Buyer.IdentityGuid == user)
            .Select(o => new OrderSummary { OrderNumber = o.Id })
            .ToListAsync();
    }
}

public interface IOrderQueries
{
    Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(Guid buyerId);

    Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(string userId);
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "order-queries-get-orders", "outcome": "verified", "at": "src/Ordering.API/Application/Queries/OrderQueries.Summaries.cs:10" }
  ]
}
```

## hosted-service-override

A Service with eShopCustom's Hosted Service stereotype is a `BackgroundService`; its modelled `ExecuteAsync` is the class's override. A `CancellationToken` is never modelled, so the model's Operation has no parameter. The claim is the method's existence; nothing compares the `override` modifier itself, which no structure kind states.

```json case
{ "phase": 2, "kinds": ["member.exists"], "traces": ["ESC-SVC-72"], "requirements": ["R2", "R28", "R41", "R53"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "types": {
        "Service": { "typeKinds": ["class"], "locations": ["src/**"], "note": "eShopCustom: a Service with no interface is described by its class" }
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
        "id": "services", "name": "OrderProcessor",
        "elements": [
          { "id": "grace-period", "type": "Service", "name": "GracePeriodManagerService",
            "stereotypes": { "Hosted Service": {} },
            "children": [
              { "id": "grace-period-execute", "type": "Operation", "name": "ExecuteAsync" }
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

```csharp file src/OrderProcessor/Services/GracePeriodManagerService.cs
namespace eShop.OrderProcessor.Services;

public class GracePeriodManagerService(ILogger<GracePeriodManagerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogDebug("GracePeriodManagerService background task is doing background work.");
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "grace-period-execute", "outcome": "verified", "at": "src/OrderProcessor/Services/GracePeriodManagerService.cs:5" }
  ]
}
```

## hosted-service-override-missing

The class overrides `StartAsync` instead, so the modelled `ExecuteAsync` isn't there: contradicted, `services.hosted-service.operation.missing` in eShopCustom's terms.

```json case
{ "phase": 2, "base": "hosted-service-override", "kinds": ["member.exists"], "traces": ["ESC-SVC-72"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```csharp file src/OrderProcessor/Services/GracePeriodManagerService.cs
namespace eShop.OrderProcessor.Services;

public class GracePeriodManagerService(ILogger<GracePeriodManagerService> logger) : BackgroundService
{
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("GracePeriodManagerService is starting.");
        return base.StartAsync(cancellationToken);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "grace-period-execute", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "grace-period-execute", "messageContains": "isn't in the code" }
  ]
}
```

## grpc-operation-override

A gRPC Service's Operation is an `override` in the class that derives the generated `Basket.BasketBase`. This case shows that half of ESC-SVC-78: the method exists in the class. The other half, an `rpc GetBasket` in `service Basket` of the `.proto` file, isn't shown: no reader reads `.proto` files, and a second rule reading the same claim from the `.proto` text can't sit beside the structure rule, since exactly one rule reads a claim.

```json case
{ "phase": 2, "kinds": ["member.exists"], "traces": ["ESC-SVC-78"], "requirements": ["R2", "R28", "R41", "R53"], "packs": ["intent.core", "intent.services", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "types": {
        "Service": { "typeKinds": ["class"], "locations": ["src/**"], "note": "eShopCustom: a Service with no interface is described by its class" }
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
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "get-basket-request", "type": "DTO", "name": "GetBasketRequest", "stereotypes": { "Proto Message": {} } },
          { "id": "customer-basket-response", "type": "DTO", "name": "CustomerBasketResponse", "stereotypes": { "Proto Message": {} } },
          { "id": "basket-service", "type": "Service", "name": "BasketService",
            "stereotypes": { "gRPC Service": { "Proto File": "Proto/basket.proto", "Proto Service": "Basket" } },
            "children": [
              { "id": "basket-service-get-basket", "type": "Operation", "name": "GetBasket", "typeRef": { "element": "customer-basket-response" },
                "children": [
                  { "id": "basket-service-get-basket-request", "type": "Parameter", "name": "request", "typeRef": { "element": "get-basket-request" } }
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

```csharp file src/Basket.API/Grpc/BasketService.cs
namespace eShop.Basket.API.Grpc;

public class BasketService(IBasketRepository repository) : Basket.BasketBase
{
    public override async Task<CustomerBasketResponse> GetBasket(GetBasketRequest request, ServerCallContext context)
    {
        var data = await repository.GetBasketAsync(context.GetUserIdentity());
        return data is null ? new() : MapToCustomerBasketResponse(data);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "basket-service-get-basket", "outcome": "verified", "at": "src/Basket.API/Grpc/BasketService.cs:5" }
  ]
}
```

## component-members

A User Interface Component's Property is a `[Parameter]` property, its Event Emitter a `[Parameter] EventCallback` of that name, and its Component Operation a method of that name, read from the component's `@code` block. The User Interface designer's vocabulary arrives with the `intent.ui` pack, in phase 6.

```json case
{ "phase": 6, "kinds": ["member.exists"], "traces": ["ESC-FE-16", "ESC-FE-22", "ESC-FE-25"], "requirements": ["R2", "R33", "R41"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop.WebAppComponents",
        "elements": [
          { "id": "catalog-search", "type": "Component", "name": "CatalogSearch",
            "children": [
              { "id": "catalog-search-brand-id", "type": "Property", "name": "BrandId", "typeRef": { "type": "int", "nullable": true } },
              { "id": "catalog-search-brand-changed", "type": "Event Emitter", "name": "BrandChanged", "typeRef": { "type": "int", "nullable": true } },
              { "id": "catalog-search-on-brand-selected", "type": "Component Operation", "name": "OnBrandSelected",
                "children": [
                  { "id": "catalog-search-on-brand-selected-brand-id", "type": "Parameter", "name": "brandId", "typeRef": { "type": "int", "nullable": true } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```razor file src/WebAppComponents/Catalog/CatalogSearch.razor
<div class="catalog-search">
    @foreach (var brand in catalogBrands)
    {
        <a href="" @onclick="() => OnBrandSelected(brand.Id)">@brand.Brand</a>
    }
</div>

@code {
    IEnumerable<CatalogBrand> catalogBrands = [];

    [Parameter] public int? BrandId { get; set; }

    [Parameter] public EventCallback<int?> BrandChanged { get; set; }

    private async Task OnBrandSelected(int? brandId)
    {
        await BrandChanged.InvokeAsync(brandId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "catalog-search-brand-id", "outcome": "verified", "at": "src/WebAppComponents/Catalog/CatalogSearch.razor:11" },
    { "kind": "member.exists", "element": "catalog-search-brand-changed", "outcome": "verified", "at": "src/WebAppComponents/Catalog/CatalogSearch.razor:13" },
    { "kind": "member.exists", "element": "catalog-search-on-brand-selected", "outcome": "verified", "at": "src/WebAppComponents/Catalog/CatalogSearch.razor:15" }
  ]
}
```

## component-property-not-a-parameter

`BrandId` is a plain property, with no `[Parameter]`, `[SupplyParameterFromQuery]` or `[CascadingParameter]`. A parent can't set it, so it isn't the modelled Property: contradicted, `property.missing` in eShopCustom's terms.

```json case
{ "phase": 6, "base": "component-members", "kinds": ["member.exists"], "traces": ["ESC-FE-16"], "requirements": ["R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebAppComponents/Catalog/CatalogSearch.razor
<div class="catalog-search">
    @foreach (var brand in catalogBrands)
    {
        <a href="" @onclick="() => OnBrandSelected(brand.Id)">@brand.Brand</a>
    }
</div>

@code {
    IEnumerable<CatalogBrand> catalogBrands = [];

    public int? BrandId { get; set; }

    [Parameter] public EventCallback<int?> BrandChanged { get; set; }

    private async Task OnBrandSelected(int? brandId)
    {
        await BrandChanged.InvokeAsync(brandId);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "catalog-search-brand-id", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "catalog-search-brand-id" }
  ]
}
```

## page-property-bound-from-the-form

`UpdateQuantityId` is bound from the posted form (`[SupplyParameterFromForm]`), so it isn't the modelled Property: the User Interface designer models a Page's Properties only for its route and query parameters ("Everything else is written in code"). Contradicted, with a finding that says to remove the Property rather than make it a parameter.

```json case
{ "phase": 6, "kinds": ["member.exists"], "requirements": ["R2", "R8"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "WebApp", "type": "User Interface Package",
        "elements": [
          { "id": "cart-page", "type": "Page", "name": "CartPage", "stereotypes": { "Page": { "Route": "/cart" } },
            "children": [
              { "id": "cart-page-update-quantity-id", "type": "Property", "name": "UpdateQuantityId", "typeRef": { "type": "int", "nullable": true } }
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

```razor file src/WebApp/Components/Pages/Cart/CartPage.razor
@page "/cart"

<form method="post" @formname="update-cart" @onsubmit="UpdateQuantityAsync">
    <AntiforgeryToken />
</form>

@code {
    [SupplyParameterFromForm]
    public int? UpdateQuantityId { get; set; }

    private Task UpdateQuantityAsync() => Task.CompletedTask;
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "cart-page-update-quantity-id", "outcome": "contradicted", "actual": "bound from the form", "at": "src/WebApp/Components/Pages/Cart/CartPage.razor:8" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "cart-page-update-quantity-id", "messageContains": "Remove the Property" }
  ]
}
```

## react-component-members

The standard User Interface designer (2.0.0) read against a React function component by the stock React reader. A Property is a prop, from the props type or the names the component destructures, or its state (a `useState` value); an Event Emitter is an `on...` callback prop of its name, `Selected` being `onSelected`; a Component Operation is a function the component declares, here through `useCallback`. A component wrapped in `memo` is the component.

```json case
{ "phase": 6, "kinds": ["member.exists"], "requirements": ["R2", "R33", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "product-card", "type": "Component", "name": "ProductCard",
            "children": [
              { "id": "product-card-product-id", "type": "Property", "name": "ProductId", "typeRef": { "type": "string" } },
              { "id": "product-card-compact", "type": "Property", "name": "Compact", "typeRef": { "type": "bool" } },
              { "id": "product-card-quantity", "type": "Property", "name": "Quantity", "typeRef": { "type": "int" } },
              { "id": "product-card-selected", "type": "Event Emitter", "name": "Selected", "typeRef": { "type": "string" } },
              { "id": "product-card-select", "type": "Component Operation", "name": "Select" }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```tsx file src/ProductCard.tsx
import { memo, useCallback, useState } from 'react';

interface ProductCardProps {
    productId: string;
    onSelected(id: string): void;
}

export const ProductCard = memo(({ productId, onSelected, compact = false }: ProductCardProps & { compact?: boolean }) => {
    const [quantity, setQuantity] = useState(1);
    const select = useCallback(() => onSelected(productId), [productId, onSelected]);
    return (
        <article className={compact ? 'compact' : undefined}>
            <input type="number" value={quantity} onChange={(e) => setQuantity(Number(e.target.value))} />
            <button onClick={select}>Select</button>
        </article>
    );
});
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "product-card-product-id", "outcome": "verified", "at": "src/ProductCard.tsx:4" },
    { "kind": "member.exists", "element": "product-card-compact", "outcome": "verified", "at": "src/ProductCard.tsx:8" },
    { "kind": "member.exists", "element": "product-card-quantity", "outcome": "verified", "at": "src/ProductCard.tsx:9" },
    { "kind": "member.exists", "element": "product-card-selected", "outcome": "verified", "at": "src/ProductCard.tsx:5" },
    { "kind": "member.exists", "element": "product-card-select", "outcome": "verified", "at": "src/ProductCard.tsx:10" }
  ]
}
```

## react-component-members-missing

The component lost its `onSelected` callback and its `select` function, and keeps `quantity` only as a prop it is given: the Event Emitter and the Component Operation are each missing, and the Property still holds.

```json case
{ "phase": 6, "base": "react-component-members", "kinds": ["member.exists"], "requirements": ["R8"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```tsx file src/ProductCard.tsx
interface ProductCardProps {
    productId: string;
    quantity: number;
    compact?: boolean;
}

export function ProductCard({ productId, quantity, compact }: ProductCardProps) {
    return <article className={compact ? 'compact' : undefined}>{productId} x {quantity}</article>;
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "product-card-quantity", "outcome": "verified", "at": "src/ProductCard.tsx:3" },
    { "kind": "member.exists", "element": "product-card-selected", "outcome": "contradicted" },
    { "kind": "member.exists", "element": "product-card-select", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "product-card-selected" },
    { "rule": "claim/member.exists", "severity": "error", "subject": "product-card-select" }
  ]
}
```

## react-store-members

A store's state and actions, whatever the store library: a Redux Toolkit slice's initial state and reducers, and what a Zustand store's creator returns. The readers every TypeScript front end shares read both, through the slice's `initialState` constant and Zustand's middleware. The wishlist has no `clear` action.

```json case
{ "phase": 6, "kinds": ["member.exists"], "requirements": ["R2", "R33", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "stores", "type": "Folder", "name": "stores", "children": [
            { "id": "cart", "type": "Store", "name": "Cart", "children": [
              { "id": "cart-lines", "type": "Property", "name": "Lines" },
              { "id": "cart-line-added", "type": "Property", "name": "LineAdded" }
            ] },
            { "id": "wishlist", "type": "Store", "name": "Wishlist", "children": [
              { "id": "wishlist-items", "type": "Property", "name": "Items" },
              { "id": "wishlist-add", "type": "Property", "name": "Add" },
              { "id": "wishlist-clear", "type": "Property", "name": "Clear" }
            ] }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```ts file src/stores/cartSlice.ts
import { createSlice } from '@reduxjs/toolkit';

const initialState = { lines: [] as string[] };

export const cartSlice = createSlice({
    name: 'cart',
    initialState,
    reducers: {
        lineAdded(state, action) {
            state.lines.push(action.payload);
        },
    },
});
```

```ts file src/stores/wishlist.ts
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

export const useWishlistStore = create<{ items: string[]; add(id: string): void }>()(
    persist(
        (set) => ({
            items: [],
            add: (id) => set((s) => ({ items: [...s.items, id] })),
        }),
        { name: 'wishlist' },
    ),
);
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "cart", "outcome": "verified", "at": "src/stores/cartSlice.ts:5" },
    { "kind": "type.exists", "element": "wishlist", "outcome": "verified", "at": "src/stores/wishlist.ts:4" },
    { "kind": "member.exists", "element": "cart-lines", "outcome": "verified", "at": "src/stores/cartSlice.ts:3" },
    { "kind": "member.exists", "element": "cart-line-added", "outcome": "verified", "at": "src/stores/cartSlice.ts:9" },
    { "kind": "member.exists", "element": "wishlist-items", "outcome": "verified", "at": "src/stores/wishlist.ts:7" },
    { "kind": "member.exists", "element": "wishlist-add", "outcome": "verified", "at": "src/stores/wishlist.ts:8" },
    { "kind": "member.exists", "element": "wishlist-clear", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "wishlist-clear" }
  ]
}
```

## model-definition

A Model Definition's Properties are the record's positional parameters, and its Operation a public method.

```json case
{ "phase": 6, "kinds": ["member.exists"], "traces": ["ESC-FE-42", "ESC-FE-45"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop.WebAppComponents",
        "elements": [
          { "id": "catalog-item", "type": "Model Definition", "name": "CatalogItem",
            "children": [
              { "id": "catalog-item-id", "type": "Property", "name": "Id", "typeRef": { "type": "int" } },
              { "id": "catalog-item-name", "type": "Property", "name": "Name", "typeRef": { "type": "string" } },
              { "id": "catalog-item-price", "type": "Property", "name": "Price", "typeRef": { "type": "decimal" } },
              { "id": "catalog-item-price-with-tax", "type": "Operation", "name": "PriceWithTax", "typeRef": { "type": "decimal" },
                "children": [
                  { "id": "catalog-item-price-with-tax-rate", "type": "Parameter", "name": "rate", "typeRef": { "type": "decimal" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/WebAppComponents/Catalog/CatalogItem.cs
namespace Shop.WebAppComponents.Catalog;

public record CatalogItem(int Id, string Name, decimal Price)
{
    public decimal PriceWithTax(decimal rate) => Price * (1 + rate);
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "catalog-item-id", "outcome": "verified", "at": "src/WebAppComponents/Catalog/CatalogItem.cs:3" },
    { "kind": "member.exists", "element": "catalog-item-price", "outcome": "verified", "at": "src/WebAppComponents/Catalog/CatalogItem.cs:3" },
    { "kind": "member.exists", "element": "catalog-item-price-with-tax", "outcome": "verified", "at": "src/WebAppComponents/Catalog/CatalogItem.cs:5" }
  ]
}
```

## service-proxy-operation

A Service Proxy's Operation is a public instance method of the proxy class.

```json case
{ "phase": 6, "kinds": ["member.exists"], "traces": ["ESC-FE-52"], "requirements": ["R2", "R41"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop.WebAppComponents",
        "elements": [
          { "id": "catalog-item", "type": "Model Definition", "name": "CatalogItem",
            "children": [
              { "id": "catalog-item-id", "type": "Property", "name": "Id", "typeRef": { "type": "int" } }
            ] },
          { "id": "catalog-service", "type": "Service Proxy", "name": "CatalogService",
            "children": [
              { "id": "catalog-service-get-item", "type": "Operation", "name": "GetCatalogItem", "typeRef": { "element": "catalog-item", "nullable": true },
                "children": [
                  { "id": "catalog-service-get-item-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/WebAppComponents/Catalog/CatalogItem.cs
namespace Shop.WebAppComponents.Catalog;

public record CatalogItem(int Id);
```

```csharp file src/WebAppComponents/Services/CatalogService.cs
using System.Net.Http.Json;
using Shop.WebAppComponents.Catalog;

namespace Shop.WebAppComponents.Services;

public class CatalogService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "api/catalog/";

    public Task<CatalogItem?> GetCatalogItem(int id)
    {
        var uri = $"{remoteServiceBaseUrl}items/{id}";
        return httpClient.GetFromJsonAsync<CatalogItem>(uri);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "catalog-service-get-item", "outcome": "verified", "at": "src/WebAppComponents/Services/CatalogService.cs:10" }
  ]
}
```

## java-attribute-field

The same claim over Java: an attribute is a field named by the Java profile's naming, `quantityOnHand` for `QuantityOnHand`.

```json case
{ "phase": 4, "kinds": ["member.exists"], "traces": ["JAV-DOM-11"], "requirements": ["R2", "R33", "R41", "R42"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "stock-item", "type": "Class", "name": "StockItem",
            "children": [
              { "id": "stock-item-quantity-on-hand", "type": "Attribute", "name": "QuantityOnHand", "typeRef": { "type": "int" } }
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

```java file src/main/java/com/shop/domain/StockItem.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class StockItem {
    @Id
    private UUID id;
    private int quantityOnHand;

    public int getQuantityOnHand() {
        return quantityOnHand;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "stock-item-quantity-on-hand", "outcome": "verified", "at": "src/main/java/com/shop/domain/StockItem.java:11" }
  ]
}
```

## java-attribute-field-missing

`StockItem` has no `quantityOnHand` field, only a computed getter of another name: the attribute is contradicted.

```json case
{ "phase": 4, "base": "java-attribute-field", "kinds": ["member.exists"], "traces": ["JAV-DOM-11"], "requirements": ["R8"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```java file src/main/java/com/shop/domain/StockItem.java
package com.shop.domain;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import java.util.UUID;

@Entity
public class StockItem {
    @Id
    private UUID id;
    private int onHand;

    public int getQuantityAvailable() {
        return onHand;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "stock-item-quantity-on-hand", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "stock-item-quantity-on-hand", "messageContains": "isn't in the code" }
  ]
}
```

## java-constructor-and-operation

A modelled constructor is a non-private Java constructor, chosen over the protected no-argument one by its parameters, and a modelled operation is a method named by the Java profile's naming, `receive` for `Receive`.

```json case
{ "phase": 4, "kinds": ["member.exists"], "traces": ["JAV-DOM-20"], "requirements": ["R2", "R33", "R41", "R42"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "stock-item", "type": "Class", "name": "StockItem",
            "children": [
              { "id": "stock-item-product-id", "type": "Attribute", "name": "ProductId", "typeRef": { "type": "guid" } },
              { "id": "stock-item-quantity-on-hand", "type": "Attribute", "name": "QuantityOnHand", "typeRef": { "type": "int" } },
              { "id": "stock-item-ctor", "type": "Class Constructor", "name": "StockItem",
                "children": [
                  { "id": "stock-item-ctor-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "guid" } }
                ] },
              { "id": "stock-item-receive", "type": "Operation", "name": "Receive",
                "children": [
                  { "id": "stock-item-receive-quantity", "type": "Parameter", "name": "quantity", "typeRef": { "type": "int" } },
                  { "id": "stock-item-receive-note", "type": "Parameter", "name": "note", "typeRef": { "type": "string" } }
                ] }
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

```java file src/main/java/com/shop/domain/StockItem.java
package com.shop.domain;

import java.util.UUID;

public class StockItem {
    private UUID productId;
    private int quantityOnHand;

    protected StockItem() {
    }

    public StockItem(UUID productId) {
        this.productId = productId;
    }

    public void receive(int quantity, String note) {
        quantityOnHand += quantity;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "stock-item-ctor", "outcome": "verified", "at": "src/main/java/com/shop/domain/StockItem.java:12" },
    { "kind": "member.exists", "element": "stock-item-receive", "outcome": "verified", "at": "src/main/java/com/shop/domain/StockItem.java:16" }
  ]
}
```

## java-operation-missing

`StockItem` has no `receive` method: the modelled operation is contradicted.

```json case
{ "phase": 4, "base": "java-constructor-and-operation", "kinds": ["member.exists"], "traces": ["JAV-DOM-20"], "requirements": ["R8"], "packs": ["intent.core", "intent.domain", "intent.java"], "language": "java" }
```

```java file src/main/java/com/shop/domain/StockItem.java
package com.shop.domain;

import java.util.UUID;

public class StockItem {
    private UUID productId;
    private int quantityOnHand;

    protected StockItem() {
    }

    public StockItem(UUID productId) {
        this.productId = productId;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "stock-item-ctor", "outcome": "verified", "at": "src/main/java/com/shop/domain/StockItem.java:12" },
    { "kind": "member.exists", "element": "stock-item-receive", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "stock-item-receive", "messageContains": "isn't in the code" }
  ]
}
```

## java-proxy-operation

The Java spike's own Front End designer has Proxies, whose Operations are the methods of an object literal in a TypeScript file, `export const api = { … }`. No stock pack knows that designer, so the storefront's house pack states the claims (R28). The TypeScript reader makes the object literal an `object` type with its methods, and the house pack's conventions name a Proxy's type in camel case, so the Proxy `Api` is `api`. A Front End Proxy Operation is named in Pascal case in the model, and TypeScript names a method in camel case, so `GetProductById` is `getProductById`.

```json case
{ "phase": 4, "kinds": ["member.exists"], "traces": ["JAV-FE-08"], "requirements": ["R2", "R28", "R30", "R51", "R53"], "packs": ["intent.core", "intent.java", "house.storefront"], "language": "typescript" }
```

```json pack
{
  "id": "house.storefront",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["typescript"],
  "designers": ["Front End"],
  "vocabulary": {
    "Front End": {
      "entries": [
        { "kind": "type.exists", "for": { "element": "Proxy" }, "value": "self", "bindings": { "Name": "name" }, "own": ["Name"], "source": "the Proxy" },
        {
          "kind": "member.exists",
          "for": { "element": "Proxy Operation" },
          "value": "self",
          "dependsOn": "parent:type.exists",
          "source": "the Proxy Operation",
          "fixHint": "Add the method to the Proxy's file, or remove the Proxy Operation."
        }
      ]
    }
  },
  "conventions": {
    "Front End": {
      "types": {
        "Proxy": { "name": "{Name|camel}", "typeKinds": ["object"], "note": "The storefront: a Proxy is the object of its methods" }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Front End": {
      "packages": [{
        "id": "storefront", "name": "Storefront",
        "elements": [
          { "id": "product-dto", "type": "Contract", "name": "ProductDto",
            "children": [
              { "id": "product-dto-id", "type": "Field", "name": "Id", "typeRef": { "type": "guid" } }
            ] },
          { "id": "api", "type": "Proxy", "name": "Api",
            "stereotypes": { "Implementation": { "Path": "src/api/client.ts" } },
            "children": [
              { "id": "api-get-product-by-id", "type": "Proxy Operation", "name": "GetProductById", "typeRef": { "element": "product-dto" },
                "children": [
                  { "id": "api-get-product-by-id-id", "type": "Parameter", "name": "id", "typeRef": { "type": "guid" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Storefront", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```typescript file src/api/client.ts
import type { ProductDto } from './types';

async function request<T>(method: string, path: string): Promise<T> {
    const res = await fetch(path, { method });
    return (await res.json()) as T;
}

export const api = {
    // GET /api/products/{id}
    getProductById(id: string): Promise<ProductDto> {
        return request<ProductDto>('GET', `/api/products/${encodeURIComponent(id)}`);
    },
};
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "api-get-product-by-id", "outcome": "verified", "at": "src/api/client.ts:10" }
  ]
}
```

## java-proxy-operation-missing

The proxy object has no `getProductById`. The Proxy's type is found and has no method of that name, so the existence claim is contradicted.

```json case
{ "phase": 4, "base": "java-proxy-operation", "kinds": ["member.exists"], "traces": ["JAV-FE-08"], "requirements": ["R8"], "packs": ["intent.core", "intent.java", "house.storefront"], "language": "typescript" }
```

```typescript file src/api/client.ts
import type { ProductDto } from './types';

async function request<T>(method: string, path: string): Promise<T> {
    const res = await fetch(path, { method });
    return (await res.json()) as T;
}

export const api = {
    // GET /api/products
    getProducts(): Promise<ProductDto[]> {
        return request<ProductDto[]>('GET', '/api/products');
    },
};
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "api-get-product-by-id", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "api-get-product-by-id" }
  ]
}
```

## java-contract-field

A Front End Contract's Field is a property of the exported interface of that name. The house pack's conventions say a Contract is an interface or type alias in `src/api/types.ts`, and the TypeScript reader gives `ProductDto` its properties, `Price` among them.

```json case
{ "phase": 4, "kinds": ["member.exists"], "traces": ["JAV-FE-17"], "requirements": ["R2", "R28", "R30", "R51", "R53"], "packs": ["intent.core", "intent.java", "house.storefront"], "language": "typescript" }
```

```json pack
{
  "id": "house.storefront",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["typescript"],
  "designers": ["Front End"],
  "vocabulary": {
    "Front End": {
      "entries": [
        { "kind": "type.exists", "for": { "element": "Contract" }, "value": "self", "bindings": { "Name": "name" }, "own": ["Name"], "source": "the Contract" },
        {
          "kind": "member.exists",
          "for": { "element": "Field" },
          "value": "self",
          "dependsOn": "parent:type.exists",
          "source": "the Contract's Field"
        }
      ]
    }
  },
  "conventions": {
    "Front End": {
      "types": {
        "Contract": { "typeKinds": ["interface", "typeAlias"], "locations": ["src/api/types.ts"], "note": "Contracts live in src/api/types.ts, one exported interface each" }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Front End": {
      "packages": [{
        "id": "storefront", "name": "Storefront",
        "elements": [
          { "id": "product-dto", "type": "Contract", "name": "ProductDto",
            "children": [
              { "id": "product-dto-id", "type": "Field", "name": "Id", "typeRef": { "type": "guid" } },
              { "id": "product-dto-price", "type": "Field", "name": "Price", "typeRef": { "type": "decimal" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Storefront", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```typescript file src/api/types.ts
// Typed interfaces mirroring the backend DTOs.
export interface ProductDto {
    Id: string;
    Name: string;
    Description: string | null;
    Price: number;
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "product-dto-id", "outcome": "verified", "at": "src/api/types.ts:3" },
    { "kind": "member.exists", "element": "product-dto-price", "outcome": "verified", "at": "src/api/types.ts:6" }
  ]
}
```

## java-contract-field-missing

`ProductDto` has no `Price`. A `Price` elsewhere in the file, in another interface, doesn't count: a Field is looked for only among its Contract's members.

```json case
{ "phase": 4, "base": "java-contract-field", "kinds": ["member.exists"], "traces": ["JAV-FE-17"], "requirements": ["R8"], "packs": ["intent.core", "intent.java", "house.storefront"], "language": "typescript" }
```

```typescript file src/api/types.ts
// Typed interfaces mirroring the backend DTOs.
export interface ProductDto {
    Id: string;
    Name: string;
    Description: string | null;
}

export interface OrderLineItemDto {
    ProductId: string;
    Price: number;
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "product-dto-id", "outcome": "verified", "at": "src/api/types.ts:3" },
    { "kind": "member.exists", "element": "product-dto-price", "outcome": "contradicted" }
  ]
}
```

## association-navigation

A navigable association end is a property of the class it starts from: `Buyer.PaymentMethods` (`association.missing` in eShopCustom's terms). Its `member.type` and `member.nullable` compare it, and `relationship.multiplicity` the end with no navigation.

```json case
{ "phase": 1, "kinds": ["member.exists"], "traces": ["ESC-DOM-54"], "requirements": ["R2"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "buyer", "type": "Class", "name": "Buyer",
            "children": [
              { "id": "buyer-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" } }
            ] },
          { "id": "payment-method", "type": "Class", "name": "PaymentMethod",
            "children": [
              { "id": "payment-method-id", "type": "Attribute", "name": "Id", "typeRef": { "type": "int" } }
            ] }
        ]
      }],
      "associations": [
        { "id": "buyer-payment-methods", "type": "Association",
          "source": { "element": "buyer", "name": "Buyer", "multiplicity": "1", "navigable": false },
          "target": { "element": "payment-method", "name": "PaymentMethods", "multiplicity": "*", "navigable": true } }
      ]
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
    private readonly List<PaymentMethod> _paymentMethods = new();

    public int Id { get; private set; }

    public IEnumerable<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();
}
```

```csharp file src/Domain/PaymentMethod.cs
namespace Ordering.Domain;

public class PaymentMethod
{
    public int Id { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "buyer-payment-methods", "outcome": "verified", "at": "src/Domain/Buyer.cs:9" }
  ]
}
```

## association-navigation-missing

`Buyer` has no `PaymentMethods` property, so the navigable end isn't in the code.

```json case
{ "phase": 1, "base": "association-navigation", "kinds": ["member.exists"], "traces": ["ESC-DOM-54"], "requirements": ["R8"], "packs": ["intent.core", "intent.domain"], "language": "csharp" }
```

```csharp file src/Domain/Buyer.cs
namespace Ordering.Domain;

public class Buyer
{
    public int Id { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "buyer-payment-methods", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "buyer-payment-methods", "messageContains": "PaymentMethods" }
  ]
}
```

## python-attribute-constructor-and-operation

The same claims over Python, named by the Python profile: an attribute is a snake-case field, declared in the class (`quantity_on_hand`) or assigned to `self` in `__init__` (`product_id`); the modelled constructor is `__init__`; an operation is its snake-case method, `receive` for `Receive`. `self` is no parameter.

```json case
{ "phase": 4, "kinds": ["member.exists"], "traces": ["ESC-DOM-33"], "requirements": ["R2", "R33", "R41", "R42"], "packs": ["intent.core", "intent.domain", "intent.python"], "language": "python" }
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "stock-item", "type": "Class", "name": "StockItem",
            "children": [
              { "id": "stock-item-product-id", "type": "Attribute", "name": "ProductId", "typeRef": { "type": "guid" } },
              { "id": "stock-item-quantity-on-hand", "type": "Attribute", "name": "QuantityOnHand", "typeRef": { "type": "int" } },
              { "id": "stock-item-ctor", "type": "Class Constructor", "name": "StockItem",
                "children": [
                  { "id": "stock-item-ctor-product-id", "type": "Parameter", "name": "productId", "typeRef": { "type": "guid" } }
                ] },
              { "id": "stock-item-receive", "type": "Operation", "name": "Receive",
                "children": [
                  { "id": "stock-item-receive-quantity", "type": "Parameter", "name": "quantity", "typeRef": { "type": "int" } },
                  { "id": "stock-item-receive-note", "type": "Parameter", "name": "note", "typeRef": { "type": "string" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```python file src/shop/domain/stock_item.py
import uuid


class StockItem:
    quantity_on_hand: int

    def __init__(self, product_id: uuid.UUID) -> None:
        self.product_id = product_id
        self.quantity_on_hand = 0

    def receive(self, quantity: int, note: str) -> None:
        self.quantity_on_hand += quantity
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "stock-item-product-id", "outcome": "verified", "at": "src/shop/domain/stock_item.py:8" },
    { "kind": "member.exists", "element": "stock-item-quantity-on-hand", "outcome": "verified", "at": "src/shop/domain/stock_item.py:5" },
    { "kind": "member.exists", "element": "stock-item-ctor", "outcome": "verified", "at": "src/shop/domain/stock_item.py:7" },
    { "kind": "member.exists", "element": "stock-item-receive", "outcome": "verified", "at": "src/shop/domain/stock_item.py:11" }
  ]
}
```

## python-operation-missing

`StockItem` has no `receive` method: the modelled operation is contradicted.

```json case
{ "phase": 4, "base": "python-attribute-constructor-and-operation", "kinds": ["member.exists"], "traces": ["ESC-DOM-33"], "requirements": ["R8"], "packs": ["intent.core", "intent.domain", "intent.python"], "language": "python" }
```

```python file src/shop/domain/stock_item.py
import uuid


class StockItem:
    quantity_on_hand: int

    def __init__(self, product_id: uuid.UUID) -> None:
        self.product_id = product_id
        self.quantity_on_hand = 0
```

```json expect
{
  "claims": [
    { "kind": "member.exists", "element": "stock-item-ctor", "outcome": "verified", "at": "src/shop/domain/stock_item.py:7" },
    { "kind": "member.exists", "element": "stock-item-receive", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/member.exists", "severity": "error", "subject": "stock-item-receive", "messageContains": "isn't in the code" }
  ]
}
```
