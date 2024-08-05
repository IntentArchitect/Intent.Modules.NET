using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.CosmosDB
{
    public static class NugetPackages
    {

        public static NugetPackageInfo NewtonsoftJson(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Newtonsoft.Json",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "13.0.3",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Newtonsoft.Json'")
            });

        public static NugetPackageInfo FinbuckleMultiTenant(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Finbuckle.MultiTenant",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "6.13.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Finbuckle.MultiTenant'")
            });

        public static NugetPackageInfo IEvangelistAzureCosmosRepository(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "IEvangelist.Azure.CosmosRepository",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.1.7",
                (>= 7, 0) => "8.1.7",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'IEvangelist.Azure.CosmosRepository'")
            });
    }
}
