# Context: Intent.Application.Dtos.Mapperly

## Purpose

Generates Mapperly `[Mapper]` classes for DTOs mapped from domain entities, and their registration. It also adds a
mapping section to the AI handler skills that other modules generate.

## Architectural Decisions

- **AI skill guidance is a "Mapping (Mapperly)" section, at Order 2.** `AISkillsExtension` appends it to every
  `AI.Context.Skills.Handler` skill with `WithSection`. Order 2 puts it after the "Persistence (EF Core)" section that
  `Intent.EntityFrameworkCore` (Order 0) and `Intent.EntityFrameworkCore.Repositories` (Order 1) write. It mirrors
  `Intent.Application.AutoMapper`'s section.
- **Two rules, no code example (1.1.7).** The section says to map entity-shaped DTOs through a Mapperly mapper, and to
  add a missing one, with nested mappers and list overloads, as `Mappings/<Aggregate>/<Entity>DtoMapper.cs`, registered
  the way the existing mappers are. It also says to build a DTO by hand only when it is not entity-shaped, with a comment
  saying why. The code example and the "cite file path and excerpt" procedure were removed. The reasoning is recorded in
  `Intent.Application.MediatR`'s `CONTEXT.md`, "Handler skills: trimmed text, no pointer rule".
- **Unlike AutoMapper, a new mapper must be registered by hand.** Mapperly mappers are not found by assembly scanning,
  so the rule names the registration step.

## Invariants & Constraints

- **`AISkillsExtension` sits in the `Intent.Modules.Application.AutoMapper.FactoryExtensions` namespace**, copied
  from the AutoMapper module. Its `Id` (`Intent.Application.Mapperly.AISkillsExtension`) is what identifies it, so the
  namespace is harmless; do not rename the `Id`.
