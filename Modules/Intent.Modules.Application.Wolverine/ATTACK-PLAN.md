# Attack Plan: Intent.Application.Wolverine

## Mapping Validation

> Phase 2.0.5 — Every Scenario Findings row from PATTERN-DOCUMENT verified against the Intent designer model.

| Scenario | Stereotype/setting that covers it | Gap found | Resolution |
|---|---|---|---|
| Single command, no response | Command element, no return type set → `Task Handle(cmd, ct)` | None | Standard Services designer Command element sufficient |
| Single query with response | Query element, return type set → `Task<T> Handle(query, ct)` | None | Standard Services designer Query element sufficient |
| Command with return value (e.g. Guid) | Command element, return type set → `Task<Guid> Handle(cmd, ct)` | None | Same as query path — return type on command drives handler signature |
| Two commands, different handlers | Two Command elements → two handler files | None | Wolverine maps by message type; no routing config needed |
| Zero-property query (list-all) | Query element with no properties → `new GetItemsQuery()` in controller; handler returns `Task<List<T>>` | None | Controller emits `new GetItemsQuery()` with no initializer block |
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
| `ICommand.cs` | `IntentTemplateBase` | (none) | `SingleFileTemplateRegistration` | Fully |
| `IQuery.cs` | `IntentTemplateBase` | (none) | `SingleFileTemplateRegistration` | Fully |
| `{Command}.cs` | `CSharpTemplateBase<CommandModel>` | `CommandModel` | `FilePerModelTemplateRegistration<CommandModel>` | Fully — implements `ICommand` |
| `{Command}Handler.cs` | `CSharpTemplateBase<CommandModel>` | `CommandModel` | `FilePerModelTemplateRegistration<CommandModel>` | Merge body — `Handle()` body is `Body = Mode.Ignore` |
| `{Query}.cs` | `CSharpTemplateBase<QueryModel>` | `QueryModel` | `FilePerModelTemplateRegistration<QueryModel>` | Fully — implements `IQuery` |
| `{Query}Handler.cs` | `CSharpTemplateBase<QueryModel>` | `QueryModel` | `FilePerModelTemplateRegistration<QueryModel>` | Merge body — `Handle()` body is `Body = Mode.Ignore` |
| `ValidationMiddleware.cs` | `IntentTemplateBase` | (none) | `SingleFileTemplateRegistration` | Fully |
| `UnitOfWorkMiddleware.cs` | `IntentTemplateBase` | (none) | `SingleFileTemplateRegistration` | Fully |
| `LoggingMiddleware.cs` | `IntentTemplateBase` | (none) | `SingleFileTemplateRegistration` | Fully |
| `PerformanceMiddleware.cs` | `IntentTemplateBase` | (none) | `SingleFileTemplateRegistration` | Fully |
| `UnhandledExceptionMiddleware.cs` | `IntentTemplateBase` | (none) | `SingleFileTemplateRegistration` | Fully |
| `AuthorizationMiddleware.cs` | `IntentTemplateBase` | (none) | `SingleFileTemplateRegistration` | Fully |
| `UseWolverine` startup block | **FactoryExtension** (`FactoryExtensionBase`) | N/A | N/A — injects into `Intent.AspNetCore.Program` template via `FindTemplateInstance` | N/A |
| Controller `IMessageBus` injection | **FactoryExtension** (`FactoryExtensionBase`) | N/A | N/A — AfterBuild on controller template (priority 500) | N/A |

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
| Reference app | ✅ Complete | Build green; Item entity + EF InMemory; 6 Wolverine middlewares implemented and verified; `ICommand`/`IQuery` markers used for middleware predicate targeting | `typeof(ICommand).Assembly` for discovery (hand-crafted app used `CreateItemCommandHandler` — corrected for module impl); `Envelope`-based middleware; `ICommand`/`IQuery` markers required; `UnitOfWorkMiddleware` uses `TransactionScope?` threading pattern; all 6 middleware types require explicit `AddTransient<>` in `AddApplication()` |
| Scaffold | ✅ Complete | 12 templates + 2 factory extensions + WolverineFx NuGet in designer; `dotnet build` exits 0; module packaged as `Intent.Application.Wolverine.1.0.0-pre.0.imod` | SDK versions bumped to Common/CSharp 3.9.0, SDK 3.11.0 to match repo standard; `<Version>1.0.0-pre.0</Version>` added to csproj |
| 1 — Core CQRS templates + ICommand/IQuery | ✅ Complete | SF generates correct POCOs, handler stubs, ICommand/IQuery interfaces; test app builds green | Old reference-app sub-folder files (Items/CreateItem/, Items/GetItemById/, Items/GetItems/) deleted — module generates flat layout; stale sub-namespace usings removed from ItemsController.cs and Program.cs |
| 2 — Middleware templates + UseWolverine startup | 🔄 In progress | 5/6 middleware template bodies implemented (see below); WolverineRegistrationFactoryExtension pending | Cross-module type IDs confirmed: ICurrentUserService=`Intent.Application.Identity.CurrentUserServiceInterface`; IValidatorProvider=`Intent.Application.FluentValidation.Dtos.ValidatorProviderInterface`; IBypassPipelineValidation=`Intent.Application.MediatR.FluentValidation.BypassPipelineValidationInterface`; IUnitOfWork=`Intent.Entities.Repositories.Api.UnitOfWorkInterface`; AuthorizeAttribute=`Intent.Application.Identity.AuthorizeAttribute`; ForbiddenAccessException=`Intent.Application.Identity.ForbiddenAccessException` |
| 3 — Controller dispatch | ⬜ Not started | — | — |

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

