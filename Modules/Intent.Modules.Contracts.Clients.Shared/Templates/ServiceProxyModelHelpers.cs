//using System.Collections.Generic;
//using System.Linq;
//using Intent.Engine;
//using Intent.Metadata.Models;
//using Intent.Modelers.ServiceProxies.Api;
//using Intent.Modelers.Services.Api;
//using Intent.Modelers.Types.ServiceProxies.Api;
//using Intent.Modules.Application.Contracts.Clients.Templates.Adapters;
//using Intent.Modules.Contracts.Clients.Http.Shared;

//namespace Intent.Modules.Application.Contracts.Clients.Templates;

//// TODO JL:
//public static class ServiceProxyModelHelpers
//{
//    public static IEnumerable<IServiceProxyModel> GetServiceProxyModels(this IMetadataManager metadataManager, IApplication application)
//    {
//        const string callServiceOperationTypeId = "3e69085c-fa2f-44bd-93eb-41075fd472f8";

//        var serviceProxiesDesigner = metadataManager.ServiceProxies(application);
//        var servicesDesigner = metadataManager.Services(application);
//        var localPackageIds = servicesDesigner.Packages.Select(x => x.Id).ToHashSet();

//        var serviceProxies = Enumerable.Empty<ServiceProxyModel>()
//            .Concat(serviceProxiesDesigner.GetServiceProxyModels())
//            .Concat(servicesDesigner.GetServiceProxyModels())
//            .Where(p => p.HasMappedEndpoints());
//        var serviceProxiesBySourceId = serviceProxies
//            .GroupBy(x => x.InternalElement.MappedElement.ElementId)
//            .ToDictionary(x => x.Key, x => x.ToArray());
//        var callServiceOperationEndpointsByParentId = servicesDesigner.GetAssociationsOfType(callServiceOperationTypeId)
//            .Where(x =>
//            {
//                var targetElement = x.TargetEnd.TypeReference?.Element as IElement;

//                return targetElement?.Package.Id != null &&
//                       !localPackageIds.Contains(targetElement.Package.Id) &&
//                       targetElement.HasHttpSettings();
//            })
//            .Select(x => (IElement)x.TargetEnd.TypeReference.Element)
//            .GroupBy(x => x.ParentId)
//            .ToDictionary(x => x.Key, x => x.ToArray());

//        var serviceProxyModels = new List<IServiceProxyModel>();

//        foreach (var (key, models) in serviceProxiesBySourceId)
//        {
//            for (var i = 0; i < models.Length; i++)
//            {
//                var model = models[i];
//                var serializeEnumsAsStrings = ProxySettingsHelper.GetSerializeEnumsAsStrings(application, model) == true;

//                if (i != 0 || !callServiceOperationEndpointsByParentId.TryGetValue(key, out var implicitEndpoints))
//                {
//                    implicitEndpoints = [];
//                }

//                serviceProxyModels.Add(new ServiceProxyModelAdapter(model, serializeEnumsAsStrings, implicitEndpoints));
//            }
//        }

//        foreach (var (key, endpoints) in callServiceOperationEndpointsByParentId)
//        {
//            if (serviceProxiesBySourceId.ContainsKey(key))
//            {
//                continue;
//            }

//            serviceProxyModels.Add(new ImplicitServiceProxyModel(endpoints, application));
//        }

//        return serviceProxyModels;
//    }
//}