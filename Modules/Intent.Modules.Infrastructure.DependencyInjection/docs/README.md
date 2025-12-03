# Intent.Infrastructure.DependencyInjection

## What This Module Does
Generates infrastructure-level DI registration code (an `AddInfrastructure` extension or similar) centralizing registration of persistence, messaging, caching, and other infrastructure services separate from application service registrations.

## Generated Artifact
- Static DI registration class from template `Infrastructure.DependencyInjection.DependencyInjection` packaging infrastructure service registrations behind a single extension method.

## Responsibilities
- Register infrastructure implementations of repository interfaces, event bus, cache, file storage, external API clients, etc.
- Keep application layer agnostic of concrete types; application adds its own services via `AddApplication` (from `Intent.Application.DependencyInjection`).
- Provide composition root clarity: Startup calls `services.AddInfrastructure(configuration)` then `services.AddApplication(configuration)`.

## Customization Points
- Merge-mode file supports adding new registrations or conditional logic (e.g., environment-based provider selection).
- Introduce health checks, instrumentation registration, or policy setups (Polly, retry strategies) here.

## Related Modules
- `Intent.Application.DependencyInjection`
- Persistence provider modules (EF Core, MongoDB, CosmosDB).
- Messaging / eventing provider modules.
