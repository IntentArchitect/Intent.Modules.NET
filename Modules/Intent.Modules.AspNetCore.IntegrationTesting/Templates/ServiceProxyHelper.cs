using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters;

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates
{
    public static class ServiceProxyHelper
    {
        public static IEnumerable<IServiceProxyModel> GetServicesAsProxyModels(this IMetadataManager metadataManager, IApplication application)
        {
            var elementsGroupedByParent = Enumerable.Empty<IElement>()
                .Concat(metadataManager.Services(application).GetCommandModels()
                    .Where(x => x.HasHttpSettings())
                    .Select(x => x.InternalElement))
                .Concat(metadataManager.Services(application).GetQueryModels()
                    .Where(x => x.HasHttpSettings())
                    .Select(x => x.InternalElement))
                .GroupBy(x => x.ParentElement);

            return Enumerable.Empty<IServiceProxyModel>()
                    .Concat(elementsGroupedByParent
                        .Select(IServiceProxyModel (x) => new CQRSModelAdapter(x.Key)))
                    .Concat(metadataManager.Services(application)
                        .GetServiceModels()
                        .Where(s => s.Operations.Any(o => o?.TryGetHttpSettings(out _) == true))
                        .Select(IServiceProxyModel (s) => new ServiceModelAdapter(s)));
        }
    }
}
