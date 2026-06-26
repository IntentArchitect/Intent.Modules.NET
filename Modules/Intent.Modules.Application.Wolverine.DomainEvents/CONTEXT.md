# CONTEXT.md — Intent.Modules.Application.Wolverine.DomainEvents

## Purpose

Generates the `DomainEventService` (dispatches domain events via Wolverine's `IMessageBus`), implicit `{EventName}Handler` stubs for every domain event in the Domain designer, and explicit `DomainEventHandler` classes for handlers modeled in the Services designer.

---

## Key Architectural Decisions

### AutoMapper isolation constraint (CRITICAL — do not relax)

This module must NOT declare `Intent.DomainEvents` as an imodspec `<dependency>`.

`Intent.DomainEvents` (the full IA module) transitively pulls in AutoMapper via its dependency chain. In Wolverine-only applications that have no AutoMapper installed, this causes a `KeyNotFoundException` at SF time — a silent, hard-to-diagnose failure.

The correct dependency is `Intent.Modelers.Domain.Events` (the designer NuGet package only). This provides the domain event designer elements without the mapper chain.

If you are tempted to add `Intent.DomainEvents` as a convenience (e.g. to avoid re-declaring a type), stop and find another path. This constraint exists because of a real runtime failure that was not immediately obvious.

### Dispatch semantics: `PublishAsync`, not `InvokeAsync`

`DomainEventService` dispatches via `IMessageBus.PublishAsync(domainEvent)`.

- `PublishAsync` is Wolverine's fan-out (pub/sub) semantic — all subscribers receive the event.
- `InvokeAsync` is point-to-point (single handler). It is wrong for domain events, which may have zero or many subscribers.

Do not change the dispatch method.

### Two handler generation modes

**Implicit (`DefaultDomainEventHandler` template):** Iterates `Domain.GetDomainEventModels()` and generates `{EventName}Handler` for every domain event that is NOT already covered by an explicit `DomainEventHandlerModel` in the Services designer. This matches MediatR's default auto-generation behaviour. No Services designer modeling required.

**Explicit (`DomainEventHandler` template):** Iterates `Services.GetDomainEventHandlerModels()` — handler elements explicitly modeled in the Services designer. Supports one handler class with multiple `Handle` overloads for different event types.

The implicit handler is suppressed for any event that has an explicit handler, preventing duplicate handler classes.

### Handler discovery via naming convention

Generated handler classes are discovered by Wolverine's naming convention (class ending in `Handler` or `Consumer`, or method named `Handle`). No `[WolverineHandler]` attribute is needed. This is Wolverine's native pattern — match it exactly.

### Handler stubs use `NotImplementedException` and `[IntentManaged(Body = Mode.Ignore)]`

Generated handler stubs start with `throw new NotImplementedException(...)`. The method body is protected by `[IntentManaged(Body = Mode.Ignore)]` so the developer can fill in the implementation without the SF cycle overwriting it on the next run.

Do not change the merge mode for handler method bodies. Changing to `Mode.Fully` would delete developer-written logic on every SF run.

### `DomainEventService` lifetime is `PerServiceCall` (scoped)

Registered as `PerServiceCall` to align with the unit-of-work pattern used by EF Core and other scoped services. A singleton `DomainEventService` would share state across requests, which is incorrect.

---

## Interactions with Other Modules

| Module | Relationship |
|---|---|
| `Intent.Modelers.Domain.Events` | Designer package (NOT `Intent.DomainEvents`). Provides domain event element types without triggering the AutoMapper dependency chain. |
| `Intent.Application.Wolverine` | Sibling module. `DomainEventService` depends on `IMessageBus` which is registered by the core Wolverine module. |

---

## Anti-Patterns

- **Do not add `Intent.DomainEvents` as an imodspec dependency.** It silently pulls in AutoMapper, breaking Wolverine-only apps at SF time.
- **Do not use `InvokeAsync` for domain event dispatch.** Domain events are pub/sub; `PublishAsync` is correct.
- **Do not make handler method bodies `[IntentManaged(Body = Mode.Fully)]`.** Developers write business logic in those bodies; overwriting on SF run destroys their work.
