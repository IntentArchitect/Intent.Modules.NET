using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates;
using Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Contracts.Clients.Http.Shared;

public static class ServiceProxyHelpers
{
    public static bool HasMappedEndpoints(this ServiceProxyModel model)
    {
        // Backwards compatibility - when we didn't have operations on service proxies
        if (model.Mapping?.Element?.IsServiceModel() == true && !model.Operations.Any())
        {
            return model.MappedService.Operations.Any(x => x.HasHttpSettings());
        }

        return model.Operations
            .Select(x => x.Mapping?.Element)
            .Cast<IElement>()
            .Any(x => x?.TryGetHttpSettings(out _) == true);
    }

    public static IEnumerable<IHttpEndpointModel> GetMappedEndpoints(this ServiceProxyModel model)
    {
        // Backwards compatibility - when we didn't have operations on service proxies
        if (model.Mapping?.Element?.IsServiceModel() == true && !model.Operations.Any())
        {
            return model.MappedService.Operations
                .Where(x => x.HasHttpSettings())
                .Select(x => HttpEndpointModelFactory.GetEndpoint(x.InternalElement));
        }

        return model.Operations
            .Select(x => x.Mapping?.Element)
            .Cast<IElement>()
            .Where(x => x?.TryGetHttpSettings(out _) == true)
            .Select(HttpEndpointModelFactory.GetEndpoint);
    }

    public static IEnumerable<IHttpEndpointModel> GetMappedEndpoints(ServiceProxyModel model, ISoftwareFactoryExecutionContext context)
    {
        // Backwards compatibility - when we didn't have operations on service proxies
        if (model.Mapping?.Element?.IsServiceModel() == true && !model.Operations.Any())
        {
            return model.MappedService.Operations
                .Where(x => x.HasHttpSettings())
                .Select(x =>
                {
                    context.TryGetHttpEndpoint(x.InternalElement, null, out var endpointModel);
                    return endpointModel!;
                });
        }

        return model.Operations
            .Select(x => x.Mapping?.Element)
            .Cast<IElement>()
            .Where(x => x?.TryGetHttpSettings(out _) == true)
            .Select(x =>
            {
                context.TryGetHttpEndpoint(x, null, out var endpointModel);
                return endpointModel!;
            });
    }

    public static IList<IServiceContractModel> GetServiceContractModels(
        this IMetadataManager metadataManager,
        string applicationId,
        params Func<string, IDesigner>[] getDesigners)
    {
        var @explicit = getDesigners
            .SelectMany(getDesigner => getDesigner(applicationId).GetServiceProxyModels())
            .Select(IServiceContractModel (p) => new HttpServiceContractModel(p))
            .Where(x => x.Operations.Count > 0);

        var @implicit = GetImplicitHttpProxyEndpoints(metadataManager, applicationId, getDesigners)
            .Select(IServiceContractModel (x) => new ImplicitServiceProxyContractModel(x));

        return @explicit.Concat(@implicit).ToArray();
    }

    /// <summary>
    /// Returns implicit http proxy endpoints (those with Invocation associations direct to the
    /// service without going via a service proxy element) grouped by their parent.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<IElement>> GetImplicitHttpProxyEndpoints(
        this IMetadataManager metadataManager,
        string applicationId,
        params Func<string, IDesigner>[] getDesigners)
    {
        var designers = getDesigners
            .Select(getDesigner => getDesigner(applicationId))
            .ToArray();

        const string callServiceOperationTypeId = "3e69085c-fa2f-44bd-93eb-41075fd472f8";
        var localPackageIds = designers
            .SelectMany(x => x.Packages)
            .Select(x => x.Id)
            .ToHashSet();

        return designers
            .SelectMany(x => x.GetAssociationsOfType(callServiceOperationTypeId))
            .Where(x =>
            {
                var targetElement = x.TargetEnd.TypeReference?.Element as IElement;

                return targetElement?.Package.Id != null &&
                       !localPackageIds.Contains(targetElement.Package.Id) &&
                       targetElement.HasHttpSettings();
            })
            .Select(x => (IElement)x.TargetEnd.TypeReference.Element)
            .GroupBy(x => x.ParentId)
            .Select(x => x.ToArray())
            .ToArray();
    }
}