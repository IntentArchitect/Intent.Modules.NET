# Attack Plan: Intent.Application.Wolverine

## Mapping Validation

> Phase 2.0.5 — Every Scenario Findings row from PATTERN-DOCUMENT verified against the Intent designer model.

| Scenario | Stereotype/setting that covers it | Gap found | Resolution |
|---|---|---|---|
| Single command, no response | Command element, no return type set → `Task Handle(cmd, ct)` | None | Standard Services designer Command element sufficient |
| Single query with response | Query element, return type set → `Task<T> Handle(query, ct)` | None | Standard Services designer Query element sufficient |
| Command with return value (e.g. Guid) | Command element, return type set → `Task<Guid> Handle(cmd, ct)` | None | Same as query path — return type on command drives handler signature |
| Two commands, different handlers | Two Command elements → two handler files | None | Wolverine maps by message type; no routing config needed |
| Instance handler classes | No stereotype — always instance, always discovered by `Handler` suffix | None | Module convention — no designer surface needed |
| Zero Wolverine usings in Application layer | No stereotype — handler body is developer-owned, no framework ref needed | None | Clean CA — this is a design advantage, not a gap |
| CancellationToken in Handle method | Always generated in skeleton — no config needed | None | Module always emits `CancellationToken cancellationToken` as last param |
| Controller dispatch | No new stereotype — FactoryExtension modifies controller template | None | `WolverineControllerDispatchExtension` injects `IMessageBus`, replaces `Send()` |

**Conclusion:** No new stereotypes or module settings required for v1. The existing Services designer Command/Query model completely drives all generation.

---

## Ecosystem Dependencies

**Not needed (this is NOT an eventing module):**
- `Intent.Eventing.Contracts` — N/A, Wolverine CQRS is in-process only for this module
- Any transport or broker modules

**Already provided by Intent (do not re-generate):**
- `CommandModel` / `QueryModel` typed model classes → `Intent.Modelers.Services.CQRS`
- `GetCommandModels()` / `GetQueryModels()` metadata manager extension → `Intent.Modelers.Services.CQRS`
- Controller template (role `Distribution.WebApi.Controller`) → `Intent.AspNetCore.Controllers`
- DI startup hook (`ContainerRegistrationRequest`) → `Intent.Application.DependencyInjection`
- `[IntentManaged]` attribute weaving → `Intent.OutputManager.RoslynWeaver`

**Modeler modules required:**
- `Intent.Modelers.Services` >= 4.0.0
- `Intent.Modelers.Services.CQRS` >= 4.1.1

**Intent events to subscribe to:**
- `ContainerRegistrationRequest` → `WolverineRegistrationFactoryExtension` emits `builder.Host.UseWolverine(...)`
- Controller template `AfterBuild` (priority 500 — Extension band) → `WolverineControllerDispatchExtension` injects `IMessageBus` field and replaces `_mediator.Send()` → `_bus.InvokeAsync()`

**Designer state:** No pre-existing Commands/Queries needed to scaffold the module. At least one Command or Query must be modeled in the Services designer before any per-model template fires.

---

## Template Inventory

| File | Base Class | Model Type | Registration | Managed |
|---|---|---|---|---|
| `{Command}.cs` | `CSharpTemplateBase<CommandModel>` | `CommandModel` | `FilePerModelTemplateRegistration<CommandModel>` | Fully |
| `{Command}Handler.cs` | `CSharpTemplateBase<CommandModel>` | `CommandModel` | `FilePerModelTemplateRegistration<CommandModel>` | Merge body — `Handle()` body is developer-owned |
| `{Query}.cs` | `CSharpTemplateBase<QueryModel>` | `QueryModel` | `FilePerModelTemplateRegistration<QueryModel>` | Fully |
| `{Query}Handler.cs` | `CSharpTemplateBase<QueryModel>` | `QueryModel` | `FilePerModelTemplateRegistration<QueryModel>` | Merge body — `Handle()` body is developer-owned |
| `UseWolverine` startup block | **FactoryExtension** — no standalone template | N/A | N/A | N/A — injected into existing startup template |
| Controller `IMessageBus` injection | **FactoryExtension** — modifies existing controller template | N/A | N/A | N/A — AfterBuild on controller template |

**GetModels implementations:**
```csharp
// Command templates
_metadataManager.Services(application).GetCommandModels()

// Query templates
_metadataManager.Services(application).GetQueryModels()
```

---

## Module Blueprint

### Module Identity
- **Module ID:** `Intent.Application.Wolverine`
- **Display name:** Wolverine
- **Version:** `1.0.0-pre.0`
- **Tags:** `csharp dotnet wolverine cqrs`

### Template Roles
| Template ID | Role |
|---|---|
| `Intent.Application.Wolverine.CommandModels` | `Application.Command` |
| `Intent.Application.Wolverine.CommandHandler` | `Application.Command.Handler` |
| `Intent.Application.Wolverine.QueryModels` | `Application.Query` |
| `Intent.Application.Wolverine.QueryHandler` | `Application.Query.Handler` |

