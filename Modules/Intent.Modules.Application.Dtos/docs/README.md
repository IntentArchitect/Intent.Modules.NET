# Intent.Application.Dtos

## What This Module Does
Generates Data Transfer Object (DTO) definitions and contract enums for application service operations. It applies configurable conventions (class vs record, setter accessibility, sealed modifier, static factory method) to produce consistent serialization-ready shapes.

## Generated Artifacts
- DTO classes/records for each mapped Service DTO model (`[Name]Dto`).
- Contract enums for enumerated types referenced in service contracts.
- Optional static factory method for easier creation when enabled.
- Property accessibility adjusted (public/private/protected/internal/init) per settings.
- Optional sealing of DTOs.

## Settings (DTO Settings Group)
- Type: `class` or `record` (records enable value-based equality and succinct initialization).
- Property Setter Accessibility: Controls mutability; important when combined with mapping modules that may need internal setters.
- Sealed: Adds `sealed` modifier for inheritance control.
- Static Factory Method: Generates `Create(...)` or similar for fluent construction.

## Design Considerations
- DTOs serve as application boundary contracts; changes propagate to clients and mapping profiles.
- Records recommended for immutable intent; use `init` setters for safe mutation during mapping.
- Private/internal setters reduce accidental mutation post-construction.

## Integration Points
- Used by Service Contract interfaces (`Intent.Application.Contracts`).
- Mapped by `Intent.Application.Dtos.AutoMapper` (and extended by DocumentDB cross-aggregate mapping module).
- Consumed in Eventing / CQRS request/response patterns.

## Customization Points
- Partial classes allow adding validation or convenience methods.
- Decorator (DataContractDTOAttributeDecorator) can be enabled to apply `[DataContract]` attributes.
- Merge-mode modifications can introduce interfaces (e.g., `IHasId`) or base types.

## When To Use
- Any application service layer requiring structured request/response models decoupled from domain entities.

## When Not To Use
- Extremely simple prototypes where direct domain entity exposure is acceptable (not recommended for production).

## Related Modules
- `Intent.Application.Contracts`
- `Intent.Application.Dtos.AutoMapper`
- `Intent.DocumentDB.Dtos.AutoMapper`
