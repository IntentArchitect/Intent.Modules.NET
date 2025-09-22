using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Contracts.Clients.Http.Shared;

public class HttpServiceContractModel : IServiceContractModel
{
    private readonly ServiceProxyModel _serviceProxyModel;

    public HttpServiceContractModel(ServiceProxyModel serviceProxyModel)
    {
        _serviceProxyModel = serviceProxyModel;
        var operationsByMappedElementId = serviceProxyModel.Operations
            .Where(x => x.Mapping.ElementId != null)
            .ToDictionary(x => x.Mapping.ElementId);

        Operations = _serviceProxyModel.GetMappedEndpoints()
            .Select(x => new OperationModel(x, operationsByMappedElementId.GetValueOrDefault(x.Id)))
            .ToArray();
    }

    public string Id => _serviceProxyModel.Id;
    public string Name => _serviceProxyModel.Name;
    public IElement InternalElement => _serviceProxyModel.InternalElement;
    public IReadOnlyList<IServiceContractOperationModel?> Operations { get; }

    private class OperationModel : IServiceContractOperationModel
    {
        private readonly IHttpEndpointModel _httpEndpointModel;

        // This can be null for legacy service proxies where the individual operations weren't mapped
        // an in such cases all the operations are implicitly mapped.
        private readonly Intent.Modelers.Types.ServiceProxies.Api.OperationModel? _operationModel;

        public OperationModel(
            IHttpEndpointModel httpEndpointModel,
            Intent.Modelers.Types.ServiceProxies.Api.OperationModel? operationModel)
        {
            _httpEndpointModel = httpEndpointModel;
            _operationModel = operationModel;
            Parameters = _httpEndpointModel.Inputs
                .Select(x => new ParameterModel(x))
                .ToArray();
        }

        public string Id => _operationModel?.Id ?? _httpEndpointModel.Id;
        public string Name => _httpEndpointModel.Name;
        public IElement InternalElement => _operationModel?.InternalElement ?? _httpEndpointModel.InternalElement;
        public ITypeReference TypeReference => _httpEndpointModel.TypeReference;
        public IReadOnlyList<IServiceContractOperationParameterModel> Parameters { get; }
    }

    private class ParameterModel(IHttpEndpointInputModel input) : IServiceContractOperationParameterModel
    {
        public string Id => input.Id;
        public string Name => input.Name;
        public ITypeReference TypeReference => input.TypeReference;
    }

    public FolderModel Folder => _serviceProxyModel.Folder;
}