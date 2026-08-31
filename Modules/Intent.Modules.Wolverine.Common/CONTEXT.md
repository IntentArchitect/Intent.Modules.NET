# CONTEXT.md — Intent.Wolverine.Common

This document contains durable architectural decisions, constraints, and patterns for the shared Wolverine host module.

## Why a module rather than shared code

Two modules each bundling their own copy of the `WolverineFx` package binds to whichever loads first and skews on version drift. A common module is the structural fix, and it's the only place a single owner for the registration can live. (Design assumption d-a3, `wolverine-eventing-module` spec.)

**One part of the original rationale was wrong and has been corrected.** This document previously claimed that "no shared-code approach can arbitrate a single `UseWolverine(...)` registration across modules that don't know about each other." Intent's own `ConfigureHostBuilderChainStatement` already arbitrates _existence_ — it is find-or-create, keyed on the emitted `builder.Host.{methodName}(` text, and `SetParameters` accumulates lambda parameters idempotently. Several modules naming `UseWolverine` have always resolved to one lambda. What the platform does **not** provide is ordering (see "Ordering is the actual problem" below), and that is what this module exists for.

## D1 — Conventional discovery stays ON

The originally-approved design had this module disable Wolverine's conventional handler discovery (`DisableConventionalDiscovery()`) and require every contributing module to register its handler types explicitly. That was reversed during design: `Intent.Application.Wolverine`'s own CONTEXT.md records its discovery model as assembly scanning by class-name suffix, and `Intent.Application.Wolverine.DomainEvents`'s CONTEXT.md explicitly calls convention discovery "Wolverine's native pattern — match it exactly". Disabling it would strand both modules' handlers unless they were rewritten to register per-type, which is a breaking change neither shipped module asked for.

This module therefore **never** calls `DisableConventionalDiscovery()`, and registers **no discovery assemblies or types of its own**. A contributing module that needs an assembly or type brought into discovery scope emits that statement from its own contribution — `Intent.Application.Wolverine`'s `opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly)` is the live example.

> An earlier revision of this document stated the guarantee differently: that this module "guarantees each contributing module's assembly is registered for discovery exactly once, via `WolverineHostConfigurationRequest.RequiringDiscoveryOf(assembly)`, de-duplicated across contributors." That was never true in practice. `RequiringDiscoveryOf` took a runtime-loaded `System.Reflection.Assembly`, but the only assemblies loaded while the Software Factory runs are Intent module/SDK assemblies — the consuming application's Application-layer assembly does not exist yet. The method had no callers and could not have produced compilable output; it has been removed along with the request type that carried it.

**Consequence — double-registering the same type is SAFE (measured, not assumed).** An earlier revision asserted that a handler type registered explicitly AND discovered conventionally would make Wolverine see two handlers for one message and fail codegen with a duplicate local (`CS0128`). **That is wrong, and was never measured.** Verified empirically against WolverineFx 5.39.5 (probe app; `HandlerGraph.ChainFor(messageType)` inspected, then the message actually invoked to force handler codegen):

| Registration of one handler type | HandlerCalls in chain | Invoke                |
| -------------------------------- | --------------------- | --------------------- |
| `IncludeAssembly` only           | 1                     | OK, body ran once     |
| `IncludeType<T>()` only          | 1                     | OK, body ran once     |
| **both**                         | **1**                 | **OK, body ran once** |

Wolverine de-duplicates by handler type **plus method**, so registering a type by both routes is idempotent. Contributors may safely emit an explicit per-type registration for a type an `IncludeAssembly` already covers — which is what R18.3's attributability requirement needs.

The genuine multi-handler case is **two distinct handler classes** for one message type. In 5.39.5 that yields 2 `HandlerCall`s combined under `MultipleHandlerBehavior.ClassicCombineIntoOneLogicalHandler`, and in the simple case measured it still generated and executed cleanly — so the `CS0128` failure mode is narrower than the old note claimed and is unrelated to double-registering a single type.

**Conventional discovery only scans the ENTRY assembly.** Also measured: a convention-named handler in a _referenced_ (non-entry) assembly is NOT found by bare conventional discovery — it needs either `IncludeAssembly` or `IncludeType<T>()`. This makes `Intent.Application.Wolverine`'s `opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly)` load-bearing rather than redundant: it is the only thing pulling the Application layer into discovery scope, and sibling modules that generate convention-named handlers into that assembly while registering nothing of their own (notably `Intent.Application.Wolverine.DomainEvents`) depend on it.

## Ordering is the actual problem

Two defects were conflated in the original design. Separating them is what let the mechanism shrink.

**The `Statements.Clear()` bug.** Before this module, `Intent.Application.Wolverine`'s `WolverineRegistrationFactoryExtension` called `lambdaBlock.Statements.Clear()` inside its `ConfigureHostBuilderChainStatement("UseWolverine", ...)` callback, before adding its own statement. Nothing stopped one module's callback from wiping out whatever an earlier module's callback had already added. **The fix for this is deleting that one line** — it never needed an architecture.

**The genuine, remaining problem is order.** The ASP.NET `ProgramFile.ConfigureHostBuilderChainStatement` accepts a `priority` parameter and **never reads it** — it is declared and dropped. The `IAppStartupFile.ConfigureServices(...)` overload it delegates to has no priority parameter at all. So statement order inside the lambda is decided purely by the order contributors queue their callbacks, which before this module was arbitrary: all three Wolverine factory extensions were `Order => 0`.