### Confirmed Implementation Notes (verified in reference app commit 4d9ef87073)

- **`ICommand`/`IQuery` as method parameters on middleware = failed.** Wolverine tried to resolve them from DI and could not.
- **`ICommand`/`IQuery` in `chain.MessageType` predicate = correct.** `opts.Policies.AddMiddleware<T>(chain => typeof(ICommand).IsAssignableFrom(chain.MessageType))` works perfectly. The interfaces are used in the host registration predicate, NOT in the middleware method signatures.
- **`Envelope` is the correct broad middleware parameter.** All 6 middleware classes use `Envelope envelope` to access the message — never `ICommand`, `IQuery`, or `object`.
- **`object` as a middleware parameter = failed.** Wolverine treats it as a service dependency.
- **Avoid multiple overloaded middleware methods on the same class for different message types.** Wolverine composes all matching methods into the same chain.
- **`UnitOfWorkMiddleware` uses Wolverine's return-value-threading pattern.** `Before()` returns `TransactionScope?`; Wolverine automatically threads it as the first parameter of `AfterAsync(TransactionScope? tx, ...)`.
- **`PerformanceMiddleware` uses the same threading pattern.** `Before()` returns `Stopwatch`; `FinallyAsync(Stopwatch stopwatch, ...)` receives it automatically.
- **`[WolverineOnException]` attribute on `UnhandledExceptionMiddleware.OnException()`** — standard Wolverine exception handling hook; no special return type needed.

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

### Increment 1 — Core CQRS Templates + ICommand/IQuery Interfaces
**Goal:** SF generates `ICommand.cs`, `IQuery.cs`, and for each modeled Command/Query: a POCO class implementing the marker interface + a handler skeleton.
**Designer prereq:** At least one Command and one Query modeled in the Services designer.
**Files:**
- [ ] `Templates/CommandInterface/CommandInterfaceTemplatePartial.cs`
- [ ] `Templates/CommandInterface/CommandInterfaceTemplateRegistration.cs`
- [ ] `Templates/QueryInterface/QueryInterfaceTemplatePartial.cs`
- [ ] `Templates/QueryInterface/QueryInterfaceTemplateRegistration.cs`
- [ ] `Templates/CommandModels/CommandModelsTemplatePartial.cs`
- [ ] `Templates/CommandModels/CommandModelsTemplateRegistration.cs`
- [ ] `Templates/CommandHandler/CommandHandlerTemplatePartial.cs`
- [ ] `Templates/CommandHandler/CommandHandlerTemplateRegistration.cs`
- [ ] `Templates/QueryModels/QueryModelsTemplatePartial.cs`
- [ ] `Templates/QueryModels/QueryModelsTemplateRegistration.cs`
- [ ] `Templates/QueryHandler/QueryHandlerTemplatePartial.cs`
- [ ] `Templates/QueryHandler/QueryHandlerTemplateRegistration.cs`

