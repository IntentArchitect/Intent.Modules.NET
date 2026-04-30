---
name: mediatr-domain-event-handler
description: implement or revise jimmy bogard-style mediatr domain event handlers in clean architecture c# solutions. use when a domain event notification handler, outbox/integration-event reaction, projection update, notification side effect, audit side effect, or cross-aggregate follow-up is incomplete or incorrect and chatgpt should update the handler while preserving clean layer boundaries and existing codebase conventions.
contentHash: 9B9516B477FAAED293B0001184BF703F48FC798E06830CA70B780E9E1DAFAAD8
---

# MediatR Domain Event Handler

Implement Jimmy Bogard-style domain event handlers in Clean Architecture. Favor existing patterns, domain language, and allowed abstractions over convenience shortcuts.

## Core rules

- Treat the existing event handler, domain event type, aggregate that raises the event, and nearby feature files as the starting point.
- Do not change the MediatR handler method signature or notification type.
- Keep Application handlers dependent only on Domain/Application abstractions.
- Never inject infrastructure types into Application code: no EF `DbContext`, Dapper, SQL clients, broker SDKs, vendor HTTP clients, provider SDKs, or concrete infrastructure services.
- If the handler needs persistence, messaging, notifications, auditing, scheduling, projection updates, or external calls, use an existing Domain/Application abstraction. If none exists, add or extend a contract in an allowed layer.
- Search first for similar domain event handlers, dispatch wrappers, outbox usage, repositories, application services, idempotency checks, error handling, logging, and result patterns.
- Keep durable business rules in aggregates, value objects, policies, specifications, or domain/application services when those patterns exist. Keep the event handler focused on orchestration and side effects.
- Thread `CancellationToken` through async calls.
- Avoid unrelated refactors.

## Event-handler intent

Assume the common Bogard pattern:

1. A command handler changes aggregate state.
2. The aggregate raises a past-tense domain event.
3. The unit of work or DbContext dispatches the event through MediatR.
4. `INotificationHandler<TEvent>` reacts with follow-up work.

Do not move the original command decision into the event handler. The handler reacts to something that already happened.

## Workflow

1. Inspect the existing handler, request, response, validator, repository interfaces, and related domain types.
2. Search for code usages of:
   - similar domain event handlers
   - repository interfaces and existing repository methods
   - domain operations on the target aggregate or entity
   - result and error patterns used in the solution
3. Infer the intended business flow from the request shape, naming, surrounding domain model, and nearby feature implementations.
4. Implement the `Handle` method using existing patterns first.
5. If the handler needs missing DAL capabilities, extend the relevant repository abstraction in an allowed layer instead of pulling infrastructure into the handler.
6. Add focused private helper methods when the flow becomes easier to read or test.
7. Ensure cancellation tokens are threaded through async calls where applicable.
8. Verify the final code preserves layer boundaries and does not introduce infrastructure leakage.

## Side-effect guidance

- Integration events: use the existing outbox/event-bus abstraction; do not inject broker clients.
- Email/SMS/push/webhooks/HTTP: use an application port such as `IEmailSender`, `INotificationService`, or a domain-specific dispatcher.
- Projections/read models: use a projection repository or read-model writer abstraction.
- Cross-aggregate changes: load the target aggregate through a repository and invoke domain behavior.
- Audit: use the existing audit abstraction and domain language.

## Idempotency and data access

- Check for at-least-once/outbox retry assumptions before adding durable side effects.
- If creating records or messages, look for existing processed-event checks, natural keys, unique constraints, or methods like `AddIfNotExistsAsync` / `EnqueueIfNotExistsAsync`.
- Do not implement durable de-duplication in memory.
- If the handler needs aggregation (`Count`, `Sum`, `Average`, `Min`, `Max`, `First`, `Last`, `GroupBy`, existence checks, summaries), do not load full collections and compute in memory. Call or add a Domain/Application abstraction that returns the shaped result directly.
- Name new repository/service methods around domain intent, such as `HasOpen...Async`, `GetPending...SummaryAsync`, `FindEligible...Async`, `GetForEventHandlingAsync`, or `AddIfNotExistsAsync`.

## Error handling

Follow nearby handlers for stale events or missing data:

- silently return if that is the convention
- throw the established domain/application exception if that is the convention
- log and return if that is the convention

Do not invent a new result type for MediatR notification handlers.

## Output expectations

Produce a concrete code update that completes or corrects the domain event handler, preserves the handler signature, extends allowed-layer abstractions only when needed, keeps infrastructure out of Application handlers, follows local patterns, and avoids unrelated changes.

## Review checklist

- Handler reacts to a past-tense event instead of re-deciding the command.
- Dependencies are Domain/Application abstractions unless local convention places the handler in Infrastructure.
- No infrastructure namespaces leak into Application/Domain code.
- External side effects go through ports, services, repositories, or outbox abstractions.
- Durable side effects are idempotent or match project retry assumptions.
- Repository additions are contract-first and domain-shaped.
- Business language matches the surrounding domain model.
- Async calls pass `CancellationToken`.
