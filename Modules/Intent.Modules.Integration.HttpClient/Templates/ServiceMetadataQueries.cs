using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using JetBrains.Annotations;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;
using ParameterModel = Intent.Modelers.Services.Api.ParameterModel;

namespace Intent.Modules.Integration.HttpClient.Templates;

// Since we have to cater for both WebAPI metadata and Azure Functions (and who knows what else in the future),
// I've decided to abstract those concerns in this class.
public static class ServiceMetadataQueries
{
    private const string StereotypeHttpServiceSettings = "Http Service Settings";
    private const string StereotypeHttpSettings = "Http Settings";
    private const string StereotypeAzureFunction = "Azure Function";
    private const string StereotypeHttpServiceParameterSettings = "Parameter Settings";
    private const string StereotypeAzureFunctionParameterSettings = "Parameter Setting";

    public static void Validate(IntentTemplateBase template, ServiceProxyModel serviceProxy)
    {
        foreach (var operation in serviceProxy.MappedService.Operations)
        {
            if (GetBodyParameter(template, operation) != null && GetFormUrlEncodedParameters(operation).Any())
            {
                throw new InvalidOperationException(
                    $"Service Proxy [{serviceProxy.Name}] is mapped to Service [{serviceProxy.MappedService.Name}] which has Operation [{operation.Name}] that has a FORM parameter and a BODY parameter.");
            }
        }
    }

    public static string GetRelativeUri(OperationModel operation)
    {
        var serviceRoute = GetRoute(operation.ParentService) ?? string.Empty;
        var serviceName = operation.ParentService.HasStereotype(StereotypeHttpServiceSettings)
            ? operation.ParentService.Name.RemoveSuffix("Controller", "Service")
            : string.Empty;
        serviceRoute = serviceRoute.Replace("[controller]", serviceName);

        if (string.IsNullOrWhiteSpace(serviceRoute))
        {
            serviceRoute = $"api{(!string.IsNullOrWhiteSpace(serviceName) ? "/" : string.Empty)}{serviceName}";
        }

        var operationRoute = GetRoute(operation) ?? string.Empty;
        operationRoute = operationRoute.Replace("[action]", operation.Name);

        return $"/{serviceRoute}{(!string.IsNullOrWhiteSpace(operationRoute) ? "/" : string.Empty)}{operationRoute}";
    }

    public static IReadOnlyCollection<ParameterModel> GetQueryParameters(IntentTemplateBase template, OperationModel operation)
    {
        if (operation.Parameters.Count(p => template.GetTypeInfo(p.TypeReference).IsPrimitive
                                            && GetSource(operation, p).IsDefault()) == 1)
        {
            return Array.Empty<ParameterModel>();
        }

        var route = GetRoute(operation);
        return operation.Parameters
            .Where(p => GetSource(operation, p).IsFromQuery() == true ||
                        (template.GetTypeInfo(p.TypeReference).IsPrimitive && GetSource(operation, p).IsDefault() &&
                         !route.Contains($"{{{p.Name.ToCamelCase()}}}")))
            .ToArray();
    }

    public record HeaderParameter(ParameterModel Parameter, string HeaderName);

    public static IReadOnlyCollection<HeaderParameter> GetHeaderParameters(OperationModel operation)
    {
        return operation.Parameters
            .Where(p => GetSource(operation, p).IsFromHeader() == true)
            .Select(s => new HeaderParameter(Parameter: s, HeaderName: GetHeaderName(operation, s)))
            .ToArray();
    }

    public static ParameterModel GetBodyParameter(IntentTemplateBase template, OperationModel operation)
    {
        return operation.Parameters
            .FirstOrDefault(p => GetSource(operation, p).IsFromBody() == true
                                 || (GetSource(operation, p).IsDefault() == true &&
                                     !template.GetTypeInfo(p.TypeReference).IsPrimitive));
    }

    public static IReadOnlyCollection<ParameterModel> GetFormUrlEncodedParameters(OperationModel operation)
    {
        return operation.Parameters
            .Where(p => GetSource(operation, p).IsFromForm() == true)
            .ToArray();
    }

    public static bool ShouldIncludeAccessTokenHandler(ServiceProxyModel proxyModel)
    {
        return proxyModel.MappedService.HasStereotype("Secured") || proxyModel.MappedService.Operations.Any(x => x.HasStereotype("Secured"));
    }

    public static string GetHttpVerb(OperationModel operation)
    {
        var webApiVerb = operation.GetStereotype(StereotypeHttpSettings)?.GetProperty<string>("Verb");
        if (webApiVerb != null)
        {
            return webApiVerb.ToLower().ToPascalCase();
        }

        var azFuncMethod = operation.GetStereotype(StereotypeAzureFunction)?.GetProperty<string>("Method");
        if (AzureFunctionIsHttpTrigger(operation) && azFuncMethod != null)
        {
            return azFuncMethod.ToLower().ToPascalCase();
        }

        return null;
    }

    private static string GetRoute(ServiceModel serviceModel)
    {
        var webApiRoute = serviceModel.GetStereotype(StereotypeHttpServiceSettings)?.GetProperty<string>("Route");
        if (webApiRoute != null)
        {
            return webApiRoute;
        }

        // Azure Function doesn't have Service-level Routes

        return null;
    }

    private static string GetRoute(OperationModel operationModel)
    {
        var webApiRoute = operationModel.GetStereotype(StereotypeHttpSettings)?.GetProperty<string>("Route");
        if (webApiRoute != null)
        {
            return webApiRoute;
        }

        var azFuncRoute = operationModel.GetStereotype(StereotypeAzureFunction)?.GetProperty<string>("Route");
        if (AzureFunctionIsHttpTrigger(operationModel) && azFuncRoute != null)
        {
            return azFuncRoute;
        }

        return null;
    }

    private static string GetSource(OperationModel operationModel, ParameterModel parameterModel)
    {
        var webApiSource = parameterModel.GetStereotype(StereotypeHttpServiceParameterSettings)?.GetProperty<string>("Source");
        if (webApiSource != null)
        {
            return webApiSource;
        }

        var azFuncSource = parameterModel.GetStereotype(StereotypeAzureFunctionParameterSettings)?.GetProperty<string>("Source");
        if (AzureFunctionIsHttpTrigger(operationModel) && azFuncSource != null)
        {
            return azFuncSource;
        }

        return null;
    }

    private static string GetHeaderName(OperationModel operationModel, ParameterModel parameterModel)
    {
        var webApiHeaderName = parameterModel.GetStereotype(StereotypeHttpServiceParameterSettings)?.GetProperty<string>("Header Name");
        if (webApiHeaderName != null)
        {
            return webApiHeaderName;
        }

        // Azure Functions Module doesn't have header support yet

        return null;
    }

    private static bool AzureFunctionIsHttpTrigger(OperationModel operationModel)
    {
        return operationModel.GetStereotype(StereotypeAzureFunction)?.GetProperty<string>("Type") == "Http Trigger";
    }

    private static bool IsDefault([CanBeNull] this string source)
    {
        return source == "Default";
    }

    private static bool IsFromQuery([CanBeNull] this string source)
    {
        return source == "From Query";
    }

    private static bool IsFromBody([CanBeNull] this string source)
    {
        return source == "From Body";
    }

    private static bool IsFromRoute([CanBeNull] this string source)
    {
        return source == "From Route";
    }

    private static bool IsFromHeader([CanBeNull] this string source)
    {
        return source == "From Header";
    }

    private static bool IsFromForm([CanBeNull] this string source)
    {
        return source == "From Form";
    }
}