# Intent.Aws.Lambda.Functions.Dispatch.Wolverine

Wires AWS Lambda Annotation Function classes to Wolverine's `IMessageBus`, replacing the default in-process CQRS dispatch with Wolverine's `InvokeAsync<T>` calls inside each generated Lambda handler method.

## What This Module Generates

This module is a factory extension only — it does not add new template files. Instead it modifies the output of the `Intent.Aws.Lambda.Functions` templates:

- `CqrsLambdaFunctionClassTemplateRegistration` — groups Commands and Queries (those with HTTP settings) by parent folder and creates one `CqrsLambdaFunctionContainerModel` per group, driving one Lambda function class per folder.
- `WolverineEndpointExtension` — hooks into each `LambdaFunctionClassTemplate` backed by a `CqrsLambdaFunctionContainerModel`, injects an `IMessageBus _sender` constructor parameter, and emits `_sender.InvokeAsync<T>(command, cancellationToken)` dispatch statements using `CancellationToken.None`.

## CancellationToken Behaviour

AWS Lambda Annotations does not support passing a `CancellationToken` to handler methods (diagnostic AWSLambda0107). The extension therefore sets `var cancellationToken = CancellationToken.None;` inside each handler method and passes that local variable to Wolverine, keeping the generated code warning-free.

## How Dispatch Is Wired

Commands are dispatched with a typed return and the result is returned as an HTTP Created response:

```csharp
public class ProductsFunctions
{
    private readonly ILogger<ProductsFunctions> _logger;
    private readonly IMessageBus _sender;

    public ProductsFunctions(ILogger<ProductsFunctions> logger, IMessageBus sender)
    {
        _logger = logger;
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Post, "/api/products")]
    public async Task<IHttpResult> CreateProductAsync([FromBody] CreateProductCommand command)
    {
        // AWSLambda0107: passing System.Threading.CancellationToken is not supported.
        var cancellationToken = CancellationToken.None;
        return await ExceptionHandlerHelper.ExecuteAsync(async () =>
        {
            var result = await _sender.InvokeAsync<Guid>(command, cancellationToken);
            return HttpResults.Created($"/api/products/{Uri.EscapeDataString(result.ToString())}", result);
        }, _logger);
    }

    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Get, "/api/products/{id}")]
    public async Task<IHttpResult> GetProductByIdAsync(string id)
    {
        var cancellationToken = CancellationToken.None;
        return await ExceptionHandlerHelper.ExecuteAsync(async () =>
        {
            if (!Guid.TryParse(id, out var idGuid))
                return HttpResults.BadRequest($"Invalid format for id: {id}");
            var result = await _sender.InvokeAsync<ProductDto>(new GetProductByIdQuery(idGuid), cancellationToken);
            return result == null ? HttpResults.NotFound() : HttpResults.Ok(result);
        }, _logger);
    }
}
```

Note: `Guid` route parameters are received as `string` and parsed manually inside the handler because the AWS Lambda Annotations framework does not reliably convert route segment strings to `Guid`.

## Startup Wiring

AWS Lambda Annotation Functions use a `[LambdaStartup]` class to configure the host. This module registers Wolverine into that startup by adding `UseWolverine` to `ConfigureHostBuilder`:

```csharp
[LambdaStartup]
public class Startup
{
    public HostApplicationBuilder ConfigureHostBuilder()
    {
        var hostBuilder = new HostApplicationBuilder();
        // ... existing service registrations ...
        hostBuilder.UseWolverine(opts => { WolverineConfiguration.Configure(opts); });
        return hostBuilder;
    }
}
```

`HostApplicationBuilder` (used by Lambda) does not expose a `.Host` property. The `UseWolverine` extension targets `IHostApplicationBuilder` directly and is called on the builder itself.

## Serverless Discovery Configuration

AWS Lambda runs in a serverless environment. Wolverine's default convention-based handler scanning sweeps the bin directory, which loads DLLs that may not be compatible with the Lambda runtime. This module configures `WolverineConfiguration` to be safe for Lambda by disabling the bin sweep and registering handlers explicitly:

```csharp
public static class WolverineConfiguration
{
    public static void Configure(WolverineOptions opts)
    {
        opts.Discovery.DisableConventionalDiscovery();

        opts.Discovery.IncludeType<CreateProductCommandHandler>();
        opts.Discovery.IncludeType<GetProductByIdQueryHandler>();
        // ... one entry per generated handler

        opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;
        opts.Durability.Mode = DurabilityMode.Serverless;

        ApplicationHandlerPolicy.Apply(opts);
    }
}
```

- `DisableConventionalDiscovery()` — turns off the bin-directory sweep.
- `IncludeType<T>()` — registers each command and query handler explicitly.
- `TypeLoadMode.Static` — disables JasperFx dynamic code generation.
- `DurabilityMode.Serverless` — disables Wolverine's inbox/outbox background workers.

## Prerequisites

This module requires the following modules to be installed:

- `Intent.Aws.Lambda.Functions` — provides the `LambdaFunctionClassTemplate` that this extension hooks into.
- `Intent.Application.Wolverine` — registers Wolverine DI services and provides `IMessageBus`.
- `Intent.Modelers.Services.CQRS` — provides the Command/Query model types consumed by the registration class.

## Related Modules

- [Intent.Aws.Lambda.Functions](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.Aws.Lambda.Functions/README.md) — generates the Lambda function classes that this module extends.
- [Intent.Application.Wolverine](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.Application.Wolverine/README.md) — registers Wolverine and generates handler classes that receive the dispatched messages.
- [Intent.FastEndpoints.Dispatch.Wolverine](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.FastEndpoints.Dispatch.Wolverine/README.md) — equivalent dispatch wiring for FastEndpoints.
