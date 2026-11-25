# Intent.MediatR.DomainEvents

This module dispatches an application's domain events through MediatR and generates the handlers that react to them.

## What it generates

- **`DomainEventNotification<TDomainEvent>`**, a MediatR `INotification` that wraps a domain event, so that a domain event can be published without the Domain layer depending on MediatR.
- **`DomainEventService`**, the implementation of `IDomainEventService` that publishes each domain event as a `DomainEventNotification<T>`.
- **Domain event handlers**, one class for each Domain Event Handler modelled in the Services designer, with a `Handle` method for each domain event it handles. Interactions modelled on a handler (calling a command, or creating or updating an entity) are implemented from the model. A handler with nothing modelled is a stub for you to write; mark its `Handle` method `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` when you write its body.
- **Default domain event handlers**, one for each domain event that no modelled handler handles, when **Create implicit Domain Event Handlers** is on.
- **`{Entity}Manager` classes**, one for each domain entity associated with domain events, implementing `INotificationHandler` for each of those events.
- **`mediatr-domain-event-handler` AI skill**, which tells a coding agent how to write a domain event handler body.

## Settings

| Setting | Effect |
|---|---|
| Domain Event Settings → Create implicit Domain Event Handlers | Generates a default handler for each domain event with no modelled handler. Turn it off to generate only the handlers you model. |

## Related modules

- `Intent.DomainEvents` provides the domain event base types and the `IDomainEventService` interface this module implements.
- `Intent.Application.MediatR` provides the MediatR pipeline that commands sent from a handler run through.
