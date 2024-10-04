using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
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
            //ApiSettings.SerializeEnumsAsStrings
            var serializeAsEnums = bool.TryParse(application.Settings.GetGroup("4bd0b4e9-7b53-42a9-bb4a-277abb92a0eb").GetSetting("97f3a1e3-2455-41e8-b28e-709f2db04230")?.Value.ToPascalCase(), out var result) && result; 

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
                    .Select(grouping => new CQRSModelAdapter(grouping.Key, serializeAsEnums))
                    .Cast<IServiceProxyModel>()
                .Concat(
                    metadataManager.Services(application)
                        .GetServiceModels()
                        .Where(s => s.Operations.Any(o => o?.TryGetHttpSettings(out _) == true))
                        .Select(s => new ServiceModelAdapter(s, serializeAsEnums))
                        .Cast<IServiceProxyModel>()
                        )
                ;

        }
    }
}
