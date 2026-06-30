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

1. Locate or create a test app (check U9). For Intent-managed apps, attempt scaffolding via the `create_application`/`create_solution` MCP tools first; fall back to `dotnet new webapi` for a plain app, or ask the user if the tools are unavailable or IA designer setup must be done by hand.
2. Hand-craft every file in the Pattern Document's "Files to Generate" table — exact class names, signatures, namespace conventions, IntentManaged attributes.
3. Wire DI registration exactly as the factory extension will emit it (word-for-word from Pattern Document).
4. `dotnet build` → exit 0 before proceeding.
5. Exercise at runtime: call an endpoint or run a test that dispatches through the handler. Confirm the handler body is reached.
6. Update `PATTERN-DOCUMENT.md` immediately if any shape needed correction. Add a Decision Log entry.
7. Record the reference app path in the Attack Plan under "Reference App".
8. Exercise every runtime dependency path — including reflectively-loaded providers (e.g. NHibernate drivers, EF Core providers, serialization adapters). These fail only at startup with no compile-time signal; compilation alone does not verify them.
9. **Model a comprehensive test domain when the module generates from domain / DTO / entity models.** Do not settle for one trivial DTO/entity with 2–3 scalar properties — deliberately exercise the OOP aspects the domain actually has: composition, aggregates (independent relationships), inheritance, nullability, collections, operations (where relevant), parameter lists, and parameterized constructors. A module verified only against a toy model silently fails on the variations developers really use. Expand to the domain's real shapes, then let the developer cut what's out of scope (don't pad artificially beyond what the domain has).

## Must Nots

1. Never proceed without a green reference app — a failing app means templates will be wrong.
2. Never diverge from Pattern Document shapes without updating the document first.
3. Never accept green build alone — an unexercised handler means unknown runtime behaviour.
4. Never use shortcuts the template won't use (`dynamic`, reflection, hiding casts).
5. Never skip because "the technology is well-documented" — documentation describes the API; running code proves the wiring.

---

## Greenfield vs Modification

This skill applies to **any change that affects generated output** — a new module *or* a modification (fix / improvement / pivot). The gate is **output impact, not novelty** (see `module-kickoff` Build Type triage):

- **Designer-only changes with no output impact** (dialog behaviour, designer-extension UX) **do not enter this skill** — there is nothing to prove in generated code.
- **Modifications that affect output** must **prove the *changed* output shape in a reference architecture before the module is changed.** Locate the existing reference architecture (or stand one up) and show "this is what the new/changed output will look like" first.
- **Don't break what works:** when modifying, the reference app must still exercise the **existing** scenarios alongside the change, so a regression in current output/behaviour is caught — not just the new shape.

---

## Phase R.0 — Runtime Dependency Classification

### Step R.0.0 — Integration Target Verification (run before anything else)

Review the Requirements Summary (U10) for all platform modules, host types, and deployment environments the module must integrate with. For each one, **verify online** whether the chosen framework has known constraints in that environment before writing any code:

- **Serverless hosts** (Azure Functions isolated worker, AWS Lambda): Does the framework support this environment? Does it require Oakton CLI command routing or runtime disk writes for codegen that serverless hosts bypass or block?
- **Transport bridges / integration modules**: Are there known compatibility issues between this framework version and the integration target?
- **Runtime-only drivers** (NHibernate drivers, EF Core providers, serialization adapters): These are reflectively loaded and fail only at startup — they produce no compile-time signal. Verify each path at runtime, not just in a build.

**If a known blocker is found, stop and escalate before proceeding.** A reference app built against an incompatible framework/environment combination proves nothing and produces wasted increment cycles.

### Step R.0.1 — Enumerate and Classify Runtime Dependencies

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

## Phase R.5 — Build the Spanning Set

After the initial (typically in-memory / simplest) reference app is green, build out the rest of the **confirmed spanning set** before handing off to `module-ecosystem-analyst`. This is not an optional "do we need more?" check — the spanning set was already identified and developer-approved in the Pattern Document's **Spanning Set** section (see **Scope Batching — The Spanning Set Principle** in `tech-pattern-researcher`). Build every scenario in that set; do not silently stop at the first green app.

**For each scenario in the confirmed spanning set:**
- **AI-spinnable dependency** (RabbitMQ, SQL Server, etc.) → bring the dependency up via docker-compose, switch the reference app's configuration to that scenario, and run the full R.0–R.4 cycle against the real dependency. A docker-compose that was generated in R.0 but never brought up and exercised does **not** count — the scenario is only complete once the path runs through the real dependency.
- **Deferred scenario** (developer-provided dependency, or a redundant shape) → do **not** build it. Record the deferral and its reason in the build state so `module-ecosystem-analyst` knows the shape is unverified and can flag it as a later runtime-only check rather than treating it as covered.

**Scope changes are still allowed mid-phase** (the spanning set is a proposal, not a cage):
- **Developer-initiated** — "I also need a scenario for X" → add it to the set and build it now.
- **AI-initiated** — AI finds a distinct shape the stress-test missed → propose it; developer confirms or rejects before building.
- **PRD-driven** — scenarios described in the PRD are each built as part of the set.

Each scenario follows the same R.0–R.4 cycle. Record all reference app paths and every deferral in `.module-builder/WORKING.md`. `module-ecosystem-analyst` synthesizes across all of them.

**Pivots are also handled here.** If new information requires reworking an existing reference app (Level 2+ pivot per the agent Pivot Scale), do so before proceeding. A pivot triggered after a PRD was provided is valid — new runtime evidence supersedes the original document.

**Major pivot — re-fire the Research Mode Gate** (from `tech-pattern-researcher`) before continuing when a pivot requires substantial re-research: a new technology is being evaluated, the CA layer assignment changes fundamentally, or the Pattern Document's Technology Profile needs to be rebuilt. Ask the user whether to proceed with built-in tools or produce a prompt for an external deep-research tool. A pivot that only corrects a file shape or a DI registration does not trigger the gate — handle it in-flow.

Only proceed to `module-ecosystem-analyst` when every scenario in the confirmed spanning set is green (or explicitly deferred with a recorded reason) and no pending pivots remain.

---

## Handoff

Once all reference apps are green and the Pattern Document is updated, load **`module-ecosystem-analyst`** and pass all reference app paths + Pattern Document as context. The ecosystem analyst reads the actual generated code across all scenarios to determine what the Intent ecosystem already provides, which SDK building blocks to use, and how to structure the Attack Plan.

> **Note for the increment loop:** these reference apps become the **Phase-1 SF target** later (the module is installed on top to confirm it reproduces this code exactly — see `module-building-strategies` §4, two-phase verification). Because these files are hand-crafted, the increment loop must reset the regeneration baseline (remove hand-added `Ignore`s / delete-to-regenerate template-owned files) before trusting any staged diff. A separate **from-scratch app** with no pre-written code is then required as Phase 2.

> If at any point a reference app cannot be made to work after 3+ attempts and Pattern Document updates, **stop and escalate to the user.** Do not proceed to ecosystem analysis with an unverified pattern.
