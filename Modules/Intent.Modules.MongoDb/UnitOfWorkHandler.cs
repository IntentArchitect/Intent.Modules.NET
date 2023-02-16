using System.Linq;
using Intent.Engine;

namespace Intent.Modules.MongoDb;

// Just a simple way to coordinate what gets installed between different modules.
public static class UnitOfWorkHandler
{
    public static bool ShouldInstallStandardIntegration(IApplication application)
    {
        // For now we check if MediatR is going to be installed otherwise install standard
        return !ShouldInstallMediatRIntegration(application);
    }

    public static bool ShouldInstallMediatRIntegration(IApplication application)
    {
        return application.InstalledModules.Any(p => p.ModuleId == "Intent.Application.MediatR.Behaviours");
    }
}