# Intent.FastEndpoints.Dispatch.Wolverine

Wires FastEndpoints-generated endpoints to Wolverine's `IMessageBus`, replacing the default in-process CQRS dispatch with Wolverine's `InvokeAsync<T>` calls.

## What This Module Generates

This module is a factory extension only — it does not add new template files. Instead it modifies the output of the `Intent.FastEndpoints` templates:

- `EndpointTemplateRegistration` — groups Commands and Queries (those with HTTP settings) by parent folder and creates one `WolverineEndpointModel` per CQRS operation, replacing the default endpoint model.
- `WolverineEndpointExtension` — hooks into each `EndpointTemplate` backed by a `WolverineEndpointModel`, injects an `IMessageBus _sender` constructor parameter, and emits the appropriate `_sender.InvokeAsync<T>(req, ct)` dispatch statement inside `HandleAsync`.

## How Dispatch Is Wired

For Commands that return a value, the extension emits a typed invocation and passes the result to a `Send.CreatedAtAsync` response:

```csharp
public class CreateProductCommandEndpoint : Endpoint<CreateProductCommand, Guid>
{
    private readonly IMessageBus _sender;

    public CreateProductCommandEndpoint(IMessageBus sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public override void Configure()
    {
        Post("/api/products");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateProductCommand req, CancellationToken ct)
    {
        var result = default(Guid);
        result = await _sender.InvokeAsync<Guid>(req, ct);
        await Send.CreatedAtAsync<GetProductByIdQueryEndpoint>(new { id = result }, result, cancellation: ct);
    }
}
```

For Queries that take no route/query parameters, the extension instantiates the request object inline:

```csharp
result = await _sender.InvokeAsync<List<ProductDto>>(new GetProductsQuery(), ct);
```

## Prerequisites

This module requires the following modules to be installed:

- `Intent.FastEndpoints` — provides the `EndpointTemplate` this extension hooks into.
- `Intent.Application.Wolverine` — registers Wolverine DI services and provides `IMessageBus`.
- `Intent.Modelers.Services.CQRS` — provides the Command/Query model types consumed by the registration class.

## Related Modules

- [Intent.FastEndpoints](https://docs.intentarchitect.com/articles/modules-dotnet/intent-fastendpoints/intent-fastendpoints.html) — generates the FastEndpoints endpoint classes that this module extends.
- [Intent.Application.Wolverine](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-wolverine/intent-application-wolverine.html) — registers Wolverine and generates handler classes that receive the dispatched messages.
- [Intent.AspNetCore.Controllers.Dispatch.Wolverine](https://docs.intentarchitect.com/articles/modules-dotnet/intent-aspnetcore-controllers-dispatch-wolverine/intent-aspnetcore-controllers-dispatch-wolverine.html) — equivalent dispatch wiring for ASP.NET Core controllers.
