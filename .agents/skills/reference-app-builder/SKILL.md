---
name: reference-app-builder
description: "Use after tech-pattern-researcher produces a Pattern Document and before module-ecosystem-analyst. Scaffolds a real Intent-managed Clean Architecture application with standard modules installed, runs the Software Factory to get the actual generated output, then hand-crafts the technology-specific files on top of that real output. Proves the code shapes compile and the handler is hit at runtime. The running app becomes the ground truth that module-ecosystem-analyst reads to understand what the ecosystem already generates. TRIGGER: mandatory after Pattern Document, before ecosystem analysis — never skip. BLOCK on this skill until the reference app is green."
argument-hint: "[Attack Plan path or module name]"
---

# Reference App Builder

## Purpose

Prove with running code that every file shape in the Pattern Document is correct before a single template is written. If the reference app cannot be made to work, the Pattern Document has an error — fix the pattern, not the code.

This is the most important gate in the chain. Template bugs from unverified patterns are expensive to diagnose in the increment loop. This skill moves that discovery cost to its cheapest point.

## Musts

1. Locate or create a test app (check U9). Scaffold with `dotnet new webapi` if needed. If scaffolding requires unattended IA designer setup, ask the user.
2. Hand-craft every file in the Pattern Document's "Files to Generate" table — exact class names, signatures, namespace conventions, IntentManaged attributes.
3. Wire DI registration exactly as the factory extension will emit it (word-for-word from Pattern Document).
4. `dotnet build` → exit 0 before proceeding.
5. Exercise at runtime: call an endpoint or run a test that dispatches through the handler. Confirm the handler body is reached.
6. Update `PATTERN-DOCUMENT.md` immediately if any shape needed correction. Add a Decision Log entry.
7. Record the reference app path in the Attack Plan under "Reference App".

## Must Nots

1. Never proceed without a green reference app — a failing app means templates will be wrong.
2. Never diverge from Pattern Document shapes without updating the document first.
3. Never accept green build alone — an unexercised handler means unknown runtime behaviour.
4. Never use shortcuts the template won't use (`dynamic`, reflection, hiding casts).
5. Never skip because "the technology is well-documented" — documentation describes the API; running code proves the wiring.

---

## Phase R.0 — Runtime Dependency Classification

Before writing any code, enumerate every runtime dependency the app will need and classify each:

| Class | Examples | AI action |
|---|---|---|
| **AI-spinnable** | RabbitMQ, SQL Server, PostgreSQL, Redis, MongoDB, Seq | Generate `docker-compose.yml` and bring them up before the run phase. |
| **Developer-provided** | Azure Service Bus, AWS SQS/SNS, Cosmos DB, licensed cloud services | Surface immediately with exactly what is needed. Block until developer confirms availability. |

**If developer-provided dependencies cannot be confirmed, halt.** Do not proceed to R.1 — a reference app that cannot be run proves nothing.

Generate the docker-compose before R.1 so infrastructure is ready when the app first starts. Include health checks so the app does not race the broker/database on startup.

---

## Phase R.1 — Locate or Create the Test App

### Tasks

1. **Check Requirements Summary U9** — is a reference/test app already identified?
   - If YES and the app exists on disk: open it, verify it is a buildable .NET solution, proceed to Phase R.2.
   - If YES but the app does not exist: scaffold it (see below), then proceed to Phase R.2.
   - If NO: ask the user if they have an existing app, or whether to scaffold one now.

2. **Scaffolding (if needed):**
   ```powershell
   # Minimal ASP.NET Core Web API — adjust as appropriate for the module type
   dotnet new webapi -n [AppName] -o [Path]
   ```
   For Intent-managed apps: open the solution in Intent Architect, run the standard bootstrap modules (Common, Common.CSharp, Application.DependencyInjection, AspNetCore.Controllers, etc.) so the DI extension method and controller infrastructure already exist before adding Wolverine/other technology files.

3. **Confirm the app baseline builds:**
   ```powershell
   dotnet build [solution or csproj] --nologo --verbosity minimal
   ```
   Must exit 0 before adding any reference code.

### Output

Note in Attack Plan:
```
## Reference App
- Path: [absolute path to .sln or .csproj]
- Status: baseline builds ✅
```

---

## Phase R.2 — Hand-Craft the Reference Files

### Tasks

For **every row** in the Pattern Document "Files to Generate" table:

1. Identify the target folder (Application layer, Infrastructure layer, etc. per the Architecture Mapping table).
2. Write the file exactly as the template will output it:
   - Use the class/method names from the Pattern Document.
   - Use the namespace derived from the project name + folder path.
   - Include `[assembly: DefaultIntentManaged(Mode.Merge)]` and `[IntentManaged(...)]` attributes — the reference app should look like SF output.
   - For merge-body files (handlers): write a minimal but real implementation (e.g. returns a hardcoded value) so runtime behaviour is observable.
3. Write the DI registration block (from Pattern Document "DI registration shape") into the appropriate location (`Program.cs` or Application layer extension method).
4. Add required NuGet packages to the relevant `.csproj`.

### Key shapes for Wolverine CQRS (copy to reference app verbatim from Pattern Document)

**Command DTO (fully generated):**
```csharp
public class CreateOrderCommand
{
    public string CustomerName { get; set; } = default!;
    // ... properties from designer model
}
```

