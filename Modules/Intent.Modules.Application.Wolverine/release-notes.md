### Version 1.0.0-pre.0

- Initial release.
- Generates Wolverine CQRS command and query handlers using convention-based discovery (no `IWolverineHandler` attribute required).
- Generates `ICommand` and `IQuery` marker interfaces in the Application layer with zero Wolverine dependency contamination.
- Generates middleware behaviours: Authorization, Validation, Unit of Work, Exception Handling, Logging, and Performance.
- Command and query models use constructor-based initialization matching the Wolverine handler method parameter name convention.
