using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions.OpenApi
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftAzureWebJobsExtensionsOpenApi(IOutputTarget outputTarget) => new(
            name: "Microsoft.Azure.WebJobs.Extensions.OpenApi",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "1.5.1",
            });
    }
}
