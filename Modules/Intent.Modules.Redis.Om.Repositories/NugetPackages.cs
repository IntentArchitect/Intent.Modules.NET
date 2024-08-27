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
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsHostingAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(RedisOMPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("0.7.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{RedisOMPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo RedisOM(IOutputTarget outputTarget) => NugetRegistry.GetVersion(RedisOMPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHostingAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHostingAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
