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
        private readonly bool _serializeEnumsAsStrings;

        public ServiceProxyModelAdapter(ServiceProxyModel model, bool serializeEnumsAsStrings)
        {
            _model = model;
            _serializeEnumsAsStrings = serializeEnumsAsStrings;
        }

        public string Name => _model.Name;

        public string Id => _model.Id;

        public IMetadataModel UnderlyingModel => _model;

        public ServiceProxyModel Model => _model;

        public FolderModel Folder => _model.Folder;

        public bool SerializeEnumsAsStrings => _serializeEnumsAsStrings;

        public IEnumerable<IHttpEndpointModel> GetMappedEndpoints()
        {
            return ServiceProxyHelpers.GetMappedEndpoints(_model);
        }
    }



}
