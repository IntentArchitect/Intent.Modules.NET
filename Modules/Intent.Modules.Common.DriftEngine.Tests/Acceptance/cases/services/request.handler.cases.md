# request.handler

A Command against its MediatR handler: exactly one class implements `IRequestHandler<X, R>` for it, with the Command's own result as R (`IRequestHandler<X>` for a Command with no type). The claim is the Command's, from the Services designer's stock vocabulary. Finding the handler means searching every file and comparing a type argument, so the rule is a plug-in of the MediatR pack, which arrives `later`. `request.handler` is an existence kind: a Command nothing handles is contradicted (R8), and two handlers are two candidates where one is expected.

The fixtures adapt eShopCustom's `Ordering.API/Application/Commands`.

Mediator (`intent.mediator`) is read the same way, with each request interface's own handler interface: `ICommandHandler<X, R>` for an `ICommand<R>`, `IQueryHandler<X, R>` for an `IQuery<R>`, `IRequestHandler<X, R>` for an `IRequest<R>`. Wolverine (`intent.wolverine`) has no handler interface: its handler is the one public `Handle`, `HandleAsync`, `Consume` or `ConsumeAsync` method (also `Handles` and `Consumes`), static or instance, whose first parameter is the request, on a public, concrete, non-generic class whose name ends in `Handler` or `Consumer`, or that `[WolverineHandler]` or `IWolverineHandler` marks. A framework reads a request only when the framework is in the code, and a request is read by one framework only.

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

## mediatr-request-beside-mediator

The handler's file is gone, and the case names both MediatR and Mediator, Mediator last so its reader runs first. The record's `IRequest<bool>` is written where the code imports neither framework, which is MediatR's, as before Mediator was read, so the finding is MediatR's.

```json case
{ "phase": "later", "base": "verified", "without": ["src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs"], "kinds": ["request.handler"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.mediatr", "intent.mediator"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command", "messageContains": "so MediatR can't handle it" }
  ]
}
```

## mediator-verified

`CancelOrderCommandHandler` implements Mediator's `ICommandHandler<CancelOrderCommand, bool>`, and the Command is `ICommand<bool>`. Both files import `Mediator`.

```json case
{ "phase": "later", "kinds": ["request.handler"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.mediator"], "language": "csharp" }
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
using Mediator;

namespace eShop.Ordering.API.Application.Commands;

public sealed record CancelOrderCommand(int OrderNumber) : ICommand<bool>;
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs
using Mediator;

namespace eShop.Ordering.API.Application.Commands;

public sealed class CancelOrderCommandHandler(IOrderRepository orderRepository) : ICommandHandler<CancelOrderCommand, bool>
{
    public async ValueTask<bool> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
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
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:5" }
  ]
}
```

## mediator-unit-verified

`ShipOrderCommand` has no type, the record is Mediator's `ICommand`, and `ShipOrderCommandHandler` implements `ICommandHandler<ShipOrderCommand>`, returning `ValueTask<Unit>`.

```json case
{ "phase": "later", "kinds": ["request.handler"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.mediator"], "language": "csharp" }
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
using Mediator;

namespace eShop.Ordering.API.Application.Commands;

public sealed record ShipOrderCommand(int OrderNumber) : ICommand;
```

```csharp file src/Ordering.API/Application/Commands/ShipOrderCommandHandler.cs
using Mediator;

namespace eShop.Ordering.API.Application.Commands;

public sealed class ShipOrderCommandHandler(IOrderRepository orderRepository) : ICommandHandler<ShipOrderCommand>
{
    public async ValueTask<Unit> Handle(ShipOrderCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetAsync(command.OrderNumber);
        orderToUpdate.SetShippedStatus();
        await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Unit.Value;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "ship-order-command", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/Commands/ShipOrderCommandHandler.cs:5" }
  ]
}
```

## mediator-no-handler-contradicted

The handler's file is gone, so nothing handles `CancelOrderCommand`, and the finding names the handler interface Mediator would need.

