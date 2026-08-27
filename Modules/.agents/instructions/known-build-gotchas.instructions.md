---
applyTo: '**/*.cs'
description: >
Template authoring pitfalls and solutions: NuGet dependency registration,
keywords: [intent architect, template authoring, nuget, build, gotchas, constants]
contentHash: F19E6242A956CC55B58BD89DC322D76288392A26EEC8DD86D8FEFE78C313402D
---
## Known Build Gotchas

### NuGet Dependencies — Not Inside `OnBuild`

Declare NuGet dependencies in the **template constructor**, not inside `OnBuild` or `AfterBuild` lambdas. Ideally declare them at the top of the constructor, but conditional registration based on model state is valid mid-constructor. The rule is strictly: never inside a build callback.

```csharp
// Correct — constructor body
public MyTemplate(...)
{
AddNugetDependency(NuGetPackages.SomePackage);
if (model.NeedsExtra()) AddNugetDependency(NuGetPackages.ExtraPackage);

CSharpFile = new CSharpFile(...);
}

// Wrong — inside OnBuild
OnBuild(file =>
{
AddNugetDependency(...); // ❌ will not work reliably
});
```

===

### `SingleFileListModel` — Filename Instability

When a template uses `SingleFileListModel` and generates multiple classes via `foreach`, `CSharpFile` derives its filename from the **first class added**. If that order is non-deterministic, the filename changes between SF runs.

The anchor-class approach (adding a dummy first class) is awkward and not recommended. For exceptional cases, use:

```csharp
// IntentIgnore
CSharpFile = new CSharpFile("DesiredFileName", folderPath)
```

`// IntentIgnore` prevents SF from overwriting that line, letting you hardcode the filename directly. Reserve this for genuinely exceptional multi-class single-file scenarios — the normal pattern is one class per template output.

===

### `FilterMessagesForThisMessageBroker` — Pass `ExecutionContext`, Not `this`

The three-argument overload requires an `ISoftwareFactoryExecutionContext`. Passing `this` (the template instance) compiles but fails silently at runtime — the filter returns incorrect results.

```csharp
// Correct
FilterMessagesForThisMessageBroker(messages, selector, ExecutionContext);

// Wrong
FilterMessagesForThisMessageBroker(messages, selector, this); // ❌
```

===

### `Constants` Class Name Conflict

If your module defines a `Constants` class, it conflicts with `Intent.Modules.Constants` from the SDK. Use an alias:

```csharp
using NServiceBusConstants = Intent.Modules.Eventing.NServiceBus.Templates.Constants;
```

===

### NuGet Package Downgrade Errors (NU1605)

You may encounter SDK package versions after an SF run, triggering `NU1605` downgrade errors. When this happens, manually correct the affected package versions in the `.csproj`. The root cause is NuGet versions drifting out of sync with the corresponding Intent module version — keep them aligned to avoid recurrence.

Packages most commonly affected: `Intent.Modules.Common`, `Intent.Modules.Common.CSharp`, `Intent.SoftwareFactory.SDK`.

===

### Consumer App Name Colliding With a Referenced Broker Library's Root Namespace

If a consumer application's own name (and therefore its root C# namespace) starts with the same
segment as a broker/framework package's own namespace — e.g. an app called `Wolverine.*` alongside
the `WolverineFx` package (namespace `Wolverine`), an app called `MassTransit.*` alongside the
`MassTransit` package (namespace `MassTransit`), or `NServiceBus.*` alongside `NServiceBus` — any
unqualified type name that exists in BOTH namespaces (`IMessageBus`, `IBus`, `Envelope`, etc.)
resolves to the WRONG one, silently.

**Why:** C# resolves an unqualified name by walking OUTWARD through enclosing namespaces before it
ever consults `using` directives. A class in namespace `Wolverine.Transport.Local.Application.Orders`
has `Wolverine` as an enclosing namespace segment — so if `WolverineFx` is referenced, its top-level
`Wolverine.IMessageBus` is visible as an enclosing-namespace member and wins over a `using` that was
written to bring in the app's own `...Application.Common.Eventing.IMessageBus`. The result compiles
against the wrong interface and fails with a misleading `CS1061 'IMessageBus' does not contain a
definition for 'Publish'` — the RIGHT interface (with `Publish`) is right there, but never reached.

This is invisible until a consumer app happens to be named the same as the broker package's own
namespace — a golden sample or test app named `WolverineEventing.*` / `MyCompany.MassTransit.*`
never collides; one named bare `Wolverine.*` / `MassTransit.*` always will, for every ambiguous type
the two namespaces share.

**How to avoid it, in priority order:**
1. **Never name a consumer/test application after the broker package's own namespace verbatim.**
   Prefix or suffix it (`WolverineEventing.*`, `AcmeWolverine.*`) so the app's root namespace segment
   never textually equals the package's own top-level namespace.
2. **Templates that emit a type whose unqualified name could collide** (any generated
   `IMessageBus`/`IBus`/similar) should consider emitting it via a fully-qualified reference or a
   distinctive alias rather than relying on an unqualified name plus a `using`, precisely because the
   template author cannot control what the consumer names their application.

===

### Template Changes Not Taking Effect

Building a module compiles the `.csproj` that represents it, and the step that packages the `.imod` runs off that compilation. If your changes were to non-C# files, the compilation may not trigger, the package step is skipped, and no new `.imod` is produced — the templates then keep generating from the previously packaged content, with nothing reported.

To force it:

```
dotnet build --no-incremental
```
