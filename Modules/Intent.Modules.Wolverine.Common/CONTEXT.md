# CONTEXT.md — Intent.Wolverine.Common

This document contains durable architectural decisions, constraints, and patterns for the shared Wolverine host module.

## Why this module exists

Wolverine is configured once per host, via a single `builder.Host.UseWolverine(opts => { ... })` call. Multiple Wolverine-based modules (CQRS dispatch, eventing) need to contribute to that one call without knowing about each other or fighting over it. Two problems fall out of that:

- **Package duplication.** If each contributing module bundled its own `WolverineFx` package reference, an app installing more than one would end up with two copies of the DLL, and the loader binds to whichever loads first — a version-skew hazard that's easy to hit and hard to diagnose. A single common module is the one place `WolverineFx` is referenced.
- **Ordering.** Intent's `ConfigureHostBuilderChainStatement` DSL already merges same-named host builder calls into one lambda — it is find-or-create, keyed on the emitted `builder.Host.{methodName}(` text, so several modules calling `UseWolverine` always resolve to one lambda for free. What it does **not** provide is control over which contributor's statement lands first inside that lambda: the ASP.NET `ConfigureHostBuilderChainStatement` accepts a `priority` parameter and never reads it, and the `ConfigureServices` overload it delegates to has no priority parameter at all. Left alone, statement order is decided purely by the order contributors happen to queue their callbacks.

This module exists to arbitrate that ordering and to be the single owner of the `WolverineFx` package reference. (Design assumption d-a3, `wolverine-eventing-module` spec.)

## Cross-module contribution mechanism (revised at 1.0.0-pre.1 — supersedes the DSL-append design below)

**As of 1.0.0-pre.1 this module owns a C# Template, `WolverineConfiguration` (`Templates/WolverineConfiguration/`), and it is the SOLE place any Wolverine module touches `Program.cs`.** `WolverineHostRegistrationExtension.SeedWolverineHostRegistration` (`Order => 0`) does two things in one `OnBuild` callback on the ASP.NET host's program template: it finds-or-creates the `builder.Host.UseWolverine(opts => { ... })` lambda via `ConfigureHostBuilderChainStatement`, **and** supplies its one and only statement — `{WolverineConfiguration}.Configure(opts, builder.Configuration);` — in the same call. Contributing modules (`Intent.Application.Wolverine`, `Intent.Eventing.Wolverine`) no longer reach into the host builder at all.

```csharp
public static class WolverineConfiguration
{
    public static void Configure(WolverineOptions opts, IConfiguration configuration)
    {
        ConfigureCqrs(opts);                        // contributed by Intent.Application.Wolverine
        ConfigureEventing(opts, configuration);      // contributed by Intent.Eventing.Wolverine
    }
}
```

Instead, each contributor finds this module's `WolverineConfigurationTemplate` by its string `TemplateId` (`"Intent.Wolverine.Common.WolverineConfiguration"` — no `ProjectReference` here, so it cannot be a compiled constant on the contributor's side) and, inside its own `OnAfterTemplateRegistrations`, registers an `OnBuild` callback on that **foreign** `CSharpFile`: it adds its own private method (`ConfigureCqrs` / `ConfigureEventing`) to the class, and one call statement into the shared `Configure` method body. This is the identical find-template + `OnBuild` + `AddMethod`/`AddStatement` idiom `Intent.Eventing.MassTransit`'s `FinbuckleConfiguratorExtension` already uses on `MassTransitConfigurationTemplate` — nothing new was invented for it.

Contribution order (which contributor's call statement lands first in `Configure`'s body) is still controlled by factory-extension `Order`, because `OnBuild` callbacks on the same `CSharpFile` fire in registration order:

| Module                            | `Order` | What it contributes |
| --------------------------------- | ------- | -------------------- |
| `Intent.Wolverine.Common` (seeds `Program.cs` only) | `0`     | — |
| `Intent.Application.Wolverine`    | `10`    | `ConfigureCqrs(opts)` |
| `Intent.Eventing.Wolverine`       | `20`    | `ConfigureEventing(opts, configuration)` |

An app with only one of the two contributor modules installed simply gets one private method and one call statement — the constructor emits both helper method NAMES unconditionally with empty bodies as an anchor (see the template source), so an app with neither eventing nor CQRS installed still compiles; each contributor's callback is independent and self-contained.

**`Order` on `Intent.Wolverine.Common`'s OWN extension still decides where the `builder.Host.UseWolverine(...)` STATEMENT lands in `Program.cs`** relative to unrelated neighbours (e.g. `Intent.Modules.AspNetCore.Logging.Serilog`'s `UseSerilog(...)`), for the same DSL-queuing reason as before — `Order => 0` is deliberate, not a leftover from the old design; do not move it negative.

**Never call `lambdaBlock.Statements.Clear()`** inside the `Program.cs` `OnBuild` callback, and never call `file.Classes.First().Methods.Clear()`/similar on `WolverineConfigurationTemplate`'s own file — both are shared with other contributors.

### Why this replaced the original DSL-append-per-module design

