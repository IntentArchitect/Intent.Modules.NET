# Intent.Application.Wolverine

Configures and wires Wolverine as the CQRS dispatcher for an Intent-managed .NET application, generating convention-based command and query handlers, marker interfaces, and a suite of middleware behaviours.

## What This Module Generates

- `ICommand` — marker interface in the Application layer; carries no Wolverine dependency.
- `IQuery<TResult>` — marker interface in the Application layer; carries no Wolverine dependency.
- `CommandModel` — one DTO class per Command element, with constructor-based initialization.
- `QueryModel` — one DTO class per Query element, with constructor-based initialization.
- `CommandHandler` — one handler class per Command, discovered by Wolverine's `Handle` method naming convention.
- `QueryHandler` — one handler class per Query, discovered by Wolverine's `Handle` method naming convention.
- `WolverineConfiguration` — configures `builder.Host.UseWolverine(...)`, sets up assembly discovery, and applies the `ApplicationHandlerPolicy`.
- `ApplicationHandlerPolicy` — `IHandlerPolicy` implementation that registers middleware and adds each middleware type as a transient service.
- `AuthorizationMiddleware` — uses an `Envelope` parameter to avoid broad convention conflicts.
- `ValidationMiddleware` — validates the incoming command or query before the handler runs.
- `UnitOfWorkMiddleware` — wraps handler execution in a unit-of-work scope.
- `LoggingMiddleware` — logs entry and exit of each handler invocation.
- `PerformanceMiddleware` — records handler execution duration.
- `UnhandledExceptionMiddleware` — catches and logs unhandled exceptions from handlers.

## Convention-Based Handler Discovery

Wolverine discovers handlers by method name (`Handle`) rather than by interface. This module generates handlers and marker interfaces that follow this convention, keeping the Application layer free of any direct Wolverine package reference.

```csharp
// ICommand marker — no Wolverine reference in Application layer
public interface ICommand { }

// Generated command model
public class CreateOrderCommand : ICommand
{
    public CreateOrderCommand(Guid customerId, IReadOnlyList<OrderItemDto> items)
    {
        CustomerId = customerId;
        Items = items;
    }

    public Guid CustomerId { get; }
    public IReadOnlyList<OrderItemDto> Items { get; }
}

// Generated handler — discovered by Wolverine via the Handle method name
public class CreateOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        // handler implementation
    }
}
```

## Wolverine Configuration

The generated `WolverineConfiguration` registers Wolverine on the host and applies assembly-level handler discovery:

```csharp
public static class WolverineConfiguration
{
    public static IHostBuilder UseWolverine(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseWolverine(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(ICommand).Assembly);
            opts.Policies.Add<ApplicationHandlerPolicy>();
        });
    }
}
```

## Middleware Pipeline

The generated `ApplicationHandlerPolicy` applies middleware in a fixed order using Wolverine's `IHandlerPolicy` mechanism:

```csharp
public class ApplicationHandlerPolicy : IHandlerPolicy
{
    public void Apply(IReadOnlyList<HandlerChain> chains, GenerationRules rules, IContainer container)
    {
        foreach (var chain in chains)
        {
            chain.Middleware.Add(new MiddlewareFrame(typeof(LoggingMiddleware)));
            chain.Middleware.Add(new MiddlewareFrame(typeof(PerformanceMiddleware)));
            chain.Middleware.Add(new MiddlewareFrame(typeof(UnhandledExceptionMiddleware)));
            chain.Middleware.Add(new MiddlewareFrame(typeof(AuthorizationMiddleware)));
            chain.Middleware.Add(new MiddlewareFrame(typeof(ValidationMiddleware)));
            chain.Middleware.Add(new MiddlewareFrame(typeof(UnitOfWorkMiddleware)));
        }
    }
}
```

## NuGet Dependencies

This module adds a reference to `WolverineFx` (version 5.39.5+) which supports .NET 8, 9, and 10.

## Migrating from MediatR to Wolverine

If your application currently uses MediatR, you can switch it to Wolverine. This is supported for **ASP.NET Core applications**, where the migration is straightforward.

There are more MediatR modules than Wolverine modules, but this does **not** mean Wolverine lacks those capabilities — much of what MediatR splits across separate modules (dispatchers, CRUD, behaviours) is handled by the Wolverine core module or wired up automatically on install.

### 1. Uninstall every MediatR module

Remove all installed MediatR modules from the application, including the dispatchers, behaviours, domain events, dependency injection, CRUD, and FluentValidation:

- `Intent.Application.MediatR`
- `Intent.Application.DependencyInjection.MediatR`
- `Intent.Application.MediatR.Behaviours`
- `Intent.Application.MediatR.CRUD`
- `Intent.Application.MediatR.FluentValidation`
- `Intent.MediatR.DomainEvents`
- `Intent.AspNetCore.Controllers.Dispatch.MediatR`
- `Intent.FastEndpoints.Dispatch.MediatR` *(if installed)*
- Any other `*.Dispatch.MediatR` module the application has installed

### 2. Install the Wolverine core module

Install `Intent.Application.Wolverine`. Once the MediatR modules have been uninstalled, installing the Wolverine core module automatically pulls in the relevant companion modules for you — the domain events, FluentValidation, and dispatch modules are added where relevant, based on what the application uses.

The technology-agnostic `Intent.Application.CQRS.CRUD` module is shared by both stacks, so leave it installed.

Once the MediatR modules are uninstalled and `Intent.Application.Wolverine` is installed, run the Software Factory to regenerate the application against Wolverine.

> ⚠️ **Warning:** If you added any custom logic to the MediatR behaviours, it will **not** carry across the migration and will be lost. Migrate that logic manually into the corresponding Wolverine middleware (see the [Middleware Pipeline](#middleware-pipeline) section above) before removing the MediatR modules.

Aside from the custom behaviour logic noted above, the migration is a safe drop-in replacement — everything else is handled automatically.

## Related Modules

- [Intent.Application.Wolverine.FluentValidation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-wolverine-fluentvalidation/intent-application-wolverine-fluentvalidation.html) — adds FluentValidation validators for commands and queries.
- [Intent.Application.Wolverine.DomainEvents](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-wolverine-domainevents/intent-application-wolverine-domainevents.html) — dispatches domain events through Wolverine's `IMessageBus`.
- [Intent.AspNetCore.Controllers.Dispatch.Wolverine](https://docs.intentarchitect.com/articles/modules-dotnet/intent-aspnetcore-controllers-dispatch-wolverine/intent-aspnetcore-controllers-dispatch-wolverine.html) — dispatches commands and queries from ASP.NET Core controllers via Wolverine.
- [Intent.FastEndpoints.Dispatch.Wolverine](https://docs.intentarchitect.com/articles/modules-dotnet/intent-fastendpoints-dispatch-wolverine/intent-fastendpoints-dispatch-wolverine.html) — dispatches commands and queries from FastEndpoints via Wolverine.
