### Version 1.0.3

- New Feature: Added `MessageBusFlushMiddleware`, which flushes the `Intent.Eventing.*` message bus after a command/query handler succeeds (mirroring MediatR's `MessageBusPublishBehaviour`), so integration events published during a handler are no longer silently dropped when an eventing module is installed alongside Wolverine. Ordered to run after `UnitOfWorkMiddleware` so the flush happens post-commit.
- Improvement: Reworded the `wolverine-command-handler` and `wolverine-query-handler` AI skill descriptions to an imperative "MUST load before..." trigger naming the handler file the agent is about to edit, so coding agents reliably load them instead of skipping them.

### Version 1.0.2

- Fixed: `WolverineRegistrationFactoryExtension` threw `More than one instance of template App.Program was found` in applications with more than one ASP.NET Core host; Wolverine is now registered on every host's `Program` file.
- Improvement: Updated generated code so warnings no longer generated.

### Version 1.0.1

- Improvement: Additional context for the AI around how to implement the handler for a Wolverine handler.

### Version 1.0.0

- New Feature: Initial release of the Wolverine application module.

> ⚠️ NOTE
> 
> When migrating from MediatR's patterns to Wolverine, any updates you made to the Pipeline Behaviours will need to be manually ported to the Middleware pipeline in Wolverine or it will be lost.
