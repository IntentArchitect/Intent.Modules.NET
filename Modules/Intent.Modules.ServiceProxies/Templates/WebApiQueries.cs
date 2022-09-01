using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.Templates;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;
using ParameterModel = Intent.Modelers.Services.Api.ParameterModel;

namespace Intent.Modules.ServiceProxies.Templates;

public static class WebApiQueries
{
    public static void Validate(ServiceProxyModel serviceProxy)
    {
        foreach (var operation in serviceProxy.MappedService.Operations)
        {
            if (GetBodyParameter(operation) != null && GetFormUrlEncodedParameters(operation).Any())
            {
                throw new InvalidOperationException(
                    $"Service Proxy [{serviceProxy.Name}] is mapped to Service [{serviceProxy.MappedService.Name}] which has Operation [{operation.Name}] that has a FORM parameter and a BODY parameter.");
            }
        }
    }

    public static string GetRelativeUri(OperationModel operation)
    {
        var serviceRoute = operation.ParentService.GetHttpServiceSettings()?.Route() ?? string.Empty;
        var serviceName = operation.ParentService.Name.RemoveSuffix("Controller", "Service");
        serviceRoute = serviceRoute.Replace("[controller]", serviceName);

        var operationRoute = operation.GetHttpSettings()?.Route() ?? string.Empty;
        operationRoute = operationRoute.Replace("[action]", operation.Name);

        return $"/{serviceRoute}/{operationRoute}";
    }

    public static IReadOnlyCollection<ParameterModel> GetQueryParameters(OperationModel operation)
    {
        return operation.Parameters
            .Where(p => p.GetParameterSettings()?.Source().IsFromQuery() == true)
            .ToArray();
    }

    public record HeaderParameter(ParameterModel Parameter, string HeaderName);

    public static IReadOnlyCollection<HeaderParameter> GetHeaderParameters(OperationModel operation)
    {
        return operation.Parameters
            .Where(p => p.GetParameterSettings()?.Source().IsFromHeader() == true)
            .Select(s => new HeaderParameter(Parameter: s, HeaderName: s.GetParameterSettings().HeaderName()))
            .ToArray();
    }

    public static ParameterModel GetBodyParameter(OperationModel operation)
    {
        return operation.Parameters
            .FirstOrDefault(p => p.GetParameterSettings()?.Source().IsFromBody() == true
                                 || (p.GetParameterSettings()?.Source().IsDefault() == true &&
                                     p.TypeReference.Element.IsDTOModel()));
    }

    public static IReadOnlyCollection<ParameterModel> GetFormUrlEncodedParameters(OperationModel operation)
    {
        return operation.Parameters
            .Where(p => p.GetParameterSettings()?.Source().IsFromForm() == true)
            .ToArray();
    }
}