The original design (each contributor independently calling `ConfigureHostBuilderChainStatement("UseWolverine", ...)` to append its OWN class's call, `Intent.Application.Wolverine`'s `WolverineConfiguration.Configure(opts)` and `Intent.Eventing.Wolverine`'s `WolverineEventingConfiguration.Configure{Transport}(opts, config)` both appending into the SAME seeded lambda) worked, but left two classes doing one job, with transport-named methods (`ConfigureLocal`, `ConfigureRabbitMq`, …) leaking the eventing transport choice into the public shape of generated code — and no single place a consumer could point at as "the" Wolverine configuration. The two alternative mechanisms considered when the DSL-append design was FIRST chosen (a dedicated request type collected and replayed; the `OnEmitOrPublished`/`EmitOrPublish` event-bus pattern `ContainerRegistrationRequest` uses) were rejected then for the same reasons they're still not used now — see the git history of this file for that reasoning if it's needed again. What changed is that "one shared class, contributors adding a private method + call statement to it" turned out to be a third option that gets the single-entry-point property the DSL-append design couldn't, using the SAME `TemplateDependency`/`OnBuild` cross-template mechanism this codebase already established elsewhere (`FinbuckleConfiguratorExtension`), not a new one.

## D1 — Conventional discovery stays ON

This module **never** calls `DisableConventionalDiscovery()`, and registers **no discovery assemblies or types of its own**. A contributing module that needs an assembly or type brought into discovery scope emits that statement from its own contribution — `Intent.Application.Wolverine`'s `opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly)` is the live example.

**Conventional discovery only scans the ENTRY assembly.** Measured against WolverineFx 5.39.5: a convention-named handler in a referenced (non-entry) assembly is NOT found by bare conventional discovery — it needs either `IncludeAssembly` or `IncludeType<T>()`. This makes `Intent.Application.Wolverine`'s `IncludeAssembly` call load-bearing rather than redundant: it is the only thing pulling the Application layer into discovery scope, and sibling modules that generate convention-named handlers into that assembly while registering nothing of their own (notably `Intent.Application.Wolverine.DomainEvents`) depend on it.

**Registering a handler type explicitly, on top of conventional discovery finding the same type, is safe — measured, not assumed.** Verified empirically against WolverineFx 5.39.5 (probe app; `HandlerGraph.ChainFor(messageType)` inspected, then the message actually invoked to force handler codegen):

| Registration of one handler type | HandlerCalls in chain | Invoke                |
| -------------------------------- | --------------------- | --------------------- |
| `IncludeAssembly` only           | 1                     | OK, body ran once     |
| `IncludeType<T>()` only          | 1                     | OK, body ran once     |
| **both**                         | **1**                 | **OK, body ran once** |

Wolverine de-duplicates by handler type **plus method**, so registering a type by both routes is idempotent — contributors may safely emit an explicit per-type registration for a type an `IncludeAssembly` already covers, which is what R18.3's attributability requirement needs (each module's registration is attributable to the module that owns the handler, not just riding in on someone else's blanket scan).

The genuine multi-handler case is **two distinct handler classes** for one message type — that is a real duplicate-handler shape, and unrelated to double-registering a single type by two routes.

## Host scope

This module targets the ASP.NET host (`App.Program`) only. Azure Functions and other non-ASP.NET hosts are out of scope — see `Intent.Application.Wolverine`'s own CONTEXT.md for why serverless hosts don't work with Wolverine's code generation model today (R8.7 of the `wolverine-eventing-module` spec).

## One C# Template, `WolverineConfiguration` — superseded, see "Cross-module contribution mechanism" above

Until 1.0.0-pre.1 this module emitted no file of its own and exposed no public types — a single Factory Extension only, touching nothing but the ASP.NET Core `Program` file. That is **no longer true**: it now owns `WolverineConfiguration` (`Templates/WolverineConfiguration/WolverineConfigurationTemplatePartial.cs`), a public static class every Wolverine app generates, whose `Configure(WolverineOptions, IConfiguration)` is the single entry point contributors extend. `Program.cs` is still the only OTHER output this module touches, and still only through the seed described above.

**Contributors still take no `ProjectReference` on this module** — they reach `WolverineConfigurationTemplate` by its string `TemplateId`, not a compiled type reference, and declare it as an `.imodspec` `<dependency>` so it is installed and both its template and its `Program.cs` seed run. This is Tier-1 coupling in `intent-module-orchestrator`'s terms (role/id-string lookup through a generic interface, `ICSharpFileBuilderTemplate`) — deliberately not Tier-2 (a typed model reference), for the same package-duplication reason as always: a `PrivateAssets="All"` `ProjectReference` would bundle a second copy of this module's DLL into each contributor alongside the copy this module itself ships, and the loader binds to whichever loads first.

This module's package references (`Intent.Modules.Common` / `Intent.Modules.Common.CSharp`) are versioned independently of its contributors'. With no `ProjectReference` between them, a version difference doesn't affect compilation, so there is no need to keep them in lockstep.
