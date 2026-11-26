# Context: Intent.Application.AutoMapper

## Purpose

Generates AutoMapper `Profile` mappings for DTOs mapped from domain entities, and the AutoMapper registration. It also
adds a mapping section to the AI handler skills that other modules generate.

## Architectural Decisions

- **AI skill guidance is a "Mapping (AutoMapper)" section, at Order 2.** `AISkillsExtension` appends it to every
  `AI.Context.Skills.Handler` skill with `WithSection`. Order 2 puts it after the "Persistence (EF Core)" section that
  `Intent.EntityFrameworkCore` (Order 0) and `Intent.EntityFrameworkCore.Repositories` (Order 1) write, which reads
  as load, change, then map. `Intent.Application.Dtos.Mapperly` uses the same order and the same section shape.
- **Two rules, no code example (5.4.1).** The section says to map entity-shaped DTOs through AutoMapper and add a
  missing profile as `Mappings/<Aggregate>/<Entity>DtoProfile.cs`, and to build a DTO by hand only when it is not
  entity-shaped, with a comment saying why. The code example and the "cite file path and excerpt" procedure were
  removed: the evaluated text without them produced the same mappings. The reasoning is recorded in
  `Intent.Application.MediatR`'s `CONTEXT.md`, "Handler skills: trimmed text, no pointer rule".
- **The section goes to every handler skill, queries included,** because queries return DTOs too.
