# Intent.Application.Wolverine.CRUD

This module automatically implements the bodies of Wolverine CQRS command and query handlers from your designer model. It generates no templates of its own — it enriches the handlers produced by `Intent.Application.Wolverine` with repository calls, AutoMapper projections, and create/read/update/delete logic.

## What This Module Generates

The module contributes a single factory extension, `CqrsHandlerCrudExtension`, that fills in `Handle` method bodies for:

- **Command handlers** modelled with a `Create Entity Action`, `Update Entity Action`, or `Delete Entity Action` — producing entity construction/lookup, repository persistence, and a return value where applicable.
- **Query handlers** modelled with a `Query Entity Action` — producing `FindByIdAsync`/`FindAllAsync` lookups and AutoMapper projections to the return DTO.
- **"Get all" query handlers modelled by convention** — see below.

Handlers with no matching model (commands without an entity action, queries that match no path) are left as `// TODO` stubs.

## Generating "Get All" Query Handlers

An unfiltered "get all" query can be modelled two equivalent ways. Both generate the same implementation:

```csharp
var products = await _productRepository.FindAllAsync(cancellationToken);
return products.MapToProductDtoList(_mapper);
```

### By convention (no association)

Model a query that returns a collection of a DTO, and give that DTO a **Map from Domain** mapping to the entity. No association is required:

- `GetProductsQuery` returns `List<ProductDto>`
- `ProductDto` is mapped from the `Product` entity

The module detects this shape and generates the `FindAllAsync` + projection body. This mirrors the `Intent.Application.MediatR.CRUD` convention and is the output of the designer's "Create CRUD" accelerator.

> [!NOTE]
> The convention path only applies when the query has **no** domain interactions, returns a collection, and the returned DTO maps from a domain entity. Nested compositional entities (children of an aggregate root) are excluded — model those with a `Query Entity Action` instead.

### By domain interaction (Query Entity Action)

Alternatively, draw a `Query Entity Action` from the query to the entity and add a `Query Entity Mapping`. For an unfiltered get-all the mapping has no mapped ends; the standard `QueryInteractionStrategy` then emits `FindAllAsync`. Use this style when the query also needs filter criteria mapped onto entity attributes.

## Related Modules

### Intent.Application.Wolverine
Provides the `CommandHandlerTemplate` and `QueryHandlerTemplate` whose bodies this module fills in. This module must never be installed without it.

### Intent.Application.DomainInteractions
Supplies the mapping resolvers and the `ImplementInteractions` extension that drive the entity-action code generation paths.

### Intent.Application.Dtos.AutoMapper
Generates the `MapTo<Dto>` / `MapTo<Dto>List` projection extensions and AutoMapper profiles that the generated query handlers call.
