using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.Metadata.WebApi.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

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

        public IEnumerable<IHttpEndpointModel> GetMappedEndpoints()
        {
            return ServiceProxyHelpers.GetMappedEndpoints(_model);
        }
    }

    internal class ServiceModelAdapter : IServiceProxyModel
    {
        private readonly ServiceModel _model;
        public ServiceModelAdapter(ServiceModel model)
        {
            _model = model;
        }

        public string Name => _model.Name;

        public string Id => _model.Id;

        public IEnumerable<IHttpEndpointModel> GetMappedEndpoints()
        {
            return _model.Operations
                .Select(x => x.InternalElement)
                .Where(x => x?.TryGetHttpSettings(out _) == true)
                .Select(HttpEndpointModelFactory.GetEndpoint);
        }
    }

    internal class CQRSModelAdapter : IServiceProxyModel
    {
        private readonly IElement _folder;
        public CQRSModelAdapter(IElement folder)
        {
            _folder = folder;
        }

        public string Name => _folder.Name;

        public string Id => _folder.Id;

        public IEnumerable<IHttpEndpointModel> GetMappedEndpoints()
        {
            return _folder.ChildElements
                .Where(x => x?.TryGetHttpSettings(out _) == true)
                .Select(HttpEndpointModelFactory.GetEndpoint);
        }
    }


}
