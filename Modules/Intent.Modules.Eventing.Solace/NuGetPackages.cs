using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Eventing.Solace
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftExtensionsHostingPackageName = "Microsoft.Extensions.Hosting";
        public const string SolaceSystemsSolclientMessagingPackageName = "SolaceSystems.Solclient.Messaging";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftExtensionsHostingPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("8.0.0"),
                        ( >= 7, 0) => new PackageVersion("8.0.0"),
                        ( >= 6, 0) => new PackageVersion("8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsHostingPackageName}'"),
                    }
                );
            NugetRegistry.Register(SolaceSystemsSolclientMessagingPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("10.25.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SolaceSystemsSolclientMessagingPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo SolaceSystemsSolclientMessaging(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SolaceSystemsSolclientMessagingPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsHosting(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsHostingPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
