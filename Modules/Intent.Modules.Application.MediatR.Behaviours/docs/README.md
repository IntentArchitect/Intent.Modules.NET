# Intent.Application.MediatR.Behaviours

## What This Module Does
Adds cross-cutting MediatR pipeline behaviours to your application request handling pipeline: logging, performance measurement, authorization, unit of work commit, unhandled exception capture, and event bus publication.

## Generated Behaviours
- `LoggingBehaviour<TRequest,TResponse>`: Logs request start/end, may include correlation and serialized request snapshot.
- `PerformanceBehaviour<TRequest,TResponse>`: Times request execution; can warn when exceeding threshold (implementation specific).
- `AuthorizationBehaviour<TRequest,TResponse>`: Enforces `[Authorize]` attribute semantics or role/claim requirements using `ICurrentUser`.
- `UnhandledExceptionBehaviour<TRequest,TResponse>`: Catches unhandled exceptions, logs, optionally rethrows or wraps.
- `UnitOfWorkBehaviour<TRequest,TResponse>`: Ensures transactional commit (via `IUnitOfWork`) after successful handler execution.
- `EventBusPublishBehaviour<TRequest,TResponse>`: Publishes integration/domain events registered during request handling via `IEventBus`.

## Order & Composition
MediatR executes behaviours in registration order (outer → inner). Typical ordering strategy:
1. Logging / Performance (outer)
2. Authorization
3. UnhandledException
4. UnitOfWork
5. EventBus publication (inner, after success)

## Dependencies
- `Intent.Application.Identity` for `ICurrentUser` used by AuthorizationBehaviour.
- `Intent.Common.UnitOfWork` and repository abstractions for UnitOfWorkBehaviour.
- `Intent.Eventing.Contracts` (via Application Eventing) for EventBusPublishBehaviour.
- `Intent.Application.DependencyInjection.MediatR` for registration of behaviours into DI container.

## Interoperability Detectors
Imodspec declares minimum versions for optional persistence/state modules (CosmosDB, MongoDb, Dapr State) to ensure compatibility when behaviours interact with their transactions / contexts.

## Customization Points
- Adjust logging detail (redaction, correlation IDs) by editing merge-mode partial class.
- Inject metrics/tracing (OpenTelemetry) inside PerformanceBehaviour.
- Authorization rules extension: custom attributes / policies.
- Event bus behaviour can filter or batch events.

## When To Use
- Medium/large applications seeking consistent cross-cutting concerns without scattering logic.

## When Not To Use
- Extremely small or script-like projects; pipeline overhead unnecessary.

## Related Modules
- `Intent.Application.Identity`
- `Intent.Eventing.Contracts`
- `Intent.Application.DependencyInjection`
