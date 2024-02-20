using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;

namespace Intent.Modules.Contracts.Clients.Shared
{
    public interface IServiceProxyMappedService
    {
        bool HasMappedEndpoints(ServiceProxyModel model);
        IReadOnlyCollection<MappedEndpoint> GetMappedEndpoints(ServiceProxyModel model);
    }

    public record MappedEndpoint(
        string Id,
        string Name,
        ITypeReference TypeReference,
        ITypeReference? ReturnType,
        IReadOnlyCollection<MappedEndpointInput> Inputs) : IHasName, IHasTypeReference, IMetadataModel;

    public record MappedEndpointInput(string Name, ITypeReference TypeReference) : IHasName, IHasTypeReference;
}