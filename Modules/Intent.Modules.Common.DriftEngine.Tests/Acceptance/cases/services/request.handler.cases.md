# request.handler

A Command against its MediatR handler: exactly one class implements `IRequestHandler<X, R>` for it, with the Command's own result as R (`IRequestHandler<X>` for a Command with no type). The claim is the Command's, from the Services designer's stock vocabulary. Finding the handler means searching every file and comparing a type argument, so the rule is a plug-in of the MediatR pack, which arrives `later`. `request.handler` is an existence kind: a Command nothing handles is contradicted (R8), and two handlers are two candidates where one is expected.

The fixtures adapt eShopCustom's `Ordering.API/Application/Commands`.

## verified

`CancelOrderCommandHandler` implements `IRequestHandler<CancelOrderCommand, bool>`, and the Command returns `bool`. Traces ESC-SVC-52.

```json case
{ "phase": "later", "kinds": ["request.handler"], "traces": ["ESC-SVC-52"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.mediatr"], "language": "csharp" }
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
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:3" }
  ]
}
```

## no-handler-contradicted

The handler's file is gone, so nothing handles `CancelOrderCommand` and MediatR would throw when it is sent. Traces ESC-SVC-52.

```json case
{ "phase": "later", "base": "verified", "without": ["src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs"], "kinds": ["request.handler"], "traces": ["ESC-SVC-52"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command", "messageContains": "isn't in the code" }
  ]
}
```

## handler-of-another-result-contradicted

The only handler implements `IRequestHandler<CancelOrderCommand, int>`, while the Command returns `bool`. That isn't the Command's handler, so the handler the model states isn't in the code. Traces ESC-SVC-52 (the same R).

```json case
{ "phase": "later", "base": "verified", "kinds": ["request.handler"], "traces": ["ESC-SVC-52"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public class CancelOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<CancelOrderCommand, int>
{
    public async Task<int> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetAsync(command.OrderNumber);
        orderToUpdate.SetCancelledStatus();
        return await orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command" }
  ]
}
```

## second-handler-ambiguous

A second class also implements `IRequestHandler<CancelOrderCommand, bool>`. MediatR resolves only one; eShopCustom compared the first and warned about the second. Here one handler is expected and two match, so the claim is can't verify, `ambiguous`, and the finding names both. Traces ESC-SVC-52 (a second handler).

```json case
{ "phase": "later", "base": "verified", "kinds": ["request.handler"], "cases": ["engine/ambiguous"], "traces": ["ESC-SVC-52"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.mediatr"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/LegacyCancelOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public class LegacyCancelOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<CancelOrderCommand, bool>
{
    public async Task<bool> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetAsync(command.OrderNumber);
        orderToUpdate.SetCancelledStatus();
        return await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "cantVerify", "reason": "ambiguous" }
  ],
  "findings": [
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command", "messageContains": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:3" },
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command", "messageContains": "src/Ordering.API/Application/Commands/LegacyCancelOrderCommandHandler.cs:3" }
  ]
}
```
