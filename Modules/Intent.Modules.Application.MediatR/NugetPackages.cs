using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Application.MediatR
{
    public class NugetPackages : INugetPackages
    {
        public const string MediatRPackageName = "MediatR";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MediatRPackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("12.4.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MediatRPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MediatRPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
