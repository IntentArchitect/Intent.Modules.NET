# Pattern Document: Intent.Application.Wolverine

## Technology Profile

- **Primary send API (no response):** `IMessageBus.InvokeAsync(command, CancellationToken)`
- **Primary send API (with response):** `IMessageBus.InvokeAsync<T>(query, CancellationToken)`
- **Handler/consumer interface:** None required. Handlers are plain classes discovered by convention.
- **Discovery model:** Assembly scanning by class name suffix (`Handler` or `Consumer`). Static classes require `[WolverineHandler]` attribute or `IWolverineHandler` interface explicitly — but Intent generates instance classes so this does not apply.
- **Lifecycle:** Built-in. `UseWolverine()` on `IHostBuilder` registers Wolverine as a hosted service — no manual lifecycle management needed.
- **DI integration:** Native. Supports both constructor injection (on instance handler classes) and method injection (additional parameters on `Handle()` beyond the message type). Intent generates instance classes using constructor injection for consistency with developer expectations.
- **Key NuGet package:** `WolverineFx` (namespace `Wolverine`). Version must be compatible with .NET 8, 9, and 10 — confirmed during reference-app-builder.
- **Application layer contamination:** **Zero.** Handler classes require no Wolverine `using` statements. `IMessageBus` is only referenced in the API layer (controllers). This is architecturally cleaner than MediatR where `IRequestHandler<,>` enters the Application layer.

---

## Scenario Findings

| Scenario | Default behaviour | Explicit config required | Mental model revision |
|---|---|---|---|
| Single command, no response | Discovered by `Handle(Cmd, CancellationToken)`, dispatched via `InvokeAsync(cmd, ct)` | None | ✅ Baseline confirmed |
| Single query, with response | `Task<T> Handle(Query, CancellationToken)` → `InvokeAsync<T>(query, ct)` | None | ✅ Confirmed |
| Command with a return value (e.g. created entity ID) | Handler returns `Task<Guid>` — caller uses `InvokeAsync<Guid>` | None — return type drives dispatch overload automatically | No MediatR-style `IRequest<Guid>` marker needed |
| Two commands, different handlers | Each handler class maps to its message type via first parameter type — no routing config | None | Unlike NServiceBus, no endpoint names required |
| Static vs instance handler classes | Both valid; static requires `[WolverineHandler]` for convention discovery | Static requires explicit attribute | **Decision:** generate instance classes — familiar pattern, constructor injection available |
| Framework refs in Application layer | Handler classes: zero Wolverine imports needed | None | Cleaner than MediatR — Application layer is truly framework-free |
| CancellationToken | Optional — Wolverine injects automatically if present in signature | Add as last param | Always include for consistency with .NET conventions |
| Controller dispatch style | `[FromServices]` or constructor injection both valid | None | Use constructor injection — consistent with existing Intent controller templates |
| Two handler classes, same message type | Both will be called in sequence by Wolverine | Intentional — Wolverine supports multiple handlers per message type | Intent generates one handler per model element; multiple registrations are a feature, not a conflict |
| Handler naming collision between two different Command types | No collision — Wolverine disambiguates by first parameter type | None | No action needed |

---

## Architecture Mapping

| Technology Type | CA Layer | Notes |
|---|---|---|
| `{Command}.cs` | Application | Plain POCO — zero framework references |
| `{Query}.cs` | Application | Plain POCO — zero framework references |
| `{Command}Handler.cs` | Application | Instance class — no Wolverine `using`, discovered by `Handler` suffix |
| `{Query}Handler.cs` | Application | Instance class — returns `Task<T>`, no Wolverine `using` |
| `IMessageBus` (controller injection) | API | Only Wolverine type entering the API layer |
| `UseWolverine(...)` host extension | Infrastructure/Startup | `Program.cs` / host builder |

### DI Registration (Program.cs)

```csharp
builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});
```

### Controller Dispatch Pattern

```csharp
// Constructor injection in controller
public MyController(IMessageBus bus) { _bus = bus; }

// Command with no return value
await _bus.InvokeAsync(command, cancellationToken);
return Ok();

// Command with return value / Query
var result = await _bus.InvokeAsync<TResponse>(message, cancellationToken);
return Ok(result);
```

### appsettings Keys

None required for the core CQRS module. Transport keys (connection strings, endpoint names) are out of scope for this module — they belong to future Wolverine transport modules.

---

## Files to Generate

| File | Layer | Generated | Notes |
|---|---|---|---|
| `{Command}.cs` | Application | Fully | Plain POCO with properties from designer |
| `{Command}Handler.cs` | Application | Merge body | Class fully generated; `Handle()` body is developer-owned |
| `{Query}.cs` | Application | Fully | Plain POCO with properties from designer |
| `{Query}Handler.cs` | Application | Merge body | Class fully generated; `Handle()` body is developer-owned |
| `Program.cs` (UseWolverine block) | Infrastructure/Startup | Fully (injected by FactoryExtension) | Added via `ContainerRegistrationRequest` / startup extension |
| Controller field + dispatch calls | API | Fully (injected by FactoryExtension) | Replaces `IMediator` field and `Send()` calls with `IMessageBus` and `InvokeAsync` |

---

## Designer Pattern

The developer models in the **Services designer** — identical to MediatR. No new designer elements or stereotypes are needed.

- **Command element** → generates `{Command}.cs` + `{Command}Handler.cs`
- **Query element** → generates `{Query}.cs` + `{Query}Handler.cs`
- **Command/Query with return type set** → handler returns `Task<T>`, controller uses `InvokeAsync<T>`
- **Command with no return type** → handler returns `Task`, controller uses `InvokeAsync` (non-generic)

---

## Anti-Patterns (Must Nots)

