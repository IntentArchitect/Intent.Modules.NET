### Version 1.0.1

- Improvement: Controllers no longer get a redundant `using Wolverine;` when their namespace already sits under `Wolverine`.
- Improvement: Requires Intent Architect 5.1.0 or later.

### Version 1.0.0

- New Feature: Initial release. Dispatches commands and queries from ASP.NET Core controllers via Wolverine's `IMessageBus.InvokeAsync` instead of MediatR.
- New Feature: Injects `IMessageBus` via controller constructor rather than action-method service injection, matching Wolverine's recommended usage pattern.
- New Feature: Supports both void commands (`InvokeAsync`) and typed query responses (`InvokeAsync<T>`), selecting the correct overload per command/query return type.
- New Feature: Generates constructor-based command/query instantiation when the model has a parameterized constructor, falling back to object initializer syntax otherwise.
