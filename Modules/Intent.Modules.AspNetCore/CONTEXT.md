# CONTEXT — Intent.Modules.AspNetCore

## Multiple web hosts in one application (multi-host / dual API)

An Intent application may contain **more than one ASP.NET Core host project** — e.g. a `App.Api` and a
`Mobile.Api` in the same application, each with its own `Program`/`Startup`, Swagger, filters and
controllers. This is achieved purely in the **Codebase Structure designer** by duplicating the relevant
`Template Output` elements into a second web project, and routing per-model templates to a host with the
`Template Output Settings` → `Registration Filter` property (a Dynamic LINQ predicate over the template's
model; for controllers `IControllerModel` implements `IHasFolder`, so `np(Folder.Name) == "Mobile"` routes
by Services-designer folder).

### Invariant: never look up a host-scoped template with the singular overload

The Software Factory engine creates **one template instance per `Template Output` element**. Duplicating a
Template Output therefore *correctly* produces N instances — one per host. The engine needs no changes to
support this.

What breaks multi-host is **consumer code**. `FindTemplateInstance(...)` resolves via `SingleOrDefault()` and
throws `More than one instance of template <X> was found` as soon as a second host exists. Any module that
reaches for a host-scoped ("Distribution"-layer) template — `App.Program`, `Distribution.SwashbuckleConfiguration`,
the `Filters/*` templates, `Configuration/*`, `CurrentUserService`, etc. — must use one of:

| Intent | Use |
|---|---|
| Apply the same treatment to every host | `FindTemplateInstances<T>(...)` + `foreach` |
| Resolve one specific host's instance | `FindTemplateInstance<T>(idOrRole, accessibleTo: OutputTarget)` |
| Existence check | `FindTemplateInstances<T>(...).Any()` — **not** `FindTemplateInstance(...) != null` |

### Overload matrix — which forms are host-safe (verified 2026-07-30)

`Intent.Modules.Common/Engine/IOutputTargetExtensions.cs` provides `FindTemplateInstance(s)` extensions
**on `IOutputTarget`** which default `accessibleTo` to the receiver — added by commit *"Required improvements for
being able to handle multiple Template Outputs for 'Single File' templates"*, first shipped in
`Intent.Modules.Common` **3.9.0-pre.3**. These are the most ergonomic tool for host-scoped resolution from
inside a template. **But only the `string` overloads actually scope.**

| Form | Scopes to one host? |
|---|---|
| `outputTarget.FindTemplateInstance<T>("idOrRole")` | ✅ passes `accessibleTo: outputTarget` |
| `outputTarget.FindTemplateInstances<T>("idOrRole")` | ✅ same |
| `ExecutionContext.FindTemplateInstance<T>("idOrRole", accessibleTo)` | ✅ explicit |
| `TemplateDependency.OnTemplate("idOrRole", accessibleTo)` then `ExecutionContext.FindTemplateInstance(dep)` | ✅ `TemplateIdTemplateDependency.LookupTemplateInstance` honours `AccessibleTo` |
| `outputTarget.FindTemplateInstance<T>(TemplateDependency.OnTemplate("idOrRole"))` | ❌ **silently unscoped — still throws** |
| `outputTarget.FindTemplateInstances<T>(TemplateDependency.OnTemplate("idOrRole"))` | ❌ same |

**Why the `ITemplateDependency` overloads fail.** `IOutputTargetExtensions` calls
`templateDependency.TryGetWithAccessibleTo(accessibleTo, out var withAccessibleTo)` and then
`ExecutionContext.FindTemplateInstance(templateDependency.TemplateId, templateDependency.IsMatch)` — an overload
whose `accessibleTo` defaults to `null`. The rebind only works if the dependency overrides
`TryGetWithAccessibleTo`, and **only `TemplateInstanceTemplateDependency` does**. The base implementation returns
`false` unconditionally, and `TemplateDependency.OnTemplate(string)` produces a `TemplateIdTemplateDependency`,
which does not override it — so `accessibleTo` is dropped and the lookup degrades to `SingleOrDefault()` across
every host. This is a trap precisely for someone reaching for these overloads to fix multi-host.

Fixing it means passing `accessibleTo` through in `IOutputTargetExtensions` (or overriding
`TryGetWithAccessibleTo` on the other dependency types) — a change to `Intent.Modules.Common`, therefore
requiring a version bump of that package and its dependents. Until then, **prefer the `string` overloads**.

