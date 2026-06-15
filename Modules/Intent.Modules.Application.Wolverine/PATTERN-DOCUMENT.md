# Pattern Document: Intent.Application.Wolverine

## Technology Profile

- **Primary send API (no response):** `IMessageBus.InvokeAsync(command, CancellationToken)`
- **Primary send API (with response):** `IMessageBus.InvokeAsync<T>(query, CancellationToken)`
- **Handler/consumer interface:** None required. Handlers are plain classes discovered by convention.
- **Discovery model:** Assembly scanning by class name suffix (`Handler` or `Consumer`). Static classes require `[WolverineHandler]` attribute or `IWolverineHandler` interface explicitly — but Intent generates instance classes so this does not apply.
- **Lifecycle:** Built-in. `UseWolverine()` on `IHostBuilder` registers Wolverine as a hosted service — no manual lifecycle management needed.
- **DI integration:** Native. Supports both constructor injection (on instance handler classes) and method injection (additional parameters on `Handle()` beyond the message type). Intent generates instance classes using constructor injection for consistency with developer expectations.
- **Key NuGet package:** `WolverineFx` (namespace `Wolverine`). `5.39.5` is validated in the reference app and is compatible with .NET 8, 9, and 10.
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
    opts.Discovery.IncludeAssembly(typeof(CreateItemCommandHandler).Assembly);
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
| `Program.cs` (UseWolverine block) | Infrastructure/Startup | Fully (injected by FactoryExtension) | Added via `ContainerRegistrationRequest` / startup extension; in split-project apps the Application assembly must be explicitly included for handler discovery |
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

## Middleware Migration Note

For the first Wolverine module increments, we will keep the former MediatR cross-cutting concerns as **explicit generated offerings** rather than immediately collapsing them into Wolverine-native conventions.

The intent is:

- preserve an obvious one-to-one feature story for `Validation`, `UnitOfWork`, `UnhandledException`, `Logging`, `Performance`, and `Authorization`
- validate each concern independently in the reference app and module output
- only replace an explicit generated concern with a Wolverine-native equivalent once we are confident the behavior, scope, and operational characteristics are a full match

This means v1 planning should assume:

- `Validation` remains an explicit concern, even if later backed by Wolverine or Wolverine.HTTP validation integration
- `UnitOfWork` remains an explicit concern, even if later backed by Wolverine transactional middleware
- `UnhandledException`, `Logging`, `Performance`, and `Authorization` remain explicit concerns, even if later reimplemented as Wolverine middleware/policies

The short-term priority is **clarity and parity**, not elegance. We can optimize toward deeper Wolverine integration after empirical confirmation.

### Explicit Middleware Learnings

The current reference-app investigation surfaced some important Wolverine conventions that should guide the module design:

- When applying conventional middleware broadly, using the concrete message type as a middleware method parameter is fragile for reusable generated middleware because Wolverine composes every matching method on the middleware type into the generated handler chain.
- Using interface-typed message parameters such as `ICommand` / `IQuery` in middleware methods did **not** work in the reference app. Wolverine failed to resolve those as the current message variable in generated code.
- Using `object` as the middleware message parameter also did **not** work. Wolverine treated `object` as a service dependency instead of the current message.
- Using `Envelope` is the promising universal hook for broad middleware because it is always available and exposes the current message through `envelope.Message`, plus structural metadata such as `Id`, `MessageType`, and `Destination`.
- Middleware types applied by policy may still need DI registration if Wolverine chooses to resolve the middleware type from the service provider for generated chains.

Practical implication for module design:

- for broad explicit behaviors, prefer `Envelope`-based middleware over interface-typed or `object`-typed message parameters
- keep one concern per middleware type
- be cautious about multiple overloaded middleware methods on the same type, because Wolverine may compose all of them into the same generated chain

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
    opts.Discovery.IncludeAssembly(typeof(CreateItemCommandHandler).Assembly);
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
| 6 | Explicitly include the Application assembly for discovery in split-project apps | Empirical — reference app (Phase R.3) | `Program` assembly scanning alone is insufficient when handlers live in a separate Application project; `opts.Discovery.IncludeAssembly(typeof(CreateItemCommandHandler).Assembly)` successfully discovered and invoked handlers | reference-app-builder |
| 7 | Pin v1 to Wolverine 5.x | Empirical — reference app (Phase R.3) and NuGet compatibility matrix | Wolverine 5.39.5 supports .NET 8/9/10 and ran successfully in the reference app. Wolverine 6.x drops .NET 8 support and changes service-location defaults in ways that would block the stated module targets | reference-app-builder |
| 8 | Keep cross-cutting concerns explicit first | User direction | Validation, unit of work, exception handling, logging, performance, and authorization should be generated as explicit offerings first, then swapped to Wolverine-native equivalents only after full confirmation | current task |
| 9 | Prefer `Envelope` for broad conventional middleware | Empirical - reference app investigation | Interface-typed (`ICommand` / `IQuery`) and `object` message parameters did not generate correctly for reusable middleware; `Envelope` is the stable universal hook to continue with | current task |

## Open Questions

| # | Question | Blocking | Raised by |
|---|---|---|---|
| 1 | Can we eliminate Wolverine 5.x service-location warnings for `DbContext` and AutoMapper-backed repositories without changing the generated Clean Architecture registration style? | Increment 1 — affects whether Wolverine 6.x can be supported later without extra generated registration strategy | Phase R.3 |
| 2 | For each explicit cross-cutting concern, which Wolverine-native feature is truly equivalent versus only approximately similar? | Increment 1+ — affects whether explicit generated middleware can later be replaced safely | current task |
| 3 | Which broad explicit concerns should remain as generated conventional middleware versus be emitted directly into handler/controller code for maximum predictability? | Increment 1+ — the reference app proved that conventional middleware shape matters a lot to Wolverine's generated chain compiler | current task |
