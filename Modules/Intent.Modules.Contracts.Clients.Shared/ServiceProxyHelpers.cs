using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Contracts.Clients.Shared;

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

    public static string GetNamespace<TModel>(
        this CSharpTemplateBase<TModel> template,
        ServiceProxyModel serviceProxyModel)
        where TModel : IHasFolder
    {
        return string.Join('.', template.GetNamespaceParts(serviceProxyModel));
    }

    public static string GetFolderPath<TModel>(
        this CSharpTemplateBase<TModel> template,
        ServiceProxyModel serviceProxyModel)
        where TModel : IHasFolder
    {
        return string.Join('/', template.GetNamespaceParts(serviceProxyModel));
    }

    private static IEnumerable<string> GetNamespaceParts<TModel>(
        this CSharpTemplateBase<TModel> template,
        ServiceProxyModel serviceProxyModel)
        where TModel : IHasFolder
    {
        var modelNamespace = template.Model.GetParentFolders()
            .Where(x => !string.IsNullOrWhiteSpace(x.Name) &&
                        (!x.HasFolderOptions() || x.GetFolderOptions().NamespaceProvider()))
            .Select(x => x.Name);

        return template.OutputTarget.GetNamespace().Split('.')
            .Append(serviceProxyModel.Name.ToPascalCase())
            .Concat(modelNamespace);
    }
}