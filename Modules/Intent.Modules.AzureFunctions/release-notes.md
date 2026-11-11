### Version 5.1.0

- Improvement: Updated NuGet package versions:
  - `AutoMapper` 16.2.0 → 16.3.0
  - `Azure.Messaging.EventHubs` 5.9.0 → 5.12.2
  - `MediatR` 14.2.0 → 14.3.0 (net10.0, net9.0, net8.0, netstandard2.0); 12.1.1 → 12.5.0 (net6.0)
  - `Microsoft.ApplicationInsights.WorkerService` 3.1.2 → 3.2.0
  - `Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore` 2.1.0 → 2.1.1
  - `Microsoft.Azure.Functions.Worker.Extensions.Storage.Queues` 5.5.4 → 5.5.5
  - `Microsoft.Azure.Functions.Worker.Sdk` 2.0.7 → 2.1.0
  - `Microsoft.Azure.WebJobs.Extensions.CosmosDB` 4.3.0 → 4.16.1
  - `Microsoft.Azure.WebJobs.Extensions.EventHubs` 5.3.0 → 5.5.0
  - `Microsoft.Azure.WebJobs.Extensions.RabbitMQ` 2.0.3 → 2.1.0
  - `Microsoft.Azure.WebJobs.Extensions.ServiceBus` 5.5.1 → 5.18.0
  - `Microsoft.Azure.WebJobs.Extensions.Storage.Queues` 5.0.0 → 5.3.10
  - `Microsoft.EntityFrameworkCore` 10.0.9 → 10.0.15 (net10.0); 9.0.17 → 9.0.23 (net8.0); 6.0.32 → 6.0.36 (net6.0)
  - `Microsoft.EntityFrameworkCore.Cosmos` 10.0.9 → 10.0.15 (net10.0); 9.0.17 → 9.0.23 (net8.0); 6.0.32 → 6.0.36 (net6.0)
  - `Microsoft.EntityFrameworkCore.Design` 10.0.9 → 10.0.15 (net10.0); 9.0.17 → 9.0.23 (net8.0); 6.0.32 → 6.0.36 (net6.0)
  - `Microsoft.EntityFrameworkCore.InMemory` 10.0.9 → 10.0.15 (net10.0); 9.0.17 → 9.0.23 (net8.0); 6.0.32 → 6.0.36 (net6.0)
  - `Microsoft.EntityFrameworkCore.Proxies` 10.0.9 → 10.0.15 (net10.0); 9.0.17 → 9.0.23 (net8.0); 6.0.32 → 6.0.36 (net6.0)
  - `Microsoft.EntityFrameworkCore.SqlServer` 10.0.9 → 10.0.15 (net10.0); 9.0.17 → 9.0.23 (net8.0); 6.0.32 → 6.0.36 (net6.0)
  - `Microsoft.EntityFrameworkCore.Tools` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0); 6.0.32 → 6.0.36 (net6.0)
  - `Microsoft.Extensions.Configuration.Abstractions` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.0); 6.0.0 → 6.0.1 (net6.0)
  - `Microsoft.Extensions.Configuration.Binder` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.0); 6.0.0 → 6.0.1 (net6.0)
  - `Microsoft.Extensions.Configuration.EnvironmentVariables` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.0); 6.0.1 → 6.0.2 (net6.0)
  - `Microsoft.Extensions.Configuration.FileExtensions` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.0); 6.0.0 → 6.0.1 (net6.0)
  - `Microsoft.Extensions.Configuration.Json` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.1); 6.0.0 → 6.0.1 (net6.0)
  - `Microsoft.Extensions.Configuration.UserSecrets` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.0); 6.0.1 → 6.0.2 (net6.0)
  - `Microsoft.Extensions.DependencyInjection` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.1); 6.0.1 → 6.0.2 (net6.0)
  - `Microsoft.Extensions.Http` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.0); 6.0.0 → 6.0.1 (net6.0)
  - `Microsoft.Extensions.Logging` 10.0.9 → 11.0.0 (net10.0); 10.0.9 → 10.0.15 (net9.0, net8.0, netstandard2.1); 6.0.0 → 6.0.1 (net6.0)
  - `Microsoft.NET.Sdk.Functions` 4.2.0 → 4.6.0
  - `RabbitMQ.Client` 7.2.1 → 7.2.2

