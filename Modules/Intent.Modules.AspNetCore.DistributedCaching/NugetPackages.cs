using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.DistributedCaching
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftExtensionsCachingAbstractionsPackageName = "Microsoft.Extensions.Caching.Abstractions";
        public const string MicrosoftExtensionsCachingMemoryPackageName = "Microsoft.Extensions.Caching.Memory";
        public const string MicrosoftExtensionsCachingStackExchangeRedisPackageName = "Microsoft.Extensions.Caching.StackExchangeRedis";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftExtensionsCachingAbstractionsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "10.0.0"),
                        ( >= 9, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "10.0.0"),
                        ( >= 8, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "10.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "10.0.0")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.6.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsCachingAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsCachingMemoryPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "10.0.0"),
                        ( >= 9, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "10.0.0"),
                        ( >= 8, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "10.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "10.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsCachingMemoryPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsCachingStackExchangeRedisPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("StackExchange.Redis", "2.7.27"),
                        ( >= 2, >= 0) => new PackageVersion("10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "10.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "10.0.0")
                            .WithNugetDependency("StackExchange.Redis", "2.7.27"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsCachingStackExchangeRedisPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftExtensionsCachingAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsCachingAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsCachingMemory(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsCachingMemoryPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsCachingStackExchangeRedis(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsCachingStackExchangeRedisPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
