# request.returns

A Command's type (its typeRef) against the result of the MediatR request it is in the code: `IRequest<R>` returns R, and `IRequest` or `IRequest<Unit>` returns nothing, which a Command with no type states. The `type` comparer ignores nullability (decision 16), and no kind compares a request result's nullability, so eShopCustom's check of it has no case here. The claim is the Command's, from the Services designer's stock vocabulary; reading R means taking the request interface apart and mapping `Unit` to nothing, so the rule is a plug-in of the MediatR pack, which arrives `later`. `request.returns` is `stated: always` with the `type` comparer.

The fixtures adapt eShopCustom's `Ordering.API/Application/Commands`.

Mediator (`intent.mediator`, martinothamar/Mediator) states it the same way, as `IRequest<R>`, `ICommand<R>` or `IQuery<R>`, with `IRequest`, `ICommand` and `Unit` for nothing; its interfaces share MediatR's names, so they are its only in code that imports `Mediator`. Wolverine (`intent.wolverine`) has no request interface: the result is what the handler Wolverine discovers returns, unwrapped from `Task<T>` or `ValueTask<T>`, and nothing for `void`, `Task` or `ValueTask`. Each reads only when its framework is in the code, and Wolverine never reads a MediatR or Mediator request.

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

## mediator-verified

`CancelOrderCommand` returns `bool`, and the record is Mediator's `ICommand<bool>`: its file imports `Mediator`.

```json case
{ "phase": "later", "kinds": ["request.returns"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.mediator"], "language": "csharp" }
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

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "cancel-order-command", "outcome": "verified", "expected": "bool", "actual": "bool",
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommand.cs:5" }
  ]
}
```

## mediator-query-verified

`GetOrderStatusQuery` returns `string`, and the record is Mediator's `IQuery<string>`.

```json case
{ "phase": "later", "kinds": ["request.returns"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.mediator"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "get-order-status-query", "type": "Query", "name": "GetOrderStatusQuery", "typeRef": { "type": "string" },
            "children": [
              { "id": "get-order-status-query-order-number", "type": "DTO-Field", "name": "OrderNumber", "typeRef": { "type": "int" } }
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

```csharp file src/Ordering.API/Application/Queries/GetOrderStatusQuery.cs
using Mediator;

namespace eShop.Ordering.API.Application.Queries;

public sealed record GetOrderStatusQuery(int OrderNumber) : IQuery<string>;
```

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "get-order-status-query", "outcome": "verified", "expected": "string", "actual": "string",
      "at": "src/Ordering.API/Application/Queries/GetOrderStatusQuery.cs:5" }
  ]
}
```

## mediator-unit-is-none

`ShipOrderCommand` has no type, and the record is Mediator's `ICommand`, which returns its `Unit`: both sides say the request returns nothing.

```json case
{ "phase": "later", "kinds": ["request.returns"], "requirements": ["R2", "R10"], "packs": ["intent.core", "intent.services", "intent.mediator"], "language": "csharp" }
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

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "ship-order-command", "outcome": "verified", "actual": "none",
      "at": "src/Ordering.API/Application/Commands/ShipOrderCommand.cs:5" }
  ]
}
```

## mediator-type-contradicted

The model says `CancelOrderCommand` returns an `int`; the record is Mediator's `ICommand<bool>`.

```json case
{ "phase": "later", "base": "mediator-verified", "kinds": ["request.returns"], "requirements": ["R1", "R42"], "packs": ["intent.core", "intent.services", "intent.mediator"], "language": "csharp" }
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
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommand.cs:5" }
  ],
  "findings": [
    { "rule": "claim/request.returns", "severity": "error", "subject": "cancel-order-command", "messageContains": "ICommand<bool>" }
  ]
}
```

## mediator-leaves-mediatr-request-unread

The record is `IRequest<bool>` in a file that imports `MediatR`, so it is MediatR's, though Mediator has an `IRequest<T>` too. With only the Mediator extension, nothing reads it: can't verify, `noRule`, never a reading of another framework's interface.

