using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Contracts.Clients.Http.Shared;
using Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates;

internal static class ServiceProxyHelpers
{
    public static IList<IServiceProxyModel> GetServiceProxyModels(
        this IMetadataManager metadataManager,
        string applicationId,
        params Func<string, IDesigner>[] getDesigners)
    {
        var @explicit = getDesigners
            .SelectMany(getDesigner => getDesigner(applicationId).GetServiceProxyModels())
            .Select(IServiceProxyModel (p) => new ServiceProxyModelAdapter(p))
            .Where(x => x.Endpoints.Count > 0);

        var @implicit = metadataManager.GetImplicitHttpProxyEndpoints(applicationId, getDesigners)
            .Select(IServiceProxyModel (x) => new ImplicitServiceProxyModel(x));

        return @explicit.Concat(@implicit).ToArray();
    }
}