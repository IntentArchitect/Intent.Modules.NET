### Version 1.0.0

- New Feature: Initial release. Establishes the single, shared `builder.Host.UseWolverine(opts => ...)` registration for an application's ASP.NET host and fixes the order other Wolverine-based modules contribute to it, so installing a second Wolverine module can never silently strand or overwrite the first one's configuration. Contributors call Intent's own `ConfigureHostBuilderChainStatement("UseWolverine", ...)` and are sequenced by factory-extension `Order`; this module also owns the `WolverineFx` package reference and the `using Wolverine` on the host file.
