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
