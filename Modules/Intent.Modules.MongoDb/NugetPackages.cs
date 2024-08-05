using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.MongoDb
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MongoFramework(IOutputTarget outputTarget) => new(
            name: "MongoFramework",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "0.29.0",
            });
    }
}
