# Intent.Application.Wolverine.DomainEvents

Wires domain event dispatching through Wolverine's `IMessageBus`, generating a scoped `DomainEventService` and handler stubs for every domain event type modeled in the Domain designer.

## What This Module Generates

- `DomainEventService` — implements `IDomainEventService`; injects `IMessageBus` and dispatches domain events via `PublishAsync`. Registered as scoped in DI.
- `{EventName}Handler` (implicit) — one handler stub per domain event type in the Domain designer that does not already have an explicit handler. Generated automatically; no Services designer modeling required.
- `DomainEventHandler` (explicit) — handler class for domain events explicitly modeled as `Domain Event Handler` elements in the Services designer. Supports multiple handled events per class.

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

## Implicit Handler Generation

For every domain event modeled in the Domain designer that does not already have an explicit handler, this module automatically generates a `{EventName}Handler` stub. Wolverine discovers it by the `Handle` method name — no interface is required:

```csharp
[IntentManaged(Mode.Merge, Signature = Mode.Fully)]
public class ProductCreatedHandler
{
    [IntentManaged(Mode.Merge)]
    public ProductCreatedHandler() { }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public async Task Handle(ProductCreated domainEvent, CancellationToken cancellationToken)
    {
        // TODO: Implement Handle (ProductCreatedHandler) functionality
        throw new NotImplementedException("Implement your handler logic here...");
    }
}
```

Add your business logic inside the `Handle` method. The `Body = Mode.Merge` attribute preserves your implementation across Software Factory runs.

## Explicit DomainEventHandler

When you need one class to handle multiple domain events, model a `Domain Event Handler` element in the Services designer and associate multiple domain event types to it. The module generates a handler class with one `Handle` overload per associated event:

```csharp
[IntentManaged(Mode.Merge, Signature = Mode.Fully)]
public class OrderDomainEventHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public async Task Handle(OrderPlacedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Implement your handler logic here...");
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public async Task Handle(OrderCancelledDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Implement your handler logic here...");
    }
}
```

Once an explicit handler is modeled for a domain event, the implicit handler for that event is suppressed to avoid duplicates.

## Designer Dependency

This module reads domain event types from the **Modelers.Domain.Events** designer package (`Intent.Modelers.Domain.Events`). It does not depend on `Intent.DomainEvents` to avoid pulling in AutoMapper and other infrastructure that conflicts with Wolverine-only applications.

## Related Modules

- [Intent.Application.Wolverine](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.Application.Wolverine/README.md) — core Wolverine CQRS module; required by this module.
- [Intent.Application.Wolverine.FluentValidation](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.Application.Wolverine.FluentValidation/README.md) — adds FluentValidation validators for commands and queries.
