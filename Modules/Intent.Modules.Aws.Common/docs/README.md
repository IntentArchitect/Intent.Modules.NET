# Intent.Aws.Common

This module provides base functionality for all Intent Architect [AWS](https://aws.amazon.com/) Modules.

In particular, it generates the following for dependency injection container registrations:

```csharp
public static class AwsConfiguration
{
    public static IServiceCollection ConfigureAws(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());

        return services;
    }
}
```

Other Modules then add services to this as required.
