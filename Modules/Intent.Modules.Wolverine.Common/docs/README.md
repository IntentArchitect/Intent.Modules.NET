# Intent.Wolverine.Common

Establishes the single, shared Wolverine host registration (`builder.Host.UseWolverine(opts => ...)`) for an application's ASP.NET host, and fixes the order in which other Wolverine-based modules contribute to it.

## Why this module exists

Intent Architect's `ConfigureHostBuilderChainStatement` is already find-or-create: it looks for an existing `builder.Host.UseWolverine(` statement and only creates one when absent, so several modules naming `UseWolverine` all resolve to the same lambda rather than emitting competing registrations.

What it does _not_ give you is order. The ASP.NET implementation accepts a `priority` argument and never reads it, and the `ConfigureServices` callback it delegates to has no priority parameter at all — so which contributor's statements land first was previously decided by arbitrary factory-extension execution order. Worse, nothing stopped a contributor calling `.Statements.Clear()` before adding its own statements, silently discarding whatever an earlier contributor had already added.

This module resolves both by owning the _creation_ of the lambda and by anchoring the contribution order.

## What this module generates

This module emits **no file of its own** — it is a factory extension. The only output it touches is the ASP.NET Core `Program` file, which `Intent.Modules.AspNetCore` owns.

- `WolverineHostRegistrationExtension` — a Factory Extension that, for every ASP.NET host program template in the application, seeds the `builder.Host.UseWolverine(opts => ...)` statement with no statements of its own, and declares the `WolverineFx` package reference and the `using Wolverine` that registration needs.

Seeding rather than waiting for a contributor is what makes the output deterministic: the position of the `UseWolverine` statement within `Program.cs` no longer depends on which contributing modules happen to be installed.

## How to contribute to the shared registration

From another module's factory extension, call the DSL directly — there is no bespoke request type to construct:

```csharp
foreach (var programTemplate in application.FindTemplateInstances<IProgramTemplate>("App.Program"))
{
    programTemplate.CSharpFile.OnBuild(file =>
    {
        programTemplate.ProgramFile.ConfigureHostBuilderChainStatement("UseWolverine", new[] { "opts" },
            (lambdaBlock, parameters) =>
            {
                var opts = parameters[0];
                var configType = programTemplate.GetTypeName(YourConfigurationTemplate.TemplateId);
                lambdaBlock.AddStatement($"{configType}.Configure({opts});");
            });
    });
}
```

Three rules apply to a contributor:

- **Never call `lambdaBlock.Statements.Clear()`.** It discards other contributors' statements and was the original defect this module exists to prevent.
- **Do not re-declare `WolverineFx` or `using Wolverine`** on the program template. This module owns both.
- **Give the factory extension an explicit `Order` above this module's.** Statements land inside the lambda in ascending factory-extension `Order`, because each contributor registers its `OnBuild` callback on the same `CSharpFile` and those callbacks fire in registration order.

### Contribution order

| Module                                       | `Order` |
| -------------------------------------------- | ------- |
| `Intent.Wolverine.Common` (seeds the lambda) | `0`     |
| `Intent.Application.Wolverine`               | `10`    |
| `Intent.Eventing.Wolverine`                  | `20`    |

A new contributor should pick a value above `0`, leaving gaps so later modules can be slotted between existing ones.

`Intent.Wolverine.Common` deliberately stays at `0` rather than moving negative. Where the generated `builder.Host.UseWolverine(...)` statement lands in `Program.cs` depends on when the DSL's `ConfigureServices` callback is queued relative to the ones adding `builder.Services.*`; seeding from a negative `Order` queues it earlier and relocates the statement below the `builder.Services` block, taking the neighbouring `builder.Host.UseSerilog(...)` call — owned by another module — along with it.

## What this module does NOT do

- It does not disable Wolverine's conventional handler discovery, and it registers no discovery assemblies of its own. A contributing module that needs an assembly or type brought into discovery scope emits that statement itself, from its own contribution.
- It targets the ASP.NET host only. Azure Functions and other non-ASP.NET hosts are out of scope.
- It generates no application-visible file — there is nothing to configure or override in a consuming application beyond installing it (which happens automatically as a dependency of any module that needs it).
