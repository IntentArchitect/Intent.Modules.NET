using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates
{
    public interface IServiceProxyModel : IMetadataModel, IHasFolder, IElementWrapper
    {
        string Name { get; }
        IEnumerable<IHttpEndpointModel> GetMappedEndpoints();
        public IMetadataModel UnderlyingModel { get; }
    }
}
