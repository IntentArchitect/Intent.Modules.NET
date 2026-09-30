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

- **`<decorators>` being empty (§9) only holds from v7.0.0 onward.** Versions before 7.0.0 shipped a real `ControllerDecorator` extension point; v7.0.0 removed it in favour of `ICSharpFileBuilderTemplate`. Against an older install, look for `ControllerDecorator` instead of assuming §7's six extension points are the whole story.
- **The multi-host trap in §9 (`FindTemplateInstances` over `FindTemplateInstance`) only applies from v7.1.6 onward**, when Controllers gained support for multiple ASP.NET Core projects per application. Below that version there is exactly one controller instance per model, and `FindTemplateInstance` is safe.

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

What the reference buys: `IControllerModel` / `IControllerOperationModel` / `IControllerParameterModel` (the typed models correlated onto the generated class — see §5, with every member listed in §6), `ControllerTemplate` and its `TemplateId`, `GetReturnStatement(...)` (the one correct way to finish a dispatch method — see §7), `FileTransferHelper` (binary upload/download plumbing — see §6.8), and the ability to register your own `IControllerModel` implementations (§7).

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

## 6. The model types — complete member reference

This section is the **full** public surface of every type §5's metadata hands back, plus the enums, helpers, and template members they pull in. It exists so you don't need to reflect over, decompile, or `GetType().GetProperties()` the installed `Intent.Modules.AspNetCore.Controllers` assembly to find out what a model exposes. The listing is complete for the version in §0, so a member not listed here is not on the interface — and code that `dynamic`- or reflection-casts its way to a concrete type's private state breaks without warning on the next release. Verified against `Templates/Controller/IControllerModel.cs`, `Templates/Controller/Models/ServiceControllerModel.cs`, `ControllerTemplatePartial.cs`, `IControllerTemplate.cs`, `FileTransferHelper.cs` and `Utils.cs` (Controllers 7.1.15), and against `Intent.Modules.Metadata.WebApi`'s `HttpEndpointModelFactory.cs` and enums and `Intent.Modules.Metadata.Security`'s `ISecurityModel.cs`.

### 6.1 Where each type lives

Only `Intent.Modules.AspNetCore.Controllers` needs an explicit `PackageReference` — everything else in this table arrives transitively through it.

| Type(s) | Namespace | NuGet package |
|---|---|---|
| `IControllerModel`, `IControllerOperationModel`, `IControllerParameterModel`, `IApiVersionModel`, `IControllerTemplate<T>`, `ControllerTemplate` | `Intent.Modules.AspNetCore.Controllers.Templates.Controller` | `Intent.Modules.AspNetCore.Controllers` |
| `ServiceControllerModel`, `ControllerOperationModel`, `ControllerParameterModel`, `ControllerApiVersionModel` | `Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models` | `Intent.Modules.AspNetCore.Controllers` |
| `FileTransferHelper` (and its nested `FileTransferHelper.FileInfo` record) | `Intent.Modules.AspNetCore.Controllers.Templates` | `Intent.Modules.AspNetCore.Controllers` |
| `HttpVerb`, `HttpMediaType`, `HttpInputSource` | `Intent.Modules.Metadata.WebApi.Models` | `Intent.Modules.Metadata.WebApi` (transitive) |
| `ISecurityModel` | `Intent.Modules.Metadata.Security.Models` | `Intent.Modules.Metadata.Security` (transitive) |
| `IHasFolder`, `FolderModel` | `Intent.Modules.Common.Types.Api` | `Intent.Modules.Common.Types` (transitive) |
| `IHasName`, `IHasTypeReference`, `IMetadataModel`, `IElement`, `ITypeReference`, `ICanBeReferencedType` | `Intent.Metadata.Models` | `Intent.SoftwareFactory.SDK` (transitive) |
| `CqrsControllerModel` | `Intent.Modules.AspNetCore.Controllers.Dispatch.MediatR.ImplicitControllers` / `...Dispatch.Wolverine.ImplicitControllers` | the dispatch module, **not** Controllers — see §6.2 |

### 6.2 `IControllerModel` — the class-level `model`

Inherited members are shown inline so the whole surface is in one place:

```csharp
public interface IControllerModel : IHasFolder, IHasName, IMetadataModel
{
    string Id { get; }                                  // IMetadataModel
    string Name { get; }                                // IHasName
    FolderModel Folder { get; }                         // IHasFolder — off by one for CQRS, see §9
    string? Route { get; }
    string? Comment { get; }
    bool RequiresAuthorization { get; }
    bool AllowAnonymous { get; }
    IReadOnlyCollection<ISecurityModel> SecurityModels { get; }
    IList<IControllerOperationModel> Operations { get; }
    IList<IApiVersionModel> ApplicableVersions { get; }
    IElement? InternalElement => null;                  // default interface member
}
```

Two implementations exist, and they fill the same members differently:

| Member | `ServiceControllerModel` (Controllers — a Service element) | `CqrsControllerModel` (Dispatch.MediatR / Dispatch.Wolverine — a folder of Commands/Queries) |
|---|---|---|
| `Id` | the Service element's id | the grouping folder's id, or `Guid.Empty.ToString()` for the package-level controller |
| `Name` | the Service's name (the class name strips `Controller`/`Service` and re-appends `Controller`) | the folder path concatenated in PascalCase (dots become `_`), or `"Default"` at package level |
| `Folder` | the Service's own folder | the *parent* of the grouping folder, and may be `null` |
| `Route` | the Service's Http Service Settings route, with the segment equal to the service name replaced by `[controller]`; `null` when unset | always `null` |
| `Comment` | the Service's comment | always `null` |
| `ApplicableVersions` | from the Service's Api Version Settings | always empty — versions live on each operation instead |
| `InternalElement` | the Service `IElement` | **always `null`** — it does not override the default interface member |

`InternalElement` being `null` on every CQRS controller is the one to watch: code that does `controller.InternalElement.GetStereotype(...)` works against service-based controllers and throws a `NullReferenceException` the first time it meets a CQRS one. Go through `Operations[i].InternalElement` (never null) instead, or null-check.

To tell the two apart, test `template.Model is ServiceControllerModel` — that type ships in Controllers itself. Testing `is CqrsControllerModel` forces a reference to a dispatch module, and there are two distinct `CqrsControllerModel` types (one per dispatch module), so it can silently miss.

### 6.3 `IControllerOperationModel` — the method-level `model`

```csharp
public interface IControllerOperationModel : IHasName, IHasTypeReference, IMetadataModel
{
    string Id { get; }                                  // == InternalElement.Id
    string Name { get; }                                // designer name, untransformed — the C# method is Name.ToPascalCase()
    ITypeReference TypeReference { get; }               // the element's own type reference; never null
    ITypeReference? ReturnType { get; }                 // TypeReference when it has an Element, otherwise null (void)
    string Comment { get; }
    HttpVerb Verb { get; }
    string? Route { get; }                              // the sub-route under the controller's [Route]; same value as the method's "route" metadata
    HttpMediaType? MediaType { get; }
    bool RequiresAuthorization { get; }
    bool AllowAnonymous { get; }
    IReadOnlyCollection<ISecurityModel> SecurityModels { get; }
    IElement InternalElement { get; }                   // never null
    IList<IControllerParameterModel> Parameters { get; }
    IList<IApiVersionModel> ApplicableVersions { get; }
    IControllerModel Controller { get; }                // back-reference to the owning controller
}
```

- `InternalElement` is a service `Operation` element for a `ServiceControllerModel`, and the `Command` or `Query` element itself for a `CqrsControllerModel`. To reach the typed designer model, use the Services designer's generated extensions rather than switching on `SpecializationType` strings: `IsOperationModel()` / `AsOperationModel()` (`Intent.Modelers.Services.Api`), `IsCommandModel()` / `AsCommandModel()` and `IsQueryModel()` / `AsQueryModel()` (`Intent.Modelers.Services.CQRS.Api`). Both packages are already transitive through Controllers.
- Test `ReturnType is null` for "no return value". `TypeReference` is never null, so `TypeReference == null` is always false and is not that test.
- `Route` marks a route parameter optional when its matching parameter's type is nullable.
- The template checks `RequiresAuthorization` before `AllowAnonymous`, so when both are true only `[Authorize]` is emitted. For a service-based controller, Controllers rejects a Service that is both secured and unsecured with an `ElementException` at generation time.

### 6.4 `IControllerParameterModel` — the parameter-level `model`