> ⚠️ **BREAKING CHANGES**
>
> This module update may change behaviour or cause compilation breaks in custom code you have written that relies on Application Insights 3.2, which reduces the default HTTP client metrics. For details see the [Application Insights release notes](https://github.com/microsoft/ApplicationInsights-dotnet/releases).

### Version 5.0.27

- Improvement: Service Bus triggered Azure Functions can now consume from a topic subscription via the new optional `Subscription Name` property on the `Azure Function` stereotype.
- Improvement: New opt-in `Trigger Names From App Settings` setting emits trigger queue, topic, subscription, event hub, Cosmos DB database/container and timer schedule values as app setting references, seeded into `local.settings.json`.
- Improvement: AI Context added to the Services designer describing how to apply the `Azure Function` stereotype (per-trigger properties, payload rules, HTTP route prefix and app-setting behaviour).

### Version 5.0.26

- Improvement: Upgraded to properly leverage the newer `EmitOrPublish` API so as to be able support advanced Codebase Structure scenarios.

### Version 5.0.25

- Improvement: Updated NuGet package versions.

### Version 5.0.24

- Improvement: Updated script to cater for possible null stereotype property value

### Version 5.0.23

- Improvement: Updated NuGet package versions.

### Version 5.0.22

- Improvement: Updated NuGet package versions.

### Version 5.0.21

- Improvement: Updated NuGet package versions.
- Fixed: Program config would under some scenarios duplicate `config.AddEnvironmentVariables();`.

### Version 5.0.20

- Improvement: Updated module documentation to use centralized documentation site.

### Version 5.0.19

- Fixed: Introducing ConfigureAppConfiguration will ensure the AddEnvironmentVariables is still invoked.

### Version 5.0.18

- Improvement: Updated NuGet package versions.

### Version 5.0.17

- Improvement: Updated NuGet package versions.
- Improvement: Isolated Processes program file template upgraded for improved extensibility from other modules.
- Fixed: Script crash when swapping Trigger types for endpoint which has association children.

### Version 5.0.16

- Improvement: Updated NuGet package versions.
- Improvement: Upgrades the Program File template to be more in-line with the ASP.NET core template .
- Improvement: Host Builder registrations catered for by listening to `ApplicationBuilderRegistrationRequest` when in Isolated Processes.

### Version 5.0.15

- Improvement: `Queue Name` is no longer a required property for `ServiceBus` and they work conventionally.

### Version 5.0.14

- Improvement: Updated NuGet package versions.
- Fixed: Corrected Exception Logging.

### Version 5.0.13

- Fixed: Nuget package management update.

### Version 5.0.12

- Improvement: Added a setting to enable global exception handling.

### Version 5.0.11

- Improvement: Updated NuGet package versions.

### Version 5.0.10

- Improvement: Locked MediatR NuGet package version
- Improvement: Updated NuGet package versions.
- Improvement: Methods with no auto-implementation will now generate with an `IntentManaged` Body mode of Merge and their `throw new NotImplemented()` statements have been commented with [IntentInitialGen to prevent them from returning when deleted](https://docs.intentarchitect.com/articles/application-development/code-management/code-management-csharp/code-management-csharp.html#the--intentinitialgen-instruction). (Note: A side effect of this improvement is that entries may appear in the Customizations tab of the Software Factory or existing entries may no longer be approved and they will need to reviewed).

### Version 5.0.9

- Improvement: Updated NuGet package versions.

### Version 5.0.8

- Improvement: Updated NuGet package versions.

### Version 5.0.7

- Improvement: Added stereotype descriptions in preparation for Intent Architect 4.5.
- Improvement: Updated NuGet package versions.

### Version 5.0.6

- Improvement: Updated NuGet package versions.

### Version 5.0.5

- Improvement: Updated NuGet package versions.

### Version 5.0.4

- Improvement: Updated NuGet package versions.
- Improvement: HTTP Trigger Azure Functions will defer to using `AzureFunctionHelper.DeserializeJsonContentAsync` to centralize the setup and configuration of `System.Text.Json`. This will simplify the use of swapping out Serializers if necessary.
- Improvement: `ConfigureFunctionsWebApplication` has been expanded to make it easier for developers to add middleware components with `//IntentIgnore` indicators.

### Version 5.0.3

- Improvement: Added "catch JsonException" in controllers to better report malformed messages.
- Fixed: Moving `Isolated Process Program Template` into a folder caused it to break.
- Fixed: For Isolated Processes, Application Insights logging on "Information" log level doesn't take place.

### Version 5.0.2

- Improvement: Included module help topic.

### Version 5.0.1

- Improvement: Updated NuGet package versions.

### Version 5.0.0

- New Feature: Azure Functions for Isolated Processes are now accessible by configuring your API project to target .NET 8 and setting the Output Type to `Console` in the Visual Studio designer. This enhancement is introduced for the following reasons:
  - **End of Support for .NET 6:** As of [12 November 2024](https://devblogs.microsoft.com/dotnet/dotnet-6-end-of-support/), .NET 6 will reach its end of support, necessitating migration to newer versions.
  - **In-Process Model Retirement:** The In-Process model will reach its end of support on [10 November 2026](https://azure.microsoft.com/en-us/updates/retirement-support-for-the-inprocess-model-for-net-apps-in-azure-functions-ends-10-november-2026), prompting the shift to Isolated Processes.
  - **Support for .NET 8 and Above:** This update provides improved compatibility and features for .NET 8 and future versions.

**Refer to the module documentation for guidance on migrating from In-Process functions.**

### Version 4.1.3

- Improvement: Updated NuGet package versions.
- Improvement: Updated module NuGet packages infrastructure.
- Improvement: Updated templates to use new NuGet package system.

### Version 4.1.2

- Improvement: Updated NuGet packages to latest stables.

### Version 4.1.1

- Fixed: _Http Settings_ stereotype will no longer be removed from operations, queries or commands unless an _Azure Function_ stereotype is present with its _Trigger_ set to anything other than `Http Trigger`.

### Version 4.1.0

- Improvement: Module project updated to .NET 8.
- Improvement: Introduced setting to generate Function names with full path to avoid conflicting names.
- Fixed: Namespace generation takes folders into account to avoid possible class name conflicts and to bring it into alignment with the other module patterns.
- Fixed: Enums can now be supplied as route parameters and parsed correctly.

### Version 4.0.16

- Improvement: Updated Interoperable dependency versions.

### Version 4.0.15

- Fixed: `Expose as HTTP` would fail when used on Commands and Queries with advanced mappings.
- Improvement: Updated icon to SVG format.

### Version 4.0.13

- New Feature: Azure Functions can now receive RabbitMQ triggers.
- Improvement: Removed `Intent.Application.DTOs` dependency since DTOs can be resolved using Role names.
- Improvement: Startup file is now generated using a File Builder Template.

### Version 4.0.11

- Fixed: Queue Trigger handler parameter name clashes with inline variable name.

### Version 4.0.9

- New Feature: Added support for CosmosDB Trigger Azure Functions.

### Version 4.0.8

- Fixed: Improved Http Trigger support collection based parameters.

### Version 4.0.7

- Improvement: Improved Http Trigger support for Header parameters.
- Improvement: Improved Queue Trigger support, including receiving messages as `QueueMessage` and output binding support for `Azure Functions`.
- Improvement: Removed compiler warning.
- Improvement: Services that produce Azure Functions will no longer dump its own functions in the root folder of the API project but in their own folders.
- Improvement: Moved away from Newtonsoft JSON and now using System.Text.Json instead.

### Version 4.0.5

- Improvement: Interop dependencies for App Services and MediatR dispatch.

### Version 4.0.3

- Improvement: Version bump of Nuget Package `Microsoft.Extensions.DependencyInjection` to 6.0.1

### Version 4.0.2

- Improvement: Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.1

- Improvement: Support for multiple paradigms of expressing Azure Functions (i.e. through Stereotypes applied to Commands / Queries / Service Operations / etc.). The appropriate supporting modules will need to be installed to enable this.
- Improvement: Supports additional trigger types: Timer Trigger, EventHub Trigger and Manual Trigger for Azure Functions.

### Version 4.0.0

- Improvement: Changed fundamental paradigm for expressing Azure Functions to explicitly modeling the functions inside of the Services designer.

### Version 3.3.9

- Improvement: Azure Service Bus Queues and File Storage Queues are now better supported.
- Improvement: Done internal code cleanup.
