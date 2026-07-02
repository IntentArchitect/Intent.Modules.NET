# CONTEXT.md — Intent.Eventing.Contracts

Durable architectural decisions and gotchas for the Eventing Contracts module.

## 🏗️ Domain-interaction mapping in processing handlers is deliberately self-contained

Integration/Domain Event Handlers can host Domain Interactions (Query, Create Entity, etc.) via
`ApplicationServiceInteractionInstaller` (`FactoryExtensions/`), which calls the SDK's
`method.ImplementInteractions(...)`. Crucially, this module does **not** depend on
`Intent.Application.DomainInteractions`. The interaction *framework* (`ImplementInteractions`,
`IInteractionStrategy`, `IMappingTypeResolver`, the mapping engine) lives in the SDK
(`Intent.Modules.Common.CSharp`), and this module supplies its **own** mapping resolvers under
`MappingTypeResolvers/` (`ProcessingHandlerDomainMappingTypeResolver`,
`ProcessingHandlerDomainUpdateMappingTypeResolver`, `MessageCreationMappingTypeResolver`) rather than
reusing the resolvers from `Intent.Application.DomainInteractions`.

**Rule:** keep it that way. Do not add a dependency on `Intent.Application.DomainInteractions` just to
borrow a resolver (e.g. `EntityCreationMappingTypeResolver`) — that would couple every eventing consumer
to the domain-interactions module. If a processing-handler resolver is missing a case that the
DomainInteractions equivalent handles, **port the specific branch** into the local resolver instead.

## ⚠️ Local resolvers must mirror the DomainInteractions equivalents' node-matching (v6.1.3)

Because the resolvers here are hand-maintained copies of a subset of DomainInteractions' logic, they can
drift. `ProcessingHandlerDomainMappingTypeResolver` originally claimed only `model.IsClassModel() ||
model.IsConstructorModel()` for object-initialisation. That misses a **collection-typed node whose element
type is a Class** — e.g. a Create Entity Mapping child like `CatalogueItems` projecting a query result's
collection onto `ICollection<CatalogueItem>`. `IsClassModel()` is true only when the node's *own* model is
a class (the root entity), not when the node's *type reference* element is a class. The missed node fell
through to the default resolver and was emitted as a bare assignment
(`CatalogueItems = dto.CatalogueItems`) instead of a `Select(...).ToList()` projection — which does not
compile (CS0266). The fix added `|| model.TypeReference?.Element?.SpecializationType == "Class"`, mirroring
the branch that `Intent.Application.DomainInteractions`'s `EntityCreationMappingTypeResolver` already had.
`ObjectInitializationMapping` detects the collection and emits the projection.

Regression coverage: `Tests\Subscribe.MassTransit.DomainInteractionsRepro` — a MassTransit Integration
Event Handler with `[call] result = GetCatalogueByIdQuery(...)` then `[create] catalogue: Catalogue`,
where `CatalogueItems` is sourced through `result`. Re-run Software Factory there after any change to the
`MappingTypeResolvers/` and confirm the projection still generates.
