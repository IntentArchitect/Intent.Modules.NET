# Intent.Eventing.Contracts

## What This Module Does
Provides application-level eventing contracts: Event Bus interface, integration event DTO / enum / message types, integration command types, and handler interfaces. It standardizes how application code publishes and consumes integration events/commands across transport implementations.

## Generated Artifacts
- `IEventBus` interface (publish events, commands; potentially subscribe/unsubscribe depending on implementation).
- Integration Event DTOs (typed payload classes) and Enums.
- Integration Event Message wrapper (metadata envelope for event distribution).
- Integration Commands (request-like messages for asynchronous processing).
- Integration Event Handler interfaces (`IIntegrationEventHandler<TEvent>`).
- Assembly attributes template allowing reflection-based scanning or version metadata.

## Design Principles
- Decoupling: Publishers depend only on `IEventBus`; transport-specific code lives in infrastructure modules.
- Explicit Contracts: Event payloads, commands and enums generated from modelling provide schema clarity.
- Handler Abstraction: Consumers implement handler interfaces enabling pipeline behaviours (logging, retries, etc.).

## Typical Workflow
1. Model integration events/commands in Eventing or Services Event Interactions modelers.
2. Generation creates event DTOs/enums and handler interfaces.
3. Implement transport (e.g. Azure Service Bus, RabbitMQ, Kafka) in separate module providing concrete `IEventBus`.
4. Application services publish via `IEventBus.PublishAsync(...)` without transport coupling.
5. Infrastructure resolves handlers and dispatches events/commands.

## Interoperability
Detects CRUD MediatR module to ensure event interactions can be woven into generated request handlers (e.g., publish domain integration events after operations). Additional transport-specific modules will implement concrete bus.

## Customization Points
- Extend Message envelope with correlation IDs, tracing information.
- Enrich handler interfaces with context objects (e.g., headers) via partial methods.
- Add semantic partitions or routing keys in enums.

## When To Use
- Distributed architectures needing clear integration boundary events.
- Systems employing event-driven or reactive patterns.

## When Not To Use
- Purely monolithic deployment where internal method calls suffice; events add overhead.

## Related Modules
- Transport modules (Azure Service Bus, RabbitMQ, etc.)
- `Intent.Application.MediatR.Behaviours` (EventBusPublishBehaviour for automatic publication)

