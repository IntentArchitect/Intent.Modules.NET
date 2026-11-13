# request.returns

A Command's type (its typeRef) against the result of the MediatR request it is in the code: `IRequest<R>` returns R, and `IRequest` or `IRequest<Unit>` returns nothing, which a Command with no type states. The `type` comparer ignores nullability (decision 16), and no kind compares a request result's nullability, so eShopCustom's check of it has no case here. The claim is the Command's, from the Services designer's stock vocabulary; reading R means taking the request interface apart and mapping `Unit` to nothing, so the rule is a plug-in of the MediatR pack, which arrives `later`. `request.returns` is `stated: always` with the `type` comparer.

The fixtures adapt eShopCustom's `Ordering.API/Application/Commands`.

## verified

`CancelOrderCommand` returns `bool`, and the record is `IRequest<bool>`. Traces ESC-SVC-51.

```json case
{ "phase": "later", "kinds": ["request.returns"], "traces": ["ESC-SVC-51"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "cancel-order-command", "type": "Command", "name": "CancelOrderCommand", "typeRef": { "type": "bool" },
            "children": [
              { "id": "cancel-order-command-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
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

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommand.cs
namespace eShop.Ordering.API.Application.Commands;

public record CancelOrderCommand(int OrderNumber) : IRequest<bool>;
```

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "cancel-order-command", "outcome": "verified",
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommand.cs:3" }
  ]
}
```

## unit-is-none

`ShipOrderCommand` has no type, and the record is `IRequest<Unit>`: MediatR's `Unit` is its "nothing", so both sides say the request returns nothing. Traces ESC-SVC-51 (Unit means none).

```json case
{ "phase": "later", "kinds": ["request.returns"], "traces": ["ESC-SVC-51"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "ship-order-command", "type": "Command", "name": "ShipOrderCommand",
            "children": [
              { "id": "ship-order-command-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
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

```csharp file src/Ordering.API/Application/Commands/ShipOrderCommand.cs
namespace eShop.Ordering.API.Application.Commands;

public record ShipOrderCommand(int OrderNumber) : IRequest<Unit>;
```

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "ship-order-command", "outcome": "verified",
      "at": "src/Ordering.API/Application/Commands/ShipOrderCommand.cs:3" }
  ]
}
```

## type-contradicted

The model says `CancelOrderCommand` returns an `int`; the record is `IRequest<bool>`. Traces ESC-SVC-51.

```json case
{ "phase": "later", "base": "verified", "kinds": ["request.returns"], "traces": ["ESC-SVC-51"], "requirements": ["R1", "R42"], "packs": ["intent.core", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "cancel-order-command", "type": "Command", "name": "CancelOrderCommand", "typeRef": { "type": "int" },
            "children": [
              { "id": "cancel-order-command-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
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

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "cancel-order-command", "outcome": "contradicted", "expected": "int", "actual": "bool",
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommand.cs:3" }
  ],
  "findings": [
    { "rule": "claim/request.returns", "severity": "error", "subject": "cancel-order-command", "messageContains": "bool" }
  ]
}
```

## nullability-ignored

The model says `CancelOrderCommand` returns a nullable `bool`; the record is `IRequest<bool>`. The `type` comparer ignores nullability, so the claim is verified, and no other claim reports the difference (decision 16).

```json case
{ "phase": "later", "base": "verified", "kinds": ["request.returns"], "requirements": ["R2", "R42"], "packs": ["intent.core", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "cancel-order-command", "type": "Command", "name": "CancelOrderCommand", "typeRef": { "type": "bool", "nullable": true },
            "children": [
              { "id": "cancel-order-command-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
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

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "cancel-order-command", "outcome": "verified", "expected": "bool?", "actual": "bool",
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommand.cs:3" }
  ],
  "findings": [
    { "rule": "claim/request.returns", "count": 0 }
  ]
}
```

## result-where-model-has-none

`ShipOrderCommand` has no type, but the record is `IRequest<bool>`. eShopCustom reported a result the model lacks as Wrong; `request.returns` is `always`, so a Command with no type states "nothing", and the code contradicts it. Traces ESC-SVC-51.

```json case
{ "phase": "later", "base": "unit-is-none", "kinds": ["request.returns"], "traces": ["ESC-SVC-51"], "requirements": ["R1", "R3"], "packs": ["intent.core", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/ShipOrderCommand.cs
namespace eShop.Ordering.API.Application.Commands;

public record ShipOrderCommand(int OrderNumber) : IRequest<bool>;
```

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "ship-order-command", "outcome": "contradicted",
      "at": "src/Ordering.API/Application/Commands/ShipOrderCommand.cs:3" }
  ],
  "findings": [
    { "rule": "claim/request.returns", "severity": "error", "subject": "ship-order-command", "messageContains": "bool" }
  ]
}
```

## no-rule

Without the MediatR pack nothing reads a request's result, so the claim is can't verify, `noRule`.

```json case
{ "phase": "later", "base": "verified", "kinds": ["request.returns"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.services"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "cancel-order-command", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/request.returns", "severity": "error", "subject": "cancel-order-command", "messageContains": "no rule" }
  ]
}
```
