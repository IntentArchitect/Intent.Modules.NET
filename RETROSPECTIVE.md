# RETROSPECTIVE.md

Append-only log of gaps, workarounds, and lessons encountered during module builds on this branch. Three buckets: Intent gaps (flag for IA team), Process gaps (update relevant skill), PRD/user gaps (strengthen kickoff questions).

---

## Entry 1 — Interoperability entries missing from Application.Wolverine imodspec

**Bucket:** PRD/user gap  
**Discovered:** feature/wolverine session  
**Symptom:** When users installed `Intent.Application.Wolverine` into an application that already had `Intent.AspNetCore.Controllers`, `Intent.Application.FluentValidation`, or `Intent.DomainEvents` installed, the bridging Wolverine dispatch/validation modules were not auto-installed. Users had to install them manually.  
**Root cause:** The `Intent.Application.Wolverine.imodspec` had no `<interoperability>` section. The MediatR module has this for all its bridging modules, but the Wolverine module was not built with the same pattern.  
**Fix applied:** Added `<interoperability>` block to `Intent.Application.Wolverine.imodspec` detecting 6 platform/feature modules and auto-installing the corresponding Wolverine bridging module:

| Detect | Install |
|---|---|
| `Intent.AspNetCore.Controllers` | `Intent.AspNetCore.Controllers.Dispatch.Wolverine` |
| `Intent.AzureFunctions` | `Intent.AzureFunctions.Dispatch.Wolverine` |
| `Intent.FastEndpoints` | `Intent.FastEndpoints.Dispatch.Wolverine` |
| `Intent.Aws.Lambda.Functions` | `Intent.Aws.Lambda.Functions.Dispatch.Wolverine` |
| `Intent.Application.FluentValidation` | `Intent.Application.Wolverine.FluentValidation` |
| `Intent.DomainEvents` | `Intent.Application.Wolverine.DomainEvents` |

**Skill/kickoff update needed:**  
- `module-kickoff`: Add question — "Which platform modules (Controllers, AzureFunctions, FastEndpoints, Lambda) and feature modules (FluentValidation, DomainEvents) should this module auto-install bridging modules for? Check the MediatR equivalent's `<interoperability>` section and mirror the pattern."  
- `module-wrap-up`: Add checklist item — "Does the imodspec have `<interoperability>` entries for all relevant bridging modules? Mirror the MediatR module's pattern."

---

## Entry 2 — Template registration class must be `public`

**Bucket:** Process gap  
**Discovered:** feature/wolverine session (AwsLambdaFunctions test app)  
**Symptom:** SF ran successfully with 0 staged changes and no Lambda function files generated. No error or warning.  
**Root cause:** `CqrsLambdaFunctionClassTemplateRegistration` was declared `internal`. The SF engine uses `Assembly.GetExportedTypes()` — internal classes are silently skipped.  
**Fix applied:** Changed `internal class` → `public class`. Same root cause was hit previously for `ImplicitControllerTemplateRegistration`.  
**Skill update applied:** Added to `module-increment-loop` SKILL.md.  

---

## Entry 3 — install_or_update_modules is the only valid reinstall path

**Bucket:** Process gap  
**Discovered:** feature/wolverine session  
**Symptom:** After rebuilding a module, assistant attempted to find and manually copy the DLL to the IA module cache.  
**Root cause:** Assistant was not aware that `install_or_update_modules` MCP tool handles cache refresh. CLAUDE.md prohibits reading/writing `.intent` folders but the rule wasn't linked clearly to reinstall workflow.  
**Fix applied:** Added to `module-increment-loop` SKILL.md.  

---

## 2026-06-23 | Intent.Application.Wolverine — Post-build validation

### Intent Gaps
- No `Intent.Application.Wolverine.CRUD` module exists → handler stubs remain `throw new NotImplementedException()` even after entity mappings are configured in the Services designer. The IA team should evaluate a Wolverine CRUD module analogous to `Intent.Application.MediatR.CRUD`, or consider a shared CRUD generation abstraction that can target both dispatchers.

### Process Gaps
- Test apps were built with Commands/Queries modeled in the Services designer but no entity mappings created → CRUD handler bodies are untestable stubs. The `reference-app-builder` skill should explicitly ask: "Do any commands/queries require entity mappings? If so, set them up in the Services designer before closing the reference app." This is distinct from route/HTTP settings, which were already covered.

