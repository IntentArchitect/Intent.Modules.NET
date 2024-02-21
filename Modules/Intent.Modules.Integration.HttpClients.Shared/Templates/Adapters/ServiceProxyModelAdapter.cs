using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.Metadata.WebApi.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Contracts.Clients.Http.Shared;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters
{
    internal class ServiceProxyModelAdapter : IServiceProxyModel
    {
        private readonly ServiceProxyModel _model;
        public ServiceProxyModelAdapter(ServiceProxyModel model)
        {
            _model = model;
        }

        public string Name => _model.Name;

        public string Id => _model.Id;

        public IMetadataModel UnderlyingModel => _model;

        public FolderModel Folder => _model.Folder;

        public IEnumerable<IHttpEndpointModel> GetMappedEndpoints()
        {
            return ServiceProxyHelpers.GetMappedEndpoints(_model);
        }
    }



}
