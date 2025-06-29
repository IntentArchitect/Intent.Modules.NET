using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Contracts.Clients.Http.Shared;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters
{
    internal class ServiceProxyModelAdapter : IServiceProxyModel
    {
        private readonly ServiceProxyModel _model;

        public ServiceProxyModelAdapter(ServiceProxyModel model)
        {
            _model = model;
            Endpoints = model.GetMappedEndpoints().ToArray();
        }

        public string Name => _model.Name;

        public string Id => _model.Id;

        public IMetadataModel UnderlyingModel => _model;

        public bool CreateParameterPerInput => true;

        public FolderModel Folder => _model.Folder;

        public IReadOnlyList<IHttpEndpointModel> Endpoints { get; }

        public IElement InternalElement => _model.InternalElement;
    }
}