### PRD / User Gaps
- module-kickoff does not ask whether CRUD implementations (entity create/read/update/delete) are required → the build produced only stub handlers. Add U-question: "Do handlers need full CRUD implementations backed by entity mappings, or are stub bodies acceptable for this module's scope? If CRUD is required, confirm whether an existing CRUD module covers the dispatch pattern or whether a new one is needed."
- module-kickoff does not ask about interoperability with sibling/platform modules early enough → the `<interoperability>` block was missing from the imodspec and only caught post-build. Add U-question: "Which other installed modules should trigger auto-installation of bridging modules from this one? Cross-check the MediatR equivalent's `<interoperability>` section."

---

## 2026-06-23 | Intent.Application.Wolverine.CRUD — Convention-based GetAll parity

**Bucket:** Process gap (module parity)
**Symptom:** Unfiltered "get all" query handlers generated empty bodies. Earlier RETROSPECTIVE entry assumed "no Wolverine CRUD module exists" — inaccurate. The module did exist but only supported the Domain Interactions path (`Query Entity Action` + `Query Entity Mapping`). `QueryInteractionStrategy.IsMatch` silently no-ops when an action has no mapping, so an unfiltered get-all produced a stub with no warning.
**Root cause:** `Intent.Application.MediatR.CRUD` ships convention-based legacy strategies (`GetAllImplementationStrategy`, `GetByIdImplementationStrategy`, etc., wired via `StrategyFactory`) that generate CRUD from a domain-mapped DTO with no association. The Wolverine CRUD module was built with only the Domain Interactions path and none of these convention strategies.
**Fix applied:** Ported a focused `ConventionGetAllStrategy` into `Intent.Application.Wolverine.CRUD/CrudStrategies/`, invoked from `CqrsHandlerCrudExtension.InstallOnQueryHandlers` where it previously `continue`d on zero interactions. Verified across all 4 Wolverine test apps (Controllers fresh-create + the other 3 regenerating identical output), all solutions build green.
**Skill/process update needed:**
- `module-ecosystem-analyst` / `tech-pattern-researcher`: when a CRUD companion module is in scope, explicitly diff the reference dispatcher's CRUD module (e.g. MediatR.CRUD) for BOTH generation paths — Domain Interactions AND convention-based legacy strategies — and decide per-path whether to port. A CRUD module that only implements the Domain Interactions path silently drops conventionally-modelled CRUD.
- Verification: ground-truth a "does the module work" claim against a real reference-dispatcher test app's modeling (e.g. `RichDomain` get-all uses no association), not just one metadata sample. The first sample (`TrainingModel.Tests`) used an empty mapping and led to the wrong conclusion that the empty mapping was the only canonical approach.
- Remaining gap (deferred per scope = "GetAll only"): convention-based `GetById`, `Create`, `Update`, `Delete` strategies are still absent from the Wolverine CRUD module. Apps modelled conventionally (no entity actions) will still stub those. Track for a future parity pass.

---

## 2026-06-23 | Runtime testing of Wolverine test apps — two further defects

**Bucket:** Process gap + Intent gap
**Context:** After committing the CRUD work, ran the test apps to verify runtime behaviour (not just compilation).

**Defect 1 (Process gap) — stale RoslynWeaver state hid stub handlers.** FastEndpoints, AwsLambdaFunctions and AzureFunctions had committed handlers that were *hybrid stubs*: CRUD had injected the repository and set `Body=Mode.Fully`, but the body was still the template's `throw new NotImplementedException("Your implementation here...")`. In-place Software Factory runs reported "0 changes" and kept reproducing the stub, so they compiled but threw at runtime. Only Controllers had real bodies. Fix: delete the fully Intent-managed handler files and re-run SF, which produced the real bodies. Lesson: "build is green" and "SF shows 0 changes" do NOT prove generated bodies are correct — run the app (or at least read the handler bodies) for CRUD-bearing apps. Deleting-to-regenerate is only safe for files that are 100% Intent-managed (no hand-written regions), which these handlers are.

**Defect 2 (Intent gap) — Azure Functions host never registers Wolverine.** The Azure Functions isolated app (`Intent.AzureFunctions.Isolated.Program` template) fails to start: DI validation throws `Unable to resolve service for type 'Wolverine.IMessageBus' while attempting to activate 'DomainEventService'`. Root cause: `Intent.Application.Wolverine`'s `WolverineRegistrationFactoryExtension` only injects `UseWolverine(...)` into templates with the `App.Program` role (the ASP.NET host). The Azure Functions Program uses a `new HostBuilder()...ConfigureServices(...)` chain that the extension does not target, so `IMessageBus` is never registered — breaking both command/query dispatch and the DomainEvents service. The generated `WolverineConfiguration.Configure` exists but is never called. Fix needed (separate task): extend the Wolverine host-registration so the isolated Functions `HostBuilder` gets `UseWolverine(opts => WolverineConfiguration.Configure(opts))`. Open question for the IA team: is running Wolverine's runtime inside an Azure Functions isolated worker an intended/supported scenario, and if so how should `IMessageBus` be hosted there (mediator-only vs full messaging runtime)?

