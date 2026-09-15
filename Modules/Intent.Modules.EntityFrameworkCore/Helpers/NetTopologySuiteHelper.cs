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
        return dbContextInstance.DomainPackageModel.Classes
            .SelectMany(c => c.Attributes)
            .Any(a => GeometryTypeNames.Contains(a.TypeReference.Element.Name));
    }
}
