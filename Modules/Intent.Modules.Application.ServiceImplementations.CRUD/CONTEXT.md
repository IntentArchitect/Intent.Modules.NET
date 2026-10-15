# Context: Intent.Application.ServiceImplementations.Conventions.CRUD

## Purpose

Implements the bodies of traditional service operations from modelled mappings and Domain
Interactions (`OperationMappingImplementationStrategy`), or from naming conventions
(`MethodImplementationStrategies`). It targets Service Call Handler `Handle` methods instead of
service operations when `Intent.Application.ServiceCallHandlers` is installed
(`CrudServiceFactoryExtension` looks for the `Application.Implementation.Custom` role first).

## Architectural Decisions

- **Implemented bodies are `Body = Mode.Fully`.** Intent owns a CRUD-implemented body. To customise
  one, the developer or AI switches it to `Body = Mode.Ignore` (the `application-service-implementation`
  skill says so), after which mapping changes no longer reach it. While this module is installed,
  `Intent.Application.ServiceImplementations` generates operations it does not implement as
  `Body = Mode.Merge`. With no services CRUD module installed, it generates them as `Body = Mode.Ignore`
  (from 4.7.6).
- **Rejected: `Body = Mode.Merge` on implemented bodies.** Tried as 5.3.6-pre.0 (never published) and
  withdrawn, as in the CQRS CRUD modules: Merge's "keep generated statements, add your own" contract
  confused the AI. `Intent.Application.MediatR`'s `CONTEXT.md` has the full reasoning.
- **Attributes on generated helper methods stay `Fully`.** For example, the private methods added by
  `CreateImplementationStrategy`/`UpdateImplementationStrategy`. Only the operation (or `Handle`) is the
  developer's extension point.

## Module Interactions

- **Intent.Application.ServiceImplementations** — referenced as the 4.5.14 NuGet package. Its
  `OperationBodyModeExtensions` (4.7.6) keys the default attribute to this module being installed;
  this module does not call it, so its dependency floor is unaffected.
