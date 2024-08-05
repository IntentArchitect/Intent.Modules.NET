using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.MultiTenancy
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreInMemory(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.EntityFrameworkCore.InMemory",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                (>= 6, 0) => "7.0.20",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.EntityFrameworkCore.InMemory'")
            });

        public static NugetPackageInfo FinbuckleMultiTenant(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Finbuckle.MultiTenant",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "6.13.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Finbuckle.MultiTenant'")
            });

        public static NugetPackageInfo FinbuckleMultiTenantAspNetCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Finbuckle.MultiTenant.AspNetCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "6.13.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Finbuckle.MultiTenant.AspNetCore'")
            });

        public static NugetPackageInfo FinbuckleMultiTenantEntityFrameworkCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Finbuckle.MultiTenant.EntityFrameworkCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "6.13.1",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Finbuckle.MultiTenant.EntityFrameworkCore'")
            });
    }
}