```csharp
public interface IControllerParameterModel : IHasName, IHasTypeReference, IMetadataModel
{
    string Id { get; }
    string Name { get; }                                // already camelCased from the designer element's name
    ITypeReference TypeReference { get; }
    HttpInputSource? Source { get; }
    string? HeaderName { get; }                         // from Parameter Settings; meaningful when Source == FromHeader
    string? QueryStringName { get; }                    // from Parameter Settings; meaningful when Source == FromQuery
    ICanBeReferencedType? MappedPayloadProperty { get; }
    string? Value { get; }                              // the designer element's Value (default value), usually null
}
```

How the list is built determines what you will actually find in it:

- A **service operation** gets one parameter per designer parameter. `Id` is that parameter element's id, and `MappedPayloadProperty` is that same element.
- A **Command/Query** on a `Get`/`Delete` gets one parameter per field, and `Id` and `MappedPayloadProperty` are that field element.
- A **Command/Query** on a `Post`/`Put`/`Patch` gets one parameter only for each field bound outside the body — a field with explicit Parameter Settings, or one named in the route. Every remaining field is folded into a **single synthetic body parameter** named `command` or `query`. It has `Source == FromBody`, a `TypeReference` pointing at the Command/Query itself, `MappedPayloadProperty == null`, and an `Id` equal to the Command/Query element's id — the same `Id` as the owning operation. The fields folded into the body do not appear as parameters of their own.
- Without explicit Parameter Settings, `Source` is inferred in this order: `FromRoute` for a scalar/enum named in the route, then `FromQuery` on a `Get`/`Delete`, then `FromBody` for a non-scalar on a `Post`/`Put`, and otherwise `null`. So `null` is a real value — for example a scalar service parameter on a `Post` that isn't in the route, or any unconfigured parameter on a `Patch`. Handle it explicitly rather than treating it as `FromBody`.

### 6.5 `IApiVersionModel`

```csharp
public interface IApiVersionModel
{
    string? DefinitionName { get; }                     // the owning API Version definition's name
    string Version { get; }                             // e.g. "1.0"
    bool IsDeprecated { get; }
}
```

### 6.6 Enums and `ISecurityModel`

| Enum | Members — the complete set |
|---|---|
| `HttpVerb` | `Get`, `Post`, `Put`, `Patch`, `Delete` |
| `HttpMediaType` | `ApplicationJson` — the only member; `MediaType == null` means the default |
| `HttpInputSource` | `FromQuery`, `FromBody`, `FromForm`, `FromRoute`, `FromHeader` |

```csharp
public interface ISecurityModel : IEquatable<ISecurityModel>
{
    IReadOnlyCollection<string> Roles { get; }
    IReadOnlyCollection<string> Policies { get; }
    string EquatableRoles { get; }                      // Roles sorted and comma-joined — used only for equality
    string EquatablePolicies { get; }                   // Policies sorted and comma-joined — used only for equality
    static ISecurityModel Empty { get; }                // no roles, no policies
}
```

`SecurityModels` holds one entry per applied `Secured` stereotype, looked up on the element and then its parents when the element has none. A `Roles` value written with `+` (`Admin+Auditor`) is split into one entry per group, so every group must pass. The controller emits one `[Authorize]` attribute per entry, adding `Roles = ...` and/or `Policy = ...` arguments only when that entry has any. The concrete `SecurityModel` class is `internal` — use `ISecurityModel.Empty` or your own implementation, never `new SecurityModel(...)`.

### 6.7 `ControllerTemplate` members you can call

`ControllerTemplate` is `CSharpTemplateBase<IControllerModel>, ICSharpFileBuilderTemplate, IControllerTemplate<IControllerModel>`. Beyond the standard `CSharpTemplateBase` surface (`Model`, `CSharpFile`, `ExecutionContext`, `GetTypeName`, `AddTypeSource`, `UseType`, ...), its own additions are exactly these:

```csharp
public const string TemplateId = "Intent.AspNetCore.Controllers.Controller";
public CSharpStatement GetReturnStatement(IControllerOperationModel operationModel);   // see §9 — hardcodes `result`

public interface IControllerTemplate<TModel> : ICSharpFileBuilderTemplate, IIntentTemplate<TModel>, IIntentTemplate
{
    string GetTypeName(IHasTypeReference hasTypeReference);
    ClassTypeSource AddTypeSource(string templateId, string collectionFormat);
    CSharpStatement GetReturnStatement(IControllerOperationModel operationModel);
}
```

