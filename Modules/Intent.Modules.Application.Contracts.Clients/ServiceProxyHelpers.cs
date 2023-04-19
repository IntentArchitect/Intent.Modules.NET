using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Application.Contracts.Clients;

public static class ServiceProxyHelpers
{
    public static bool HasMappedEndpoints(this ServiceProxyModel model)
    {
        if (model.MappedService != null)
        {
            return model.MappedService.Operations.Any(x => x.HasHttpSettings());
        }

        return model.Operations
            .Select(x => x.Mapping?.Element)
            .Cast<IElement>()
            .Any(x => x?.TryGetHttpServiceSettings(out _) == true);
    }

    public static IEnumerable<IHttpEndpointModel> GetMappedEndpoints(this ServiceProxyModel model)
    {
        if (model.MappedService != null)
        {
            return model.MappedService.Operations
                .Where(x => x.HasHttpSettings())
                .Select(x => HttpEndpointModelFactory.GetEndpoint(x.InternalElement));
        }

        return model.Operations
            .Select(x => x.Mapping?.Element)
            .Cast<IElement>()
            .Where(x => x?.TryGetHttpServiceSettings(out _) == true)
            .Select(HttpEndpointModelFactory.GetEndpoint);
    }
}