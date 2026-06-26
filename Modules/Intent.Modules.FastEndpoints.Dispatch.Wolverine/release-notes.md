### Version 1.0.0

- New Feature: Adds Wolverine dispatch support for FastEndpoints by injecting `IMessageBus` into each generated endpoint and routing Commands and Queries via `InvokeAsync<T>`.
