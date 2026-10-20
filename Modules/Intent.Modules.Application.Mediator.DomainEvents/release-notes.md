### Version 1.0.0

- New Feature: domain events are dispatched through Mediator. `DomainEventService` publishes each event as a notification, and a typed notification and a notification factory wrap every modelled domain event. Handlers run one at a time through the core module's `SequentialNotificationPublisher`, which stops at the first failure.
- New Feature: each Domain Event Handler in the Domain Events designer generates an `INotificationHandler` stub whose body is written by an AI agent. The `Create implicit domain event handlers` setting (off by default) generates a default handler for every domain event that has no modelled handler.
- New Feature: a designer rule on the Domain Event Handler element and a generated handler skill tell an agent to revisit the handler when its associations change.
- Improvement: the module has its own icon, a teal hub-and-spoke badge shared by every Mediator module and distinct from MediatR's.
