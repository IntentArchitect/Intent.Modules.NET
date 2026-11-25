# Intent.Application.Dtos.Mapperly

## What This Module Does
Generates [Mapperly](https://mapperly.riok.app) mapping classes and extension methods for DTOs produced by `Intent.Application.Dtos`, enabling automatic mapping between Domain Entities and their DTO representations.

## Generated Artifacts
- Mapper classes (e.g., `[Entity]To[Dto]Mapper`) with `Map` methods for converting entities to DTOs. Location configurable by template; often `Mappings/`.
- Mapping extension methods (`MapTo[Dto]`, `MapTo[Dto]List`) for convenient projection in queries or service logic.
- Factory extension (`MapperlyDtoFactoryExtension`) hooking mapper generation / consolidation.

## Mapping Behaviours
- Maps scalar properties and nested DTO references when source model mappings exist.
- Generates null-safe logic conditioned on DTO property nullability.

## Integration Points
- Consumed by service implementations, query handlers, and event handlers to convert entities to DTOs.
- Works alongside MediatR request handlers for returning DTO results.

> [!NOTE]
>
> This will only generate mappings in one direction: outbound.
> Using dynamic mapping systems like Mapperly is discouraged from being used to map from DTOs to Entities. This can have unintended consequences to your data which is why using systems like `Intent.Application.DomainInteractions` is more preferred to ensure explicit mapping code is generated to project data from DTOs to Entities.

## Customization Points
- Extend generated mapper classes via partial classes to add custom resolvers / value transforms.
- Override or add mapping in extension methods for bespoke projection logic.
- Combine with cross-aggregate module to handle relationships not directly navigable in entity model.

## AI skill guidance
The module adds a "Mapping (Mapperly)" section to the handler and service skills that other modules generate. It tells an agent to return entity-shaped DTOs through a Mapperly mapper rather than `new XxxDto { … }`, to add a missing `[Mapper]` class under `Mappings/<Aggregate>/` and register it the way existing mappers are, and to build a DTO by hand only when it is not entity-shaped, with a comment saying why.

## Related Modules
- `Intent.Application.Dtos`