```json case
{ "phase": "later", "base": "mediator-verified", "without": ["src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs"], "kinds": ["request.handler"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.mediator"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command", "messageContains": "no class implements `ICommandHandler<CancelOrderCommand, bool>`" }
  ]
}
```

## mediator-handler-of-another-result-contradicted

The only handler implements `ICommandHandler<CancelOrderCommand, int>`, while the Command is `ICommand<bool>`: not the Command's handler.

```json case
{ "phase": "later", "base": "mediator-verified", "kinds": ["request.handler"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.mediator"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs
using Mediator;

namespace eShop.Ordering.API.Application.Commands;

public sealed class CancelOrderCommandHandler(IOrderRepository orderRepository) : ICommandHandler<CancelOrderCommand, int>
{
    public async ValueTask<int> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
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
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command", "messageContains": "which Mediator never uses for `ICommand<bool>`" }
  ]
}
```

## mediator-request-beside-mediatr

The case names both frameworks, MediatR last so its reader runs first. The record is `IRequest<bool>`, a name both frameworks have, in a file that imports `Mediator`, so it is Mediator's, and with no handler the finding is Mediator's, not MediatR's.

```json case
{ "phase": "later", "base": "mediator-verified", "without": ["src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs"], "kinds": ["request.handler"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.mediator", "intent.mediatr"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommand.cs
using Mediator;

namespace eShop.Ordering.API.Application.Commands;

public sealed record CancelOrderCommand(int OrderNumber) : IRequest<bool>;
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command", "messageContains": "`IRequestHandler<CancelOrderCommand, bool>`, so Mediator can't handle it" }
  ]
}
```

## mediator-leaves-mediatr-request-unread

The request and its handler are MediatR's: both files import `MediatR`. With only the Mediator extension, nothing reads the handler, though Mediator has an `IRequestHandler<X, R>` of the same name: can't verify, `noRule`.

```json case
{ "phase": "later", "base": "verified", "kinds": ["request.handler"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.mediator"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommand.cs
using MediatR;

namespace eShop.Ordering.API.Application.Commands;

public record CancelOrderCommand(int OrderNumber) : IRequest<bool>;
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs
using MediatR;

namespace eShop.Ordering.API.Application.Commands;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
{
    public Task<bool> Handle(CancelOrderCommand command, CancellationToken cancellationToken) => Task.FromResult(true);
}
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command", "messageContains": "intent.mediator" }
  ]
}
```

## wolverine-verified

The record implements nothing; `CancelOrderCommandHandler` is a class Wolverine discovers by its name, and its `Handle` method's first parameter is the command. Wolverine is in the code: Program.cs imports it. The handler is the method.

```json case
{ "phase": "later", "kinds": ["request.handler"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
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

```csharp file src/Ordering.API/Program.cs
using Wolverine;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWolverine();

var app = builder.Build();
app.Run();
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommand.cs
namespace eShop.Ordering.API.Application.Commands;

