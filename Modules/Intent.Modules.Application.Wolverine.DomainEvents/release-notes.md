### Version 1.0.0

- Fixed: Corrected the declared `Intent.Common`/`Intent.Common.CSharp` module dependency versions to match what this module actually requires.
- New Feature: Initial release.
- New Feature: Generates `DomainEventService` implementing `IDomainEventService`, dispatching domain events via Wolverine's `IMessageBus.PublishAsync`. Registered as a scoped service in DI.
- New Feature: Generates an implicit `{EventName}Handler` stub for every domain event in the Domain designer that does not have an explicit handler, matching MediatR's auto-generation behaviour.
- New Feature: Generates explicit `DomainEventHandler` classes for domain events modeled as `Domain Event Handler` elements in the Services designer, supporting multiple handled events per class.
- New Feature: Generates an AI agent skill file (`.agents/skills/wolverine-domain-event-handler/SKILL.md`) describing how to implement or revise Wolverine domain event handlers, matching MediatR's equivalent skill template.
