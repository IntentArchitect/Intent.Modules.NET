# Context: Intent.Application.MediatR.CRUD

## Purpose

Implements the bodies of `Intent.Application.MediatR` Command and Query handlers. It works from
modelled mappings and Domain Interactions (`CqrsHandlerCrudExtension`), or from the legacy
convention strategies (`StrategyFactory` / `CrudStrategies`). It finds handlers by the MediatR
`CommandHandlerTemplate`/`QueryHandlerTemplate` `TemplateId`, so it never touches Wolverine handlers.

## Architectural Decisions

- **Implemented `Handle` bodies are `Body = Mode.Fully`.** Intent owns a CRUD-implemented body and
  keeps it in line with the model. To customise one, the developer or AI switches it to
  `Body = Mode.Ignore` (the `mediatr-command-handler`/`mediatr-query-handler` skills say so). From
  then on, mapping changes no longer reach that handler. While this module is installed, the base
  MediatR module generates handlers it does not implement as `Body = Mode.Merge`, so a mapping added
  later is still implemented (Merge→Fully). See its `CONTEXT.md`.
- **Rejected: `Body = Mode.Merge` on implemented bodies.** Tried on 2026-10-13 as 7.2.3-pre.0 (never
  published) so hand-added statements could live alongside generated ones that keep tracking the
  model. Withdrawn because Merge confused the AI: it has to tell generated statements from its own,
  and editing a generated statement in place makes Intent re-add it as a duplicate. Fully/Ignore
  gives it one rule: switch to Ignore before writing code.
- **Attributes on other generated helper methods stay `Fully`.** Examples are the private methods
  added by `CreateImplementationStrategy`/`UpdateImplementationStrategy`/`DomainCtorImplementationStrategy`.
  Only `Handle` is the developer's extension point.