public record CancelOrderCommand(int OrderNumber);
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public class CancelOrderCommandHandler
{
    public async Task<bool> Handle(CancelOrderCommand command, IOrderRepository orderRepository, CancellationToken cancellationToken)
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
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:5" }
  ]
}
```

## wolverine-static-handler-verified

The handler is a static method of a static class.

```json case
{ "phase": "later", "base": "wolverine-verified", "kinds": ["request.handler"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public static class CancelOrderCommandHandler
{
    public static bool Handle(CancelOrderCommand command)
    {
        return command.OrderNumber > 0;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:5" }
  ]
}
```

## wolverine-handle-async-verified

The handler is a `HandleAsync` method on a class whose name ends in `Consumer`, the other suffix Wolverine discovers.

```json case
{ "phase": "later", "base": "wolverine-verified", "without": ["src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs"], "kinds": ["request.handler"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderConsumer.cs
namespace eShop.Ordering.API.Application.Commands;

public class CancelOrderConsumer(IOrderRepository orderRepository)
{
    public async Task<bool> HandleAsync(CancelOrderCommand command, CancellationToken cancellationToken)
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
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/Commands/CancelOrderConsumer.cs:5" }
  ]
}
```

## wolverine-attribute-handler-verified

`OrderCancellation` fits no naming convention, but `[WolverineHandler]` marks it, so Wolverine discovers its `Handle` method.

```json case
{ "phase": "later", "base": "wolverine-verified", "without": ["src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs"], "kinds": ["request.handler"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/OrderCancellation.cs
using Wolverine.Attributes;

namespace eShop.Ordering.API.Application.Commands;

[WolverineHandler]
public class OrderCancellation
{
    public bool Handle(CancelOrderCommand command) => command.OrderNumber > 0;
}
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/Commands/OrderCancellation.cs:8" }
  ]
}
```

## wolverine-no-handler-contradicted

The handler's file is gone, so nothing handles `CancelOrderCommand`. This is the one finding: `request.returns`, which reads its result from the handler, is `blocked` and makes none.

```json case
{ "phase": "later", "base": "wolverine-verified", "without": ["src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs"], "kinds": ["request.handler"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command", "messageContains": "no Wolverine handler takes `CancelOrderCommand`" },
    { "rule": "claim/request.returns", "count": 0 }
  ]
}
```

## wolverine-undiscovered-class-contradicted

The method that takes the command is on `CancelOrderService`, whose name ends in neither `Handler` nor `Consumer` and which nothing marks, so Wolverine never discovers it: the handler isn't in the code, and the finding names the class Wolverine skips.

```json case
{ "phase": "later", "base": "wolverine-verified", "without": ["src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs"], "kinds": ["request.handler"], "requirements": ["R8"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderService.cs
namespace eShop.Ordering.API.Application.Commands;

public class CancelOrderService
{
    public bool Handle(CancelOrderCommand command) => command.OrderNumber > 0;
}
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command", "messageContains": "Wolverine doesn't discover `CancelOrderService`" },
    { "rule": "claim/request.returns", "count": 0 }
  ]
}
```

## wolverine-second-handler-ambiguous

A second class's `Handle` also takes `CancelOrderCommand`. Wolverine runs both, and the model states one handler, so the claim is can't verify, `ambiguous`, naming both. Both return `bool`, so the result itself is verified and makes no finding.

```json case
{ "phase": "later", "base": "wolverine-verified", "kinds": ["request.handler"], "cases": ["engine/ambiguous"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/LegacyCancelOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public class LegacyCancelOrderCommandHandler
{
    public async Task<bool> Handle(CancelOrderCommand command, IOrderRepository orderRepository, CancellationToken cancellationToken)
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
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "cantVerify", "reason": "ambiguous" },
    { "kind": "request.returns", "element": "cancel-order-command", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command", "messageContains": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:5" },
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command", "messageContains": "src/Ordering.API/Application/Commands/LegacyCancelOrderCommandHandler.cs:5" },
    { "rule": "claim/request.returns", "count": 0 }
  ]
}
```

## wolverine-absent-no-rule

Program.cs is gone, and nothing else imports Wolverine, so the class that fits its conventions isn't read as its handler: can't verify, `noRule`.

```json case
{ "phase": "later", "base": "wolverine-verified", "without": ["src/Ordering.API/Program.cs"], "kinds": ["request.handler"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/request.handler", "severity": "error", "subject": "cancel-order-command", "messageContains": "intent.wolverine" }
  ]
}
```

## mediatr-request-beside-wolverine

A codebase with both: Program.cs uses Wolverine, and `CancelOrderCommand` is MediatR's `IRequest<bool>`, whose handler also fits Wolverine's conventions. A MediatR request is MediatR's to read, so the handler is the class MediatR finds, not the method Wolverine would. The Wolverine extension is listed last so its reader runs first.

```json case
{ "phase": "later", "base": "verified", "kinds": ["request.handler"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.mediatr", "intent.wolverine"], "language": "csharp" }
```

```csharp file src/Ordering.API/Program.cs
using Wolverine;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWolverine();

var app = builder.Build();
app.Run();
```

```json expect
{
  "claims": [
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:3" }
  ]
}
```
