# Intent.Dapr.AspNetCore.Configuration

This module implements patterns for working with Dapr (Distributed Application Runtime) Configuration component.

## What's is Dapr Configuration component?

The Dapr Configuration component is a fundamental building block of the Dapr framework, designed to manage the configuration settings of microservices-based applications. It enables developers to externalize configuration parameters from their code, storing them in various configuration stores such as file-based or key-value stores. By utilizing the Configuration component, application settings like API keys, connection strings, and timeouts are dynamically retrieved at runtime through Dapr APIs, fostering a clear separation between code and configuration. This approach streamlines the management of configuration changes, allowing for easier updates, scalability, and maintenance of distributed applications.

For more information on Dapr Configuration component, check out their [official site](https://docs.dapr.io/developing-applications/building-blocks/configuration/).

## What's in this module?

This module loads the configured Dapr Configurations, and loads them into the application's `IConfiguration` for consumption in the application. Dapr Configuration stores can be configured in as per the Dapr documentation.
Out the box this module will generate a local Configuration store for ease of usage in the development environment. 

## Module Limitations

Because this module is designed to work with `Man.Dapr.Sidekick.AspNetCore`, there are a few limitations

- The fully configured `IConfiguration` is only available after `ConfigureServices` (Startup.cs), as this is when SideKick starts up the Sidecar, which is needed to fetch the secrets.

## appsettings.json configuration

In the `Dapr.Configuration` section you can configure the following settings

- **StoreName**, which secrets store to connect to. 
- **Keys**, comma separated list of which configuration keys you would like to load.

```json

  "Dapr.Secrets": {
    "StoreName": "configuration-store",
    "Keys": "ConnectionStrings:DefaultConnection"
  }
```

