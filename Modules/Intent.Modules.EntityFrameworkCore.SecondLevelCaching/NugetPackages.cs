using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.SecondLevelCaching
{
    public class NugetPackages : INugetPackages
    {
        public const string EFCoreSecondLevelCacheInterceptorPackageName = "EFCoreSecondLevelCacheInterceptor";
        public const string MessagePackPackageName = "MessagePack";

        public void RegisterPackages()
        {
            NugetRegistry.Register(EFCoreSecondLevelCacheInterceptorPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("4.6.0"),
                        ( >= 7, 0) => new PackageVersion("4.6.0"),
                        ( >= 6, 0) => new PackageVersion("4.6.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{EFCoreSecondLevelCacheInterceptorPackageName}'"),
                    }
                );
            NugetRegistry.Register(MessagePackPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("2.5.172"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MessagePackPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo EFCoreSecondLevelCacheInterceptor(IOutputTarget outputTarget) => NugetRegistry.GetVersion(EFCoreSecondLevelCacheInterceptorPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MessagePack(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MessagePackPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
