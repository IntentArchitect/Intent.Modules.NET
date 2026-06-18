# WORKING.md — feature/wolverine

## SF Cycle Status

All SF cycles are closed. The target solution builds with 0 errors and SF produces 0 staged changes.

---

## Next planned work

See `plans/wolverine-roadmap/plan.mdx` for the full roadmap. Priority order:

1. **Dandré creates three module shells** (Azure Functions, Fast Endpoints, AWS Lambda dispatch) — prerequisite for all dispatch work
2. **`Intent.Application.Wolverine.FluentValidation`** — CommandValidatorTemplate + QueryValidatorTemplate filling `TemplateRoles.Application.Validation.Command/.Query`
3. **`Intent.Application.Wolverine.DomainEvents`** — DomainEventService template using `IMessageBus.PublishAsync(domainEvent)` directly; optional handler stub factory extension
4. **Outbox integration** — factory extensions in MassTransit and NServiceBus modules to swap `UnitOfWorkMiddleware` for an `IDbContextTransaction`-based variant when EF/SQL outbox is active (depends on items 1–3 being stable)
