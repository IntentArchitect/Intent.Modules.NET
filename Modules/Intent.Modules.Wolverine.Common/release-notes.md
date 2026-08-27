### Version 1.0.0

- New Feature: Initial release. Owns the single, shared `builder.Host.UseWolverine(opts => ...)` registration for an application's ASP.NET host. Any other Wolverine-based module (e.g. `Intent.Application.Wolverine`, `Intent.Eventing.Wolverine`) contributes to that one registration via `WolverineHostRegistrationExtension.Contribute(...)` instead of registering its own, so installing a second Wolverine module can never silently strand or overwrite the first one's handlers.
