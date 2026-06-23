# Intent.Application.Wolverine.DomainEvents

Wires domain event dispatching through Wolverine's `IMessageBus`, generating a scoped `DomainEventService` and one handler stub per domain event type modeled in the Domain Events designer.

## What This Module Generates

- `DomainEventService` — implements `IDomainEventService`; injects `IMessageBus` and dispatches domain events via `PublishAsync`. Registered as scoped in DI.
- `DomainEventHandler` — one handler class per domain event type in the Domain Events designer, with a `Handle` method stub discovered by Wolverine naming convention.

## DomainEventService

The generated service bridges the domain layer's `IDomainEventService` abstraction with Wolverine's message bus:

```csharp
public class DomainEventService : IDomainEventService
{
    private readonly IMessageBus _messageBus;

    public DomainEventService(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task Publish(object domainEvent, CancellationToken cancellationToken = default)
    {
        await _messageBus.PublishAsync(domainEvent);
    }
}
```

The service is registered as scoped in the DI container so it shares the same lifetime as the handler and unit-of-work scope.

## DomainEventHandler

One handler class is generated per domain event type. Wolverine discovers each handler by the `Handle` method name — no interface is required:

```csharp
[IntentManaged(Mode.Merge)]
public class OrderPlacedDomainEventHandler
{
    [IntentManaged(Mode.Merge)]
    public async Task Handle(OrderPlacedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Implement your domain event handler here.");
    }
}
```

Add the event-specific logic inside the `Handle` method. The `[IntentManaged(Mode.Merge)]` attribute preserves custom code across Software Factory runs.

## Designer Dependency

This module reads domain event types from the **Modelers.Domain.Events** designer package (`Intent.Modelers.Domain.Events`). It does not depend on `Intent.DomainEvents` to avoid pulling in AutoMapper and other infrastructure that conflicts with Wolverine-only applications.

## Related Modules

- [Intent.Application.Wolverine](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.Application.Wolverine/README.md) — core Wolverine CQRS module; required by this module.
- [Intent.Application.Wolverine.FluentValidation](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.Application.Wolverine.FluentValidation/README.md) — adds FluentValidation validators for commands and queries.