1. **Do not add `IWolverineHandler` or `[WolverineHandler]` to generated handler classes.** Intent generates instance classes whose names end in `Handler` — convention discovery works automatically. Adding an explicit attribute is redundant and couples the Application layer to Wolverine unnecessarily.
2. **Do not add a Wolverine `using` to handler classes.** If the handler only processes the message and calls injected services, it requires zero Wolverine references. Add one only if the developer explicitly needs `IMessageBus` for cascading — and that is their responsibility in the body, not the generated skeleton.
3. **Do not copy MediatR's `IRequest<T>` / `IRequestHandler<,>` pattern.** Wolverine's whole value proposition is the absence of these marker interfaces. Generating them would defeat the purpose of the module.
4. **Do not generate `IMediator` references anywhere.** This module is a replacement, not a companion.
5. **Do not use `[FromServices] IMessageBus bus` in action method signatures.** Use constructor injection — consistent with Intent's existing controller templates and easier to unit-test.

---

## Test Strategy

**Increment 1 dispatch:** In-process local — no external infrastructure, no Docker required.

**Minimum designer model:**
- One Command with no return type (`CreateItemCommand`)
- One Query with a return type (`GetItemByIdQuery → ItemDto`)

**Expected generated file skeletons:**

```csharp
// CreateItemCommand.cs — Application layer
namespace MyApp.Application.Items.CreateItem;

public class CreateItemCommand
{
    public string Name { get; set; }
}
```

```csharp
// CreateItemCommandHandler.cs — Application layer
using System.Threading;
using System.Threading.Tasks;

namespace MyApp.Application.Items.CreateItem;

[IntentManaged(Mode.Merge)]
public class CreateItemCommandHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public async Task Handle(CreateItemCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Your implementation here.");
    }
}
```

```csharp
// GetItemByIdQuery.cs — Application layer
using System;

namespace MyApp.Application.Items.GetItemById;

public class GetItemByIdQuery
{
    public Guid Id { get; set; }
}
```

```csharp
// GetItemByIdQueryHandler.cs — Application layer
using System.Threading;
using System.Threading.Tasks;

namespace MyApp.Application.Items.GetItemById;

[IntentManaged(Mode.Merge)]
public class GetItemByIdQueryHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public async Task<ItemDto> Handle(GetItemByIdQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Your implementation here.");
    }
}
```

```csharp
// Program.cs addition — injected by FactoryExtension
builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});
```

```csharp
// OrdersController.cs — controller field + action (injected by FactoryExtension)
private readonly IMessageBus _bus;

public ItemsController(IMessageBus bus)
{
    _bus = bus;
}

[HttpPost]
public async Task<ActionResult> CreateItem([FromBody] CreateItemCommand command, CancellationToken cancellationToken)
{
    await _bus.InvokeAsync(command, cancellationToken);
    return Ok();
}

[HttpGet("{id}")]
public async Task<ActionResult<ItemDto>> GetItemById([FromRoute] Guid id, CancellationToken cancellationToken)
{
    var result = await _bus.InvokeAsync<ItemDto>(new GetItemByIdQuery { Id = id }, cancellationToken);
    return Ok(result);
}
```

**Reference app location:** To be scaffolded at `Tests/Wolverine.CQRS.TestApplication` in `Intent.Modules.NET.Tests.isln` — `reference-app-builder` is responsible.

**Success criteria — Increment 1:**
- `dotnet build` exits 0
- POST to `/api/items` results in `NotImplementedException` (proving handler was reached by Wolverine)
- GET to `/api/items/{id}` results in `NotImplementedException` (proving query handler was reached)
- DI container resolves without exception at startup
- Zero MediatR packages in `.csproj`

**Increment 2:** Implement handler bodies (return hardcoded data), verify HTTP response flows correctly end-to-end.

---

## Decision Log

| # | Decision | Basis | Rationale | Closed by |
|---|---|---|---|---|
| 1 | Instance handler classes over static | Intent Convention — MediatR module pattern; developer familiarity | Constructor injection is available and expected; static classes require explicit `[WolverineHandler]` attribute which adds framework coupling to Application layer | Phase 1.1.5 |
| 2 | Constructor injection over method injection for handler dependencies | Intent Convention — consistent with existing module patterns | Developers migrating from MediatR expect constructor injection; method injection is a Wolverine power feature that developers can add manually in the handler body | Phase 1.1.5 |
| 3 | Zero Wolverine usings in Application layer | Standard — Wolverine docs + CA principle | Handler classes discovered by name suffix; no framework type required in Application layer; cleaner than MediatR | Phase 1.2 |
| 4 | Controller uses constructor injection for `IMessageBus` | Intent Convention — existing controller template pattern | Consistent with all other Intent controller templates; unit-testable | Phase 1.2 |
| 5 | No appsettings keys in v1 | Standard — in-process CQRS needs no transport config | Transport configuration belongs to future out-of-process Wolverine modules | Phase 1.2 |
| 6 | `IncludeAssembly(typeof(Program).Assembly)` for discovery | Standard — Wolverine docs example | Scans the entry-point assembly where all generated handlers live; simple and correct for a single-assembly Clean Architecture app | Phase 1.3 |

## Open Questions

| # | Question | Blocking | Raised by |
|---|---|---|---|
| 1 | Exact minimum `WolverineFx` NuGet version compatible with .NET 8, 9, and 10 | Increment 1 — needed for `.imodspec` NuGet declaration | Phase 1.1 |
| 2 | Does `IncludeAssembly(typeof(Program).Assembly)` cover handlers in a separate Application class library (multi-project Clean Architecture), or must each assembly be listed separately? | Increment 1 — affects DI registration template | Phase 1.3 |
