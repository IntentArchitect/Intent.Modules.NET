using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.MongoDb
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MongoFramework(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "MongoFramework",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "0.29.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'MongoFramework'")
            });
    }
}
