# Intent.Application.DependencyInjection.MediatR

MediatR is an open-source library for .NET applications that implements the Mediator pattern. It enables components in an application to communicate indirectly by sending messages (requests and notifications) to a central mediator. The mediator then dispatches these messages to their corresponding handlers, promoting loose coupling and separation of concerns. MediatR simplifies the implementation of the Mediator pattern in C# projects, leading to better code organization, testability, and maintenance in complex applications. It is a valuable tool for managing communication flow between components in modern .NET development.

This module handles dependency injection concerns around using MediatR.

This could be visualized in code as follows, which could be found in your dependency injection registrations:

```csharp
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
    cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
    cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
});
```

Any modules wanting to register up their own `PipelineBehaviour`s can do so using the `ContainerRegistrationRequest` event.

```csharp
ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister($"typeof({ClassName}<,>)")
                .ForInterface("typeof(IPipelineBehavior<,>)")
                .WithPriority(4)
                .ForConcern("MediatR")
                .RequiresUsingNamespaces("MediatR")
                .HasDependency(this));
```

For more information on MediatR, check out their [official GitHub](https://github.com/jbogard/MediatR/).
