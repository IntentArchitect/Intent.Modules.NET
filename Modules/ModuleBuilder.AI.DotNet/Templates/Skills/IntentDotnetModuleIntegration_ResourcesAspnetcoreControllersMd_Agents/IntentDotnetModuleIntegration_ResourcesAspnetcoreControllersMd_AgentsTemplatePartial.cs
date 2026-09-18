using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace ModuleBuilder.AI.DotNet.Templates.Skills.IntentDotnetModuleIntegration_ResourcesAspnetcoreControllersMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IntentDotnetModuleIntegration_ResourcesAspnetcoreControllersMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.DotNet.Skills.IntentDotnetModuleIntegration_ResourcesAspnetcoreControllersMd_Agents";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntentDotnetModuleIntegration_ResourcesAspnetcoreControllersMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("aspnetcore-controllers", relativeLocation: "intent-dotnet-module-integration/resources")
                .FromMarkdown("""
# ASP.NET Core Controllers (`Intent.AspNetCore.Controllers`)

## 0. Version compatibility

Verified against `Intent.AspNetCore.Controllers` **7.1.15**. Two facts stated below are version-gated, not always true — check the installed version (its `.imodspec`'s `<version>`) before trusting them:

- **`<decorators>` being empty (§8) only holds from v7.0.0 onward.** Versions before 7.0.0 shipped a real `ControllerDecorator` extension point; v7.0.0 removed it in favour of `ICSharpFileBuilderTemplate`. Against an older install, look for `ControllerDecorator` instead of assuming §6's six extension points are the whole story.
- **The multi-host trap in §8 (`FindTemplateInstances` over `FindTemplateInstance`) only applies from v7.1.6 onward**, when Controllers gained support for multiple ASP.NET Core projects per application. Below that version there is exactly one controller instance per model, and `FindTemplateInstance` is safe.

Everything else in this file is expected to hold for any 7.x release. If the installed version falls outside a range stated here — including anything below 7.0.0 — re-verify the specific fact against Controllers' own source rather than trust this file blindly.

## 1. What it generates, and what it deliberately leaves empty

`Intent.Modules.AspNetCore.Controllers`'s `ControllerTemplate` emits a full ASP.NET Core controller class per service/CQRS grouping: the class declaration, `[ApiController]`, `[Route]` and `[Authorize]` attributes, XML doc comments, the method signatures for every operation, and every method's parameters (route, query, body, form). **Action-method bodies are emitted empty by design** — the Controllers module's job stops at the contract; nothing about *how* a request is handled is its concern. That single fact is the reason a dispatch module (`Dispatch.MediatR`, `Dispatch.ServiceContract`, `Dispatch.Wolverine`) exists at all: without one installed, a generated controller compiles and returns 200s that do nothing, and that is expected behaviour, not a defect to work around.

## 2. Taking the dependency

Reference the module the same way its own dispatch modules do — as a NuGet package, never a `ProjectReference`:

```xml
<!-- .csproj -->
<PackageReference Include="Intent.Modules.AspNetCore.Controllers" Version="7.1.*" />
```

```xml
<!-- .imodspec -->
<dependency id="Intent.AspNetCore.Controllers" version="7.1.0" />
```

Both version numbers above are illustrative floors, not values to copy verbatim — check `Intent.AspNetCore.Controllers.imodspec`'s own `<version>` for the number actually installed in this repo at the time you write the dependency, the same way `intent-modelers-integration`'s Must Not #3 warns against a stale copied version.

| Item | Value |
|---|---|
| NuGet PackageId | `Intent.Modules.AspNetCore.Controllers` |
| Intent module id | `Intent.AspNetCore.Controllers` — no `Modules.` |
| Namespace | `Intent.Modules.AspNetCore.Controllers` (consumed directly — there is no separate designer-style `.Api` namespace here) |

The two identities are easy to cross — the NuGet package id keeps `Modules.`, the `.imodspec` dependency id and `modules.config` entry never do. Get this backwards and the reference resolves at the wrong time and fails confusingly late.

`Dispatch.MediatR` and `Dispatch.ServiceContract` reference Controllers with a `ProjectReference` — that is safe only because both live in this same repo, next to Controllers itself. `Dispatch.Wolverine` uses a `PackageReference` deliberately, to keep the package boundary honest. **A module authored outside this repo has no `ProjectReference` option — always take the `PackageReference` shape**, even when working inside this repo, unless you are one of Controllers' own in-repo siblings.

What the reference buys: `IControllerModel` / `IControllerOperationModel` / `IControllerParameterModel` (the typed models correlated onto the generated class — see §5), `ControllerTemplate` and its `TemplateId`, `GetReturnStatement(...)` (the one correct way to finish a dispatch method — see §6), `FileTransferHelper` (binary upload/download plumbing), and the ability to register your own `IControllerModel` implementations (§6).

## 3. Templates

| TemplateId | Role constant | Role string |
|---|---|---|
| `Intent.AspNetCore.Controllers.Controller` | `TemplateRoles.Distribution.WebApi.Controller` | `Distribution.WebApi.Controller` |
| `Intent.AspNetCore.Controllers.ExceptionFilter` | *(none — use the string literal)* | `Distribution.ExceptionFilter` |
| `Intent.AspNetCore.Controllers.BinaryContentFilter` | *(none)* | `Distribution.BinaryContentFilter` |
| `Intent.AspNetCore.Controllers.BinaryContentAttribute` | *(none)* | `Distribution.Controller.BinaryContentAttribute` |
| `Intent.AspNetCore.Controllers.JsonResponse` | *(none)* | `Distribution.Controller.JsonResponse` |

`TemplateRoles.Application.Services.Controllers` is `[Obsolete]` and its value is the controller's **TemplateId**, not a role string — do not pass it where a role is expected.

## 4. Generated shape

Exactly one class per generated file — reach it with `file.Classes.First()`. The class is named `{Model.Name.RemoveSuffix("Controller","Service")}Controller`, derives from `ControllerBase`, and always carries an **empty constructor, present deliberately** so a dispatch or enrichment module can add its own constructor parameters to it. Each action method is named `operation.Name.ToPascalCase()`, is always `async`, and returns `Task<ActionResult>` or `Task<ActionResult<T>>` depending on whether the operation has a return type. **The last parameter of every action method is always `CancellationToken cancellationToken = default`** — any parameter you add of your own goes before it, never after.

## 5. The metadata contract

Every generated class, method, and parameter carries `AddMetadata` entries that correlate it back to the designer model that produced it. Always read them with `TryGetMetadata`, never `GetMetadata` — a miss should be a graceful skip, not an exception.

| Node | Key | Type |
|---|---|---|
| class | `model` | `IControllerModel` |
| class | `modelId` | `string` |
| method | `model` | `IControllerOperationModel` |
| method | `modelId` | `string` |
| method | `route` | `string` |
| parameter | `model` | `IControllerParameterModel` |
| parameter | `modelId` | `string` |
| parameter | `mappedPayloadProperty` | `ICanBeReferencedType` |

Never correlate by name — method and parameter names are transformed (`ToPascalCase`), de-duplicated, and can be overloaded, so a name match that works today silently breaks the next time the designer model changes shape.

## 6. Extension points

All on the typed route, all wired from a `FactoryExtensionBase.OnAfterTemplateRegistrations`:

- **Enrich an action method.** Find the controller templates, add a `CSharpFile.OnBuild(fileBuilder, priority)` callback, then `foreach (var method in file.Classes.First().Methods)` and `method.TryGetMetadata<IControllerOperationModel>("model", out var op)`. From there, add attributes, add parameters, or add statements to the (still-empty) body.
- **Dispatch.** The same walk as above, discriminating further on `template.Model is not CqrsControllerModel` where the shape differs, injecting the constructor dependency via `ctor.AddParameter(...).IntroduceReadonlyField(...)`, and finishing every method by calling `template.GetReturnStatement(operationModel)` — never hand-write the return statement yourself; that call is what keeps a dispatch module correct as Controllers' own return-shape logic evolves. Assign the dispatch call's return value into a local variable named exactly `result` before calling it — see the Traps section for why the name is not optional.
- **Contribute new controller models.** Ship your own template registration — a `FilePerModelTemplateRegistration<IControllerModel>` whose `TemplateId` returns `ControllerTemplate.TemplateId` — and Intent merges your model set with Controllers' own at generation time. Your `.imodspec` declares no `<template>` entry of its own for this; this is exactly the mechanism implicit CQRS controllers use today.
- **`Distribution.Custom.Dispatcher`.** Publish a template with this role implementing `IControllerTemplate<IControllerModel>` and `Dispatch.ServiceContract` will target it instead of the real controller. Only `ServiceContract` honours this role — the other dispatch modules do not.
- **Chain onto `AddControllers()`.** Use the startup-invocation metadata tags `configure-services-controllers-generic` / `configure-endpoints-controllers-generic` to append your own configuration onto the same call, the way `JsonOptionsExtension` does.

## 7. Worked example: `MediatRControllerInstaller`

`Dispatch.MediatR`'s `MediatRControllerInstaller` is the reference implementation of "dispatch onto Controllers" — every dispatch module in this repo follows its shape. In emission order:

1. `AddTypeSource` is called three times, registering the types the enrichment needs to resolve by name.
2. The controller's constructor gets `ctor.AddParameter(UseType("MediatR.ISender"), "mediator", p => p.IntroduceReadonlyField((_, a) => a.ThrowArgumentNullException()))` — one dependency, injected once, guarded against null.
3. Per method: an upload prologue for binary parameters, a back-fill step that copies route-bound values onto the payload object, a `BadRequest()` guard for cross-checking route and payload consistency, `await _mediator.Send(payload, cancellationToken)`, and finally `template.GetReturnStatement(operationModel)`.

Compare against its two siblings when deciding your own module's shape: `Dispatch.ServiceContract` additionally emits validation, unit-of-work, and event-bus-flush statements, and tags each one so a later module can find and adjust them; `Dispatch.Wolverine` detects whether the target constructor uses an object initializer or not before deciding how to emit its own dispatch call, since the two are not interchangeable there.

## 8. Traps

- **`GetReturnStatement` hardcodes the identifier `result` — it is not a parameter you pass in.** Every branch of its implementation (`Utils.cs`) emits a bare `result` reference: the success expression (`Ok(result)` / the `JsonResponse<T>` wrap), the `CreatedAtAction` route values (`new { id = result }`), the `NotFound()` null-check (`result == null ? NotFound() : ...`), and the file-download branch (`result.{StreamField}` etc.). There is no way to tell it a different name. Whatever local variable holds your dispatch call's return value **must be declared as `result`**, exactly that spelling, or the statement it emits references an undefined identifier and the file fails to compile. `Dispatch.MediatR`'s own `MediatRControllerInstaller` follows this: `var result = await _mediator.Send(...)`. Verified directly against `GetReturnStatement`'s source in `Utils.cs` — this is not visible from the method's signature, so don't rely on decompiling the shipped assembly to rediscover it; that's what this bullet is for.
- **Multi-host duplication.** `FindTemplateInstance(templateId, modelId)` throws *"more than one instance"* the moment a solution has a controller generated per host project (a common multi-host WebApi shape). Use `FindTemplateInstances` and filter the results on the model id yourself — never assume a single instance.
- **`IControllerModel.Folder` is off by one, and it is inconsistent between implementations.** `ServiceControllerModel.Folder` (in Controllers itself) is the service's own folder; `CqrsControllerModel.Folder` (in `Dispatch.MediatR`'s and `Dispatch.Wolverine`'s own `ImplicitControllers/CqrsControllerModel.cs`, set as `folderElement?.ParentElement?.AsFolderModel()`) is the *parent* of the grouping folder. A Registration Filter written as `np(Folder.Name) == "App"` will therefore silently generate no controller for a Command sitting directly in `App` — the folder check is one level off for CQRS. Verified directly against both `CqrsControllerModel.cs` files — neither module has a `CONTEXT.md` recording this, so read the source, not a doc, before writing a folder-based filter of your own.
- **`<decorators></decorators>` is empty, and stays empty.** Controllers has no decorator path and defines no decorator interface — don't go looking for one; every extension point is one of the six in §6.
- **Auto-install surprises.** Controllers' `<interoperability>` entry auto-installs `Dispatch.MediatR`, `Dispatch.ServiceContract`, or `Swashbuckle` on detection of the right conditions, so a consumer application can already have a dispatch module installed that nobody explicitly asked for. Don't assume a bare Controllers install means no dispatch exists yet — check.
- **An untyped variant exists in this repo — recognise it, don't copy it.** `OutputCaching.Redis`, `ODataQuery`, `Pagination`, and `IdentityService` all reach the generated controller through the lower-level `ICSharpFileBuilderTemplate` and a raw `modelId` string, with no package reference to Controllers at all. It works, but it forfeits everything in §2's "what the reference buys" list — no `IControllerOperationModel`, no `GetReturnStatement`, no `FileTransferHelper`. That shape is a fallback for modules that can't take the dependency, not a pattern to reach for by default.
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}