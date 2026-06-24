### Version 1.0.0

- New Feature: Adds Wolverine dispatch support for AWS Lambda Annotation Functions by injecting `IMessageBus` and routing Commands and Queries via `InvokeAsync<T>` with `CancellationToken.None` per AWSLambda0107.
- Improvement: Added `RegisterWolverineOnLambdaStartup` that wires `hostBuilder.UseWolverine(...)` into the generated `Startup.ConfigureHostBuilder()`, completing the Wolverine host registration for the isolated Lambda worker.
- Improvement: Added serverless-safe Wolverine configuration that calls `DisableConventionalDiscovery()`, registers each handler type explicitly via `IncludeType<T>()`, sets `TypeLoadMode.Static`, and sets `DurabilityMode.Serverless`.
