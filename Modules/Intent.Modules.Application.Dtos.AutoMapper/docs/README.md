# Intent.Application.Dtos.AutoMapper

## What This Module Does
Generates AutoMapper mapping profiles and extension methods for DTOs produced by `Intent.Application.Dtos`, enabling automatic mapping between Domain Entities and their DTO representations.

## Generated Artifacts
- `DtoMappingProfile` class (mapping configuration per DTO including property maps). Location configurable by template; often `Mappings/`.
- Mapping extension methods (`MapTo[Dto]`, `MapTo[Dto]List`) for convenient projection in queries or service logic.
- Factory extension (`AutoMapperDtoFactoryExtension`) hooking profile generation / consolidation.

## Mapping Behaviours
- Maps scalar properties and nested DTO references when source model mappings exist.
- Generates null-safe logic conditioned on DTO property nullability.
- Facilitates composition with other AutoMapper customization modules (e.g. DocumentDB cross-aggregate mapping).

## Integration Points
- Consumed by service implementations, query handlers, and event handlers to convert entities to DTOs.
- DocumentDB cross-aggregate enhancements layer additional `AfterMap` actions for multi-aggregate resolution.
- Works alongside MediatR request handlers for returning DTO results.

> [!NOTE]
>
> This will only generate mappings in one direction: outbound.
> Using dynamic mapping systems like Automapper is discouraged from being used to map from DTOs to Entities. This can have unintended consequences to your data which is why using systems like `Intent.Application.DomainInteractions` is more preferred to ensure explicit mapping code is generated to project data from DTOs to Entities.

## Customization Points
- Extend `DtoMappingProfile` via partial class to add custom resolvers / value transforms.
- Override or add mapping in extension methods for bespoke projection logic.
- Combine with cross-aggregate module to handle relationships not directly navigable in entity model.

## Related Modules
- `Intent.Application.Dtos`
- `Intent.DocumentDB.Dtos.AutoMapper`