### imodspec Dependency Block
```xml
<dependency id="Intent.Application.DependencyInjection" version="4.1.14" />
<dependency id="Intent.AspNetCore.Controllers" version="7.1.0" />
<dependency id="Intent.Common" version="3.11.0" />
<dependency id="Intent.Common.CSharp" version="3.10.5" />
<dependency id="Intent.Common.Types" version="3.4.0" />
<dependency id="Intent.Modelers.Services" version="4.0.0" />
<dependency id="Intent.Modelers.Services.CQRS" version="4.1.1" />
<dependency id="Intent.OutputManager.RoslynWeaver" version="4.9.10" />
```

### NuGet Package Registration (`NugetPackages.cs`)
Package: `WolverineFx` — always registered, no conditional flag.

Framework-specific minimum versions (to be confirmed by reference-app-builder):
| Target Framework | WolverineFx version |
|---|---|
| net8.0 | 5.39.5 |
| net9.0 | 5.39.5 |
| net10.0 | 5.39.5 |

Pattern (from `DependencyInjection.MediatR/NugetPackages.cs`):
```csharp
packages.AddNugetPackage("WolverineFx", version, targetFramework);
```

### Module Settings
None for v1. The file consolidation setting (present in MediatR) is deferred — noted for a later increment.

### Folder Scaffold
```
Intent.Modules.Application.Wolverine/
├── FactoryExtensions/
│   ├── WolverineRegistrationFactoryExtension.cs   (UseWolverine on host builder)
│   └── WolverineControllerDispatchExtension.cs    (IMessageBus in controllers)
├── Templates/
│   ├── CommandModels/
│   │   ├── CommandModelsTemplatePartial.cs
│   │   └── CommandModelsTemplateRegistration.cs
│   ├── CommandHandler/
│   │   ├── CommandHandlerTemplatePartial.cs
│   │   └── CommandHandlerTemplateRegistration.cs
│   ├── QueryModels/
│   │   ├── QueryModelsTemplatePartial.cs
│   │   └── QueryModelsTemplateRegistration.cs
│   └── QueryHandler/
│       ├── QueryHandlerTemplatePartial.cs
│       └── QueryHandlerTemplateRegistration.cs
├── NugetPackages.cs
└── Intent.Application.Wolverine.imodspec
```

---

## Progress Tracker

| Increment | Status | Outcome / Blocker | Decisions made |
|---|---|---|---|
| Scaffold | ⬜ Not started | — | — |
| 1 — Core templates + DI | ⬜ Not started | — | — |
| 2 — Controller dispatch | ⬜ Not started | — | — |
| Reference app | ✅ Complete | Reference app builds and dispatches through Wolverine; split-project discovery requires explicit Application assembly include | Pin v1 to Wolverine 5.39.5; use `IncludeAssembly(typeof(CreateItemCommandHandler).Assembly)` |
| Explicit middleware investigation | 🟨 In progress | Interface-typed and `object` middleware message parameters both failed in generated Wolverine chains; `Envelope` is the current working direction under investigation | Record findings immediately; bias module design toward `Envelope`-based conventional middleware |

---

## Cross-Cutting Concern Strategy

For this module, the current implementation stance is:

- make the MediatR-era behaviors explicit first
- prove each concern in generated output and in the reference app
- only then substitute Wolverine-native middleware/features where the equivalence is fully understood

The explicit concerns to track are:

- validation
- unit of work / transaction handling
- unhandled exception handling
- logging
- performance timing
- authorization

This is intentionally conservative. The goal is to avoid prematurely hiding behavior behind Wolverine features before we can prove feature parity.

### Confirmed Implementation Notes

- Prefer `Envelope` as the universal middleware input when a behavior must apply across many messages.
- Do not rely on interface-typed middleware message parameters like `ICommand` / `IQuery` for broad conventional middleware. The reference app produced unresolvable generated variables.
- Do not rely on `object` as the broad middleware message parameter. The reference app showed Wolverine treating that as a service dependency, not the current message.
- Avoid multiple overloaded middleware methods for different concrete message types on the same middleware class when applying that middleware broadly. Wolverine composed all matching methods into the same generated chain and produced invalid generated code.
- If we keep conventional middleware as the explicit offering, one concern per middleware type and `Envelope`-based access is the safest current direction.

## Implementation Increments

### Increment Scaffold — imodspec + project skeleton
**Goal:** Module project builds. `.imodspec` registered in the Module Builder solution. Zero templates yet.
**Files:**
- [ ] `Intent.Application.Wolverine.imodspec`
- [ ] `Intent.Application.Wolverine.csproj`
- [ ] `NugetPackages.cs` — registers `WolverineFx` for .NET 8/9/10
**Success criteria:**
- `dotnet build` on the module `.csproj` exits 0
- Module appears in the Module Builder designer

