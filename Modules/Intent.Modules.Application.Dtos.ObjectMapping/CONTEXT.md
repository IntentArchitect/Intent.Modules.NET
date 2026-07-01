# CONTEXT: Intent.Application.Dtos.ObjectMapping

## Purpose

Generates explicit C# object mapping extension methods for DTOs that have a `[map from]` domain entity mapping in the Services designer. Drop-in replacement for `Intent.Application.Dtos.AutoMapper` that produces readable, zero-reflection code with no third-party library dependency.

---

## Architecture Decisions

### Template-only, no runtime library
Generated code uses plain C# — object initializers, LINQ, and explicit casts. No NuGet runtime dependency is introduced into the user's application.

### Role: `Application.EntityDtoMappingExtensions`
The template fulfills `TemplateRoles.Application.EntityDtoMappingExtensions`. The MediatR query handler template (`Intent.Application.MediatR.QueryHandler`) checks for this role and generates `entity.MapToXxxDto()` call sites automatically. No factory extension is needed to rewire the handlers.

### No factory extension
The module has no `*FactoryExtension`. A stub (`ObjectMappingCrudFactoryExtension.cs`) was scaffolded by Module Builder but was removed — it was a permanent no-op (both lifecycle overrides empty). The handler template's role-awareness (see below) makes cross-module rewiring unnecessary; there was no plausible future need identified for it either, so it was deleted rather than kept "just in case." If co-existence logic with AutoMapper is ever needed beyond `CanRunTemplate()`, re-add a factory extension via the Module Builder designer at that point — don't recreate a stub speculatively.

### Mutual exclusion with AutoMapper
`CanRunTemplate()` returns `false` when `Intent.Application.Dtos.AutoMapper` is installed. This prevents duplicate mapping files when both modules are present during a migration. The user must uninstall AutoMapper first.

### Output location: `Mappings/` folder, namespace stripped
The template strips `.Mappings` from the namespace (`this.GetNamespace().Replace(".Mappings", "")`) while keeping it in the folder path (`this.GetFolderPath()`), so generated `{DtoName}MappingExtensions.cs` files land in `Mappings/<EntityFolder>/` on disk but their namespace mirrors the entity's own namespace exactly (e.g. `ObjectMappingTest.Application.Orders`, not `...Application.Mappings.Orders`). This is required because the extension methods are called bare (`entity.MapToXxxDto()`) with no `using` — C# implicit parent-namespace scoping only works if the namespace matches. Matches `Intent.Application.Dtos.AutoMapper`'s `DtoMappingProfile` precedent exactly.

The imodspec declares this explicitly: `<role>Application.Mappings</role>` + `<location>Mappings</location>` on the `MappingExtensions` C# Template element (Module Builder designer stereotype `Template Settings` → `Default Location`). Mechanism note: the physical `Mappings/` folder is actually anchored per-consuming-application in that app's **Codebase Structure** designer (a `Folder` element with `Namespace Provider=true` and a `Template Output` binding to this template) — not derived from the imodspec at SF time. The imodspec's `Default Location` only matters for documentation/clarity and for apps that don't yet have the anchor (e.g. a brand-new install). Confirmed via `run_designer_script` + `get_staged_file_diffs`: setting it explicitly produced a one-line imodspec diff and 0 changes to any generated `.cs` file.

---

## MappingHelper Design

`Templates/MappingHelper.cs` is a static helper shared by `MappingExtensionsTemplatePartial.cs`. It dispatches each mapped DTO field to the right expression builder:

| Case | Detection | Expression emitted |
|---|---|---|
| Expression mapping (special chars) | `IsExpression(pathTargets)` | PascalCased direct expression |
| FK extraction (association end, primitive field) | `pathTargets.Count == 1 && .IsAssociationEndModel() && IsPrimitive` | `projectFrom.XxxId` or `.Select(x => x.Id)` |
| Nested DTO | `TryGetTemplate(MappingExtensionsTemplate.TemplateId, nestedElementId)` | `projectFrom.Nav?.MapToNavDto()` |
| Collection multi-hop | `IsCollection && pathTargets.Count >= 2` | `projectFrom.Col?.Select(x => x.Prop).ToList() ?? []` |
| Enum cast | `dtoType.SpecializationTypeId == EnumSpecializationId && dtoTypeId != domainTypeId` | `(DtoEnum)projectFrom.Prop` |
| Default | — | `projectFrom.Prop` |

