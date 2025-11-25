# Intent.Application.Contracts.Clients

## What This Module Does
Generates client-facing service contracts (interfaces) and associated DTO/enum/paged result shapes for integration service proxies. These contracts define the application boundary for remote calls or generated client code (e.g., REST or gRPC clients).

## Generated Artifacts
- Service Contracts (`I[Service]Client`) representing remote service operations.
- DTO Contracts for data payloads exchanged with client services.
- Enum Contracts for value sets used in requests/responses.
- `PagedResult<T>` contract for paginated responses.

## Design Principles
- Separation: Distinct client contracts prevent coupling internal application interfaces with external consumption patterns.
- Versioned Boundary: Changes to client contracts intentionally reflect externally visible API surface.
- Strong Typing: DTO/Enum contracts provide schema clarity for generated client implementations and documentation.

## Usage Flow
1. Model integration service proxies in Service Proxies / CQRS designers.
2. Module generates client contracts in `IntegrationServices/` structure.
3. Additional modules (e.g., HTTP client generation) implement these interfaces or generate typed clients.
4. Application code depends on interfaces for DI-based substitution (e.g., mock clients in tests).

## Pagination
`PagedResult<T>` surfaces page metadata (e.g., items, total count or cursor info depending variant) enabling client UIs to drive paging logic.

## Customization Points
- Add custom headers / correlation metadata via partial methods or wrapper types.
- Extend DTO contracts with validation attributes if required by serialization pipeline.
- Introduce base interfaces for common behaviours (e.g., `IRemoteCall`) using merge mode.

## When To Use
- Systems exposing remote endpoints consumed by separately deployed clients or front-end applications.
- Need for generated strongly typed clients.

## When Not To Use
- Internal-only modules with no external consumer; standard application service interfaces may suffice.

## Related Modules
- Transport / client generation modules (e.g., HTTP client generation, gRPC).
- `Intent.Application.Contracts` (internal service contracts for server side).
