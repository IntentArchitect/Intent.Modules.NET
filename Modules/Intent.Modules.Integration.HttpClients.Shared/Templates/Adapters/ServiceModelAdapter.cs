using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters
{
    internal class ServiceModelAdapter : IServiceProxyModel
    {
        private readonly ServiceModel _model;
        public ServiceModelAdapter(ServiceModel model)
        {
            _model = model;
            Folder = model.InternalElement.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId
                ? new FolderModel(model.InternalElement.ParentElement)
                : null;
            Endpoints = model.Operations
                .Select(x => x.InternalElement)
                .Where(x => x?.TryGetHttpSettings(out _) == true)
                .Select(HttpEndpointModelFactory.GetEndpoint)
                .ToArray();
        }

        public string Name => _model.Name;

        public string Id => _model.Id;

        public IMetadataModel UnderlyingModel => _model;

        public bool CreateParameterPerInput => true;

        public FolderModel? Folder { get; }

        public IReadOnlyList<IHttpEndpointModel> Endpoints { get; }

        public IElement InternalElement => _model.InternalElement;
    }
}
