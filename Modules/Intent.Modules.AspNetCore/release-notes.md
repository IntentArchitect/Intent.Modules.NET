### Version 6.0.4

- Improvement: Updated module NuGet packages infrastructure.

### Version 6.0.3

- Fixed: Program.cs generation more robust for Top-level statements and Minimal Hosting model configuration.

### Version 6.0.2

- Improvement: Dependent modules changed, version bumped.

### Version 6.0.1

- Improvement: Dependent modules changed, version bumped.

### Version 6.0.0

> ⚠️ **NOTE**
>
> Module authors: This major version upgrade removes the `StartupDecorator`. All Intent authored modules have been updated accordingly, but if you're running custom modules which are using this decorator they will need to be updated or they will cause an exception to occur during software factory execution. As always, we're available through our various support channels, including [GitHub](https://github.com/IntentArchitect/Support/), should you have any queries.

- New feature: [Top-level statements](https://learn.microsoft.com/dotnet/csharp/fundamentals/program-structure/top-level-statements) support in Program.cs, to enable, check the [property](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.VisualStudio.Projects/README.md#use-top-level-statements) on the project's stereotype in the Visual Studio designer.
- New feature: [Minimal hosting model startup](https://learn.microsoft.com/aspnet/core/migration/50-to-60#use-startup-with-the-new-minimal-hosting-model) support, to enable, check the [property](https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.VisualStudio.Projects/README.md#use-minimal-hosting-model) on the project's stereotype in the Visual Studio designer.

### Version 5.1.2

- Fixed: In some cases there are "traceId" data already specified when the Generic Exception handler executes, this will ensure that the code won't break when that is the case.

### Version 5.1.1

- Fixed: Minor fixes around Trace Id and Startup.cs code generation.

### Version 5.1.0

- New: Added `UseExceptionHandler` to catch unhandled exceptions and return a `ProblemDetails` response for an HTTP 500 status code to allow for the `Trace ID` to be returned in the response for using with Telemetry systems like Azure Application Insights in order to troubleshoot errors. To make use of the `Trace ID` examine this example `"00-7e7ba8aacdb26b4300226a11d2e3db91-fc881cc2439952cf-01"`. Copy the number between the first and second `-` as this will be your `Operation Id`: `7e7ba8aacdb26b4300226a11d2e3db91`.

### Version 5.0.0

- Updated Program template to also use the Builder Pattern.

### Version 4.0.2

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 4.0.0

- New: Upgraded Templates to use new Builder Pattern paradigm.
