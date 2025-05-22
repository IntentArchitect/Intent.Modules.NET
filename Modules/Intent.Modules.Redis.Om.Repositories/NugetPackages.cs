using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Redis.Om.Repositories
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftExtensionsHostingAbstractionsPackageName = "Microsoft.Extensions.Hosting.Abstractions";
        public const string RedisOMPackageName = "Redis.OM";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftExtensionsHostingAbstractionsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.4"),
                        ( >= 8, >= 0) => new PackageVersion("9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.4"),
                        ( >= 2, >= 1) => new PackageVersion("9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.4"),
                        ( >= 2, >= 0) => new PackageVersion("9.0.4")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.Diagnostics.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.4")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.4")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.5.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsHostingAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(RedisOMPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("1.0.0")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "8.0.0")
                            .WithNugetDependency("Newtonsoft.Json", "13.0.1")
                            .WithNugetDependency("StackExchange.Redis", "2.7.17")
                            .WithNugetDependency("System.Text.Json", "8.0.5")
                            .WithNugetDependency("Ulid", "1.2.6"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{RedisOMPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo RedisOM(IOutputTarget outputTarget) => NugetRegistry.GetVersion(RedisOMPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHostingAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHostingAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
