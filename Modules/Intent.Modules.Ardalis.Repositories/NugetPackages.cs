using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Ardalis.Repositories;

public static class NugetPackages
{
    public static readonly INugetPackageInfo ArdalisSpecification = new NugetPackageInfo("Ardalis.Specification", "6.1.0");
    public static readonly INugetPackageInfo ArdalisSpecificationEntityFrameworkCore = new NugetPackageInfo("Ardalis.Specification.EntityFrameworkCore", "6.1.0");
    public static readonly INugetPackageInfo ArdalisGuardClauses = new NugetPackageInfo("Ardalis.GuardClauses", "4.0.1");
}