**Verified working at runtime:** Controllers (full CRUD incl. convention GetAll) and FastEndpoints (full CRUD incl. convention GetAll), both against EF Core InMemory. AwsLambdaFunctions not runtime-tested (environment not yet set up). AzureFunctions blocked by Defect 2.

---

## 2026-06-24 | Serverless dispatch fix — AzureFunctions + AwsLambdaFunctions

**Bucket:** Process gap + Intent gap  
**Context:** Followed up on Defect 2 above. Implemented `ApplyServerlessDiscovery` in both dispatch bridge modules and `RegisterWolverineOnLambdaStartup` in the Lambda bridge.

### Learnings

**1 — Cross-module `FindTemplateInstance<T>` requires interface types, not concrete types (Intent gap)**  
Using a concrete template class (e.g. `FindTemplateInstance<WolverineConfigurationTemplate>`) across module boundaries returns `null` even when the template is registered. IA may load each module's DLL in an isolated `AssemblyLoadContext`, so the concrete type from module A's context does not match the type registered by module B in its context. Fix: use interface types from shared NuGet packages.  
- `ICSharpFileBuilderTemplate` (from `Intent.Modules.Common.CSharp.Templates`) — for templates with `CSharpFile`  
- `IIntentTemplate<TModel>` (from `Intent.Templates`) — for templates with a typed model; `CommandModel`/`QueryModel` implement `IMetadataModel`, matching the `GetTypeName(templateId, model)` overload  
**Skill update needed:** `intent-module-orchestrator` and `module-increment-loop` — add explicit rule: cross-module `FindTemplateInstance` MUST use an interface type, not the concrete template class.

**2 — `TypeLoadMode` is in `JasperFx.CodeGeneration`, not `JasperFx` (PRD gap)**  
`TypeLoadMode.Static` is defined in the `JasperFx.CodeGeneration` namespace. Using `file.AddUsing("JasperFx")` alone produces `CS0103`. Fix: `file.AddUsing("JasperFx.CodeGeneration")`.  
**Kickoff update needed:** When using `TypeLoadMode`, document that the `using` must be `JasperFx.CodeGeneration`.

**3 — `HostApplicationBuilder` has no `.Host` property (PRD/process gap)**  
`WebApplicationBuilder.Host` exists; `HostApplicationBuilder` (used by Lambda and generic-host apps) does not. The correct call is `hostBuilder.UseWolverine(...)` directly — Wolverine's `UseWolverine` extension targets `IHostApplicationBuilder`, which `HostApplicationBuilder` implements.  
**Kickoff update needed:** When wiring Wolverine into a `HostApplicationBuilder` context (Lambda, worker services), document that `UseWolverine` is called directly on the builder, not on `.Host`.

**4 — Do not reinstall a module when the version is unchanged (Process gap)**  
When a module's version matches what is installed in the application, IA reloads the local compiled DLL automatically on every SF run. Calling `install_or_update_modules` when the version has not changed causes cache file lock contention, can trigger `Exceeded maximum retries to save module` failures, and has caused catastrophic staged-change cascades (mass deletions of template outputs). Only reinstall when: (a) the version number changes, or (b) the cache is confirmed corrupt and a fresh install is the only path forward.  
**Skill update needed:** `module-increment-loop` — add explicit rule and call out the distinction between "rebuild DLL" (safe, needed every time) and "reinstall module" (only on version change or confirmed cache corruption).

**5 — IA session state corruption produces `hasErrors: true, errors: []` with mass deletion staging (Intent gap)**  
After repeated rapid install→SF→install sequences, the Lambda app SF began proposing to delete 8 files and revert all generated changes, with `hasErrors: true` and an empty `errors` array. The root cause was stale IA session state accumulated across multiple conflicting operations. Fix: close and reopen Intent Architect in a fresh instance. On the fresh instance, SF ran correctly with 1 clean change and 0 errors.  
**Skill update needed:** `module-increment-loop` — add: "If SF produces mass deletions or `hasErrors: true` with `errors: []`, close IA completely and reopen the solution in a new instance before investigating further. Do not apply the bad staged changes."