**Generated shapes:**
```csharp
// ICommand.cs — Application/Common/Interfaces — single file
public interface ICommand { }

// IQuery.cs — Application/Common/Interfaces — single file
public interface IQuery { }

// Command POCO — implements ICommand
public class CreateItemCommand : ICommand { public string Name { get; set; } = null!; }

// Command handler — Merge+Fully; Handle body = Ignore
[IntentManaged(Mode.Merge, Signature = Mode.Fully)]
public class CreateItemCommandHandler
{
    [IntentManaged(Mode.Merge)]
    public CreateItemCommandHandler() { }

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public async Task Handle(CreateItemCommand command, CancellationToken cancellationToken)
        => throw new NotImplementedException("Your implementation here...");
}

// Query POCO — implements IQuery
public class GetItemByIdQuery : IQuery { public Guid Id { get; set; } }

// Query handler — same pattern; return type from Query.TypeReference
[IntentManaged(Mode.Merge, Signature = Mode.Fully)]
public class GetItemByIdQueryHandler
{
    [IntentManaged(Mode.Merge)]
    public GetItemByIdQueryHandler() { }

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public async Task<ItemDto> Handle(GetItemByIdQuery query, CancellationToken cancellationToken)
        => throw new NotImplementedException("Your implementation here...");
}
```

**Handler return type logic:** `model.TypeReference.Element == null` → `Task`; otherwise → `Task<{GetTypeName(model.TypeReference)}>`

**Success criteria:**
- `ICommand.cs` and `IQuery.cs` generated in `Application/Common/Interfaces`
- 4 files per Command + Query pair; all implement the appropriate marker interface
- `dotnet build` exits 0
**Skills:** `file-builder-expert`, `intent-module-orchestrator`

### Increment 2 — Middleware Templates + UseWolverine Startup
**Goal:** Six `Envelope`-based Wolverine middlewares generated; `UseWolverine()` with full middleware policy registration in `Program.cs`.
**Designer prereq:** Increment 1 complete.
**Files:**
- [ ] `Templates/Behaviours/ValidationMiddleware/...TemplatePartial.cs + ...TemplateRegistration.cs`
- [ ] `Templates/Behaviours/UnitOfWorkMiddleware/...TemplatePartial.cs + ...TemplateRegistration.cs`
- [ ] `Templates/Behaviours/LoggingMiddleware/...TemplatePartial.cs + ...TemplateRegistration.cs`
- [ ] `Templates/Behaviours/PerformanceMiddleware/...TemplatePartial.cs + ...TemplateRegistration.cs`
- [ ] `Templates/Behaviours/UnhandledExceptionMiddleware/...TemplatePartial.cs + ...TemplateRegistration.cs`
- [ ] `Templates/Behaviours/AuthorizationMiddleware/...TemplatePartial.cs + ...TemplateRegistration.cs`
- [ ] `FactoryExtensions/WolverineRegistrationFactoryExtension.cs`

**Key shapes (all verified in reference app):**
```csharp
// ValidationMiddleware — Envelope + IValidatorProvider
public class ValidationMiddleware
{
    public async Task BeforeAsync(Envelope envelope, IValidatorProvider validatorProvider, CancellationToken cancellationToken) { ... }
}

// UnitOfWorkMiddleware — TransactionScope threading pattern (Commands only)
public class UnitOfWorkMiddleware
{
    public static TransactionScope? Before(IUnitOfWork dataSource) { ... }
    public static async Task AfterAsync(TransactionScope? tx, IUnitOfWork dataSource, CancellationToken cancellationToken) { ... }
}

// PerformanceMiddleware — Stopwatch threading pattern
public class PerformanceMiddleware
{
    public Stopwatch Before(Envelope envelope) { ... }
    public async Task FinallyAsync(Stopwatch stopwatch, Envelope envelope, ILogger logger, ...) { ... }
}

// UnhandledExceptionMiddleware
public class UnhandledExceptionMiddleware
{
    [WolverineOnException]
    public void OnException(Exception exception, Envelope envelope, ILogger logger) { ... }
}
```

