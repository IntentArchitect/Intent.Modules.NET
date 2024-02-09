using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Redis.Om.Repositories
{
    internal static class NugetDependencies
    {
        public static readonly INugetPackageInfo RedisOM = new NugetPackageInfo("Redis.OM", "0.6.1");
        public static INugetPackageInfo MicrosoftExtensionsHostingAbstractions(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.Extensions.Hosting.Abstractions",
            version: GetDotNetSupportedVersion(outputTarget));
        
        private static string GetDotNetSupportedVersion(IOutputTarget outputTarget) => outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.0",
            (6, 0) => "6.0.0",
            (7, 0) => "7.0.0",
            _ => "8.0.0"
        };
    }
}
