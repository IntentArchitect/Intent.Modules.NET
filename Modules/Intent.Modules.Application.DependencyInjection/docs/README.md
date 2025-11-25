# Intent.Application.DependencyInjection

Generates Application-layer service registration configuration for ASP.NET Core's built-in dependency injection container. This module creates an `AddApplication` extension method that wires up all application services, making them available for dependency injection.

## What is the Intent.Application.DependencyInjection Module?

This module automatically generates the "glue code" needed to register application-layer services with the DI container. Instead of manually writing service registrations, Intent Architect discovers services from your application layer and generates the appropriate `Add{Layer}` extension methods.

The module works with:
- **Intent.Application.ServiceImplementations** - Registers traditional services and their interfaces
- **Intent.Application.MediatR** - Registers MediatR handlers, behaviors, and pipeline components
- **Intent.Application.AutoMapper** - Registers AutoMapper profiles
- Other application-layer components requiring DI registration

## What This Module Generates

**DependencyInjection.cs** file with extension method:

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // AutoMapper profile registration
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            // Application service registrations
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IProductService, ProductService>();
            
            // MediatR registration (if using Intent.Application.MediatR)
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            
            // Additional application-layer services
            // ... other registrations

            return services;
        }
    }
}
```

### Key Characteristics

**Extension Method Pattern:**
- Generates fluent `AddApplication` extension method on `IServiceCollection`
- Follows ASP.NET Core convention (like `AddControllers`, `AddDbContext`, etc.)
- Returns `IServiceCollection` for method chaining
- Uses `this` modifier for extension method syntax

**Service Lifetime Management:**
- Automatically determines appropriate service lifetime (Transient, Scoped, Singleton)
- Defaults to `Transient` for application services
- Respects explicit lifetime configurations when specified
- Works with dependency injection best practices

## Manual Customization

The generated `DependencyInjection.cs` is managed in **Fully** mode (meaning that Intent Architect will always overwrite developer changes):

```csharp
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddAutoMapper(Assembly.GetExecutingAssembly());
    services.AddTransient<ICustomerService, CustomerService>();
    
    // IntentIgnore
    services.AddTransient<IMyCustomService, MyCustomService>();
    // IntentIgnore    
    services.Configure<MyOptions>(options =>
    {
        options.CustomSetting = "value";
    });
    
    return services;
}
```

Use `IntentIgnore` comments for code statements you want to preserve across Software Factory runs.
Alternatively if you need to add many custom registrations rather introduce a method in the following manner:

1. Setup a method and decorate it with the `[IntentIgnore]` attribute.
2. Insert your registration code into the method.
3. Add a invocation to your method inside the `AddApplication` method and add a `IntentIgnore` comment above it.

```csharp
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddAutoMapper(Assembly.GetExecutingAssembly());
    services.AddTransient<ICustomerService, CustomerService>();
    
    // IntentIgnore
    services.CustomRegistrations();
    
    return services;
}

[IntentIgnore]
private static IServiceCollection CustomRegistrations(this IServiceCollection services)
{
    services.AddTransient<IMyCustomService, MyCustomService>();
    services.Configure<MyOptions>(options =>
    {
        options.CustomSetting = "value";
    });
}
```

## External Resources

- [Dependency Injection in .NET](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)
- [Service Lifetimes](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection#service-lifetimes)
- [Extension Methods (C#)](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/extension-methods)
- [ASP.NET Core Dependency Injection](https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection)
