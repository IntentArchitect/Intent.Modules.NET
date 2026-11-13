# optin/unmodelled.exposed

The opt-in rule that reports an in-scope type an endpoint exposes, a request or response type, that no element describes (rules.md, "Opt-in must be modelled rules"). It replaces eShopCustom Services' exposure check: a type that crosses the API boundary should be in the model, so a consumer's contract and the code can't part ways unseen. By principle 1 the rule is off until the Root Folder's `Drift Check` lists it under `Opt-in Rules`.

A pack's `exposed` conventions say where a type is exposed: each find ends at a parameter, and the rule follows the parameter's type to its declaration, which needs the reader's `declared` resolution. A type an element describes is located by that element's claims and is never reported. A finding's subject is the file and the line where the exposed type is declared.

## off-by-default

The `Orders` router maps two endpoints. `CancelOrderCommand`, the cancel endpoint's body, is a modelled DTO; `ShipOrderCommand`, the ship endpoint's body, isn't modelled. No `Opt-in Rules` row turns the rule on, so nothing is reported (principle 1).

```json case
{ "phase": 4, "cases": ["optin/unmodelled.exposed"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "cancel-order-command-dto", "type": "DTO", "name": "CancelOrderCommand",
            "children": [
              { "id": "cancel-order-command-dto-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "orders", "type": "Service", "name": "Orders",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } },
                "children": [
                  { "id": "cancel-order-command", "type": "Parameter", "name": "command", "typeRef": { "element": "cancel-order-command-dto" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Body" } } }
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

```json pack
{
  "id": "house.ordering",
  "version": "1.0.0",
  "requires": [{ "id": "intent.aspnetcore", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "exposed": [
        { "kind": "request body", "find": "type with \\.MapGroup\\( > method > parameter with \\[FromBody\\]", "files": "src/**/Apis/**/*.cs", "note": "A router handler's body parameter; the rule follows its type" }
      ]
    }
  }
}
```

```csharp file src/Ordering.API/Apis/OrdersApi.cs
namespace Ordering.API.Apis;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapPut("/cancel", CancelOrderAsync);
        api.MapPut("/ship", ShipOrderAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CancelOrderAsync([FromBody] CancelOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Cancel order failed to process.");
    }

    public static async Task<Results<Ok, BadRequest<string>>> ShipOrderAsync([FromBody] ShipOrderCommand command, [AsParameters] OrderServices services)
    {
        var commandResult = await services.Mediator.Send(command);
        return commandResult ? TypedResults.Ok() : TypedResults.BadRequest("Ship order failed to process.");
    }
}
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommand.cs
namespace Ordering.API.Application.Commands;

public record CancelOrderCommand(int OrderNumber) : IRequest<bool>;
```

```csharp file src/Ordering.Contracts/Commands/ShipOrderCommand.cs
namespace Ordering.Contracts.Commands;

