using System.Collections.Generic;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Metadata.WebApi.Models;
using SharedServiceProxyHelpers = Intent.Modules.Contracts.Clients.Shared.ServiceProxyHelpers;

namespace Intent.Modules.Application.Contracts.Clients;

public static class ServiceProxyHelpers
{
    public static bool HasMappedEndpoints(this ServiceProxyModel model) => SharedServiceProxyHelpers.HasMappedEndpoints(model);

    public static IEnumerable<IHttpEndpointModel> GetMappedEndpoints(this ServiceProxyModel model) => SharedServiceProxyHelpers.GetMappedEndpoints(model);
}