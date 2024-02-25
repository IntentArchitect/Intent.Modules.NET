using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Contracts.Clients.Shared;

namespace Intent.Modules.Contracts.Clients.Http.Shared;

public class HttpServiceProxyMappedService : IServiceProxyMappedService
{
    public bool HasMappedEndpoints(ServiceProxyModel model)
    {
        return model.HasMappedEndpoints();
    }

    public IReadOnlyCollection<MappedEndpoint> GetMappedEndpoints(ServiceProxyModel model)
    {
        return model.GetMappedEndpoints()
            .Select(endpoint => new MappedEndpoint(
                Id: endpoint.Id,
                Name: endpoint.Name,
                TypeReference: endpoint.TypeReference,
                ReturnType: endpoint.ReturnType,
                Inputs: endpoint.Inputs
                    .Select(input => new MappedEndpointInput(input.Id, input.Name, input.TypeReference))
                    .ToArray(),
                InternalElement: endpoint.InternalElement))
            .ToArray();
    }
}