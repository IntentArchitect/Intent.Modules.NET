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
