### Version 1.0.0

- New Feature: Added `MappingExtensions` template that generates `{DtoName}MappingExtensions.cs` static classes with explicit `MapTo{Name}()` and `MapTo{Name}List()` extension methods for every DTO with a `[map from]` domain mapping.
- New Feature: Generated mapping bodies handle flat primitives, nullable navigation properties (`?.`), nested DTO composition (recursive `MapTo` calls), collection projections (`Select(...).ToList()`), FK extraction from association ends, collection multi-hop paths, and explicit enum-to-enum casts.
- New Feature: `CanRunTemplate()` guard prevents this module activating when `Intent.Application.Dtos.AutoMapper` is installed, allowing controlled migration between the two mapping providers.
- Improvement: `MappingExtensions` template now declares an explicit `Mappings` Default Location (matching the `Intent.Application.Dtos.AutoMapper`/`Mapperly` convention) instead of relying on an implicit default. No change to generated output.
- Improvement: Removed the unused `ObjectMappingCrudFactoryExtension` scaffold stub. It was a permanent no-op with no generated output; the module has no factory extension.
