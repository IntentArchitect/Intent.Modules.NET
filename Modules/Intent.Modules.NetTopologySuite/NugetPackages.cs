using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.NetTopologySuite;

public static class NugetPackages
{
    public static readonly INugetPackageInfo NetTopologySuite = new NugetPackageInfo("NetTopologySuite", "2.5.0");
    public static readonly INugetPackageInfo NetTopologySuiteIoGeoJson4Stj = new NugetPackageInfo("NetTopologySuite.IO.GeoJSON4STJ", "4.0.0");
}