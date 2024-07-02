using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters
{
    internal class ServiceModelAdapter : IServiceProxyModel
    {
        private readonly ServiceModel _model;
        private readonly bool _serializeEnumsAsStrings;
        public ServiceModelAdapter(ServiceModel model, bool serializeEnumsAsStrings = false)
        {
            _serializeEnumsAsStrings = serializeEnumsAsStrings;
            _model = model;
            Folder = ((model.InternalElement.ParentElement?.SpecializationTypeId == "4d95d53a-8855-4f35-aa82-e312643f5c5f") ? new FolderModel(model.InternalElement.ParentElement) : null);
        }
        public string Name => _model.Name;
        public string Id => _model.Id;

        public IMetadataModel UnderlyingModel => _model;

        public bool SerializeEnumsAsStrings => _serializeEnumsAsStrings;

        public FolderModel Folder { get; }

        public IEnumerable<IHttpEndpointModel> GetMappedEndpoints()
        {
            return _model.Operations
                .Select(x => x.InternalElement)
                .Where(x => x?.TryGetHttpSettings(out _) == true)
                .Select(HttpEndpointModelFactory.GetEndpoint);
        }
    }
}
