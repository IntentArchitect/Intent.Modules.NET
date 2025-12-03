# Intent.Entities.Repositories.Api

## What This Module Does
Generates repository interfaces, unit of work contracts, and paging abstractions for your Domain model. It separates domain persistence concerns behind interfaces (e.g. `IEntityRepository<T>`), enabling infrastructure implementations (EF Core, DocumentDB, Dapr State, etc.) to be swapped without impacting domain or application layers.

## Generated Artifacts
- Entity repository interfaces (`I[Entity]Repository`) for each aggregate/root configured.
- Generic repository contract (`IRepository<T>` or similar) for base constraints.
- Paging abstractions: `IPagedList<T>` and `ICursorPagedList<T>` (cursor-based pagination) when selected.
- Optional custom repository interfaces/classes (via Custom Repository templates).
- `IUnitOfWork` interface for committing transactional changes.

## Key Templates
- `EntityRepositoryInterface` – per entity aggregation root; exposes query + persistence operations.
- `CustomRepositoryInterface` / `CustomRepository` – opt-in extensibility for specialized access patterns.
- `RepositoryInterface` – base contract type for common repository operations.
- `PagedListInterface` / `CursorPagedListInterface` – paging result shapes.
- `UnitOfWorkInterface` – abstraction for atomic persistence boundary.

## Design Principles
- Interface Segregation: Each entity gets its own repository interface to avoid large, catch-all interfaces.
- Persistence Ignorance: Domain entities do not depend on specific persistence provider implementations.
- Testability: Application services consume repository interfaces allowing in-memory or mocked implementations in tests.

## Interoperability
The imodspec declares detection rules for provider modules (e.g. EF Core, Dapr State) to ensure minimum compatible versions are present before generation.

## Typical Usage Flow
1. Add persistence provider module (e.g. `Intent.EntityFrameworkCore.Repositories`).
2. Add this module; generation produces repository interfaces in your Domain project.
3. Provider module generates concrete implementations in Infrastructure layer satisfying these contracts.
4. Application layer depends only on interfaces; provider swaps require no application changes.
5. Unit of Work orchestrates commit semantics for grouped changes.

## Customization Points
- Partial classes on custom repositories allow adding bespoke queries.
- Decorators/factory extensions referenced (CustomDbProviderFactoryExtension, CustomRepositoryMethodsExtension) can augment or inject methods.
- Extend paging interfaces with additional metadata (e.g. total count, has next page) via merge mode.

## When To Use
- Any layered architecture requiring clean separation between domain logic and persistence specifics.
- Need for multiple persistence providers (SQL, NoSQL) under uniform contracts.

## When Not To Use
- Extremely simple applications where direct DbContext usage is acceptable and repository abstraction adds overhead.
- CQRS-only designs relying on direct projections without a repository pattern.

## Related Modules
- `Intent.EntityFrameworkCore.Repositories`
- `Intent.Dapr.AspNetCore.StateManagement`
- Application-level modules consuming repositories (e.g., CRUD handlers).