## Cross-module contribution mechanism

This module **seeds** the lambda: from `OnAfterTemplateRegistrations`, inside a `CSharpFile.OnBuild(...)` callback on the ASP.NET host's program template, it calls `ConfigureHostBuilderChainStatement("UseWolverine", new[] { "opts" })` with **no configure callback**. Contributing modules then call the same DSL method with their own callback; find-or-create resolves them onto the seeded lambda.

Contribution order is factory-extension `Order`, because each contributor registers its `OnBuild` callback on the same `CSharpFile` during its own `OnAfterTemplateRegistrations`, and `OnBuild` callbacks fire in registration order:

| Module                            | `Order` |
| --------------------------------- | ------- |
| `Intent.Wolverine.Common` (seeds) | `0`     |
| `Intent.Application.Wolverine`    | `10`    |
| `Intent.Eventing.Wolverine`       | `20`    |

**`Order` also decides where the statement LANDS in `Program.cs`, not just contribution order.** This was measured, not predicted. Seeding from `Order => -10` queues the DSL's `ConfigureServices` callback earlier than the previous implementation did, which relocates the whole `builder.Host.UseWolverine(...)` statement from _above_ the `builder.Services.*` block to _below_ it — and drags the neighbouring `builder.Host.UseSerilog(...)` call, owned by `Intent.Modules.AspNetCore.Logging.Serilog`, along with it. `0` is the value the previous implementation effectively used, so it preserves placement; the contributors move out to `10`/`20` instead. **Do not move this module negative to make it "more first".**

**Why there is no request type any more.** The previous design routed contributions through a `WolverineHostConfigurationRequest` recorded in a static `ConditionalWeakTable` keyed by the `IProgramTemplate`, consumed inside this module's own `OnBuild`. It was removed because it bought nothing the DSL does not already provide, and cost three things:

- It was not a _request_ in the sense `ContainerRegistrationRequest` is. A request is declarative data the owning template interprets; that type carried a raw `(lambdaBlock, parameters)` callback, so the contributor still wrote the statement string itself and still had to know the lambda's shape (`parameters[0]` is `opts`, and `Intent.Eventing.Wolverine` hardcoded `builder.Configuration`, a variable belonging to the host template's scope). The encapsulation it advertised leaked straight back out.
- Correctness depended on an unenforceable call-site rule — contributors had to call `Contribute()` eagerly and never from inside `OnBuild`, or the contribution silently vanished — documented only in a comment triplicated across three files.
- Its `WithPriority` had no callers (both contributors sat at the default `0`), and its `RequiringDiscoveryOf` was unimplementable (see D1).

**Why not the `OnEmitOrPublished`/`EmitOrPublish` event bus** (the pattern `Intent.Modules.Application.DependencyInjection`'s `DependencyInjectionTemplate` uses for `ContainerRegistrationRequest`): that pattern requires the _owning_ template to subscribe `OnEmitOrPublished<T>` in its own constructor. Here the "owning" template (`IProgramTemplate`, role `App.Program`) belongs to `Intent.Modules.AspNetCore`, a module this one does not own and cannot add a constructor subscription to. That reasoning still holds — but the conclusion drawn from it originally (a static contribution registry) skipped the option that turned out to be correct: use the DSL, which already merges.

## Host scope

This module targets the ASP.NET host (`App.Program`) only. Azure Functions and other non-ASP.NET hosts are explicitly out of scope — see `Intent.Application.Wolverine`'s own removal of its Azure Functions registration (R8.7 of the `wolverine-eventing-module` spec), which was never functioning correctly under any `TypeLoadMode` in that hosting model.

## No C# template and no API surface of its own

This module emits no file and exposes no public types for other modules to reference. It is a single Factory Extension. The only output it touches is the ASP.NET Core `Program` file, owned by `Intent.Modules.AspNetCore`.

That is deliberate: **contributors take no `ProjectReference` on this module.** They declare it as an `.imodspec` `<dependency>` so it is installed and its seed runs, but they reference none of its types — Tier-0 coupling in `intent-module-orchestrator`'s terms. A `PrivateAssets="All"` `ProjectReference` would bundle a second copy of this module's DLL into each contributor alongside the copy this module itself ships, and the loader binds to whichever loads first.

Removing those `ProjectReference`s also retired a documented hazard. This section previously warned that a module taking a `ProjectReference` here while referencing `Intent.Modules.Common.CSharp` at a different version could fail to compile with a misleading error (e.g. `'CSharpLambdaBlock' does not contain a definition for 'AddStatement'`, whose actual cause in `Intent.Eventing.Wolverine` turned out to be a missing `using Intent.Modules.Common.CSharp.Builder`). It also claimed this module's package references had been bumped to match `Intent.Application.Wolverine`'s "to keep the three Wolverine modules on one consistent floor" — **that was never true on disk**: this module builds against `Intent.Modules.Common` 3.7.2 / `.CSharp` 3.8.1 while both contributors use 3.11.2 / 3.10.10. With no `ProjectReference` between them the skew no longer affects compilation, so the versions have been left alone rather than churned.
