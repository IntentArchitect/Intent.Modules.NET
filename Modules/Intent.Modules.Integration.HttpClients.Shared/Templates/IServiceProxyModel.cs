using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates
{
    public interface IServiceProxyModel : IMetadataModel, IHasFolder 
    {
        string Name { get; }
        IEnumerable<IHttpEndpointModel> GetMappedEndpoints();

        bool SerializeEnumsAsStrings { get; }

        public IMetadataModel UnderlyingModel { get; }
    }
}
