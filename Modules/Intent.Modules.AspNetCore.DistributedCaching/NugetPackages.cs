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
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsCachingAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsCachingStackExchangeRedisPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.7"),
                        ( >= 7, 0) => new PackageVersion("7.0.20"),
                        ( >= 6, 0) => new PackageVersion("6.0.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsCachingStackExchangeRedisPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftExtensionsCachingAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsCachingAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsCachingStackExchangeRedis(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsCachingStackExchangeRedisPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
