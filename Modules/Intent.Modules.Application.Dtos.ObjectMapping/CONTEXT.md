# CONTEXT: Intent.Application.Dtos.ObjectMapping

## Purpose

Generates explicit C# object mapping extension methods for DTOs that have a `[map from]` domain entity mapping in the Services designer. Drop-in replacement for `Intent.Application.Dtos.AutoMapper` that produces readable, zero-reflection code with no third-party library dependency.

---

## Architecture Decisions

### Template-only, no runtime library
Generated code uses plain C# — object initializers, LINQ, and explicit casts. No NuGet runtime dependency is introduced into the user's application.

### Role: `Application.EntityDtoMappingExtensions`
The template fulfills `TemplateRoles.Application.EntityDtoMappingExtensions`. The MediatR query handler template (`Intent.Application.MediatR.QueryHandler`) checks for this role and generates `entity.MapToXxxDto()` call sites automatically. No factory extension is needed to rewire the handlers.

### Factory extension stub
`ObjectMappingCrudFactoryExtension.cs` was scaffolded but is a no-op. The handler template's role-awareness makes it unnecessary for clean-install scenarios. Left in place for potential future co-existence logic.

### Mutual exclusion with AutoMapper
`CanRunTemplate()` returns `false` when `Intent.Application.Dtos.AutoMapper` is installed. This prevents duplicate mapping files when both modules are present during a migration. The user must uninstall AutoMapper first.

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

---

## Constraints

- Do not install this module alongside `Intent.Application.Dtos.AutoMapper` — both target the same role; `CanRunTemplate()` guards against it but designer confusion may result.
- `supportedClientVersions` in the imodspec must be manually verified after any SDK version bump — it does not auto-derive from the csproj package reference.