**Command handler (merge body — write a minimal implementation):**
```csharp
public class CreateOrderCommandHandler
{
    public async Task Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        // Minimal implementation — proves handler is reached
        await Task.CompletedTask;
    }
}
```

**Query DTO + handler (returning a value):**
```csharp
public class GetOrderQueryHandler
{
    public async Task<OrderDto> Handle(GetOrderQuery query, CancellationToken cancellationToken)
    {
        return new OrderDto { Id = query.Id, CustomerName = "Test" };
    }
}
```

**UnitOfWorkMiddleware (if UoW module detected):**
```csharp
public class UnitOfWorkMiddleware
{
    private readonly IUnitOfWork _unitOfWork;
    public UnitOfWorkMiddleware(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
    public async Task After(CancellationToken cancellationToken)
        => await _unitOfWork.SaveChangesAsync(cancellationToken);
}
```

**DI registration (in Application layer AddApplicationServices):**
```csharp
services.AddWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(Assembly.GetExecutingAssembly());
    opts.Policies.AddMiddleware<UnitOfWorkMiddleware>(); // if UoW present
    opts.Policies.AddMiddleware<LoggingMiddleware>();
    opts.Policies.AddMiddleware<PerformanceMiddleware>();
});
```

**Controller (if AspNetCore.Controllers is present):**
```csharp
private readonly IMessageBus _messageBus;
// ...
await _messageBus.InvokeAsync(command, cancellationToken);
var result = await _messageBus.InvokeAsync<OrderDto>(query, cancellationToken);
```

---

## Phase R.3 — Build and Verify

### Tasks

1. **Build:**
   ```powershell
   dotnet build [solution] --no-incremental --nologo --verbosity minimal
   ```
   Exit code must be 0. If not, diagnose and fix — **update Pattern Document if the shape was wrong**.

2. **Run:**
   ```powershell
   dotnet run --project [WebApi project]
   ```

3. **Exercise the handler** via one of:
   - `curl` / `Invoke-WebRequest` hitting the command endpoint
   - A minimal xUnit/NUnit test that resolves `IMessageBus` from DI and calls `InvokeAsync`
   - A log statement in the handler body confirmed in stdout

4. **Confirm observable behaviour:**
   - Command handler: no exception thrown, 200/204 response (or test passes)
   - Query handler: correct DTO returned
   - Middleware: log lines appear in stdout showing `Before`/`After` execution (for Logging middleware)

### Failure modes and responses

| Failure | Diagnosis | Action |
|---|---|---|
| Build error: type not found | NuGet package missing or wrong version | Fix package, note correct version in Pattern Document |
| Build error: method signature mismatch | Wolverine API changed in target version | Update Pattern Document shape, note in Decision Log |
| Runtime: handler not found | Assembly discovery config wrong | Fix `IncludeAssembly` call, update Pattern Document DI block |
| Runtime: DI cannot resolve middleware | `IUnitOfWork` not registered | Check UoW module install; adjust conditionality rule in Pattern Document |
| Runtime: wrong assembly returned by `GetExecutingAssembly()` | DI extension method is in wrong project | Move registration, update Architecture Mapping in Pattern Document |

---

## Phase R.4 — Lock and Record

### Tasks

1. **Update Pattern Document** with any shape corrections found in R.3. Add a Decision Log entry for each correction:
   ```
   | N | [what changed] | Empirical — reference app (Phase R.3) | [why the original shape was wrong] | reference-app-builder |
   ```

2. **Update Attack Plan** — mark the Reference App section complete:
   ```
   ## Reference App
   - Path: [path]
   - Status: green ✅ — build exits 0, handler hit confirmed
   - Corrections to Pattern Document: [list or "none"]
   ```

3. **Update Progress Tracker** in Attack Plan — mark Reference App row ✅ Complete.

4. **Commit the reference app files** (or note their location) so the `module-increment-loop` can diff generated output against them during verification.

---

## Phase R.5 — Multi-Scenario Loop

After the initial reference app is green, check whether additional scenarios are needed before handing off to `module-ecosystem-analyst`.

**Either the developer or the AI may trigger an additional scenario:**
- **Developer-initiated** — "I also need a scenario for X" → build a new reference app for that scenario now.
- **AI-initiated** — AI identifies a scenario material to the module design not covered by the current app → propose it; developer confirms or rejects.
- **PRD-driven** — multiple scenarios described in the PRD are each built as separate reference apps.

Each additional scenario follows the same R.0–R.4 cycle. Record all reference app paths in `.intent-build-state.md`. `module-ecosystem-analyst` synthesizes across all of them.

**Pivots are also handled here.** If new information requires reworking an existing reference app (Level 2+ pivot per the agent Pivot Scale), do so before proceeding. A pivot triggered after a PRD was provided is valid — new runtime evidence supersedes the original document.

Only proceed to `module-ecosystem-analyst` when all required scenarios are green and no pending pivots remain.

---

## Handoff

Once all reference apps are green and the Pattern Document is updated, load **`module-ecosystem-analyst`** and pass all reference app paths + Pattern Document as context. The ecosystem analyst reads the actual generated code across all scenarios to determine what the Intent ecosystem already provides, which SDK building blocks to use, and how to structure the Attack Plan.

> If at any point a reference app cannot be made to work after 3+ attempts and Pattern Document updates, **stop and escalate to the user.** Do not proceed to ecosystem analysis with an unverified pattern.