public record ShipOrderCommand(int OrderNumber) : IRequest<bool>;
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.exposed", "count": 0, "note": "The rule is off" }
  ]
}
```

## on

The same model and code, with the rule on. `ShipOrderCommand` is the body of an endpoint and no element describes it, so it is reported, an error by default (`services.exposure.unmodelled` in eShopCustom). `CancelOrderCommand` is located by its DTO's `type.exists` claim. eShopCustom also followed exposed types transitively, through their properties, and exposed commands and messages as well as endpoints; a convention's find ends at the place a type is exposed, and the spec doesn't say whether the rule follows that type's own members further, so this case shows the direct exposure.

```json case
{ "phase": 4, "base": "off-by-default", "cases": ["optin/unmodelled.exposed"], "traces": ["ESC-SVC-57"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "cancel-order-command-dto", "type": "DTO", "name": "CancelOrderCommand",
            "children": [
              { "id": "cancel-order-command-dto-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "orders", "type": "Service", "name": "Orders",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } },
                "children": [
                  { "id": "cancel-order-command", "type": "Parameter", "name": "command", "typeRef": { "element": "cancel-order-command-dto" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Body" } } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.exposed" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "cancel-order-command-dto", "outcome": "verified", "at": "src/Ordering.API/Application/Commands/CancelOrderCommand.cs:3" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.exposed", "severity": "error", "subject": "src/Ordering.Contracts/Commands/ShipOrderCommand.cs:3", "messageContains": "ShipOrderCommand" },
    { "rule": "optin/unmodelled.exposed", "subject": "src/Ordering.API/Application/Commands/CancelOrderCommand.cs:3", "count": 0 }
  ]
}
```

## out-of-scope

The rule is on, but `ShipOrderCommand` is declared in the shared contracts project, which a scope row excludes. The endpoint exposing it is in scope, but the type isn't: opt-in rules report only in-scope code (R49), and the excluded file is never read (R22), so nothing is reported.

```json case
{ "phase": 4, "base": "off-by-default", "cases": ["optin/unmodelled.exposed", "scope/gate"], "requirements": ["R22", "R49"], "packs": ["intent.core", "intent.services", "intent.aspnetcore", "house.ordering"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.Services",
        "elements": [
          { "id": "cancel-order-command-dto", "type": "DTO", "name": "CancelOrderCommand",
            "children": [
              { "id": "cancel-order-command-dto-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
            ] },
          { "id": "orders", "type": "Service", "name": "Orders",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                "stereotypes": { "Http Settings": { "Verb": "PUT", "Route": "cancel" } },
                "children": [
                  { "id": "cancel-order-command", "type": "Parameter", "name": "command", "typeRef": { "element": "cancel-order-command-dto" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Body" } } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [
            { "Glob": "src/**", "Scope": "In" },
            { "Glob": "src/Ordering.Contracts/**", "Scope": "Out", "Reason": "Shared contracts, modelled in the Contracts team's own application" }
          ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.exposed" } ] }
        } }]
    }
  }
}
```

```json expect
{
  "findings": [
    { "rule": "optin/unmodelled.exposed", "count": 0 }
  ],
  "filesRead": { "include": ["src/Ordering.API/Apis/OrdersApi.cs"], "exclude": ["src/Ordering.Contracts/**"] }
}
```

## java-request-body

The Java spike's check: every record the API exposes is a DTO. The house convention finds every `@RequestBody` parameter of a controller. `RemoveWishlistItemRequest` is a modelled DTO; `AddWishlistItemRequest` isn't, so it is reported. The spike also followed return types and record components, and skipped generic types; the spec doesn't say which type the rule follows when a find ends at a method, or whether it follows an exposed type's members, so this case shows request bodies.

```json case
{ "phase": 4, "cases": ["optin/unmodelled.exposed"], "traces": ["JAV-SVC-21"], "requirements": ["R49"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc", "house.backend"], "language": "java" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "eShop.Services",
        "elements": [
          { "id": "remove-wishlist-item-request", "type": "DTO", "name": "RemoveWishlistItemRequest",
            "children": [
              { "id": "remove-wishlist-item-request-product-id", "type": "DTO-Field", "name": "ProductId", "typeRef": { "type": "guid" } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop.Backend", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.exposed" } ] }
        } }]
    }
  }
}
```

```json pack
{
  "id": "house.backend",
  "version": "1.0.0",
  "requires": [{ "id": "intent.springmvc", "version": "^1.0.0" }],
  "languages": ["java"],
  "designers": ["Services"],
  "conventions": {
    "Services": {
      "exposed": [
        { "kind": "request body", "find": "type with @RestController\\b > method > parameter with @RequestBody\\b", "files": "src/main/java/**/api/**/*.java" }
      ]
    }
  }
}
```

```java file src/main/java/com/eshop/backend/api/WishlistsController.java
package com.eshop.backend.api;

import com.eshop.backend.application.WishlistService;
import java.util.UUID;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class WishlistsController {
    private final WishlistService wishlists;

    public WishlistsController(WishlistService wishlists) {
        this.wishlists = wishlists;
    }

    @PostMapping("/api/customers/{customerId}/wishlist")
    public void addItem(@PathVariable UUID customerId, @RequestBody AddWishlistItemRequest request) {
        wishlists.addItem(customerId, request.productId());
    }

    @PostMapping("/api/customers/{customerId}/wishlist/remove")
    public void removeItem(@PathVariable UUID customerId, @RequestBody RemoveWishlistItemRequest request) {
        wishlists.removeItem(customerId, request.productId());
    }
}
```

```java file src/main/java/com/eshop/backend/api/AddWishlistItemRequest.java
package com.eshop.backend.api;

import java.util.UUID;

public record AddWishlistItemRequest(UUID productId) {
}
```

```java file src/main/java/com/eshop/backend/api/RemoveWishlistItemRequest.java
package com.eshop.backend.api;

import java.util.UUID;

public record RemoveWishlistItemRequest(UUID productId) {
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "remove-wishlist-item-request", "outcome": "verified", "at": "src/main/java/com/eshop/backend/api/RemoveWishlistItemRequest.java:5" }
  ],
  "findings": [
    { "rule": "optin/unmodelled.exposed", "severity": "error", "subject": "src/main/java/com/eshop/backend/api/AddWishlistItemRequest.java:5", "messageContains": "AddWishlistItemRequest" },
    { "rule": "optin/unmodelled.exposed", "subject": "src/main/java/com/eshop/backend/api/RemoveWishlistItemRequest.java:5", "count": 0 }
  ]
}
```
