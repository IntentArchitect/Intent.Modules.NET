---
name: intent-module-orchestrator
description: Wire cross-module logic, DI/appsettings events, priority bands, and template lookups.
argument-hint: "[event type | factory extension scenario] [target template role or id]"
---

# Intent Module Orchestrator

> [!IMPORTANT]
> **Resource Read Constraint:** You are forbidden from reading the resource files under `/resources/` unless a `dotnet build` fails or a type resolution error occurs.

## Musts
1. **Safe Resolution:** Prefer Role-based lookup via `TemplateRoles.*`. Guard template before accessing `.CSharpFile` (use `?.` or check null).
2. **Callbacks:** Use `TryGetModel<T>` inside callbacks to verify the model shape. Use `TryGetTemplate(...)` for multi-fallback chains.
3. **DI Events:** Publish `ContainerRegistrationRequest` from `OnBeforeTemplateExecution` for standard DI receptors. If generating own `Add*` extensions, register inline instead.
4. **Config Events:** Publish `AppSettingRegistrationRequest` from `OnBeforeTemplateExecution` for JSON config.
5. **Dependencies & Concern:** Declare dependencies with `.HasDependency(...)`. Set `ForConcern` for specific startup target files.
6. **Priority Bands:** Pass explicit priorities to `AfterBuild`: **0 = Core, 100 = Enrichment, 500 = Extension, 1000 = Final**.
7. **Startup DSL:** Prefer `IAppStartupFile` DSL (e.g., `AddServiceConfiguration`, `AddAppConfiguration`, `AddUseEndpointsStatement`) over manual `FindMethod` for Startup/Program.
8. **Broker Filter:** Filter broker event subscriptions using `.FilterMessagesForThisMessageBroker(ExecutionContext, ...)` (pass `ExecutionContext`, NOT `this`).
9. **Project References:** For module projects (e.g. `.csproj` of modules), favor referencing the NuGet package of dependent modules over project references (`<ProjectReference>`) if available.
10. **NuGet Packaging:** Companion or dispatch modules do not need to install the target library's NuGet package onto target projects if the core module already registers and installs it (e.g., MediatR Dispatch doesn't install MediatR because core MediatR does).

## Must Nots
1. Never use Regex to modify `Program.cs` or `appsettings.json`.
2. Never publish registration requests from `OnAfterTemplateRegistrations`.
3. Never call `AddAppConfigurationLambda("UseEndpoints", ...)`; use `AddUseEndpointsStatement` instead.
