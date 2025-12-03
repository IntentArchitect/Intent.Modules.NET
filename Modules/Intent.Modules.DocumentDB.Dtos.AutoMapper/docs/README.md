# Intent.DocumentDB.Dtos.AutoMapper

## What This Module Does
Extends the base DTO + AutoMapper generation so that DTOs whose field mappings traverse DocumentDB aggregate boundaries are correctly populated. It injects repository lookups and mapping logic for cross-aggregate fields after AutoMapper's standard property mapping has run.

## Why You Need It
When a DTO field maps to data located in a different aggregate (e.g. navigating through an association chain that crosses aggregate roots), a simple AutoMapper configuration cannot materialize that data because it requires additional repository queries. This module detects those situations and generates an `AfterMap` hook with a `MappingAction` class that performs the necessary loads.

Use it when:
- You have DTOs whose field mapping path includes an aggregational association (collection or nullable end) not directly exposed on the source entity's public properties.
- You need to populate DTO fields from related aggregates using repository access within AutoMapper.

## How It Works
During template registration a factory extension (`DtoAutoMapperFactoryExtension`) runs `CrossAggregateMappingConfigurator.Execute(...)`:

1. Locates all DTO templates (`DtoModelTemplate`) that are mapped (`templateModel.Mapping` != null).
2. For each DTO, inspects each mapped field's path segments. If any segment is an aggregational association and the source entity does not expose a matching property, the DTO is considered a cross-aggregate mapping candidate.
3. Modifies the generated mapping profile (location determined by the Application.Dtos settings: Profile in DTO class or separate profile) to append `AfterMap<MappingAction>()` to the mapping chain.
4. Generates a nested `MappingAction` class that implements `IMappingAction<SourceEntity, Dto>`.
5. Injects required repositories (resolved via `EntityRepositoryInterfaceTemplate`) and `IMapper` into the `MappingAction` constructor.
6. In `Process(source, destination, context)` it:
   - Computes ordered load instructions for each aggregate path (ensuring parent aggregates load first).
   - Uses repository `FindByIdAsync` / `FindByIdsAsync` to materialize each aggregate based on foreign key attributes.
   - Throws a `NotFoundException` when a required (non-nullable) single aggregate cannot be loaded.
   - Assigns destination DTO properties from loaded aggregates, mapping nested DTO-valued fields using generated `MapTo{Dto}` helper functions.
   - Handles optional and "expression optional" segments with null checks.
7. If DTO property setter accessibility is configured as `private` or `init` (DTO Settings), property setters for affected fields are elevated to `internal` to allow mutation in the AfterMap stage.

## Generated Artifacts
- Augmented mapping chain: `AfterMap<MappingAction>()`.
- Nested `MappingAction` class containing repository fields and a `Process` method.
- Internal setters for certain DTO properties (only when necessary to enable assignment post-map).

## Key Implementation Details
- Aggregational association detection: An association end is considered aggregational if the source end is a collection or nullable.
- Load ordering: Aggregates load in ascending path length to respect dependency ordering.
- Foreign key resolution: For each associated aggregate the code searches the target entity for a FK attribute referencing the association's target end; absence raises an exception.
- Optional path handling: Two booleans distinguish optional association vs optional expression segment (`IsOptional` and `IsExpressionOptional`). Expression optionality currently relies on a simple `?` presence check in the path expression.
- Nested DTO mapping: DTO-type fields invoke generated `MapTo{Dto}` or `MapTo{Dto}List` helpers with `_mapper`.
- Error handling: Required single aggregate null -> throws `NotFoundException` including the attempted key value.
- Synchronous waits: Repository async calls are awaited via `.Result` inside the mapping action; consider refactoring for fully async pipelines if necessary.

## Configuration & Dependencies
- Depends on base modules: `Intent.Application.Dtos`, `Intent.Application.Dtos.AutoMapper`, `Intent.Entities.Repositories.Api`.
- Respects setting: DTO AutoMapper Profile Location (profile in DTO vs separate profile affects where AfterMap is injected).
- Respects DTO Settings: Property Setter Accessibility (may elevate to internal).

## Performance Considerations
- Multiple repository calls per mapped DTO can incur N+1 queries. Group or batch logic may be desirable for large collections – you can customize the generated `MappingAction` because its body is produced in merge mode.
- `.Result` usage can block; in high-throughput asynchronous contexts consider customizing to use fully async mapping patterns.

## Customization Points
- The nested `MappingAction` class can be modified (merge mode) to add caching, batching, or alternative loading strategies.
- You can introduce additional safety checks or logging around repository calls.
- Adjust exception messages or replace with domain-specific error handling as needed.

## Limitations
- Optional expression detection is heuristic (searches for `?` in path expression); complex optional patterns may require manual refinement.
- Assumes FK attributes exist; if foreign key modeling is absent generation will throw at build time.
- Does not attempt to minimize duplicate repository loads when multiple DTO fields traverse identical aggregate paths (can be manually optimized).

## When Not To Use
- DTOs confined to a single aggregate root (standard AutoMapper from `Intent.Application.Dtos.AutoMapper` suffices).
- Read models where manual projection queries (e.g., LINQ with joins) are more efficient than per-field repository lookups.

## Related Modules
- `Intent.DocumentDB` (aggregate modeling & persistence abstraction)
- `Intent.Application.Dtos`
- `Intent.Application.Dtos.AutoMapper`
- `Intent.Entities.Repositories.Api`

## Example (Conceptual)
A `OrderDetailsDto` maps `Customer.Email` where `Customer` is a separate aggregate not directly exposed as a navigation property on `Order`. The module:
1. Detects that `Customer` association segment requires cross-aggregate access.
2. Adds `AfterMap<MappingAction>()`.
3. In `MappingAction.Process` loads the `Customer` via `ICustomerRepository.FindByIdAsync(order.CustomerId)`.
4. Assigns `destination.CustomerEmail = customer.Email`.

## Extending Further
If you need batching, consider collecting all keys first, performing a single `FindByIdsAsync` per aggregate, and indexing results for assignment.

## Documentation Link
Published at: https://docs.intentarchitect.com/articles/modules-dotnet/intent-documentdb-dtos-automapper/intent-documentdb-dtos-automapper.html

