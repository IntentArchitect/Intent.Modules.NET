using Intent.Metadata.Models;
using Intent.Modules.Metadata.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates
{
    public interface IServiceProxyModel : IMetadataModel
    {
        string Name { get; }
        IEnumerable<IHttpEndpointModel> GetMappedEndpoints();

        public IMetadataModel UnderlyingModel { get; }
    }
}
