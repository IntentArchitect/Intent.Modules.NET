using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects
{
    [IntentMergeBody]
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftExtensionsHostingPackageName = "Microsoft.Extensions.Hosting";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftExtensionsHostingPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.9"),
                        ( >= 9, >= 0) => new PackageVersion("10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.9"),
                        ( >= 8, >= 0) => new PackageVersion("10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.9"),
                        ( >= 2, >= 1) => new PackageVersion("10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.9"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.9")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Binder", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.CommandLine", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.EnvironmentVariables", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.UserSecrets", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Hosting.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Configuration", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Console", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Debug", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventLog", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Logging.EventSource", "10.0.9")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.9")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.6.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsHostingPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftExtensionsHosting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHostingPackageName, outputTarget.GetMaxNetAppVersion());
        public static INugetPackageInfo MicrosoftAspNetWebApi = new NugetPackageInfo("Microsoft.AspNet.WebApi", "5.2.6");
        public static INugetPackageInfo MicrosoftAspNetWebApiClient = new NugetPackageInfo("Microsoft.AspNet.WebApi.Client", "5.2.6")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("System.Net.Http.Formatting", "5.2.6.0", "31bf3856ad364e35"));
        public static INugetPackageInfo MicrosoftAspNetWebApiCore = new NugetPackageInfo("Microsoft.AspNet.WebApi.Core", "5.2.6")
            .WithAssemblyRedirect(new AssemblyRedirectInfo("System.Web.Http", "5.2.6.0", "31bf3856ad364e35"));
        public static INugetPackageInfo MicrosoftAspNetWebApiWebHost = new NugetPackageInfo("Microsoft.AspNet.WebApi.WebHost", "5.2.6");
        public static INugetPackageInfo NewtonsoftJson = new NugetPackageInfo("Newtonsoft.Json", "9.0.1")
    .WithAssemblyRedirect(new AssemblyRedirectInfo("Newtonsoft.Json", "9.0.0.0", "30ad4fe6b2a6aeed"));

    }
}
