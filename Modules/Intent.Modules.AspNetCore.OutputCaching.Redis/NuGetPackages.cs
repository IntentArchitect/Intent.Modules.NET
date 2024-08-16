using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using NuGet.Versioning;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.OutputCaching.Redis
{
    public class NugetPackages
    {
        public const string StackExchangeRedisPackageName = "Microsoft.AspNetCore.OutputCaching.StackExchangeRedis";

        static NugetPackages()
        {
            NugetRegistry.Register(StackExchangeRedisPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.8"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{StackExchangeRedisPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo StackExchangeRedis(IOutputTarget outputTarget) => NugetRegistry.GetVersion(StackExchangeRedisPackageName, outputTarget.GetMaxNetAppVersion());
    }

}
