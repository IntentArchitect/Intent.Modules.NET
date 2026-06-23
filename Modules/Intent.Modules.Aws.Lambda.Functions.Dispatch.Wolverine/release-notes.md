### Version 1.0.0

- New Feature: Adds Wolverine dispatch support for AWS Lambda Annotation Functions by injecting `IMessageBus` and routing Commands and Queries via `InvokeAsync<T>` with `CancellationToken.None` per AWSLambda0107.
