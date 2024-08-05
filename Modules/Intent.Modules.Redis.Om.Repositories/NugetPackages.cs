using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Redis.Om.Repositories
{
    public static class NugetPackages
    {

        public static NugetPackageInfo RedisOM(IOutputTarget outputTarget) => new(
            name: "Redis.OM",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "0.7.4",
            });

        public static NugetPackageInfo MicrosoftExtensionsHostingAbstractions(IOutputTarget outputTarget) => new(
            name: "Microsoft.Extensions.Hosting.Abstractions",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "8.0.0",
                (7, 0) => "8.0.0",
                _ => "8.0.0",
            });
    }
}
