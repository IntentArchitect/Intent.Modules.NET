# Context: Intent.Application.ServiceCallHandlers

## Purpose

Moves each traditional service operation's body into its own Service Call Handler class, and turns
the service operation into a dispatcher that resolves the handler and calls `Handle`
(`ServiceImplementationExtension`).

## Architectural Decisions

- **The handler's `Handle` body mode comes from `Intent.Application.ServiceImplementations`'
  `OperationBodyModeExtensions`.** With `Intent.Application.ServiceImplementations.Conventions.CRUD`
  installed it is `Fully` + `Body = Mode.Merge`, unchanged from published versions. CRUD implements
  `Handle` instead of the service operation and overrides it to `Body = Mode.Fully`. Without CRUD it is
  `Fully` + `Body = Mode.Ignore`, as nothing can implement it. Sharing the helper keeps the module list
  in one place; see ServiceImplementations' `CONTEXT.md` for the reasoning.
- **The dispatcher stays `Body = Mode.Fully`.** `ServiceImplementationExtension` forces it, overriding
  the attribute the service template set. The dispatcher is generated plumbing, not an extension point,
  so it must keep tracking the model. ServiceImplementations' skill tells the AI to leave it alone when
  this module is installed.

## Module Interactions

- **Intent.Application.ServiceImplementations** — referenced as a project, not a package. Written
  against 4.7.6: `GetOperationMethodIntentManagedAttribute` was added in 4.7.6, so the `.imodspec`
  floor is 4.7.6. Against an older version the template would fail with a missing method at Software
  Factory time.
