using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.GoogleCloud.PubSub;

public static class NugetPackages
{
    public static INugetPackageInfo GoogleCloudPubSubV1 = new NugetPackageInfo("Google.Cloud.PubSub.V1", "3.3.0");
    public static INugetPackageInfo MicrosoftExtensionsHostingAbstractions(IOutputTarget outputTarget) => new NugetPackageInfo("Microsoft.Extensions.Hosting.Abstractions", GetExtensionsHostingAbstractionsVersion(outputTarget.GetProject()));

    private static string GetExtensionsHostingAbstractionsVersion(ICSharpProject project)
    {
        return project switch
        {
            _ when project.IsNetApp(5) => "5.0.0",
            _ when project.IsNetApp(6) => "6.0.0",
            _ when project.IsNetApp(7) => "7.0.0",
            _ => throw new Exception("Not supported version of .NET Core") 
        };
    }
}