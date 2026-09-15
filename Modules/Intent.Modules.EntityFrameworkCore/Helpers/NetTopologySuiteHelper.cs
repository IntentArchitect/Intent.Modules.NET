using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.EntityFrameworkCore.Templates;

namespace Intent.Modules.EntityFrameworkCore.Helpers;

public static class NetTopologySuiteHelper
{
    private static readonly HashSet<string> GeometryTypeNames = new(StringComparer.Ordinal)
    {
        "Point",
        "MultiPolygon"
    };

    public static bool IsInstalled(ISoftwareFactoryExecutionContext executionContext)
    {
        return executionContext.InstalledModules.Any(mod => mod.ModuleId == "Intent.NetTopologySuite");
    }

    public static bool MapsGeometryTypes(ISoftwareFactoryExecutionContext executionContext, DbContextInstance dbContextInstance)
    {
        // DomainPackageModel.Classes only looks at elements directly under the package root and
        // does not descend into Folders, so entities organised into folders (e.g. Entities/Geometry/)
        // would be invisible to it. GetClassModels() is designer-wide and folder-recursive.
        var packageId = dbContextInstance.DomainPackageModel.UnderlyingPackage.Id;

        return executionContext.MetadataManager.Domain(executionContext.GetApplicationConfig().Id)
            .GetClassModels()
            .Where(c => c.InternalElement.Package.Id == packageId)
            .SelectMany(c => c.Attributes)
            .Any(a => GeometryTypeNames.Contains(a.TypeReference.Element.Name));
    }
}
