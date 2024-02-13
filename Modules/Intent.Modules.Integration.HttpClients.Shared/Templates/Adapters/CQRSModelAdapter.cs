using Intent.Metadata.Models;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters
{
    internal class CQRSModelAdapter : IServiceProxyModel
    {
        private readonly IElement _folder;
        public CQRSModelAdapter(IElement folder)
        {
            _folder = folder;
        }

        public string Name => _folder.Name;
        public string Id => _folder.Id;

        public IMetadataModel UnderlyingModel => null;

        public IEnumerable<IHttpEndpointModel> GetMappedEndpoints()
        {
            return _folder.ChildElements
                .Where(x => x?.TryGetHttpSettings(out _) == true)
                .Select(HttpEndpointModelFactory.GetEndpoint);
        }
    }
}
