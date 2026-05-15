---
name: intent-module-orchestrator
description: "Use when wiring cross-module logic in Intent Architect .NET modules: dispatching DI container or appsettings registration events via EventDispatcher, finding and modifying templates from other modules using FindTemplateInstance(s) / TryGetTemplate, registering OnBuild or AfterBuild callbacks with explicit priority bands, or authoring FactoryExtension classes. USE FOR: ContainerRegistrationRequest, AppSettingRegistrationRequest, ServiceConfigurationRequest, factory extensions, priority ordering, safe template resolution. DO NOT USE FOR: consuming stereotype properties or filtering model collections — use intent-metadata-consumer for those."
argument-hint: "[event type | factory extension scenario] [target template role or id]"
---

# Intent Module Orchestrator

## Workflow Overview

```
OnBeforeTemplateExecution()          ──►  Publish registration events
OnAfterTemplateRegistrations()       ──►  FindTemplateInstance(s) → resolve → guard → AfterBuild
                                      └►  Priority bands enforce execution order
```

## Musts

### Safe Template Resolution

1. **Prefer Role-based lookup** using `TemplateRoles.*` constants as the primary resolution strategy. Fall back to `TemplateId` constant only when no TemplateRole covers the target.
2. **Always guard the result before calling `.CSharpFile`.** Use `?.` null-conditional or an explicit `if (template == null) return;` guard.
3. **Use `TryGetModel<T>` inside `OnBuild`/`AfterBuild` callbacks** whenever the callback must inspect the template-model. Skip the template silently with `continue` when the guard fails.
4. **Prefer `TryGetTemplate(...)` for multi-fallback resolution** (ordered alternatives) over calling `FindTemplateInstance` separately for each candidate.

### Event Orchestration

5. **Publish `ContainerRegistrationRequest`** from `OnBeforeTemplateExecution(...)` to register a type in the DI container. Never write DI wire-up code directly into configuration templates as raw strings.
6. **Publish `AppSettingRegistrationRequest`** from `OnBeforeTemplateExecution(...)` for every key/value the module needs in `appsettings.json`. The framework de-duplicates and idempotently merges entries.
7. **Declare template dependencies** with `.HasDependency(this)` or `.HasDependency(template)` so the Software Factory can sequence file output correctly.
8. **Set `ForConcern`** when the DI registration must land in a specific startup configuration file (e.g., `"Application"`, `"Infrastructure"`).

### Priority Bands for AfterBuild

9. **Always pass an explicit integer** as the second argument to `AfterBuild`. Never omit it for any callback that depends on elements built by another template.

| Band | Integer | Callback type | Purpose |
|---|---|---|---|
| Core | `0` | `OnBuild` | Owning template creates primary class structure |
| Enrichment | `100` | `OnBuild` | Same-module cross-cutting additions (attributes, logging) |
| Extension | `500` | `AfterBuild` | Factory extensions from other modules enrich the file |
| Final | `1000` | `AfterBuild` | `FindMethod`/`FindClass` calls that depend on all prior bands finishing |

### Startup & Service Configuration

10. **When modifying Startup or Program files, prefer high-level `IAppStartupFile` DSL methods over manual `FindMethod` calls.** The DSL transparently handles both the minimal hosting model (`Program.cs`) and the generic hosting model (`Startup.cs`), making the same code work for both project types.
    - `AddServiceConfiguration` / `AddServiceConfigurationLambda` for service-collection entries.
    - `AddAppConfiguration` / `AddAppConfigurationLambda` for middleware pipeline entries.
    - `AddUseEndpointsStatement` for endpoint mapping.
    - `AddServiceConfigurationLambda` / `AddContainerRegistrationLambda` merge safely — calling them multiple times with the same `methodName` appends to the same lambda block rather than generating duplicate method calls.
    - Use `context.Parameters[0]` inside the `(statement, lambda, context)` configure delegate to refer to the lambda's first parameter name (e.g. `"opt"`, `"x"`), ensuring the generated name stays consistent with whatever was declared in `parameters`.

