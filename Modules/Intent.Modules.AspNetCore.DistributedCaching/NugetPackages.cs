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
        public const string MicrosoftExtensionsCachingStackExchangeRedisPackageName = "Microsoft.Extensions.Caching.StackExchangeRedis";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftExtensionsCachingAbstractionsPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "9.0.0"),
                        ( >= 2, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "9.0.0")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.5.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsCachingAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsCachingStackExchangeRedisPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.0")
                            .WithNugetDependency("StackExchange.Redis", "2.7.27"),
                        ( >= 8, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.0")
                            .WithNugetDependency("StackExchange.Redis", "2.7.27"),
                        ( >= 2, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.0")
                            .WithNugetDependency("StackExchange.Redis", "2.7.27"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsCachingStackExchangeRedisPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftExtensionsCachingAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsCachingAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsCachingStackExchangeRedis(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsCachingStackExchangeRedisPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