### Critical: ShouldCast implementation

`ShouldCast` must NOT use:
- `GetTypeInfo(field.TypeReference).IsPrimitive` — returns `false` for user-defined enum types
- `field.Mapping.Element?.TypeReference` — `field.Mapping.Element` is the **mapping root entity**, not the leaf property

Correct implementation:
```csharp
private static bool ShouldCast(CSharpTemplateBase<DTOModel> template, DTOFieldModel field)
{
    var dtoTypeElement = field.TypeReference.Element;
    if (dtoTypeElement?.SpecializationTypeId != EnumSpecializationId) return false;
    var lastTarget = field.Mapping?.Path?.LastOrDefault();
    if (lastTarget == null) return false;
    var domainEnumId = lastTarget.Element?.TypeReference?.Element?.Id;
    return domainEnumId != null && dtoTypeElement.Id != domainEnumId;
}
```

The last path target's element holds the domain property; its `TypeReference.Element.Id` is the domain enum type. Compare against the DTO field's type element ID to detect a type mismatch requiring a cast.

### FK extraction and association ends

Extension methods `IsAssociationEndModel()` and `AsAssociationEndModel()` live on `ICanBeReferencedType` (namespace `Intent.Modelers.Domain.Api`). They are NOT on `IElement`. Call them directly on `pathTargets[0].Element` — no cast to `IElement` required.

---

## Type Sources

The template registers these type sources in the constructor (order matters for resolution priority):
1. `TemplateRoles.Domain.Entity.Primary`
2. `TemplateRoles.Domain.Entity.Interface`
3. `TemplateRoles.Domain.ValueObject`
4. `TemplateRoles.Domain.DataContract`
5. `TemplateRoles.Application.Contracts.Dto`
6. `TemplateId` (self — for nested DTO name resolution)

Domain enum types are resolved via type inference at the point of use rather than via a registered type source.

---

## Test App

`E:\Intent.Modules.NET\Tests\ObjectMappingTest\` — an Intent-managed Clean Architecture app with a comprehensive domain model covering all 16 mapping shapes. AutoMapper is NOT installed. SF produces 0 staged changes when the module is correctly implemented.

### `Tests/ObjectMappingTest.Tests.csproj` (hand-authored xUnit project)
Not modeled/generated by any module — a plain xUnit project, referencing `NSubstitute` (matching the convention in other Intent test apps, e.g. `CleanArchitecture.Comprehensive.Application.Tests`) for faking repository interfaces.
- `MappingExtensionsTests.cs` exercises the 16 mapping shapes directly against domain entities (bypasses handlers/repositories entirely).
- `OrderHandlerTests.cs` / `CustomerHandlerTests.cs` / `ProductHandlerTests.cs` exercise the generated MediatR query handlers (`Intent.Application.MediatR.QueryHandler` output) — repo-returns-entity → mapped DTO, and repo-returns-null → `NotFoundException`, for all 7 handlers in the app.

**Gotcha:** the project's disk folder is named `Tests`, not `ObjectMappingTest.Tests` (unlike every other project in this app, where folder name == project name). It was originally **not wired into `ObjectMappingTest.sln`** at all — `dotnet build/test` on the `.sln` silently skipped it; only `dotnet test` directly on the `.csproj` ran it. Fixed by modeling it properly in the Codebase Structure designer (`designerId` `0701433c-36c0-4569-b1f4-9204986b587d`): a `"5 - Tests"` Solution Folder + `C# Project (.NET)` element named `ObjectMappingTest.Tests`, with `C# Project Options` → `Relative Location` set to `Tests` (the override needed because the disk folder name doesn't match the element name). Confirmed via SF: it produces only a `.sln` diff, never touches the hand-authored `.csproj` content.

---

## Constraints

- Do not install this module alongside `Intent.Application.Dtos.AutoMapper` — both target the same role; `CanRunTemplate()` guards against it but designer confusion may result.
- `supportedClientVersions` in the imodspec must be manually verified after any SDK version bump — it does not auto-derive from the csproj package reference.
