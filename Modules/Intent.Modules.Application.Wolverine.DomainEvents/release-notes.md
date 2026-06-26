### Version 1.0.0

- New Feature: Initial release.
- New Feature: Generates `DomainEventService` implementing `IDomainEventService`, dispatching domain events via Wolverine's `IMessageBus.PublishAsync`. Registered as a scoped service in DI.
- New Feature: Generates an implicit `{EventName}Handler` stub for every domain event in the Domain designer that does not have an explicit handler, matching MediatR's auto-generation behaviour.
- New Feature: Generates explicit `DomainEventHandler` classes for domain events modeled as `Domain Event Handler` elements in the Services designer, supporting multiple handled events per class.
