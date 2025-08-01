using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients
{
    public class NugetPackages : INugetPackages
    {
        public const string DuendeAccessTokenManagementPackageName = "Duende.AccessTokenManagement";
        public const string IdentityModelAspNetCorePackageName = "IdentityModel.AspNetCore";
        public const string MicrosoftAspNetCoreHttpPackageName = "Microsoft.AspNetCore.Http";
        public const string MicrosoftAspNetCoreHttpExtensionsPackageName = "Microsoft.AspNetCore.Http.Extensions";
        public const string MicrosoftAspNetCoreWebUtilitiesPackageName = "Microsoft.AspNetCore.WebUtilities";
        public const string MicrosoftExtensionsHttpPackageName = "Microsoft.Extensions.Http";
        public const string SystemTextJsonPackageName = "System.Text.Json";

        public void RegisterPackages()
        {
            NugetRegistry.Register(DuendeAccessTokenManagementPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("3.2.0", locked: true)
                            .WithNugetDependency("Duende.IdentityModel", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Http", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.0")
                            .WithNugetDependency("System.IdentityModel.Tokens.Jwt", "8.0.1"),
                        ( >= 8, >= 0) => new PackageVersion("3.2.0", locked: true)
                            .WithNugetDependency("Duende.IdentityModel", "7.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Http", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "8.0.0")
                            .WithNugetDependency("System.IdentityModel.Tokens.Jwt", "8.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{DuendeAccessTokenManagementPackageName}'"),
                    }
                );
            NugetRegistry.Register(IdentityModelAspNetCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 6, >= 0) => new PackageVersion("4.3.0", locked: true),
                        ( >= 2, >= 0) => new PackageVersion("2.0.0", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{IdentityModelAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreHttpPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Http.Abstractions", "2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.WebUtilities", "2.3.0")
                            .WithNugetDependency("Microsoft.Extensions.ObjectPool", "8.0.11")
                            .WithNugetDependency("Microsoft.Extensions.Options", "8.0.2")
                            .WithNugetDependency("Microsoft.Net.Http.Headers", "2.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreHttpPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreHttpExtensionsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Http.Abstractions", "2.3.0")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Net.Http.Headers", "2.3.0")
                            .WithNugetDependency("System.Buffers", "4.6.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreHttpExtensionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreWebUtilitiesPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.7")
                            .WithNugetDependency("Microsoft.Net.Http.Headers", "9.0.7"),
                        ( >= 8, >= 0) => new PackageVersion("8.0.18")
                            .WithNugetDependency("Microsoft.Net.Http.Headers", "8.0.18")
                            .WithNugetDependency("System.IO.Pipelines", "8.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("2.3.0")
                            .WithNugetDependency("Microsoft.Net.Http.Headers", "2.3.0")
                            .WithNugetDependency("System.Text.Encodings.Web", "8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreWebUtilitiesPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsHttpPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.7"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.7"),
                        ( >= 2, >= 0) => new PackageVersion("9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.7")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.7"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsHttpPackageName}'"),
                    }
                );
            NugetRegistry.Register(SystemTextJsonPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.7"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.7")
                            .WithNugetDependency("System.IO.Pipelines", "9.0.7")
                            .WithNugetDependency("System.Text.Encodings.Web", "9.0.7"),
                        ( >= 2, >= 0) => new PackageVersion("9.0.7")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "9.0.7")
                            .WithNugetDependency("System.Buffers", "4.5.1")
                            .WithNugetDependency("System.IO.Pipelines", "9.0.7")
                            .WithNugetDependency("System.Memory", "4.5.5")
                            .WithNugetDependency("System.Runtime.CompilerServices.Unsafe", "6.0.0")
                            .WithNugetDependency("System.Text.Encodings.Web", "9.0.7")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.5.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SystemTextJsonPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo DuendeAccessTokenManagement(IOutputTarget outputTarget) => NugetRegistry.GetVersion(DuendeAccessTokenManagementPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo IdentityModelAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(IdentityModelAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreHttp(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreHttpPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreHttpExtensions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreHttpExtensionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreWebUtilities(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreWebUtilitiesPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHttp(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHttpPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo SystemTextJson(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SystemTextJsonPackageName, outputTarget.GetMaxNetAppVersion());
    }
}