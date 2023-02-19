using System.Linq;
using Intent.Engine;

namespace Intent.Modules.Eventing.GoogleCloud.PubSub;

public static class IntegrationCoordinator
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