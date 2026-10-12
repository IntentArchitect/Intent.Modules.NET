# Context: Intent.Application.Dtos

## Purpose
Generates the C# DTO classes/records and contract enums for the Services designer's DTOs. The DTO Settings group
(class/record, Property Setter Accessibility, Sealed, Static Factory Method) shapes how each DTO is constructed.

## Architectural Decisions
- **The default value order warning lives here.** `designers/DTO Services Extensions` holds a DTO Element Extension
  whose Extension Settings › Validate Function warns when a field without a default value follows one with a default
  value. The DTO template's protected constructor and static `Create` method silently drop any default that is not
  part of the trailing run (`ShouldSetDefaultValue`), so the warning belongs with the template that drops it.
  Rejected: leaving it in `Intent.Modelers.Services` (its home until 4.0.16). There it read this module's settings by
  GUID and fired in applications without this module.
- **The check runs only when a constructor or `Create` method gets parameters.** It is skipped when Property Setter
  Accessibility is public, init or unset, and either Static Factory Method is off or the DTO is abstract. The
  template sets defaults with property initialisers in that case, and ordering doesn't matter. This mirrors
  `IsNonPublicPropertyAccessors()` and `!Model.IsAbstract && StaticFactoryMethod()` in `DtoModelTemplatePartial`.
  Static Factory Method counts as on only for the value `true`, matching the C# `bool.TryParse`. The previous gate
  in the designer module warned on abstract DTOs and treated an unset accessibility as non-public. If the template's
  conditions change, change the script's gate with them.

## Invariants & Constraints
- The script reads DTO Settings fields by id (`aac8a446-…` group, `1b43bdbc-…` accessibility, `c2df571f-…` static
  factory). Never delete and recreate those settings fields.
- The script checks only the DTO's own fields. The template orders constructor parameters with
  `GetFieldsHierarchally` (inherited fields first), so an ordering problem that spans a base DTO isn't flagged.

## Module Interactions
- **Intent.Modelers.Services**: floor 4.0.17 (written against `4.0.17-pre.2`), the first version without its own copy
  of this check. Its `<interoperability>` updates this module to 4.6.2 in return. Lowering either side lets an
  application run both copies (a duplicate warning) or neither.
