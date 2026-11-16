# command.identified

Whether a Command is an Identified Command, against the code: eShopCustom handles an Identified Command twice, once with its ordinary MediatR handler and once with a class deriving its own `IdentifiedCommandHandler<X, R>`, which drops a request id it has already seen. The model side is eShopCustom's module stereotype `Identified Command`, so the house pack brings the vocabulary (`true` when the stereotype is applied, else `false`), and its module's claim reader (`house.readers`) reads the code: a type deriving `IdentifiedCommandHandler<X, …>` reads `true`, and none reads `false`. `command.identified` is `stated: always`, so it is compared both ways and the code's silence states `false`. It arrives `later`, with the other MediatR kinds.

The fixtures adapt eShopCustom's `Ordering.API/Application/Commands/CancelOrderCommandHandler.cs`.

## verified

`CancelOrderCommand` is an Identified Command, and `CancelOrderIdentifiedCommandHandler` derives `IdentifiedCommandHandler<CancelOrderCommand, bool>`. Traces ESC-SVC-53.

```json case
{ "phase": "later", "kinds": ["command.identified"], "traces": ["ESC-SVC-53"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "cancel-order-command", "type": "Command", "name": "CancelOrderCommand", "typeRef": { "type": "bool" },
            "stereotypes": { "Identified Command": { "Validated": false } },
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
          "kind": "command.identified",
          "for": { "element": "Command" },
          "value": "hasStereotype('Identified Command')",
          "bindings": { "Command": "name" },
          "source": "the Command's Identified Command stereotype"
        }
      ]
    }
  }
}
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public class CancelOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<CancelOrderCommand, bool>
{
    public async Task<bool> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }

        orderToUpdate.SetCancelledStatus();
        return await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}

// Use for Idempotency in Command process
public class CancelOrderIdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager, ILogger<CancelOrderIdentifiedCommandHandler> logger) : IdentifiedCommandHandler<CancelOrderCommand, bool>(mediator, requestManager, logger)
{
    protected override bool CreateResultForDuplicateRequest()
    {
        return true; // Ignore duplicate requests for processing order.
    }
}
```

```json expect
{
  "claims": [
    { "kind": "command.identified", "element": "cancel-order-command", "outcome": "verified", "expected": "true", "actual": "true",
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:19" }
  ]
}
```

## no-identified-handler-contradicted

The Command is still an Identified Command, but only its ordinary handler is left. Nothing derives `IdentifiedCommandHandler<CancelOrderCommand, …>`, so the code states `false`, from silence. Traces ESC-SVC-53.

```json case
{ "phase": "later", "base": "verified", "kinds": ["command.identified"], "traces": ["ESC-SVC-53"], "requirements": ["R1", "R3"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public class CancelOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<CancelOrderCommand, bool>
{
    public async Task<bool> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }

        orderToUpdate.SetCancelledStatus();
        return await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "command.identified", "element": "cancel-order-command", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/command.identified", "severity": "error", "subject": "cancel-order-command" }
  ]
}
```

## identified-handler-unmodelled-contradicted

The Command has no `Identified Command` stereotype, but `CancelOrderIdentifiedCommandHandler` is in the code. eShopCustom reported this direction as Backlog; `command.identified` is `always`, so the model's `false` is a claim and the code contradicts it. Traces ESC-SVC-53 (both ways).

```json case
{ "phase": "later", "base": "verified", "kinds": ["command.identified"], "traces": ["ESC-SVC-53"], "requirements": ["R1", "R3"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
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
    { "kind": "command.identified", "element": "cancel-order-command", "outcome": "contradicted", "expected": "false", "actual": "true",
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:19" }
  ],
  "findings": [
    { "rule": "claim/command.identified", "severity": "error", "subject": "cancel-order-command" }
  ]
}
```

## plain-command-verified-from-silence

The Command has no `Identified Command` stereotype and only its ordinary handler is in the code: both say it isn't an Identified Command, the code by saying nothing. Traces ESC-SVC-53.

```json case
{ "phase": "later", "base": "identified-handler-unmodelled-contradicted", "kinds": ["command.identified"], "traces": ["ESC-SVC-53"], "requirements": ["R3"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public class CancelOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<CancelOrderCommand, bool>
{
    public async Task<bool> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }

        orderToUpdate.SetCancelledStatus();
        return await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "command.identified", "element": "cancel-order-command", "outcome": "verified", "expected": "false", "actual": "false", "fromSilence": true }
  ]
}
```
