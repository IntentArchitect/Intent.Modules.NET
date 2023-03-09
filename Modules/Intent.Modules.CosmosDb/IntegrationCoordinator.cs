using System.Linq;
using Intent.Engine;

namespace Intent.CosmosDb;

// Just a simple way to coordinate what gets installed between different modules.
public static class IntegrationCoordinator
{
	public static bool ShouldInstallMediatRIntegration(IApplication application)
	{
		return application.InstalledModules.Any(p => p.ModuleId == "Intent.Application.MediatR.Behaviours");
	}
}