Runtime evidence for the safe form (both verified 2026-07-30 against a two-host application):

1. `Intent.Modules.AspNetCore.Swashbuckle/FactoryExtensions/SwashbuckleStartupConfigurationExtension.cs`
   loops the Startup templates and calls
   `template.OutputTarget.FindTemplateInstance<IClassProvider>(SwashbuckleConfigurationTemplate.TemplateId)`
   per host. With two `SwashbuckleConfiguration` instances present it resolved each host's own instance
   without throwing, giving `App.Api/Program.cs` and `Mobile.Api/Startup.cs` their own `ConfigureSwagger`
   + namespace.
2. `Intent.Modules.AspNetCore.Controllers` `BinaryContentFilterTemplatePartial.CanRunTemplate` was converted
   to `OutputTarget.FindTemplateInstance<ICSharpFileBuilderTemplate>("Distribution.SwashbuckleConfiguration") != null`.
   Note `CanRunTemplate` short-circuits on `FileTransferHelper.NeedsFileUploadInfrastructure(...)`, which is
   `metadataManager.Services(applicationId).Elements.Any(x => x.HasStereotype("d30e48e8-…"))` — so the clause is
   only reached once some Command/Query/Operation carries the `File Transfer` stereotype. With such a Command
   modelled, the lookup ran once per host against two Swashbuckle instances and generated
   `BinaryContentFilter.cs` into **both** projects, each `using` its own project's `BinaryContentAttribute`
   and registered in its own `SwashbuckleConfiguration`. No exception.

Caveat on that second one: `NeedsFileUploadInfrastructure` is **application-wide**, not per-host — a single
`File Transfer` element anywhere causes the file-transfer infrastructure to be emitted into *every* host that
has a Swashbuckle configuration, even hosts exposing no file-transfer endpoints. Pre-existing behaviour, not
introduced by the host-scoping change, but worth knowing when reasoning about what lands where.

Per-host settings must be read **inside** the loop, not once outside it. The hosting model is the canonical
example: one host may set `Use minimal hosting model` while the other does not, so
`OutputTarget.GetProject()...GetNETSettings()` must be evaluated per template instance. Reading it once and
applying the result to all hosts is a silent-wrong-output bug, not just a crash.

### Cross-template type references are already host-correct

`IntentTemplateBase.GetTemplate<T>(string templateId)` tries `FindTemplateInstance(templateId, accessibleTo: OutputTarget)`
**first** and only falls back to the unscoped lookup when that returns null. Because `GetTypeName` / `UseType`
route through it, a host's `Program.cs`/`Startup.cs` naturally references its *own* project's `ExceptionFilter`,
`BoundedLoggingDestructuringPolicy`, `ConfigureSwagger` and so on. Do not add manual `accessibleTo` plumbing to
`GetTypeName` call sites — it is already handled. The unscoped fallback is only reached when the type is not
accessible to the calling project, and *that* is the case which will throw under multi-host.

### Known latent issue

`AppStartupFile.IsApplicable` (`Templates/AppStartupFile.cs`) has deliberate multi-host scaffolding —
`_hasMultipleInstances` plus an `IsAccessibleTo` filter so a host only picks up container registrations whose
class provider it can actually see. However the dependency itself is still resolved with the **singular**
`FindTemplateInstance<IClassProvider>(dependency)` on the line *above* that guard, so a host-scoped dependency
would throw before the guard can filter it. It does not bite in practice today because container-registration
dependencies are Application/Infrastructure-layer single-instance classes. Fix by resolving with
`accessibleTo: _template.OutputTarget` if a host-scoped registration is ever introduced.

### History

Multi-host support was an in-flight, partially completed migration. Several modules had already been converted
to the plural pattern (`Swashbuckle/TypeSchemaFilterExtension`, `Swashbuckle/HideRouteParametersFromBodyOperationFilterExtension`,
`Controllers/BinaryContentFilterExtension`, `Security.JWT/CurrentUserHelper`, Serilog's `RegisterSerilogConfiguration`)
before the remainder were finished. Modules **not** yet converted, and which will still break multi-host if
installed: `AspNetCore.ODataQuery`, `AspNetCore.OData.EntityFramework`, `AspNetCore.MultiTenancy`,
`NetTopologySuite`.
