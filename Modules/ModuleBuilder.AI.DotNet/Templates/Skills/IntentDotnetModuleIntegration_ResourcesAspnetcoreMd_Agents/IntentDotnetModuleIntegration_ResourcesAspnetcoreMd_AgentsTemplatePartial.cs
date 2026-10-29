using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace ModuleBuilder.AI.DotNet.Templates.Skills.IntentDotnetModuleIntegration_ResourcesAspnetcoreMd_Agents
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class IntentDotnetModuleIntegration_ResourcesAspnetcoreMd_AgentsTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.AI.DotNet.Skills.IntentDotnetModuleIntegration_ResourcesAspnetcoreMd_Agents";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntentDotnetModuleIntegration_ResourcesAspnetcoreMd_AgentsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile("aspnetcore", relativeLocation: "intent-dotnet-module-integration/resources")
                .FromMarkdown("""
# ASP.NET Core host (`Intent.AspNetCore`)

This file covers the host: `Program.cs`, `Startup.cs` and how other modules add services, middleware and endpoints to them. For controllers, see [ASP.NET Core Controllers](./aspnetcore-controllers.md).

## 0. Version compatibility

Verified against `Intent.AspNetCore` **6.1.7-pre.0**. These facts only hold from a particular version, so check the installed version (its `.imodspec`'s `<version>`) before trusting them:

- **`Program.cs` is generated in Merge mode, and the host run call is re-emitted from the existing file, from 6.1.7.** Before that, `Program.cs` is `Mode.Fully` and the run call is always `app.Run();` or `CreateHostBuilder(args).Build().Run();`. Finding the run call by its `host-run` metadata (§5) works on every 6.x version; finding it by its text does not from 6.1.7.
- **`UseAuthorization()` can be absent from 6.1.5**, when the `Enable Authorization` setting is off. Before 6.1.5 it is always emitted.
- **`UseHttpsRedirection()` can be absent from 6.0.9**, when `Enable HTTPS Redirect` is off.
- **Several ASP.NET Core hosts in one application are supported from 6.1.3.** Below that there is one `Program`/`Startup` per application.
- **`IAppStartupFile.ExposeProgramClass()` takes effect from 6.1.0.**
- **Minimal hosting, top-level statements and the whole `IAppStartupFile` API arrived in 6.0.0**, which also removed `StartupDecorator`. Below 6.0.0 nothing in this file applies.

Two gates come from `Intent.Modules.Common.CSharp`, which owns the interfaces in §6:

- `IAppStartupTemplate.HasStartupFile` and `IAppStartupFile.ExposeProgramClass()` arrived together. They are absent in 3.8.0 and present in 3.9.5.
- `ICSharpProject.HasMicrosoftNetSdkWeb()`, which every in-repo consumer uses to skip non-web hosts (§9), first ships in **3.10.10**. That version needs `Intent.Common` 3.11.0, `Intent.SoftwareFactory.SDK` 3.14.0 and Intent Architect 5.0.0. `Intent.AspNetCore` itself only requires `Intent.Common.CSharp` 3.9.5.

## 1. What it generates, and what it leaves to other modules

`Intent.AspNetCore` owns the web host's entry point and pipeline skeleton, one set per ASP.NET Core project:

- `Program.cs`, in all four hosting shapes (§4).
- `Startup.cs`, under the generic hosting model only.
- `Program.Partial.cs`, only when the project uses top-level statements and some module called `ExposeProgramClass()`.
- `Configuration/ProblemDetailsConfiguration.cs`, for .NET 6 and later.
- `Security/Permissions.cs`, only when a Services or Domain designer defines roles or policies, or applies `Secured` or data-masking stereotypes.

It adds its own statements to the pipeline through the same mechanism other modules use:

| Section | Statement | Priority | Condition |
|---|---|---|---|
| Services | `services.ConfigureProblemDetails()` | 0 (request) | .NET 6+ |
| App | `app.UseExceptionHandler()` | -40 | .NET 6+ |
| App | `app.UseHttpsRedirection()` | -30 | `Enable HTTPS Redirect` |
| App | `app.UseRouting()` | -20 | always |
| App | `app.UseAuthorization()` | -5 | `Enable Authorization` |
| App | `app.UseEndpoints(endpoints => ...)` | 0 | always; flattened to `app.MapX()` under minimal hosting |

It does not generate:

- `AddControllers()` or `MapControllers()`. `Intent.AspNetCore.Controllers` adds both.
- `UseAuthentication()`. `Intent.Application.Identity` adds it at -10.
- `appsettings*.json`, `launchSettings.json` or `web.config`. Those belong to `Intent.VisualStudio.Projects`, which handles `AppSettingRegistrationRequest`, `ConnectionStringRegistrationRequest`, `LaunchProfileRegistrationRequest` and `EnvironmentVariableRegistrationRequest`. `Intent.AspNetCore` handles none of them.
- `AddApplication()` and `AddInfrastructure()`. The DependencyInjection modules publish those as `ServiceConfigurationRequest`s, and this host places them.

## 2. Taking the dependency

| Item | Value |
|---|---|
| NuGet PackageId | `Intent.Modules.AspNetCore` (published on nuget.org) |
| Intent module id | `Intent.AspNetCore`, without `Modules.` |
| Namespaces | `Intent.Modules.AspNetCore.Templates.Program`, `...Templates.Startup`, `...Templates.ProblemDetailsConfiguration`, `...Templates.Permissions`, `...Settings`, `...Events`, and `...Templates` for `TemplateExtensions` |
| Where the integration contract lives | `Intent.Modules.Common.CSharp`, namespace `Intent.Modules.Common.CSharp.AppStartup`, not this package |

```xml
<!-- .imodspec -->
<dependency id="Intent.AspNetCore" version="6.1.0" />
```

```xml
<!-- .csproj, only if you need a type from the list below -->
<PackageReference Include="Intent.Modules.AspNetCore" Version="6.1.*" />
```

Both versions are illustrative floors. Check `Intent.AspNetCore.imodspec`'s own `<version>` before writing yours. Most in-repo dependants declare `6.1.0`.

Unlike Controllers, the package reference buys little, and most consumers never use it:

- `ProgramTemplate.TemplateId` and `StartupTemplate.TemplateId`.
- The `GetProgramName()` / `GetStartupName()` extensions.
- `GetASPNETCoreSettings()`. `Intent.AspNetCore.Controllers` is the only in-repo user.
- `SecureTokenServiceHostedEvent` (see §9).

The typed API you actually call (`IAppStartupTemplate`, `IAppStartupFile`, `IProgramTemplate`, `IProgramFile`) comes from `Intent.Modules.Common.CSharp`, which every C# module already references. So declare the `.imodspec` dependency always, and add the `PackageReference` only when you need one of the types above. In this repo, HealthChecks, Grpc and Scalar integrate through `IAppStartupTemplate.RoleName` with no reference to this package at all. A module authored outside this repo has no `ProjectReference` option: if you reference it, use `PackageReference`.

## 3. Templates

| TemplateId | `.imodspec` role | Extra runtime role | Location | Emits a file when |
|---|---|---|---|---|
| `Intent.AspNetCore.Program` | `Startup;App.Program` | `AppStartupTemplate`, minimal hosting only | project root | always |
| `Intent.AspNetCore.Startup` | `Startup;App.Startup` | `AppStartupTemplate`, generic hosting only | project root | generic hosting; the instance still exists under minimal hosting, with `CanRunTemplate()` false |
| `Intent.AspNetCore.ProgramPartial` | `Startup;App.Program.Partial` | none | project root, `Program.Partial.cs` | top-level statements on and `ExposeProgramClass()` called |
| `Intent.AspNetCore.ProblemDetailsConfiguration` | `Startup.ProblemDetailsConfiguration` | none | `Configuration` | not .NET Core 2/3, .NET 4 or .NET 5 |
| `Intent.AspNetCore.Permissions` | `Application.Security` | `Security.Permissions` | `Security` | at least one role or policy constant found |

Lookup strings to use:

- `IAppStartupTemplate.RoleName` (`"AppStartupTemplate"`) is the template that owns startup for a host. Use it for the `IAppStartupFile` API in §6.
- `"App.Program"` is this module's `Program` template whatever the hosting model. Use it for `IProgramFile`. In this repo no other template carries this role; the Worker `Program` in `Intent.VisualStudio.Projects` does not.
- `"App.Startup"` exists, but no in-repo module looks it up.

The constants `TemplateRoles.Distribution.WebApi.Program` (`"App.Program"`), `TemplateRoles.Distribution.WebApi.Startup` (`"App.Startup"`) and `Roles.Security.Permissions` (`"Security.Permissions"`) live in `Intent.Modules.Constants`, which is not published. Write the literal strings.

## 4. Generated shape

The shape is chosen per host project, from that project's `.NET Settings` in the Visual Studio designer. Read it per template instance with `IProgramFile.UsesMinimalHostingModel`, or `OutputTarget.GetProject()?.InternalElement?.AsCSharpProjectNETModel()?.GetNETSettings()` and then `UseMinimalHostingModel()` / `UseTopLevelStatements()` (namespace `Intent.Modules.VisualStudio.Projects.Api`).

| Minimal hosting | Top-level statements | Owns `AppStartupTemplate` | Service configuration | App configuration | Endpoints |
|---|---|---|---|---|---|
| off | off or on | `Startup` | `Startup.ConfigureServices(IServiceCollection services)` | `Startup.Configure(IApplicationBuilder app, IWebHostEnvironment env)` | inside `app.UseEndpoints(endpoints => { ... })` |
| on | off | `Program` | a nested block inside `static void Main(string[] args)` | `Main` itself | flattened to `app.MapX();` lines |
| on | on | `Program` | a nested block in the top-level statements | the top-level statements | flattened |

Under generic hosting, `Program.cs` holds only the run call and `CreateHostBuilder` (a static method, or a local function with top-level statements), which returns `Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())`. `Startup` has a constructor that takes `IConfiguration` into a read-only `Configuration` property. `Configure` opens with `if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }`.

Under minimal hosting, `Program` is generated in this order:

1. `var builder = WebApplication.CreateBuilder(args);`
2. `// Add services to the container.`
3. `builder.Host.X(...)` and other host-builder statements.
4. Service configuration statements, then container registrations.
5. A blank line, then `var app = builder.Build();`
6. `// Configure the HTTP request pipeline.`
7. App configuration, with the endpoint `app.MapX()` lines where `UseEndpoints` sat.
8. The host run call.

Within a section, statements are ordered by placement priority (§9). Container registrations always come after service configuration statements, in both hosting models.

`Program.cs` carries `[assembly: DefaultIntentManaged(Mode.Merge)]` from 6.1.7. `Startup.cs` is file-level `Mode.Fully` with `[IntentManaged(Mode.Merge)]` on the class.

## 5. The metadata contract

This module links generated code back to nothing in a designer: there is no model behind `Program` or `Startup`. What it does stamp is statement metadata, so you can find its statements. Find them with `FindStatement`, which searches nested blocks too, and `HasMetadata` / `TryGetMetadata`. Never match on statement text.

| Key | Value | On | Shapes |
|---|---|---|---|
| `host-run` | `true` | the host run call, in whatever form the developer wrote it | all |
| `is-builder-statement` | `true` | `var builder = WebApplication.CreateBuilder(args);` | minimal |
| `is-add-services-to-container-comment` | `true` | the `// Add services to the container.` comment | minimal |
| `is-configure-request-pipeline-comment` | `true` | the `// Configure the HTTP request pipeline.` comment | minimal |
| `is-host-builder-configuration-statement` | `true` | each `builder.Host.X(...)` and each `AddHostBuilderConfigurationStatement` statement | minimal |
| `lambda-registration-for` | `string` method name, e.g. `"AddControllers"`, `"UseEndpoints"` | the invocation created by any `Add*Lambda` call | all |
| `startup-statement-priority` | `int` | every statement placed through §6's API or a request | all |
| `startup-statement-type` | a **private** enum | every statement placed by `Add*Configuration` / `AddContainerRegistration*` | all; test only with `HasMetadata` |
| `is-startup-file-placeholder` | `true` | an empty statement in `UseEndpoints`, removed once anything else is added | all |

`configure-services-controllers-generic`, `configure-services-controllers` and `configure-endpoints-controllers-generic` are not this module's keys. `Intent.AspNetCore.Controllers` stamps them on `AddControllers(...)` and `MapControllers()`, and `Intent.FastEndpoints` reuses the endpoint key on `MapFastEndpoints()`.

## 6. Types: complete member reference

The interfaces were read from `Intent.Modules.Common.CSharp/AppStartup/*.cs` and its request classes, with members checked by name against the 3.9.5 package. The implementations were read from `Templates/AppStartupFile.cs`, `ProgramFile.cs`, `StartupMethod.cs`, `Program/ProgramTemplatePartial.cs`, `Startup/StartupTemplatePartial.cs`, `ProgramPartial/ProgramPartialTemplatePartial.cs`, `ProblemDetailsConfiguration/ProblemDetailsConfigurationTemplatePartial.cs`, `Permissions/PermissionsTemplatePartial.cs`, `TemplateExtensions.cs`, `Settings/*.cs` and `Events/*.cs` (Intent.AspNetCore 6.1.7-pre.0).

### 6.1 Where each type lives

| Type(s) | Namespace | NuGet package |
|---|---|---|
| `IAppStartupTemplate`, `IAppStartupFile` (and its nested contexts and delegates), `IProgramTemplate`, `IProgramFile`, `IStartupMethod` | `Intent.Modules.Common.CSharp.AppStartup` | `Intent.Modules.Common.CSharp` |
| `ServiceConfigurationRequest`, `ContainerRegistrationRequest`, `ApplicationBuilderRegistrationRequest` | `Intent.Modules.Common.CSharp.DependencyInjection` | `Intent.Modules.Common.CSharp` |
| `HasMicrosoftNetSdkWeb()` (3.10.10+) | `Intent.Modules.Common.CSharp.VisualStudio` | `Intent.Modules.Common.CSharp` |
| `ProgramTemplate`, `StartupTemplate`, `ProgramPartialTemplate`, `ProblemDetailsConfigurationTemplate`, `PermissionsTemplate`, `TemplateExtensions`, `ASPNETCoreSettings`, `SecureTokenServiceHostedEvent` | `Intent.Modules.AspNetCore.*` | `Intent.Modules.AspNetCore` |

`AppStartupFile`, `ProgramFile`, `StartupMethod`, `ExposeProgramClassRequest`, `ExistingProgramRunCall` and `ASPNETCoreSettingsDefaults` are `internal`.

### 6.2 `IAppStartupTemplate` and `IProgramTemplate`

```csharp
public interface IAppStartupTemplate : ICSharpFileBuilderTemplate
{
    public static string RoleName => "AppStartupTemplate";
    IAppStartupFile StartupFile { get; }   // throws InvalidOperationException on a template that doesn't own startup
    bool HasStartupFile { get; }
}

public interface IProgramTemplate : ICSharpFileBuilderTemplate
{
    IProgramFile ProgramFile { get; }      // always present on Intent.AspNetCore.Program
}
```

`ProgramTemplate` implements both. `StartupTemplate` implements only `IAppStartupTemplate`.

### 6.3 `IAppStartupFile`

```csharp
public interface IAppStartupFile
{
    delegate void ConfigureServiceStatement<in TStatement>(TStatement statement, IServiceConfigurationContext context) where TStatement : CSharpStatement;
    delegate void ConfigureServiceLambda(CSharpInvocationStatement statement, CSharpLambdaBlock lambdaStatement, IServiceConfigurationLambdaContext context);
    delegate void ConfigureAppStatement<in TStatement>(TStatement statement, IAppConfigurationContext context) where TStatement : CSharpStatement;
    delegate void ConfigureEndpointStatement<in TStatement>(TStatement statement, IEndpointConfigurationContext context) where TStatement : CSharpStatement;
    delegate void ConfigureAppLambda(CSharpInvocationStatement statement, CSharpLambdaBlock lambdaStatement, IAppConfigurationLambdaContext context);

    IAppStartupFile ConfigureServices(Action<IHasCSharpStatements, IServiceConfigurationContext> configure);
    IAppStartupFile ConfigureApp(Action<IHasCSharpStatements, IAppConfigurationContext> configure);
    IAppStartupFile ConfigureEndpoints(Action<IHasCSharpStatements, IEndpointConfigurationContext> configure);   // block can be null, see §9
    IAppStartupFile AddMethod(string returnType, string name, Action<IStartupMethod> configure = null, int? priority = null);   // priority ignored
    IAppStartupFile ExposeProgramClass();

    IAppStartupFile AddServiceConfiguration(Func<IServiceConfigurationContext, CSharpStatement> create, ConfigureServiceStatement<CSharpStatement> configure = null, int? priority = null);
    IAppStartupFile AddServiceConfiguration<TStatement>(Func<IServiceConfigurationContext, TStatement> create, ConfigureServiceStatement<TStatement> configure = null, int? priority = null) where TStatement : CSharpStatement;
    IAppStartupFile AddServiceConfigurationLambda(string methodName, IEnumerable<string> parameters, ConfigureServiceLambda configure = null, int? priority = null);

    IAppStartupFile AddContainerRegistration(Func<IServiceConfigurationContext, CSharpStatement> create, ConfigureServiceStatement<CSharpStatement> configure = null, int? priority = null);   // drops priority, see §9
    IAppStartupFile AddContainerRegistration<TStatement>(Func<IServiceConfigurationContext, TStatement> create, ConfigureServiceStatement<TStatement> configure = null, int? priority = null) where TStatement : CSharpStatement;
    IAppStartupFile AddContainerRegistrationLambda(string methodName, IEnumerable<string> parameters, ConfigureServiceLambda configure = null, int? priority = null);

    IAppStartupFile AddAppConfiguration(Func<IAppConfigurationContext, CSharpStatement> create, ConfigureAppStatement<CSharpStatement> configure = null, int? priority = null);   // drops priority, see §9
    IAppStartupFile AddAppConfiguration<TStatement>(Func<IAppConfigurationContext, TStatement> create, ConfigureAppStatement<TStatement> configure = null, int? priority = null) where TStatement : CSharpStatement;
    IAppStartupFile AddAppConfigurationLambda(string methodName, IEnumerable<string> parameters, ConfigureAppLambda configure = null, int? priority = null);   // throws for "UseEndpoints"

    IAppStartupFile AddUseEndpointsStatement(Func<IEndpointConfigurationContext, CSharpStatement> create, ConfigureEndpointStatement<CSharpStatement> configure = null, int? priority = null);
    IAppStartupFile AddUseEndpointsStatement<TStatement>(Func<IEndpointConfigurationContext, TStatement> create, ConfigureEndpointStatement<TStatement> configure = null, int? priority = null) where TStatement : CSharpStatement;

    public interface IServiceConfigurationContext { string Configuration { get; } string Services { get; } }
    public interface IAppConfigurationContext { string Configuration { get; } string App { get; } string Env { get; } }
    public interface IEndpointConfigurationContext : IAppConfigurationContext { string Endpoints { get; } }
    public interface ILambdaConfigurationContext { IReadOnlyList<string> Parameters { get; } }
    public interface IServiceConfigurationLambdaContext : ILambdaConfigurationContext, IServiceConfigurationContext { }
    public interface IAppConfigurationLambdaContext : ILambdaConfigurationContext, IAppConfigurationContext { }
}
```

`Intent.AspNetCore` fills the context strings with these expressions. Always use the context, never hardcode them:

| Property | Generic hosting | Minimal hosting |
|---|---|---|
| `Services` | `services` | `builder.Services` |
| `Configuration` | `Configuration` | `builder.Configuration` |
| `App` | `app` | `app` |
| `Env` | `env` | `app.Environment` |
| `Endpoints` | `endpoints` | `app` |

An `Add*Lambda` call for a `methodName` that already has an invocation reuses it (matched by `lambda-registration-for`), and only appends parameter names when you ask for more than it already has.

### 6.4 `IProgramFile` and `IStartupMethod`

```csharp
public interface IProgramFile
{
    delegate void HostBuilderChainStatementConfiguration(CSharpLambdaBlock lambdaBlock, IReadOnlyList<string> parameters);

    bool UsesMinimalHostingModel { get; }
    IProgramFile ConfigureHostBuilderChainStatement(string methodName, IEnumerable<string> parameters, HostBuilderChainStatementConfiguration configure = null, int priority = 0);
    IProgramFile ConfigureAppConfiguration(bool requiresContext, HostBuilderChainStatementConfiguration configure = null, int priority = 0);   // "ConfigureAppConfiguration", parameters [context,] config
    IProgramFile ConfigureAppLogging(bool requiresContext, HostBuilderChainStatementConfiguration configure = null, int priority = 0);         // "ConfigureLogging", parameters [context,] logBuilder
    IProgramFile AddHostBuilderConfigurationStatement(string statement, Action<CSharpStatement> configure = null, int priority = 0);
    IProgramFile AddHostBuilderConfigurationStatement<TStatement>(TStatement statement, Action<TStatement> configure = null, int priority = 0) where TStatement : CSharpStatement;
    IProgramFile ConfigureMainStatementsBlock(Action<IHasCSharpStatements> configure);
    IProgramFile AddMethod(string returnType, string name, Action<IStartupMethod> configure = null, int priority = 0);
}

public interface IStartupMethod : IHasCSharpStatements
{
    IStartupMethod Static();
    IStartupMethod Async();
    IStartupMethod AddParameter(string type, string name, Action<CSharpParameter> configure = null);
    IStartupMethod AddGenericParameter(string typeName);
    IStartupMethod AddGenericParameter(string typeName, out CSharpGenericParameter param);
    IStartupMethod AddGenericTypeConstraint(string genericParameterName, Action<CSharpGenericTypeConstraint> configure);
}
```

How `Intent.AspNetCore` implements them:

- `ConfigureHostBuilderChainStatement`, generic hosting: finds or creates a `.methodName(lambda)` link in the `CreateHostBuilder` chain, above `ConfigureWebHostDefaults`.
- `ConfigureHostBuilderChainStatement`, minimal hosting: finds or creates `builder.Host.methodName(lambda);` below the last host-builder statement, or below the builder statement when there is none yet.
- A repeat call for the same `methodName` reuses the existing lambda. The lambda's parameter names are passed back to you: use `parameters[^1]` instead of assuming `config`.
- `AddHostBuilderConfigurationStatement` throws `InvalidOperationException` unless minimal hosting is on.
- `ConfigureMainStatementsBlock` hands you `Main`'s body or the top-level statements, in an `OnBuild` at priority 0.
- Every `priority` parameter on `IProgramFile` is ignored.
- Both `AddMethod`s add a class method, or a local function when the project uses top-level statements.

### 6.5 `Intent.Modules.AspNetCore`'s own public members

```csharp
// Intent.Modules.AspNetCore.Templates.Program
public partial class ProgramTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate, IAppStartupTemplate, IProgramTemplate
{
    public const string TemplateId = "Intent.AspNetCore.Program";
    public IAppStartupFile StartupFile { get; }    // throws unless minimal hosting
    public bool HasStartupFile { get; }
    public IProgramFile ProgramFile { get; }
}

// Intent.Modules.AspNetCore.Templates.Startup
public partial class StartupTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate, IAppStartupTemplate
{
    public const string TemplateId = "Intent.AspNetCore.Startup";
    public IAppStartupFile StartupFile { get; }    // throws under minimal hosting
    public bool HasStartupFile { get; }
}

// Intent.Modules.AspNetCore.Templates
public static class TemplateExtensions
{
    public static string GetProgramName<T>(this IntentTemplateBase<T> template);
    public static string GetStartupName<T>(this IntentTemplateBase<T> template);
}

// Intent.Modules.AspNetCore.Templates.ProblemDetailsConfiguration
public partial class ProblemDetailsConfigurationTemplate
{
    public const string TemplateId = "Intent.AspNetCore.ProblemDetailsConfiguration";
    public static bool IsExceptionHandlerSupported(IOutputTarget outputTarget);
}

// Intent.Modules.AspNetCore.Settings
public static class ModuleSettingsExtensions { public static ASPNETCoreSettings GetASPNETCoreSettings(this IApplicationSettingsProvider settings); }
public class ASPNETCoreSettings : IGroupSettings
{
    public bool EnableHTTPSRedirect();
    public bool AddRequiredAttributeToParameters();
    public bool EnableAuthorization();             // an absent value reads as false, see §9
}

// Intent.Modules.AspNetCore.Events
public class SecureTokenServiceHostedEvent { public SecureTokenServiceHostedEvent(string port); public string Port { get; set; } }
```

`PermissionsTemplate` (`TemplateId = "Intent.AspNetCore.Permissions"`) emits a static `Permissions` class with one `const string` per role or policy, plus `All()`, `Roles()` and `Policies()` iterators. Reach it through the `Security.Permissions` role.

## 7. Extension points

All are wired after template registration: a factory extension's `OnAfterTemplateRegistrations`, or a template's `AfterTemplateRegistration()` / `BeforeTemplateExecution()`. The timing rules are in `intent-module-orchestrator`. There is no decorator, and this module has no factory extensions of its own.

- **One-line host wiring: publish a request.** `ServiceConfigurationRequest` produces `{Services}.X({Configuration}?)` in the service section. `ApplicationBuilderRegistrationRequest` produces `{App}.X(...)` in the pipeline. A `ContainerRegistrationRequest` that no DependencyInjection template claimed produces `{Services}.AddTransient/AddScoped/AddSingleton<...>()` with the container registrations. The host collects these in `OnBuild` at `int.MinValue + 1`, then keeps listening so later requests are still placed. `Cors`, `HealthChecks`, `Hangfire` and `ProblemDetailsConfiguration` itself take this route. Mind the priority defaults in §9.
- **A lambda, a relative position, endpoints or a condition: use the `IAppStartupFile` API.** Resolve with `FindTemplateInstances<IAppStartupTemplate>(IAppStartupTemplate.RoleName)`, skip non-web hosts (§9), and inside `template.CSharpFile.OnBuild(...)` call `template.StartupFile`. Controllers' `ExceptionFilterExtension` (the `AddControllers` lambda at -10,000,000, plus `MapControllers`) and Swashbuckle (§8) are the references.
- **Host builder, configuration sources and logging: use `IProgramFile`.** Resolve with `FindTemplateInstances<IProgramTemplate>("App.Program")` and branch on `ProgramFile.UsesMinimalHostingModel`. Under minimal hosting, call `AddHostBuilderConfigurationStatement(...)` with a `builder.Configuration...` statement; under generic hosting, call `ConfigureAppConfiguration(...)`. `Azure.KeyVault`, `HashiCorp.Vault` and `Aws.SecretsManager` follow this shape. Serilog uses `ConfigureHostBuilderChainStatement("UseSerilog", ...)`.
- **Restructure `Program.cs`: change the file directly, found by metadata.** Use an `OnBuild` / `AfterBuild` on the `App.Program` template and find nodes through §5's keys, never through text. Serilog wraps everything around `host-run` in `try`/`catch`; `Blazor.Authentication` inserts `MapAdditionalIdentityEndpoints()` above `host-run`.
- **Make `Program` visible to a test project:** call `StartupFile.ExposeProgramClass()` from `AfterTemplateRegistration()`, guarded by `HasStartupFile`, as `AspNetCore.IntegrationTesting` does.
- **A switch in the "ASP.NET Core Settings" group** (group id `9677c813-b334-4b0d-be2c-3b149f1f6ae8`): declare a `<moduleSettingsExtensions><groupExtension groupId=...>` in your `.imodspec` and read it through a local accessor keyed on the group id, as `Application.Identity` does for `Enable Authentication`. Treat an absent value as enabled.

Which to choose: publish a request when the statement is a single extension-method call whose position a priority can fix. Use the `IAppStartupFile` API when you need a lambda body, a context expression (`Env`, `Endpoints`), a position relative to another statement, or an endpoint mapping. Use `IProgramFile` for the host builder. Change the file directly only to restructure it, and only by metadata.

## 8. Worked example: `SwashbuckleStartupConfigurationExtension`

`Intent.AspNetCore.Swashbuckle`'s factory extension, in `OnAfterTemplateRegistrations`:

1. `application.FindTemplateInstances<IAppStartupTemplate>(IAppStartupTemplate.RoleName)` uses the plural form, so a second host does not throw.
2. `if (!template.OutputTarget.GetProject().HasMicrosoftNetSdkWeb()) continue;` skips Worker and Lambda hosts that also hold the role.
3. `template.OutputTarget.FindTemplateInstance<IClassProvider>(SwashbuckleConfigurationTemplate.TemplateId)` finds this host's own configuration class; the `IOutputTarget` overload keeps the lookup inside that host. Then `AddTemplateDependency(...)` and `template.AddUsing(configurationTemplate.Namespace)`.
4. `template.OutputTarget.EmitDefaultLaunchUrlPathRequest("swagger")`. `Intent.VisualStudio.Projects` handles the launch URL, not this module.
5. `template.CSharpFile.OnBuild(_ => { ... }, 100)`, and inside it:
   - `startup.AddServiceConfiguration(ctx => $"{ctx.Services}.ConfigureSwagger({ctx.Configuration});")`, with no priority, so it is the last service statement.
   - `startup.ConfigureApp((statements, ctx) => ...)`. When `Intent.AspNetCore.Mvc` is installed, it inserts above the `UseEndpoints` statement, because Swagger UI fails if mapped after MVC endpoints. Otherwise it calls `startup.AddAppConfiguration(...)` with no priority, which places `UseSwashbuckle` after the endpoint mappings.

For the pure request route, compare `Intent.AspNetCore.Cors`' `CorsConfigurationTemplate.BeforeTemplateExecution()`: `ServiceConfigurationRequest.ToRegister("ConfigureCors", ParameterType.Configuration).HasDependency(this)` and `ApplicationBuilderRegistrationRequest.ToRegister("UseCors").WithPriority(-12)`. It picks -12 so the call lands before `UseAuthentication` (-10) and `UseAuthorization` (-5).

## 9. Traps

- **`AppStartupTemplate` is not web-only.** `Intent.VisualStudio.Projects`' Worker `Program` and `Intent.Aws.Lambda.Functions`' `Startup` also fill the role, so an application-wide `FindTemplateInstance<IAppStartupTemplate>(RoleName)` (singular) throws as soon as a web host and a Worker share an application. Loop over `FindTemplateInstances` and skip hosts where `HasMicrosoftNetSdkWeb()` is false (needs `Intent.Modules.Common.CSharp` 3.10.10, §0). Below that version, resolve `"App.Program"` instead, which only this module carries. Inside a template, use `ExecutionContext.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName, OutputTarget)`, as `HealthChecks` does. Several in-repo modules still use the singular, unscoped form; don't copy them.
- **Under generic hosting, `App.Program` does not own startup.** `ProgramTemplate` implements `IAppStartupTemplate`, but its `StartupFile` throws unless minimal hosting is on. Resolve startup by `IAppStartupTemplate.RoleName`, or check `HasStartupFile` first.
- **Middleware without a negative priority lands after the endpoints.** Calls to the `IAppStartupFile` API without a priority default to `int.MaxValue`; requests without `WithPriority` default to `0`. Equal priorities keep insertion order, and the host adds its own `UseEndpoints` (priority 0) before it processes requests. The existing bands:

  | Middleware | Priority | Set by |
  |---|---|---|
  | `UseCloudEvents` | -200 | Dapr.AspNetCore.Pubsub |
  | `UseSerilogRequestLogging` | -100 | Logging.Serilog |
  | `UseExceptionHandler` | -40 | Intent.AspNetCore |
  | `UseHttpsRedirection` | -30 | Intent.AspNetCore |
  | `UseRouting` | -20 | Intent.AspNetCore |
  | `UseCors` | -12 | AspNetCore.Cors |
  | `UseAuthentication` | -10 | Application.Identity |
  | `UseAuthorization` | -5 | Intent.AspNetCore |
  | `UseOutputCache` | -1 | OutputCaching.Redis |
  | `UseEndpoints` / `MapX` | 0 | Intent.AspNetCore |
  | `UseHangfire` | 100 | Hangfire |

  Priority sets position in the file, not the order callbacks run in.
- **The non-generic `AddContainerRegistration` and `AddAppConfiguration` silently drop `priority`.** Their default interface bodies forward only `create` and `configure`, and `Intent.AspNetCore` does not override them. A lambda returning a `string` (`ctx => $"..."`) can only bind to that overload. So `AddAppConfiguration(ctx => $"{ctx.App}.UseX();", priority: -15)` lands at `int.MaxValue`. Call the generic form with a `CSharpStatement`: `AddAppConfiguration<CSharpStatement>(ctx => new CSharpStatement(...), priority: -15)`. `AddServiceConfiguration` and `AddUseEndpointsStatement` do forward the priority. Found by reading source; not reproduced.
- **Use the API inside a build callback.** The `Startup` / `Program` class and its methods are built at priority `int.MinValue`, and the `UseEndpoints` lambda is created in an `OnBuild` at `int.MinValue + 1`. Under minimal hosting it is flattened into `app.MapX()` lines by an `AfterBuild` at `int.MaxValue`. Call `ConfigureEndpoints` before that `OnBuild`, or at that final `AfterBuild`, and your callback gets a null block.
- **Serilog moves `Main` into a `try` block at `AfterBuild` 10 under minimal hosting.** `FindStatement` still finds nodes, because it searches nested blocks. But `ConfigureApp` placement only looks at `Main`'s direct statements, so a call made in an `AfterBuild` above 10 would land outside the `try`. Found by reading source; not reproduced. Do this work in `OnBuild`, or an `AfterBuild` below 10.
- **Match on metadata, never text.** From 6.1.7 the run call can be `await app.RunAsync();` or `.Wait()`; find it with `host-run`. From 6.1.5, `app.UseAuthorization();` may not exist at all, so code that finds it by text and calls `First(...)` throws once `Enable Authorization` is off.
- **`ApplicationBuilderRegistrationRequest` environment parameters emit a literal `env`.** `ParameterType.HostEnvironment` and `ParameterType.WebHostEnvironment` both become `env`, which only exists in `Startup.Configure`, so under minimal hosting the generated `Program.cs` would not compile. Found by reading source; not reproduced. Use `ctx.Env` through the `IAppStartupFile` API instead. `ServiceConfigurationRequest` accepts only `ParameterType.Configuration`; anything else throws `ArgumentOutOfRangeException` during generation.
- **This host ignores `ContainerRegistrationRequest.WithResolveFromContainer()`.** It emits a plain `Add{Lifetime}<TInterface, TConcrete>()`, and any lifetime other than singleton or per-service-call becomes `AddTransient`. `ApplicationBuilderRegistrationRequest` has no `ForConcern`, so every host the request reaches, and that can see its dependencies, places it.
- **`Program.cs` is Merge mode (6.1.7+).** Statements a developer added survive, and a statement your module stops generating is removed. Never assume your statement is the only one of its kind in the file.
- **The settings accessors read an absent value as `false`.** `ASPNETCoreSettings.EnableAuthorization()` returns `false` when the setting was never saved, but the host still emits `UseAuthorization()` in that case. Don't use the accessor to predict whether `UseAuthorization()` exists.
- **`ExposeProgramClass()` is solution-wide and only works with top-level statements.** Every host whose project uses top-level statements emits `Program.Partial.cs`. `Intent.Modules.Common.CSharp` 3.10.x has its own public `ExposeProgramClassRequest`; this module does not listen to it, so always go through the method.
- **With several hosts, read per-host settings inside your loop.** Hosting model, target framework and project settings differ per host project; never read them once outside the loop.
- **`SecureTokenServiceHostedEvent` is never published.** Two modules subscribe to it, but nothing in this repo publishes it. Don't rely on it.
- **Auto-installs.** Its `<interoperability>` block raises Controllers, HealthChecks, Interop.Angular, Logging.Serilog, MultiTenancy, SignalR, Swashbuckle, Azure.KeyVault, the Dapr modules, HotChocolate.GraphQL.AspNetCore, OpenTelemetry, VisualStudio.Projects and RoslynWeaver to minimum versions when they are detected. Don't assume a bare install has none of them.
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}