**6 — `JasperFx.AssemblyFinder.FindAssemblies()` bin sweep crashes isolated-worker hosts (Intent gap)**  
Wolverine's default convention discovery sweeps the bin directory with `JasperFx.AssemblyFinder.FindAssemblies()`. In Azure Functions isolated worker and AWS Lambda environments, this loads host-process DLLs (e.g. `Microsoft.Azure.WebJobs.Host`) that are not present in the isolated worker process, causing `FileNotFoundException` at startup. The fix pattern is:
```
opts.Discovery.DisableConventionalDiscovery();
// one per generated handler:
opts.Discovery.IncludeType<SomeCommandHandler>();
opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;
opts.Durability.Mode = DurabilityMode.Serverless;
```
**For IA team:** The dispatch bridge module's factory extension must generate this pattern at priority 500 into `WolverineConfiguration.Configure`. It should not be left to the user to add manually. The `Intent.Application.Wolverine.WolverineConfiguration` template generates a base form that assumes a full server host; serverless dispatch bridges must override it.

**Status after fix:**
- `Wolverine.AzureFunctions.sln` — builds 0 errors, serverless shape applied ✓  
- `Wolverine.AwsLambdaFunctions.sln` — builds 0 errors, serverless shape applied, `hostBuilder.UseWolverine(...)` wired ✓  
- `Wolverine.AspNetCore.Controllers.sln` — builds 0 errors ✓  
- `Wolverine.AspNetCore.FastEndpoints.sln` — builds 0 errors ✓
# Module Build Retrospective

> Append-only. Add new entries at the bottom. Never reformat existing entries.

---

## 2026-06-23 | Intent.Eventing.NServiceBus — v1.0.0-pre.3 NHibernate / EnableInstallers

### Intent Gaps

- `install_or_update_modules` fails consistently with `[object Object]` SignalR errors even when both solutions are confirmed open by `get_status`. Two consecutive retries both fail. → IA team should investigate SignalR instability for the module install path; the MCP should surface the inner error rather than `[object Object]`.
- `NuGet Package` designer only supports a fixed set of `Minimum Target Framework` values. `Any,Version=v0.0` generates `( >= 0, >= 0)` in the C# switch expression — not `( >= 2, >= 0)` as a developer might hand-edit. The mapping is undocumented in the designer UI. → Document the framework-to-generated-code mapping in the Module Builder designer schema or tooltip so the developer knows what each option generates.

### Process Gaps

- **`lookupById` not `getElementById`:** The designer script API exposes `lookupById(id)` to find an element by GUID. The first attempt used `getElementById` (from DOM familiarity) which threw `ReferenceError`. → Add a "Known wrong name" note in `intent-architect-mcp` skill under Known Gotchas: "Use `lookupById(id)`, NOT `getElementById(id)` — the latter is not defined in the designer script context."
- **`install_or_update_modules` MCP fallback is unclear:** When the tool fails twice, the skill says to manually edit `modules.config`. But the fallback was rejected by the developer mid-session (PowerShell read was denied). The correct fallback should be asking the developer to run the module update from the IA UI directly. → Update `intent-architect-mcp` skill gotcha for `install_or_update_modules`: add "If the SignalR error persists after two attempts and a solution re-open, STOP and ask the developer to update the module version in the Intent Architect UI (Modules panel). Do NOT attempt a `modules.config` hand-edit."
- **CONTEXT.md stated no NHibernate test apps exist, but they were built.** Two apps were created: `N_ServiceBus.Persistence.NHibernate.Publish` and `N_ServiceBus.Persistence.NHibernate.Subscribe`. → CONTEXT.md acceptance matrix updated in this session — ensure future sessions check CONTEXT.md currency before stating "no test app exists."
- **`NugetPackages.cs` hand-edits are silently overwritten.** The developer had to correct hand-edits made in a previous session because they differed from what the designer generates. → Reinforce in `module-increment-loop` that any NuGet package version edit MUST be made via the Module Builder designer, not by hand-editing `NugetPackages.cs`, since the file is `[DefaultIntentManaged(Mode.Fully)]`.

### PRD / User Gaps

- **NuGet version data in training knowledge is unreliable.** The agent incorrectly assumed `NServiceBus.NHibernate 11.1.0` did not exist (old training data said only `10.1.2` existed for NSB 9.x). The developer had to correct this. → Add to `tech-pattern-researcher` skill: "Always verify NuGet package versions against the live NuGet API (`https://api.nuget.org/v3/registration5-gz-semver2/<packageid>/index.json` or the catalog page). Never rely on training-data version knowledge — NuGet packages publish frequently and training cutoffs mean versions are outdated."

---

## 2026-06-23 | Intent.Eventing.NServiceBus — NHibernate runtime verification (end-to-end)

### Intent Gaps

