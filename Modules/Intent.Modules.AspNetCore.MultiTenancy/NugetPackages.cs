using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.MultiTenancy
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreInMemory(IOutputTarget outputTarget) => new(
            name: "Microsoft.EntityFrameworkCore.InMemory",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "7.0.20",
                _ => "8.0.7",
            });

        public static NugetPackageInfo FinbuckleMultiTenant(IOutputTarget outputTarget) => new(
            name: "Finbuckle.MultiTenant",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "6.13.1",
            });

        public static NugetPackageInfo FinbuckleMultiTenantAspNetCore(IOutputTarget outputTarget) => new(
            name: "Finbuckle.MultiTenant.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "6.13.1",
            });

        public static NugetPackageInfo FinbuckleMultiTenantEntityFrameworkCore(IOutputTarget outputTarget) => new(
            name: "Finbuckle.MultiTenant.EntityFrameworkCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                _ => "6.13.1",
            });
    }
}
