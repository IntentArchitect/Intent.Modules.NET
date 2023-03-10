using System.Linq;
using Intent.Engine;

namespace Intent.Modules.Eventing.GoogleCloud.PubSub;

public static class InteropCoordinator
{
    public static bool ShouldInstallStandardInterop(IApplication application)
    {
        // For now we check if MediatR is going to be installed otherwise install standard
        return !ShouldInstallMediatRInterop(application);
    }

    public static bool ShouldInstallMediatRInterop(IApplication application)
    {
        return application.InstalledModules.Any(p => p.ModuleId == "Intent.Application.MediatR.Behaviours");
    }

    public static bool ShouldInstallMongoDbInterop(IApplication application)
    {
        return application.InstalledModules.Any(p => p.ModuleId == "Intent.MongoDb");
    }
}