### Version 1.0.0

- New Feature: Initial release.
- New Feature: Generates Wolverine CQRS command and query handlers using convention-based discovery (no `IWolverineHandler` attribute required).
- New Feature: Generates `ICommand` and `IQuery` marker interfaces in the Application layer with zero Wolverine dependency contamination.
- New Feature: Generates middleware behaviours: Authorization, Validation, Unit of Work, Exception Handling, Logging, and Performance.
- New Feature: Command and query models use constructor-based initialization matching the Wolverine handler method parameter name convention.
