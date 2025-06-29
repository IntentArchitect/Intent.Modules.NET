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
        IReadOnlyList<IHttpEndpointModel> Endpoints { get; }
        public IMetadataModel UnderlyingModel { get; }

        /// <summary>
        /// Historically the signature of the generated service proxies had a parameter per input which
        /// is annoying for CQRS. When <see langword="true"/> the legacy scheme it used, otherwise for
        /// CQRS there is a single parameter for the command/query.
        /// </summary>
        bool CreateParameterPerInput { get; }
    }
}