### Increment 1 — Core CQRS Templates + UseWolverine Startup
**Goal:** For each modeled Command/Query, SF generates a POCO class and a handler class. `UseWolverine()` appears in the target app's `Program.cs`.
**Designer prereq:** At least one Command and one Query modeled in the Services designer of the target application.
**Files:**
- [ ] `Templates/CommandModels/CommandModelsTemplatePartial.cs`
- [ ] `Templates/CommandModels/CommandModelsTemplateRegistration.cs`
- [ ] `Templates/CommandHandler/CommandHandlerTemplatePartial.cs`
- [ ] `Templates/CommandHandler/CommandHandlerTemplateRegistration.cs`
- [ ] `Templates/QueryModels/QueryModelsTemplatePartial.cs`
- [ ] `Templates/QueryModels/QueryModelsTemplateRegistration.cs`
- [ ] `Templates/QueryHandler/QueryHandlerTemplatePartial.cs`
- [ ] `Templates/QueryHandler/QueryHandlerTemplateRegistration.cs`
- [ ] `FactoryExtensions/WolverineRegistrationFactoryExtension.cs`
**Generated shapes:**
```csharp
// Command POCO — fully generated
public class CreateItemCommand { public string Name { get; set; } }

// Command handler — merge body
[IntentManaged(Mode.Merge)]
public class CreateItemCommandHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public async Task Handle(CreateItemCommand command, CancellationToken cancellationToken)
        => throw new NotImplementedException();
}

// Query POCO — fully generated
public class GetItemByIdQuery { public Guid Id { get; set; } }

// Query handler — merge body
[IntentManaged(Mode.Merge)]
public class GetItemByIdQueryHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public async Task<ItemDto> Handle(GetItemByIdQuery query, CancellationToken cancellationToken)
        => throw new NotImplementedException();
}

// Program.cs addition
builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});
```
**Success criteria:**
- SF runs with exit code 0
- 4 files generated per modeled Command + Query pair
- `WolverineFx` appears in `.csproj`
- `UseWolverine(...)` present in `Program.cs`
- `dotnet build` on target app exits 0 (handlers throw `NotImplementedException` — that is correct)
**Skills:** `file-builder-expert`, `intent-module-orchestrator`

### Increment 2 — Controller Dispatch
**Goal:** Generated ASP.NET Core controllers inject `IMessageBus` (not `IMediator`) and dispatch via `InvokeAsync`.
**Designer prereq:** At least one Command/Query mapped to a controller endpoint.
**Files:**
- [ ] `FactoryExtensions/WolverineControllerDispatchExtension.cs`
**Generated change (per controller):**
```csharp
// Field + constructor injection
private readonly IMessageBus _bus;
public ItemsController(IMessageBus bus) { _bus = bus; }

// Action — command, no return
await _bus.InvokeAsync(command, cancellationToken);
return Ok();

// Action — query / command with return
var result = await _bus.InvokeAsync<ItemDto>(query, cancellationToken);
return Ok(result);
```
**Success criteria:**
- No `IMediator` / `ISender` / MediatR references in any generated controller
- `using Wolverine;` present in controller files
- `dotnet build` on target app exits 0
**Skills:** `intent-module-orchestrator`, `file-builder-expert`

### Reference App — Hand-crafted verification
**Goal:** The golden output files from PATTERN-DOCUMENT.md compile, start up cleanly, and a controller → handler round-trip is observable.
**Responsibility:** `reference-app-builder` skill
**Location:** `Tests/Wolverine.CQRS.TestApplication` in `Intent.Modules.NET.Tests.isln`
**Status:** green ✅
**Evidence:**
- `dotnet build` passed for `Wolverine.CQRS.TestApplication.Api`
- `POST /api/items` returned `201`, proving Wolverine discovered and executed `CreateItemCommandHandler`
- `GET /api/items/{id}` returned `404` from `GetItemByIdQueryHandler`'s `NotFoundException` path, proving inline query dispatch executed the handler
**Corrections captured from reference app:**
- In a split-project Clean Architecture app, Wolverine must explicitly include the Application assembly for handler discovery
- Wolverine `5.39.5` is the validated package line for .NET 8/9/10 support
**Steps:**
1. Scaffold a Clean Architecture application using Intent Architect
2. Install standard modules (AspNetCore.Controllers, DependencyInjection, etc.)
3. Run SF — get MediatR baseline output
4. Swap out MediatR references for Wolverine equivalents manually
5. Verify `dotnet build` exits 0
6. Start the app and hit controller endpoints — confirm Wolverine dispatch reaches the generated handlers

---

## Skills to Load per Increment
| Increment | Skills |
|---|---|
| Scaffold | `intent-module-builder` |
| 1 | `file-builder-expert`, `intent-module-orchestrator` |
| 2 | `intent-module-orchestrator`, `file-builder-expert` |
| Reference app | `reference-app-builder` |