**Application-layer DI registration (also required in `AddApplication()`):**

`WolverineRegistrationFactoryExtension` must also inject 6 `AddTransient<>` calls into the Application layer's `AddApplication()` method via `ContainerRegistrationRequest` or direct template mutation:

```csharp
services.AddTransient<AuthorizationMiddleware>();
services.AddTransient<LoggingMiddleware>();
services.AddTransient<PerformanceMiddleware>();
services.AddTransient<UnhandledExceptionMiddleware>();
services.AddTransient<UnitOfWorkMiddleware>();
services.AddTransient<ValidationMiddleware>();
```

**UseWolverine injection (all 6 AddMiddleware calls):**
```csharp
builder.Host.UseWolverine(opts =>
{
    // Use typeof(ICommand) as the stable assembly anchor — ICommand is generated by this module
    // and always lives in the Application project. Do NOT use typeof(CreateItemCommandHandler)
    // (user type) — FactoryExtensions cannot reference user-generated types directly.
    opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly);
    // ICommand/IQuery used in chain.MessageType predicate — NOT as method params
    opts.Policies.AddMiddleware<AuthorizationMiddleware>(chain => typeof(ICommand).IsAssignableFrom(chain.MessageType) || typeof(IQuery).IsAssignableFrom(chain.MessageType));
    opts.Policies.AddMiddleware<ValidationMiddleware>(chain => typeof(ICommand).IsAssignableFrom(chain.MessageType) || typeof(IQuery).IsAssignableFrom(chain.MessageType));
    opts.Policies.AddMiddleware<LoggingMiddleware>(chain => typeof(ICommand).IsAssignableFrom(chain.MessageType) || typeof(IQuery).IsAssignableFrom(chain.MessageType));
    opts.Policies.AddMiddleware<PerformanceMiddleware>(chain => typeof(ICommand).IsAssignableFrom(chain.MessageType) || typeof(IQuery).IsAssignableFrom(chain.MessageType));
    opts.Policies.AddMiddleware<UnhandledExceptionMiddleware>(chain => typeof(ICommand).IsAssignableFrom(chain.MessageType) || typeof(IQuery).IsAssignableFrom(chain.MessageType));
    opts.Policies.AddMiddleware<UnitOfWorkMiddleware>(chain => typeof(ICommand).IsAssignableFrom(chain.MessageType));
});
```

**Success criteria:**
- All 6 middleware files generated
- `UseWolverine(...)` with all 6 `AddMiddleware` calls in `Program.cs`
- `WolverineFx` in API project `.csproj`
- `dotnet build` exits 0
**Skills:** `file-builder-expert`, `intent-module-orchestrator`

### Increment 3 — Controller Dispatch
**Goal:** Generated controllers inject `IMessageBus` and dispatch via `InvokeAsync`.
**Designer prereq:** At least one Command/Query mapped to a controller endpoint.
**Files:**
- [ ] `FactoryExtensions/WolverineControllerDispatchExtension.cs`

**Generated changes (per controller, verified in reference app):**
```csharp
private readonly IMessageBus _messageBus;
public ItemsController(IMessageBus messageBus) { _messageBus = messageBus; }

// Command returning Guid — uses InvokeAsync<T>
var id = await _messageBus.InvokeAsync<Guid>(command, cancellationToken);
return CreatedAtAction(nameof(GetItemById), new { id }, id);

// Query with return type
var result = await _messageBus.InvokeAsync<ItemDto>(new GetItemByIdQuery { Id = id }, cancellationToken);
return result == null ? NotFound() : Ok(result);
```

**Success criteria:**
- No `IMediator`/`ISender`/MediatR references in any generated controller
- `using Wolverine;` in controller files
- `dotnet build` exits 0
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
| 1 — Core CQRS + interfaces | `file-builder-expert`, `intent-module-orchestrator` |
| 2 — Middleware + startup | `file-builder-expert`, `intent-module-orchestrator` |
| 3 — Controller dispatch | `intent-module-orchestrator`, `file-builder-expert` |
| Reference app | `reference-app-builder` ✅ Complete |