(The real declaration names its type parameter `IControllerModel`, shadowing the interface of the same name — it is a generic parameter, not a constraint.) Hold a controller template as `IControllerTemplate<IControllerModel>` when you want code that also works against a `Distribution.Custom.Dispatcher` stand-in (§7).

The static `Utils` class in `Intent.Modules.AspNetCore.Controllers` — `CanReturnNoContent`, `CanReturnNotFound`, `ShouldBeJsonResponseWrapped`, `TryGetMultiTenancyRoute`, `TryGetIsIgnoredForApiExplorer` — is **`internal`**, and there is no `InternalsVisibleTo`. None of those are callable from your module. `GetReturnStatement` is reachable only through the public instance method above. If you need the same decision (for example "would this return `NotFound()`?"), re-derive it from the operation model yourself rather than reflecting into `Utils`.

### 6.8 `FileTransferHelper` (public static)

```csharp
public const string FileTransferStereotypeId = "d30e48e8-389e-4b70-84fd-e3bac44cfe19";
public const string StreamTypeId = "fd4ead8e-92e9-47c2-97a6-81d898525ea0";

bool IsFileDownloadOperation(IControllerOperationModel operation);   // File Transfer stereotype + a Stream field on the return DTO
bool IsFileUploadOperation(IControllerOperationModel operation);     // File Transfer stereotype + a Stream parameter or a Stream field on the FromBody DTO
FileInfo GetDownloadTypeInfo(IControllerOperationModel operation);
FileInfo GetUploadTypeInfo(IControllerOperationModel operation);     // returns null when neither shape applies
string GetStreamFieldName(ITypeReference type);
string GetStreamFieldName(IControllerOperationModel operation);
bool IsStreamType(ITypeReference? typeReference);
void AddControllerStreamLogic(ICSharpFileBuilderTemplate template, CSharpClassMethod method, IControllerOperationModel operation);
bool NeedsFileUploadInfrastructure(IMetadataManager metadataManager, string applicationId);

public record FileInfo(string StreamField, string? FileNameField, string? ContentTypeField, string? ContentLemgthField)
{
    bool HasFilename();
    bool HasContentType();
    bool HasContentLength();
}
```

`ContentLemgthField` really is spelt that way in the shipped record — `ContentLengthField` does not compile. `FileInfo` here is the nested `FileTransferHelper.FileInfo`, not `System.IO.FileInfo`; qualify it if both namespaces are in scope.

## 7. Extension points

All on the typed route, all wired from a `FactoryExtensionBase.OnAfterTemplateRegistrations`:

- **Enrich an action method.** Find the controller templates, add a `CSharpFile.OnBuild(fileBuilder, priority)` callback, then `foreach (var method in file.Classes.First().Methods)` and `method.TryGetMetadata<IControllerOperationModel>("model", out var op)`. From there, add attributes, add parameters, or add statements to the (still-empty) body.
- **Dispatch.** The same walk as above, discriminating further on `template.Model is not CqrsControllerModel` where the shape differs, injecting the constructor dependency via `ctor.AddParameter(...).IntroduceReadonlyField(...)`, and finishing every method by calling `template.GetReturnStatement(operationModel)` — never hand-write the return statement yourself; that call is what keeps a dispatch module correct as Controllers' own return-shape logic evolves. Assign the dispatch call's return value into a local variable named exactly `result` before calling it — see the Traps section for why the name is not optional.
- **Contribute new controller models.** Ship your own template registration — a `FilePerModelTemplateRegistration<IControllerModel>` whose `TemplateId` returns `ControllerTemplate.TemplateId` — and Intent merges your model set with Controllers' own at generation time. Your `.imodspec` declares no `<template>` entry of its own for this; this is exactly the mechanism implicit CQRS controllers use today.
- **`Distribution.Custom.Dispatcher`.** Publish a template with this role implementing `IControllerTemplate<IControllerModel>` and `Dispatch.ServiceContract` will target it instead of the real controller. Only `ServiceContract` honours this role — the other dispatch modules do not.
- **Chain onto `AddControllers()`.** Use the startup-invocation metadata tags `configure-services-controllers-generic` / `configure-endpoints-controllers-generic` to append your own configuration onto the same call, the way `JsonOptionsExtension` does.

