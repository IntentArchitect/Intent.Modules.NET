using System.Linq;
using Intent.Engine;

namespace Intent.Modules.EntityFrameworkCore.Helpers;

public static class NetTopologySuiteHelper
{
    public static bool IsInstalled(ISoftwareFactoryExecutionContext executionContext)
    {
        return executionContext.InstalledModules.Any(mod => mod.ModuleId == "Intent.NetTopologySuite");
    }
}