using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters
{
    internal class CQRSModelAdapter : IServiceProxyModel
    {
        private readonly bool _serializeEnumsAsStrings;
        public CQRSModelAdapter(IElement folder)
        {
            InternalElement = folder;
            Folder = folder.AsFolderModel();
        }

        public string Name => InternalElement.Name;
        public string Id => InternalElement.Id;

        public IMetadataModel UnderlyingModel => null;

        public FolderModel Folder { get; }

        public IEnumerable<IHttpEndpointModel> GetMappedEndpoints()
        {
            return InternalElement.ChildElements
                .Where(x => x?.TryGetHttpSettings(out _) == true)
                .Select(HttpEndpointModelFactory.GetEndpoint);
        }

        public IElement InternalElement { get; }
    }
}
