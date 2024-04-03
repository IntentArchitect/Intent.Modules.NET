using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            return
                elementsGroupedByParent
                    .Select(grouping => new CQRSModelAdapter(grouping.Key))
                    .Cast<IServiceProxyModel>()
                .Concat(
                    metadataManager.Services(application)
                        .GetServiceModels()
                        .Where(s => s.Operations.Any(o => o?.TryGetHttpSettings(out _) == true))
                        .Select(s => new ServiceModelAdapter(s))
                        .Cast<IServiceProxyModel>()
                        )
                ;

        }
    }
}