### Metadata Consumption

11. **When consuming stereotype properties inside a factory extension**, delegate the logic to the typed extension methods defined in `*StereotypeExtensions.cs`. See **intent-metadata-consumer** for the full consumption workflow.

## Must Nots

1. **Never use Regex** to modify `Program.cs`, `appsettings.json`, or any other generated file. All structural modifications must go through the CSharpFile builder API or published events.
2. **Never access `template.CSharpFile`** without first confirming the template is non-null.
3. **Never use string literals for stereotype names** when a typed extension method exists for the target stereotype.
4. **Never omit the priority integer** in `AfterBuild` for callbacks that call `FindMethod`, `FindClass`, or `FindStatement` on elements created by other templates.
5. **Never publish registration requests from `OnAfterTemplateRegistrations`** — that lifecycle phase is for finding templates and scheduling build callbacks, not for event dispatch.
6. **Never call `AddAppConfigurationLambda("UseEndpoints", ...)`.** The SDK throws for this path; use `AddUseEndpointsStatement(...)` instead.

## Safe Resolution Hierarchy (Must Follow Order)

```
Step 1  ──►  application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Xxx.Yyy)
Step 2  ──►  if null: application.FindTemplateInstance<ICSharpFileBuilderTemplate>(SomeTemplate.TemplateId)
Step 3  ──►  if null: application.TryGetTemplate(TemplateRoles.Xxx, modelId, out var t) || application.TryGetTemplate(OtherRole, modelId, out t)
Step 4  ──►  Guard before .CSharpFile: if (template == null) return;  OR  template?.CSharpFile.AfterBuild(...)
```

## Pattern Index

Read the relevant pattern file for this skill before generating code:

| Scenario | File to read first |
|---|---|
| `ContainerRegistrationRequest` fluent API | `resources/orchestration-cheatsheet.md` §DI Registration |
| `AppSettingRegistrationRequest` constructor | `resources/orchestration-cheatsheet.md` §AppSettings |
| Full factory extension skeleton | `resources/orchestration-cheatsheet.md` §Factory Extension |
| Safe template resolution and guard patterns | `resources/orchestration-cheatsheet.md` §Resolution |
| AfterBuild priority ordering and FindMethod | `resources/orchestration-cheatsheet.md` §Priority Bands |
| Modifying Startup / Program files (services, middleware, endpoints) | `resources/orchestration-cheatsheet.md` §Startup & Service Configuration |
| Consuming stereotype properties | Load **intent-metadata-consumer** skill |

## Source of Truth

Public repository: <https://github.com/IntentArchitect/Intent.Modules>

Key files:
- `Modules/Intent.Modules.Common.CSharp/DependencyInjection/ContainerRegistrationRequest.cs`
- `Modules/Intent.Modules.Common.CSharp/Configuration/AppSettingRegistrationRequest.cs`
- `Modules/Intent.Modules.Common.CSharp/FactoryExtensions/CSharpFileBuilderFactoryExtension.cs`
- `Modules/Intent.Modules.Common/Plugins/FactoryExtensionBase.cs`
- `Modules/Intent.Modules.Common/Templates/TemplateExtensions.cs` (`TryGetModel<T>` implementation)
- `Modules/Intent.Modules.Common/FileBuilders/FileBuilderHelper.cs` (sort order authority)
- `Modules/Intent.Modules.Common.CSharp/AppStartup/IAppStartupFile.cs` (`IAppStartupFile` DSL interface)
- `Modules/Intent.Modules.Common.CSharp/AppStartup/IAppStartupTemplate.cs` (`IAppStartupTemplate` and `RoleName`)
- `Modules/Intent.Modules.AspNetCore/Templates/AppStartupFile.cs` (reference implementation of `IAppStartupFile`)
