using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using System;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Azure.BlobStorage;

public static class NuGetPackages
{
    public static readonly INugetPackageInfo AzureStorageBlobs = new NugetPackageInfo("Azure.Storage.Blobs", "12.13.0");

    public static INugetPackageInfo MicrosoftExtensionsConfigurationBinder(IOutputTarget outputTarget) => new NugetPackageInfo("Microsoft.Extensions.Configuration.Binder", GetVersion(outputTarget.GetProject()));

    private static string GetVersion(ICSharpProject project)
    {
        return project switch
        {
            _ when project.IsNetCore2App() => throw new Exception(".NET Core 2.x is no longer supported."),
            _ when project.IsNetCore3App() => "3.0.0",
            _ when project.IsNetApp(5) => "5.0.0",
            _ when project.IsNetApp(6) => "6.0.0",
            _ when project.IsNetApp(7) => "7.0.0",
            _ => "6.0.0"
        };
    }
}