### Version 1.0.0

- New Feature: Initial release.
- New Feature: Generates `DomainEventService` implementing `IDomainEventService`, dispatching domain events via Wolverine's `IMessageBus.PublishAsync`. Registered as a scoped service in DI.
- New Feature: Generates one `DomainEventHandler` stub per domain event type modeled in the Domain Events designer, discovered by Wolverine naming convention without requiring an interface.
