using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Eventing.GoogleCloud.PubSub;

public static class NugetPackages
{
    public static readonly INugetPackageInfo GoogleCloudPubSubV1 = new NugetPackageInfo(
        name: "Google.Cloud.PubSub.V1",
        version: "3.3.0");
    public static INugetPackageInfo MicrosoftExtensionsHostingAbstractions(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "Microsoft.Extensions.Hosting.Abstractions",
        version: GetDotNetSupportedVersion(outputTarget));
    public static INugetPackageInfo MicrosoftExtensionsOptionsConfigurationExtensions(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "Microsoft.Extensions.Options.ConfigurationExtensions",
        version: GetDotNetSupportedVersion(outputTarget));
    public static INugetPackageInfo MicrosoftExtensionsLoggingAbstractions(IOutputTarget outputTarget) => new NugetPackageInfo(
        name: "Microsoft.Extensions.Logging.Abstractions",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.0",
            (6, 0) => "6.0.4",
            (7, 0) => "7.0.1",
            _ => "8.0.0"
        });

    private static string GetDotNetSupportedVersion(IOutputTarget outputTarget) => outputTarget.GetMaxNetAppVersion() switch
    {
        (5, 0) => "5.0.0",
        (6, 0) => "6.0.0",
        (7, 0) => "7.0.0",
        _ => "8.0.0"
    };
}