using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions.OpenApi
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsOpenApi(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Azure.WebJobs.Extensions.OpenApi",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 2, 0) => "1.5.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.Azure.WebJobs.Extensions.OpenApi'")
            });
    }
}
