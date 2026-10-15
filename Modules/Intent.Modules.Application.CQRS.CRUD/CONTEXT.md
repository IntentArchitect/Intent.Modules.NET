# Context: Intent.Application.CQRS.CRUD

## Purpose

Transport-agnostic implementation of Command and Query handler bodies from modelled Domain
Interactions, plus "get all" / paged conventions for queries without interactions. It finds
handlers by `TemplateRoles.Application.Handler.Command` / `.Query`, so it implements both MediatR
and Wolverine handlers.

## Architectural Decisions

- **Implemented `Handle` bodies are `Body = Mode.Fully`.** This covers `CqrsHandlerCrudExtension` and
  `ConventionGetAllStrategy`. To customise one, the developer or AI switches it to
  `Body = Mode.Ignore`. Both transport modules key their default for handlers this module does not
  implement to a CRUD module being installed: `Body = Mode.Merge` when one is (unchanged from published
  versions), `Body = Mode.Ignore` when none is. MediatR also checks for `Intent.Application.MediatR.CRUD`;
  Wolverine checks for this module alone. See their `CONTEXT.md` files.
- **Rejected: `Body = Mode.Merge` on implemented bodies.** Tried as 1.0.1-pre.0 (never published) and
  withdrawn because Merge's "keep generated statements, add your own" contract confused the AI.
  `Intent.Application.MediatR`'s `CONTEXT.md` has the full reasoning.