## 8. Worked example: `MediatRControllerInstaller`

`Dispatch.MediatR`'s `MediatRControllerInstaller` is the reference implementation of "dispatch onto Controllers" — every dispatch module in this repo follows its shape. In emission order:

1. `AddTypeSource` is called three times, registering the types the enrichment needs to resolve by name.
2. The controller's constructor gets `ctor.AddParameter(UseType("MediatR.ISender"), "mediator", p => p.IntroduceReadonlyField((_, a) => a.ThrowArgumentNullException()))` — one dependency, injected once, guarded against null.
3. Per method: an upload prologue for binary parameters, a back-fill step that copies route-bound values onto the payload object, a `BadRequest()` guard for cross-checking route and payload consistency, `await _mediator.Send(payload, cancellationToken)`, and finally `template.GetReturnStatement(operationModel)`.

Compare against its two siblings when deciding your own module's shape: `Dispatch.ServiceContract` additionally emits validation, unit-of-work, and event-bus-flush statements, and tags each one so a later module can find and adjust them; `Dispatch.Wolverine` detects whether the target constructor uses an object initializer or not before deciding how to emit its own dispatch call, since the two are not interchangeable there.

## 9. Traps

- **`GetReturnStatement` hardcodes the identifier `result` — it is not a parameter you pass in.** Every branch of its implementation (`Utils.cs`) emits a bare `result` reference: the success expression (`Ok(result)` / the `JsonResponse<T>` wrap), the `CreatedAtAction` route values (`new { id = result }`), the `NotFound()` null-check (`result == null ? NotFound() : ...`), and the file-download branch (`result.{StreamField}` etc.). There is no way to tell it a different name. Whatever local variable holds your dispatch call's return value **must be declared as `result`**, exactly that spelling, or the statement it emits references an undefined identifier and the file fails to compile. `Dispatch.MediatR`'s own `MediatRControllerInstaller` follows this: `var result = await _mediator.Send(...)`. Verified directly against `GetReturnStatement`'s source in `Utils.cs` — this is not visible from the method's signature, so don't rely on decompiling the shipped assembly to rediscover it; that's what this bullet is for.
- **Controller method parameters are rebuilt at `AfterBuild` priority `1000`.** `ControllerTemplate`'s `WorkaroundForGetTypeNameIssue` replaces every parameter carrying `model` metadata with a fresh `CSharpParameter`. It re-resolves the type name and copies across only the default value, the XML doc comment, the attributes, and the three metadata keys in §5 (`model`, `modelId`, `mappedPayloadProperty`). Any other metadata you added to a parameter is dropped, and a `CSharpParameter` reference you captured earlier no longer belongs to the method. Re-find parameters through `method.Parameters` plus `TryGetMetadata` whenever you need them, and put your own correlation data on the method rather than the parameter.
- **Multi-host duplication.** `FindTemplateInstance(templateId, modelId)` throws *"more than one instance"* the moment a solution has a controller generated per host project (a common multi-host WebApi shape). Use `FindTemplateInstances` and filter the results on the model id yourself — never assume a single instance.
- **`IControllerModel.Folder` is off by one, and it is inconsistent between implementations.** `ServiceControllerModel.Folder` (in Controllers itself) is the service's own folder; `CqrsControllerModel.Folder` (in `Dispatch.MediatR`'s and `Dispatch.Wolverine`'s own `ImplicitControllers/CqrsControllerModel.cs`, set as `folderElement?.ParentElement?.AsFolderModel()`) is the *parent* of the grouping folder. A Registration Filter written as `np(Folder.Name) == "App"` will therefore silently generate no controller for a Command sitting directly in `App` — the folder check is one level off for CQRS. Verified directly against both `CqrsControllerModel.cs` files — neither module has a `CONTEXT.md` recording this, so read the source, not a doc, before writing a folder-based filter of your own.
- **`<decorators></decorators>` is empty, and stays empty.** Controllers has no decorator path and defines no decorator interface — don't go looking for one; every extension point is one of the six in §7.
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