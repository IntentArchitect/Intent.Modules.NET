# Intent.Wolverine.Common

Owns the single, shared Wolverine host registration (`builder.Host.UseWolverine(opts => ...)`) for an application's ASP.NET host, so that installing more than one Wolverine-based module never results in two competing registrations.

## Why this module exists

Before this module, each Wolverine-based module (e.g. `Intent.Application.Wolverine`) registered its own `UseWolverine(...)` call. Intent Architect's `ConfigureHostBuilderChainStatement` already finds-or-creates a single lambda rather than adding a second one, but nothing stopped one module's factory extension from calling `.Statements.Clear()` before adding its own statements — silently discarding whatever an earlier contributor had already added, with the outcome depending on arbitrary factory-extension execution order.

This module fixes that by being the only place the `UseWolverine(...)` lambda is actually built. Every other Wolverine-based module contributes to it instead of registering its own.

## What this module generates

This module emits **no file of its own** — it is a factory extension plus an API surface. The only output it touches is the ASP.NET Core `Program` file, which `Intent.Modules.AspNetCore` owns.

- `WolverineHostRegistrationExtension` — a Factory Extension that, for every ASP.NET host program template in the application, builds the single `UseWolverine(opts => ...)` lambda from every contributed request, sorted by priority, and adds one `opts.Discovery.IncludeAssembly(...)` per distinct assembly a contributor asked to be discovered. It never calls `DisableConventionalDiscovery()` — conventional discovery stays on, so a handler in a contributing module's assembly is found without that module needing an explicit per-type registration.
- `WolverineHostConfigurationRequest` (`Intent.Wolverine.Common.Api`) — the contribution type. A contributing module's factory extension builds one of these (statements to add inside the `opts` lambda, a priority, and any assemblies it needs discovered) and calls `WolverineHostRegistrationExtension.Contribute(programTemplate, request)`.

## How to contribute to the shared registration

From another module's factory extension:

```csharp
foreach (var programTemplate in application.FindTemplateInstances<IProgramTemplate>("App.Program"))
{
    programTemplate.CSharpFile.OnBuild(file =>
    {
        WolverineHostRegistrationExtension.Contribute(programTemplate,
            WolverineHostConfigurationRequest
                .Configure((lambdaBlock, parameters) =>
                {
                    var opts = parameters[0];
                    lambdaBlock.AddStatement($"{yourConfigType}.Configure({opts});");
                })
                .WithPriority(0)
                .RequiringDiscoveryOf(typeof(YourMarkerType).Assembly));
    });
}
```

Do **not** call `ConfigureHostBuilderChainStatement("UseWolverine", ...)` directly from another module — only this module does that, exactly once per host.

## What this module does NOT do

- It does not disable Wolverine's conventional handler discovery. Every contributing module's handlers are found by Wolverine's own naming convention; this module only guarantees each contributor's assembly is registered for that discovery exactly once.
- It targets the ASP.NET host only. Azure Functions and other non-ASP.NET hosts are out of scope.
- It generates no application-visible file — there is nothing to configure or override in a consuming application beyond installing it (which happens automatically as a dependency of any module that needs it).
