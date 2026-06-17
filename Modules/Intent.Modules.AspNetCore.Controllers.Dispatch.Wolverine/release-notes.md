### Version 1.0.0-pre.0

- Initial release.
- Dispatches commands and queries from ASP.NET Core controllers via Wolverine's `IMessageBus.InvokeAsync` instead of MediatR.
- Injects `IMessageBus` via controller constructor rather than action-method service injection.
- Supports both void commands (`InvokeAsync`) and typed query responses (`InvokeAsync<T>`).
- Generates constructor-based command/query instantiation when the model has a parameterized constructor, falling back to object initializer syntax otherwise.
