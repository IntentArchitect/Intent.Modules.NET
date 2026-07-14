### Version 1.0.0

- Fixed: Corrected the declared `Intent.Common`/`Intent.Common.CSharp` module dependency versions, and the stale `Intent.Application.Wolverine.FluentValidation`/`Intent.Application.Wolverine.DomainEvents` interoperability install versions, to match what this module actually requires.
- Improvement: Updated the `wolverine-command-handler` and `wolverine-query-handler` AI agent skill descriptions to match the equivalent MediatR skill wording.
- Improvement: Renamed the `Handle` method's command/query parameter from `command`/`query` to `request` on generated command and query handlers, for a uniform parameter name across both handler kinds.
- Improvement: Updated NuGet package versions.
- Fixed: Command and query model properties now expose `"model"` metadata so that controller dispatch extensions can generate the route-parameter-to-field assignment (e.g. `if (command.Id == Guid.Empty) { command.Id = id; }`) before the identity-check guard, fixing PUT requests that always returned `BadRequest` when the `Id` was in the route but not the body.
- New Feature: Initial release.
- New Feature: Generates Wolverine CQRS command and query handlers using convention-based discovery (no `IWolverineHandler` attribute required).
- New Feature: Generates `ICommand` and `IQuery` marker interfaces in the Application layer with zero Wolverine dependency contamination.
- New Feature: Generates middleware behaviours: Authorization, Validation, Unit of Work, Exception Handling, Logging, and Performance.
- New Feature: Command and query models use constructor-based initialization matching the Wolverine handler method parameter name convention.
