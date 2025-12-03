# Intent.Bugsnag

This module integrates [Bugsnag](https://www.bugsnag.com/) for error monitoring and reporting into your application.

## What is Bugsnag?

Bugsnag is a full-stack error monitoring and stability management platform. It captures unhandled exceptions, diagnostic data, and allows you to proactively manage and fix errors in your applications.

For more information, see the [official Bugsnag documentation](https://docs.bugsnag.com/).

## What This Module Generates

This module adds the necessary NuGet packages and configuration to automatically report exceptions to your Bugsnag dashboard.

- **NuGet Packages**: Adds the `Bugsnag.AspNet.Core` NuGet package to your project.
- **Configuration**: Adds Bugsnag to your `appsettings.json` and configures it in your application's startup (`Program.cs` or `Startup.cs`).
- **Middleware**: Registers the Bugsnag middleware to automatically capture unhandled exceptions in the ASP.NET Core pipeline.

## Module Settings

### API Key

Your Bugsnag project's API Key. This is required to send errors to the correct project in Bugsnag. You can find this in your Bugsnag project settings.

## How to use the module

1.  Install this module.
2.  Configure the `API Key` in the module settings.
3.  Run the software factory.

The module will automatically add the required configuration to your application. Any unhandled exceptions will now be reported to your Bugsnag dashboard.
