using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.MongoDb;

public static class NugetPackages
{
    public static readonly INugetPackageInfo MongoDbDataUnitOfWork = new NugetPackageInfo("MongoDB.Data.UnitOfWork", "1.1.5");
    public static readonly INugetPackageInfo MongoDBDataGenerators = new NugetPackageInfo("MongoDB.Data.Generators", "1.1.5");

    public static readonly INugetPackageInfo MongoFramework = new NugetPackageInfo("MongoFramework", "0.29.0");    
}