# CONTEXT.md — Intent.Wolverine.Common

This document contains durable architectural decisions, constraints, and patterns for the shared Wolverine host module.

## Why a module rather than shared code

Two modules each bundling their own copy of the `WolverineFx` package binds to whichever loads first and skews on version drift, and no shared-code approach can arbitrate a single `UseWolverine(...)` registration across modules that don't know about each other. A common module is the structural fix, and it's the only place a single registration can be owned. (Design assumption d-a3, `wolverine-eventing-module` spec.)

## D1 — Conventional discovery stays ON

The originally-approved design had this module disable Wolverine's conventional handler discovery (`DisableConventionalDiscovery()`) and require every contributing module to register its handler types explicitly. That was reversed during design: `Intent.Application.Wolverine`'s own CONTEXT.md records its discovery model as assembly scanning by class-name suffix, and `Intent.Application.Wolverine.DomainEvents`'s CONTEXT.md explicitly calls convention discovery "Wolverine's native pattern — match it exactly". Disabling it would strand both modules' handlers unless they were rewritten to register per-type, which is a breaking change neither shipped module asked for.

This module therefore **never** calls `DisableConventionalDiscovery()`. It only guarantees that each contributing module's assembly is registered for discovery exactly once (via `WolverineHostConfigurationRequest.RequiringDiscoveryOf(assembly)`), de-duplicated across contributors.

**Consequence — double-registering the same type is SAFE (measured, not assumed).** An earlier revision of this document asserted that a handler type registered explicitly AND discovered conventionally would make Wolverine see two handlers for one message and fail codegen with a duplicate local (`CS0128`). **That is wrong, and was never measured.** Verified empirically against WolverineFx 5.39.5 (probe app; `HandlerGraph.ChainFor(messageType)` inspected, then the message actually invoked to force handler codegen):

| Registration of one handler type | HandlerCalls in chain | Invoke |
|---|---|---|
| `IncludeAssembly` only | 1 | OK, body ran once |
| `IncludeType<T>()` only | 1 | OK, body ran once |
| **both** | **1** | **OK, body ran once** |

Wolverine de-duplicates by handler type **plus method**, so registering a type by both routes is idempotent. Contributors may safely emit an explicit per-type registration for a type an `IncludeAssembly` already covers — which is what R18.3's attributability requirement needs.

The genuine multi-handler case is **two distinct handler classes** for one message type. In 5.39.5 that yields 2 `HandlerCall`s combined under `MultipleHandlerBehavior.ClassicCombineIntoOneLogicalHandler`, and in the simple case measured it still generated and executed cleanly — so the `CS0128` failure mode is narrower than the old note claimed and is unrelated to double-registering a single type.

**Conventional discovery only scans the ENTRY assembly.** Also measured: a convention-named handler in a *referenced* (non-entry) assembly is NOT found by bare conventional discovery — it needs either `IncludeAssembly` or `IncludeType<T>()`. This makes `Intent.Application.Wolverine`'s `opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly)` load-bearing rather than redundant: it is the only thing pulling the Application layer into discovery scope, and sibling modules that generate convention-named handlers into that assembly while registering nothing of their own (notably `Intent.Application.Wolverine.DomainEvents`) depend on it.

## The `Statements.Clear()` bug this module exists to fix

Before this module, `Intent.Application.Wolverine`'s `WolverineRegistrationFactoryExtension` called `lambdaBlock.Statements.Clear()` inside its `ConfigureHostBuilderChainStatement("UseWolverine", ...)` callback, before adding its own statement. `ConfigureHostBuilderChainStatement` already finds-or-creates a single lambda across modules, but nothing stopped one module's callback from wiping out whatever an earlier module's callback had already added — and whether that happened depended on arbitrary factory-extension execution order.

## Cross-module contribution mechanism

Wolverine's host registration is built exactly once, by `WolverineHostRegistrationExtension`, inside a `CSharpFile.OnBuild(...)` callback on the ASP.NET host's program template. Contributing modules do not call `ConfigureHostBuilderChainStatement` themselves — they call `WolverineHostRegistrationExtension.Contribute(programTemplate, request)` from their own `OnAfterTemplateRegistrations`, where `request` is a `WolverineHostConfigurationRequest` built via its fluent API (`.Configure(...)`, `.WithPriority(...)`, `.RequiringDiscoveryOf(...)`).

**Why this is order-independent regardless of factory-extension `Order` values:** contributions are recorded during the `AfterTemplateRegistrations` phase (any extension, any `Order`), and only consumed inside a later `CSharpFile.OnBuild` callback — which runs in the SF's subsequent Build phase, strictly after every factory extension's `OnAfterTemplateRegistrations` has completed application-wide. The `Order` property on `FactoryExtensionBase` therefore has no bearing on whether a contribution is captured.

**Why not the `OnEmitOrPublished`/`EmitOrPublish` event bus** (the pattern `Intent.Modules.Application.DependencyInjection`'s `DependencyInjectionTemplate` uses for `ContainerRegistrationRequest`): that pattern requires the _owning_ template to subscribe `OnEmitOrPublished<T>` in its own constructor. Here the "owning" template (`IProgramTemplate`, role `App.Program`) belongs to `Intent.Modules.AspNetCore`, a module this one does not own and cannot add a constructor subscription to. A plain static contribution registry, keyed by the `IProgramTemplate` instance (`ConditionalWeakTable`), was used instead.

## Host scope

This module targets the ASP.NET host (`App.Program`) only. Azure Functions and other non-ASP.NET hosts are explicitly out of scope — see `Intent.Application.Wolverine`'s own removal of its Azure Functions registration (R8.7 of the `wolverine-eventing-module` spec), which was never functioning correctly under any `TypeLoadMode` in that hosting model.

## No C# Template of its own

This module emits no file — it is a Factory Extension plus an API surface (`WolverineHostConfigurationRequest`). The only output it touches is the ASP.NET Core `Program` file, owned by `Intent.Modules.AspNetCore`.

## Package-version alignment across ProjectReference consumers

Any module that both (a) takes a `ProjectReference` to this module's `.csproj` and (b) directly references `Intent.Modules.Common.CSharp` at a _different_ version than this module does can fail to compile with a misleading error — e.g. `'CSharpLambdaBlock' does not contain a definition for 'AddStatement'` — even though both versions individually expose that member. NuGet/MSBuild's assembly-conflict resolution unifies same-named-assembly references to a single version for the compile, but only correctly when every project in the reference graph is restored against versions whose public surface didn't shift between them; a stale `using` gap (missing `Intent.Modules.Common.CSharp.Builder`) can produce the exact same symptom, so check usings _before_ touching package versions. `Intent.Eventing.Wolverine` hit this when wiring its `WolverineHostConfigurationRequest.Configure(...)` contribution (T3.5) — the actual cause there was a missing `using`, not a version mismatch, but this module's own package references (`Intent.Modules.Common`/`.CSharp`/`Intent.SoftwareFactory.SDK`) were bumped to match `Intent.Application.Wolverine`'s anyway, to keep the three Wolverine modules on one consistent floor and avoid re-diagnosing this class of error later.
