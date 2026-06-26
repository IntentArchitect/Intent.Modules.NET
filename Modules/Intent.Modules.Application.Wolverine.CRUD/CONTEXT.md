# CONTEXT.md — Intent.Application.Wolverine.CRUD

Durable architectural decisions, constraints, and patterns for the Wolverine CRUD module.

## 🏗️ Purpose

This module automatically implements CRUD handler bodies for Wolverine command and query handlers by hooking into the Domain Interactions pattern. It is a pure factory extension module — it generates no templates of its own; it only enriches handlers produced by `Intent.Application.Wolverine`.

---

## 🏗️ Architectural Constraints & Rules

### 1. Single Factory Extension — `CqrsHandlerCrudExtension`

`CqrsHandlerCrudExtension` fires in `OnAfterTemplateRegistrations` and scans all registered `CommandHandlerTemplate` and `QueryHandlerTemplate` instances. For each handler whose model has at least one Domain Interaction configured, it registers an `AfterBuild` callback that:

1. Calls `AddTypeSource` for the four domain entity roles (Primary, ValueObject, DataContract, Behaviour) so type resolution works inside generated code.
2. Clears the stub body of the `Handle` method and flips its code-management to `Mode.Fully`.
3. Wires all standard mapping resolvers (`EntityCreation`, `EntityUpdate`, `StandardDomain`, `ValueObject`, `DataContract`, `ServiceOperation`, `EnumCollection`, `CommandQuery`, `TypeConverting`).
4. Sets `SetFromReplacement` so the source variable name inside generated statements is `"command"` for commands and `"query"` for queries — matching the Wolverine handler method parameter names.
5. Calls `ImplementInteractions(interactions)` to emit the full CRUD body.
6. Appends return statements via `GetReturnStatements` if the handler has a non-void return type.

### 2. Handlers Without Interactions — Convention Fallback

When a handler's model has zero Domain Interactions the extension does **not** immediately skip query handlers. Instead `ConventionGetAllStrategy.TryApply` (in `CrudStrategies/`) runs as a convention-based fallback that mirrors the `Intent.Application.MediatR.CRUD` `GetAllImplementationStrategy`:

- It matches only when the model has **no** domain interactions, returns a **collection**, and the returned DTO is **mapped from a domain entity** (`Map from Domain`), and that entity is **not** a nested compositional child (those must go through their aggregate root and are left to the interactions path).
- On a match it injects the entity repository (`TemplateRoles.Repository.Interface.Entity`) and `AutoMapper.IMapper`, clears the stub body, flips the `Handle` method to `Body = Mode.Fully`, and emits `var <entities> = await _<repo>.FindAllAsync(cancellationToken); return <entities>.MapTo<Dto>List(_mapper);`.

This means a "get all" query modelled purely by convention (collection return + mapped DTO, no `Query Entity Action`) now generates a full implementation instead of remaining a `// TODO` stub. Command handlers and non-matching query handlers without interactions are still left untouched.

> The companion modeling style — a `Query Entity Action` carrying an empty `Query Entity Mapping` — also produces an unfiltered `FindAllAsync` via the standard `QueryInteractionStrategy`. Both approaches are supported and yield equivalent output; the convention path simply requires no association.

### 3. Paged-Result Special Case (Query Handlers Only)

If the query's return type contains `"PagedResult"` and the query has an `OrderBy` property (case-insensitive), the extension:
- Adds `using static System.Linq.Dynamic.Core.DynamicQueryableExtensions;` to the template.
- Registers the `System.Linq.Dynamic.Core` NuGet dependency via `AddNugetDependency(SharedNuGetPackages.SystemLinqDynamicCore)`.

This handles dynamic ordering on paged queries without requiring explicit model metadata.

### 4. Query Return-Statement Guard

For query handlers the return statement block is only appended when `ExecutionPhases.Response` contains no statements already — preventing double-emission if `ImplementInteractions` already placed a return as part of the interaction body.

For command handlers there is no such guard; `GetReturnStatements` is always appended when a return type is present.

### 5. Dependency on `Intent.Application.Wolverine`

This module depends on `Intent.Application.Wolverine` for the `CommandHandlerTemplate` and `QueryHandlerTemplate` template IDs. It must never be installed without that module also being present.

### 6. Dependency on `Intent.Application.DomainInteractions`

All mapping resolvers and the `ImplementInteractions` extension method come from `Intent.Application.DomainInteractions`. This is declared as both a NuGet package reference (in the `.csproj`) and a module dependency (in the `.imodspec`), so the IA runtime enforces correct load order.

---

## ⚠️ Anti-Patterns (Must Nots)

1. **Do not add templates here.** Code generation templates belong in `Intent.Application.Wolverine`. This module is extension-only.
2. **Do not use `ProjectReference` to `Intent.Modules.Application.DomainInteractions`.** It must be a `PackageReference` so the packaged module resolves correctly in consumer projects.
3. **Do not register resolvers outside `AfterBuild`.** `SetFromReplacement` and mapping resolvers must be configured inside the `AfterBuild` callback, after the `CSharpFile` structure is fully built.
4. **Do not call `ImplementInteractions` without clearing the stub body first.** The stub `// TODO` statements must be removed before calling `ImplementInteractions`, otherwise both the stub and the generated CRUD code are emitted.
