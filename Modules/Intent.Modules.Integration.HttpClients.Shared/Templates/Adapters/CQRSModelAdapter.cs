using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters
{
    internal class CQRSModelAdapter : IServiceProxyModel
    {
        public CQRSModelAdapter(IElement folder)
        {
            InternalElement = folder;
            Folder = folder.AsFolderModel();
            Endpoints = folder.ChildElements
                .Where(x => x?.TryGetHttpSettings(out _) == true)
                .Select(HttpEndpointModelFactory.GetEndpoint)
                .ToArray();
        }

        public string Name => InternalElement.Name;
        public string Id => InternalElement.Id;

        public IMetadataModel UnderlyingModel => null;
        public bool CreateParameterPerInput => true;

        public FolderModel Folder { get; }

        public IReadOnlyList<IHttpEndpointModel> Endpoints { get; }

        public IElement InternalElement { get; }
    }
}
