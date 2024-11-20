using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.FluentValidation.Shared;

public static class NugetPackages
{
    public const string FluentValidationPackageName = "FluentValidation";

    public static NugetPackageInfo FluentValidation(IOutputTarget outputTarget) => NugetRegistry.GetVersion(FluentValidationPackageName, outputTarget.GetMaxNetAppVersion());
}