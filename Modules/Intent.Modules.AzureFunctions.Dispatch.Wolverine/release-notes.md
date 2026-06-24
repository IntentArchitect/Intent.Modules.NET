### Version 1.0.0

- New Feature: Initial release. Dispatches commands and queries from Azure Function HTTP triggers via Wolverine's `IMessageBus.InvokeAsync` instead of MediatR.
- New Feature: Injects `IMessageBus` via the function class constructor and validates that route parameters present in the request match the command or query payload.
- New Feature: Supports both void commands (`InvokeAsync`) and typed query responses (`InvokeAsync<T>`), selecting the correct overload per command/query return type.
- New Feature: Maps HTTP verbs to appropriate response shapes: POST returns `Created`, GET returns `Ok`/`NotFound`, PUT returns `NoContent`, and DELETE returns `Ok`.
- Improvement: Added serverless-safe Wolverine configuration that calls `DisableConventionalDiscovery()`, registers each handler type explicitly via `IncludeType<T>()`, sets `TypeLoadMode.Static`, and sets `DurabilityMode.Serverless` to prevent bin-sweep assembly loading crashes in the Azure Functions isolated worker process.
