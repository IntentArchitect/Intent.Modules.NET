# request.validated

A Command's FluentValidation validator, against eShopCustom's module stereotypes: `Validated` on a Command states a validator deriving `AbstractValidator<X>`, and `Identified Command` with its `Validated` property checked states one deriving `AbstractValidator<IdentifiedCommand<X, R>>`, which validates the idempotency wrapper. Both stereotypes are eShopCustom's own, so the house pack brings the vocabulary (two entries for one kind, told apart by a discriminator), and its module's claim reader (`house.readers`) reads the validator from the code. `request.validated` is an existence kind: a stated validator the code lacks is contradicted (R8). A validator the model doesn't state is unmodelled code, which is never drift by itself (principle 1). The kind arrives `later`, with the other MediatR kinds.

The fixtures adapt eShopCustom's `Ordering.API/Application/Validations`.

## validated-verified

`CancelOrderCommand` is `Validated`, and `CancelOrderCommandValidator` derives `AbstractValidator<CancelOrderCommand>`. Traces ESC-SVC-55.

```json case
{ "phase": "later", "kinds": ["request.validated"], "traces": ["ESC-SVC-55"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "cancel-order-command", "type": "Command", "name": "CancelOrderCommand", "typeRef": { "type": "bool" },
            "stereotypes": { "Validated": {} },
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

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "vocabulary": {
    "Services": {
      "entries": [
        {
          "kind": "request.validated",
          "for": { "element": "Command", "where": "hasStereotype('Validated')" },
          "value": "'present'",
          "bindings": { "Validates": "name" },
          "discriminator": "'Validated'",
          "source": "the Command's Validated stereotype"
        },
        {
          "kind": "request.validated",
          "for": { "element": "Command", "where": "hasStereotype('Identified Command') && stereotype('Identified Command').Validated == true" },
          "value": "'present'",
          "bindings": { "Validates": "'IdentifiedCommand<' + name" },
          "discriminator": "'Identified Command'",
          "source": "the Command's Identified Command stereotype's Validated"
        }
      ]
    }
  }
}
```

```csharp file src/Ordering.API/Application/Validations/CancelOrderCommandValidator.cs
namespace eShop.Ordering.API.Application.Validations;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator(ILogger<CancelOrderCommandValidator> logger)
    {
        RuleFor(order => order.OrderNumber).NotEmpty().WithMessage("No orderId found");
    }
}
```

```json expect
{
  "claims": [
    { "kind": "request.validated", "element": "cancel-order-command", "key": "Validated", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/Validations/CancelOrderCommandValidator.cs:3" }
  ]
}
```

## validated-contradicted

The validator's file is gone, so nothing validates `CancelOrderCommand` before its handler runs. Traces ESC-SVC-55.

```json case
{ "phase": "later", "base": "validated-verified", "without": ["src/Ordering.API/Application/Validations/CancelOrderCommandValidator.cs"], "kinds": ["request.validated"], "traces": ["ESC-SVC-55"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "request.validated", "element": "cancel-order-command", "key": "Validated", "outcome": "contradicted", "expected": "present", "actual": "none" }
  ],
  "findings": [
    { "rule": "claim/request.validated", "severity": "error", "subject": "cancel-order-command", "messageContains": "isn't in the code" }
  ]
}
```

## identified-validated-verified

`CreateOrderCommand` is an Identified Command with `Validated` checked, and `IdentifiedCommandValidator` derives `AbstractValidator<IdentifiedCommand<CreateOrderCommand, bool>>`. The Command has no `Validated` stereotype of its own, so this is its only `request.validated` claim. Traces ESC-SVC-54.

```json case
{ "phase": "later", "base": "validated-verified", "without": ["src/Ordering.API/Application/Validations/CancelOrderCommandValidator.cs"], "kinds": ["request.validated"], "traces": ["ESC-SVC-54"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "create-order-command", "type": "Command", "name": "CreateOrderCommand", "typeRef": { "type": "bool" },
            "stereotypes": { "Identified Command": { "Validated": true } },
            "children": [
              { "id": "create-order-command-user-id", "type": "DTO-Field", "name": "UserId", "typeRef": { "type": "string" } }
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

```csharp file src/Ordering.API/Application/Validations/IdentifiedCommandValidator.cs
namespace eShop.Ordering.API.Application.Validations;

public class IdentifiedCommandValidator : AbstractValidator<IdentifiedCommand<CreateOrderCommand, bool>>
{
    public IdentifiedCommandValidator(ILogger<IdentifiedCommandValidator> logger)
    {
        RuleFor(command => command.Id).NotEmpty();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "request.validated", "element": "create-order-command", "key": "Identified Command", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/Validations/IdentifiedCommandValidator.cs:3" },
    { "kind": "request.validated", "element": "create-order-command", "key": "Validated", "absent": true }
  ]
}
```

## identified-validator-of-the-bare-command-contradicted

The validator validates `CreateOrderCommand` itself, not the `IdentifiedCommand<CreateOrderCommand, bool>` the endpoint sends, so MediatR never runs it on that request. The Identified Command's `Validated` states a validator of the wrapper, which isn't in the code. Traces ESC-SVC-54.

```json case
{ "phase": "later", "base": "identified-validated-verified", "kinds": ["request.validated"], "traces": ["ESC-SVC-54"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Validations/IdentifiedCommandValidator.cs
namespace eShop.Ordering.API.Application.Validations;

public class IdentifiedCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public IdentifiedCommandValidator(ILogger<IdentifiedCommandValidator> logger)
    {
        RuleFor(command => command.UserId).NotEmpty();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "request.validated", "element": "create-order-command", "key": "Identified Command", "outcome": "contradicted", "expected": "present", "actual": "none" },
    { "kind": "request.validated", "element": "create-order-command", "key": "Validated", "absent": true }
  ],
  "findings": [
    { "rule": "claim/request.validated", "severity": "error", "subject": "create-order-command", "messageContains": "isn't in the code" }
  ]
}
```

## unmodelled-validator-not-claimed

`CancelOrderCommand` has no `Validated` stereotype, and `CancelOrderCommandValidator` is in the code. eShopCustom reported the validator as Backlog; here there is no claim, and nothing is reported (principle 1). Traces ESC-SVC-55 (the code-to-model direction).

```json case
{ "phase": "later", "base": "validated-verified", "kinds": ["request.validated"], "traces": ["ESC-SVC-55"], "requirements": ["R4"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
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

```json expect
{
  "claims": [
    { "kind": "request.validated", "element": "cancel-order-command", "absent": true }
  ],
  "findings": [
    { "rule": "claim/request.validated", "count": 0 }
  ]
}
```
