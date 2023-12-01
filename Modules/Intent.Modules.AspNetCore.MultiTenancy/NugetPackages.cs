using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.MultiTenancy;

public static class NugetPackages
{
    public static NugetPackageInfo EntityFrameworkCoreInMemory(IOutputTarget outputTarget) => new(
        name: "Microsoft.EntityFrameworkCore.InMemory",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.17",
            (6, 0) => "6.0.25",
            (7, 0) => "7.0.14",
            _ => "8.0.0"
        });

    public static readonly NugetPackageInfo FinbuckleMultiTenant = new("Finbuckle.MultiTenant", "6.12.0");

    public static readonly NugetPackageInfo FinbuckleMultiTenantAspNetCore = new("Finbuckle.MultiTenant.AspNetCore", "6.12.0");

    public static readonly NugetPackageInfo FinbuckleMultiTenantEntityFrameworkCore = new("Finbuckle.MultiTenant.EntityFrameworkCore", "6.12.0");
}