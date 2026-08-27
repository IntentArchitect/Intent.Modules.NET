### Version 1.1.0

- Fixed `Intent.Wolverine.Common` not being declared as an imodspec dependency, so installing this module now automatically brings it in instead of requiring a separate manual install.
- New Feature: `WolverineConfiguration.Configure` now emits an explicit `opts.Discovery.IncludeType<THandler>()` for every CQRS command and query handler this module generates, so each handler registration is attributable to the module that owns the handler rather than riding in on a blanket assembly scan. The existing `opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly)` is retained deliberately: Wolverine's conventional discovery only scans the entry assembly, so that line is the only thing bringing the Application layer into discovery scope, and sibling modules that generate convention-named handlers there without registering their own types (notably `Intent.Application.Wolverine.DomainEvents`) still depend on it. Registering the same handler type by both routes is idempotent — verified against WolverineFx 5.39.5 that the handler chain holds exactly one call for that type/method.
- Breaking Change: `WolverineRegistrationFactoryExtension` no longer registers `builder.Host.UseWolverine(...)` directly. It now depends on the new `Intent.Wolverine.Common` module and contributes its CQRS configuration statement to that module's single, shared registration instead. This removes the `lambdaBlock.Statements.Clear()` call that could silently discard another Wolverine module's contribution to the same lambda depending on factory-extension execution order.
- Removed: the Azure Functions host registration. Wolverine's host registration is now ASP.NET-host-only; it never worked correctly under any `TypeLoadMode` in the Azure Functions isolated worker model, so nothing that previously ran successfully is lost. An application running `Intent.Application.Wolverine` on an Azure Functions host should not upgrade past this version without reviewing that impact.

### Version 1.0.3

- New Feature: Added `MessageBusFlushMiddleware`, which flushes the `Intent.Eventing.*` message bus after a command/query handler succeeds (mirroring MediatR's `MessageBusPublishBehaviour`), so integration events published during a handler are no longer silently dropped when an eventing module is installed alongside Wolverine. Ordered to run after `UnitOfWorkMiddleware` so the flush happens post-commit.

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