```json case
{ "phase": "later", "base": "mediator-verified", "kinds": ["request.returns"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.mediator"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommand.cs
using MediatR;

namespace eShop.Ordering.API.Application.Commands;

public sealed record CancelOrderCommand(int OrderNumber) : IRequest<bool>;
```

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "cancel-order-command", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/request.returns", "severity": "error", "subject": "cancel-order-command", "messageContains": "intent.mediator" }
  ]
}
```

## wolverine-verified

`CancelOrderCommand` returns `bool`. The record implements nothing; Wolverine's handler is `CancelOrderCommandHandler.Handle`, whose first parameter is the command, and it returns `Task<bool>`, so the request returns `bool`. Wolverine is in the code: Program.cs imports it.

```json case
{ "phase": "later", "kinds": ["request.returns"], "requirements": ["R2", "R42"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
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
    { "kind": "request.returns", "element": "cancel-order-command", "outcome": "verified", "expected": "bool", "actual": "bool",
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:5" }
  ]
}
```

## wolverine-static-handler

The handler is a static method of a static class, which Wolverine discovers as it does an instance one, and returns `ValueTask<bool>`.

```json case
{ "phase": "later", "base": "wolverine-verified", "kinds": ["request.returns"], "requirements": ["R2", "R42"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public static class CancelOrderCommandHandler
{
    public static ValueTask<bool> Handle(CancelOrderCommand command)
    {
        return ValueTask.FromResult(command.OrderNumber > 0);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "cancel-order-command", "outcome": "verified", "expected": "bool", "actual": "bool",
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:5" }
  ]
}
```

## wolverine-handle-async

The handler method is `HandleAsync`, one of the names Wolverine discovers, on a class with a primary constructor.

```json case
{ "phase": "later", "base": "wolverine-verified", "kinds": ["request.returns"], "requirements": ["R2", "R42"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public class CancelOrderCommandHandler(IOrderRepository orderRepository)
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
    { "kind": "request.returns", "element": "cancel-order-command", "outcome": "verified", "expected": "bool", "actual": "bool",
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:5" }
  ]
}
```

## wolverine-type-contradicted

The model says `CancelOrderCommand` returns an `int`; its Wolverine handler returns `Task<bool>`.

```json case
{ "phase": "later", "base": "wolverine-verified", "kinds": ["request.returns"], "requirements": ["R1", "R42"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
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
      "at": "src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs:5" }
  ],
  "findings": [
    { "rule": "claim/request.returns", "severity": "error", "subject": "cancel-order-command", "messageContains": "CancelOrderCommandHandler.Handle" }
  ]
}
```

## wolverine-none

`ShipOrderCommand` has no type, and its Wolverine handler returns `Task`, so both sides say the request returns nothing.

```json case
{ "phase": "later", "kinds": ["request.returns"], "requirements": ["R2", "R42"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
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

```csharp file src/Ordering.API/Program.cs
using Wolverine;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWolverine();

var app = builder.Build();
app.Run();
```

```csharp file src/Ordering.API/Application/Commands/ShipOrderCommand.cs
namespace eShop.Ordering.API.Application.Commands;

public record ShipOrderCommand(int OrderNumber);
```

```csharp file src/Ordering.API/Application/Commands/ShipOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public class ShipOrderCommandHandler
{
    public async Task Handle(ShipOrderCommand command, IOrderRepository orderRepository, CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetAsync(command.OrderNumber);
        orderToUpdate.SetShippedStatus();
        await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "ship-order-command", "outcome": "verified", "actual": "none",
      "at": "src/Ordering.API/Application/Commands/ShipOrderCommandHandler.cs:5" }
  ]
}
```

## wolverine-result-where-model-has-none

`ShipOrderCommand` has no type, but its Wolverine handler returns `Task<bool>`, which `InvokeAsync<bool>` answers with. `request.returns` is `always`, so the code contradicts the model's "nothing".

```json case
{ "phase": "later", "base": "wolverine-none", "kinds": ["request.returns"], "requirements": ["R1", "R3"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/ShipOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public class ShipOrderCommandHandler
{
    public async Task<bool> Handle(ShipOrderCommand command, IOrderRepository orderRepository, CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetAsync(command.OrderNumber);
        orderToUpdate.SetShippedStatus();
        return await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "ship-order-command", "outcome": "contradicted", "actual": "bool",
      "at": "src/Ordering.API/Application/Commands/ShipOrderCommandHandler.cs:5" }
  ],
  "findings": [
    { "rule": "claim/request.returns", "severity": "error", "subject": "ship-order-command", "messageContains": "bool" }
  ]
}
```

## wolverine-tuple-unreadable

The handler returns a tuple, each item of which Wolverine cascades as a message; which one answers the request depends on the type the sender asks `InvokeAsync<T>` for, so the result can't be read: can't verify, `unreadable`.

```json case
{ "phase": "later", "base": "wolverine-verified", "kinds": ["request.returns"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public static class CancelOrderCommandHandler
{
    public static (bool, OrderCancelled) Handle(CancelOrderCommand command)
    {
        return (true, new OrderCancelled(command.OrderNumber));
    }
}
```

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "cancel-order-command", "outcome": "cantVerify", "reason": "unreadable" }
  ],
  "findings": [
    { "rule": "claim/request.returns", "severity": "error", "subject": "cancel-order-command", "messageContains": "tuple" }
  ]
}
```

## wolverine-no-handler-is-one-finding

The handler's file is gone. A Wolverine request's result is its handler's, so with none there is nothing to read it from, and the missing handler is `request.handler`'s one finding: this claim is `blocked`, with none of its own.

```json case
{ "phase": "later", "base": "wolverine-verified", "without": ["src/Ordering.API/Application/Commands/CancelOrderCommandHandler.cs"], "kinds": ["request.returns", "request.handler"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "cancel-order-command", "outcome": "cantVerify", "reason": "blocked" },
    { "kind": "request.handler", "element": "cancel-order-command", "outcome": "contradicted", "expected": "present", "actual": "absent" }
  ],
  "findings": [
    { "rule": "claim/request.returns", "count": 0 },
    { "rule": "claim/request.handler", "count": 1 }
  ]
}
```

## wolverine-absent-no-rule

Program.cs is gone, and nothing else in the code imports Wolverine, so the Wolverine extension doesn't read the handler-shaped class: can't verify, `noRule`, never a reading from a convention another framework may share.

```json case
{ "phase": "later", "base": "wolverine-verified", "without": ["src/Ordering.API/Program.cs"], "kinds": ["request.returns"], "cases": ["engine/noRule"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.wolverine"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "request.returns", "element": "cancel-order-command", "outcome": "cantVerify", "reason": "noRule" }
  ],
  "findings": [
    { "rule": "claim/request.returns", "severity": "error", "subject": "cancel-order-command", "messageContains": "intent.wolverine" }
  ]
}
```

## mediatr-request-beside-wolverine

A codebase with both: Program.cs uses Wolverine, and `ShipOrderCommand` is MediatR's `IRequest<Unit>`, whose handler returns `Task<Unit>`. Its handler class also fits Wolverine's conventions, but a MediatR request is MediatR's to read, so the result is MediatR's nothing, read at the request, not Wolverine's `Unit`. The Wolverine extension is listed last so its reader runs first.

```json case
{ "phase": "later", "base": "wolverine-none", "kinds": ["request.returns"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.mediatr", "intent.wolverine"], "language": "csharp" }
```

```csharp file src/Ordering.API/Application/Commands/ShipOrderCommand.cs
namespace eShop.Ordering.API.Application.Commands;

public record ShipOrderCommand(int OrderNumber) : IRequest<Unit>;
```

```csharp file src/Ordering.API/Application/Commands/ShipOrderCommandHandler.cs
namespace eShop.Ordering.API.Application.Commands;

public class ShipOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<ShipOrderCommand, Unit>
{
    public async Task<Unit> Handle(ShipOrderCommand command, CancellationToken cancellationToken)
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
    { "kind": "request.returns", "element": "ship-order-command", "outcome": "verified", "actual": "none",
      "at": "src/Ordering.API/Application/Commands/ShipOrderCommand.cs:3" }
  ],
  "findings": [
    { "rule": "claim/request.returns", "count": 0 }
  ]
}
```
