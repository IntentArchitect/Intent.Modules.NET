using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using NuGet.Versioning;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.OutputCaching.Redis
{
    public class NugetPackages : INugetPackages
    {
        public const string StackExchangeRedisPackageName = "Microsoft.AspNetCore.OutputCaching.StackExchangeRedis";

        public void RegisterPackages()
        {
            NugetRegistry.Register(StackExchangeRedisPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "9.0.5")
                            .WithNugetDependency("Microsoft.Extensions.Options", "9.0.5")
                            .WithNugetDependency("StackExchange.Redis", "2.7.27"),
                        ( >= 8, >= 0) => new PackageVersion("8.0.16")
                            .WithNugetDependency("Microsoft.Extensions.Logging.Abstractions", "8.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Options", "8.0.2")
                            .WithNugetDependency("StackExchange.Redis", "2.7.27"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{StackExchangeRedisPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo StackExchangeRedis(IOutputTarget outputTarget) => NugetRegistry.GetVersion(StackExchangeRedisPackageName, outputTarget.GetMaxNetAppVersion());
    }

}