- The "persistent" `install_or_update_modules`/SF failures from the earlier entry were actually **transient** — caused by other concurrent IA processes. Waiting ~1 minute and retrying the same call succeeds. → IA team: the MCP should surface a "busy/contended, retry" signal instead of opaque errors so agents retry rather than escalate to version/install changes.
- Upgrading an already-installed module is **destructive to application settings**: `installApplicationSettings: false` dropped the entire module settings group (later `GetGroup(...)` returned null → template `NullReferenceException`), and `installApplicationSettings: true` reset all setting values to their defaults. → IA team: module version upgrades should preserve existing application setting values rather than dropping or resetting them.
- NHibernate persistence requires the target SQL **database to pre-exist** — `EnableInstallers()` creates the NHibernate tables but not the database itself, so a first run fails with `Cannot open database "NServiceBus"`. → Consider documenting this prominently or auto-creating the database; it is a silent first-run blocker.

### Process Gaps

- A transient SF glitch produced a config missing the (correctly-modelled) `RegisterHandlers` block; this was misdiagnosed as a stale-DLL / model bug and triggered a long wrong-path investigation, including changing module versions and reinstalling. → Add to `module-increment-loop`: "If SF output is missing content you expect from the model, re-run SF once (failures are usually transient from concurrent IA work) before investigating. Never change module version numbers or reinstall different versions to 'fix' generated output — the install state is usually already correct."
- Runtime verification caught a real module bug that compile + code-review did not: `Microsoft.Data.SqlClient` was never declared for the NHibernate path even though the generated config hardcodes `MicrosoftDataSqlClientDriver` (loaded reflectively, fails only at startup). → Reinforce in `reference-app-builder`: "Each persistence/driver path must be exercised at runtime, not just compiled — reflectively-loaded providers (NHibernate drivers, etc.) fail at startup with no compile-time signal."

---

## 2026-06-25 | Intent.AzureFunctions/Lambda.Dispatch.Wolverine — Serverless TypeLoadMode incompatibility (modules removed)

### Intent Gaps

- Wolverine's `TypeLoadMode.Static` requires `wolverine codegen write` (Oakton CLI routing via `WolverineApplication.RunAsync(args)`). Azure Functions and Lambda override the startup entry point before Oakton runs, so `dotnet run -- wolverine codegen write` silently launches the host instead of the codegen command — pre-built types are never written. → IA team: if a module targets a serverless host, it cannot rely on `wolverine codegen write`; Wolverine needs a serverless-compatible codegen path (MSBuild target, companion runner, or built-in startup hook) before `TypeLoadMode.Static` is usable in that context.
- `TypeLoadMode.Auto` writes `.cs` files to `bin/Debug/Internal/Generated/WolverineHandlers/` at runtime. Azure Functions file watcher detects these new files and restarts the isolated worker, causing "function already exists" conflicts. `TypeLoadMode.Dynamic` was investigated as a possible fix but abandoned before confirming whether it also writes to disk. → IA team: WolverineFx needs clear documentation or a dedicated mode for serverless environments where runtime disk writes are not permitted.

### Process Gaps

- `reference-app-builder` does not ask whether the target host is serverless (Azure Functions isolated worker, Lambda). If it did, the codegen incompatibility would have been discovered during the reference app phase rather than after the modules were fully built and tested. → Add to `reference-app-builder`: "If the target is a serverless host (Azure Functions, Lambda), verify that Wolverine's `TypeLoadMode` and code generation mechanism are compatible with the host's startup model before proceeding. `wolverine codegen write` requires Oakton command routing which serverless hosts bypass."

### PRD / User Gaps

- `module-kickoff` does not ask "does the target host allow runtime disk writes?" or "does the target host support Oakton CLI command routing?" For serverless hosts, both answers are no, and neither `TypeLoadMode.Static` nor `TypeLoadMode.Auto` is viable without a workaround. → Add U-question to `module-kickoff`: "Does any target deployment environment restrict runtime disk writes or override the application startup entry point (e.g. Azure Functions isolated worker, AWS Lambda)? If yes, confirm that the chosen dispatch/messaging framework supports that environment before committing to building a dispatch bridge module."

---

## 2026-06-25 | module-wrap-up skill — Version bump step is wrong for unreleased modules

### Process Gaps

- `module-wrap-up` specifies "Version bump" as the first mandatory step of wrap-up. For modules that are at `1.0.0-pre.0` and have never been published to the feed, there is nothing to bump — all changes accumulate in `pre.0` until first publish. The wrap-up skill has no gate for this case and will incorrectly bump `pre.0` → `pre.1` if followed literally. → Remove the version bump from `module-wrap-up` or add an explicit gate: "Only bump if `pre.0` has been published to the feed. If the module has never been published, skip the version bump entirely."
