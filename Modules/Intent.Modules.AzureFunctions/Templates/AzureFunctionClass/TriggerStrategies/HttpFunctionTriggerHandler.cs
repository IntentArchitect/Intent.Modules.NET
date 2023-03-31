using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

internal class HttpFunctionTriggerHandler : IFunctionTriggerHandler
{
    private readonly AzureFunctionClassTemplate _template;
    private readonly AzureFunctionModel _azureFunctionModel;

    public HttpFunctionTriggerHandler(AzureFunctionClassTemplate template, AzureFunctionModel azureFunctionModel)
    {
        _template = template;
        _azureFunctionModel = azureFunctionModel;
    }

    public void ApplyMethodParameters(CSharpClassMethod method)
    {
        method.AddParameter("HttpRequest", "req", param =>
        {
            param.AddAttribute("HttpTrigger", attr =>
            {
                var httpTriggersView = _azureFunctionModel.GetAzureFunction().GetHttpTriggerView();
                var route = !string.IsNullOrWhiteSpace(httpTriggersView.Route())
                    ? $@"""{httpTriggersView.Route()}"""
                    : @"""""";
                attr.AddArgument($"AuthorizationLevel.{httpTriggersView.AuthorizationLevel().Value}");
                attr.AddArgument(@$"""{httpTriggersView.Method().Value.ToLower()}""");
                attr.AddArgument($"Route = {route}");
            });
        });

        foreach (var parameterModel in _azureFunctionModel.Parameters.Where(IsParameterRoute))
        {
            method.AddParameter(_template.GetTypeName(parameterModel.Type), parameterModel.Name.ToParameterName());
        }

        method.AddParameter(_template.UseType("System.Threading.CancellationToken"), "cancellationToken");
    }

    public void ApplyMethodStatements(CSharpClassMethod method)
    {
        method.AddTryBlock(tryBlock =>
        {
            foreach (var param in GetQueryParams())
            {
                if (param.TypeReference.HasStringType())
                {
                    tryBlock.AddStatement($@"string {param.Name.ToParameterName()} = req.Query[""{param.Name.ToCamelCase()}""];");
                    continue;
                }

                tryBlock.AddStatement($@"{_template.GetTypeName(param.Type)} {param.Name.ToParameterName()} = {_template.GetAzureFunctionClassHelperName()}.{(param.Type.IsNullable ? "GetQueryParamNullable" : "GetQueryParam")}(""{param.Name.ToParameterName()}"", req.Query, (string val, out {_template.GetTypeName(param.Type).Replace("?", string.Empty)} parsed) => {_template.GetTypeName(param.Type).Replace("?", string.Empty)}.TryParse(val, out parsed));");
            }

            if (!string.IsNullOrWhiteSpace(GetRequestDtoType()))
            {
                tryBlock.AddStatement($@"var requestBody = await new StreamReader(req.Body).ReadToEndAsync();");
                tryBlock.AddStatement($@"var {GetRequestDtoParameterName()} = JsonConvert.DeserializeObject<{GetRequestDtoType()}>(requestBody);");
            }
        }).AddCatchBlock("FormatException", "exception", catchBlock =>
        {
            catchBlock.AddStatement($"return new BadRequestObjectResult(new {{ Message = exception.Message }});");
        });
    }

    public IEnumerable<INugetPackageInfo> GetNugetDependencies()
    {
        yield return NuGetPackages.MicrosoftExtensionsHttp;
    }

    private string GetRequestDtoParameterName()
    {
        return _azureFunctionModel.GetRequestDtoParameter().Name.ToParameterName();
    }

    private static bool IsParameterRoute(ParameterModel parameterModel)
    {
        return parameterModel.GetParameterSetting()?.Source().IsFromRoute() == true
               || (parameterModel.GetParameterSetting()?.Source().IsDefault() == true &&
                   parameterModel.TypeReference.Element.IsTypeDefinitionModel());
    }

    public string GetRequestDtoType()
    {
        var dtoParameter = _azureFunctionModel.GetRequestDtoParameter();
        return dtoParameter == null
            ? null
            : _template.GetTypeName(dtoParameter.TypeReference.Element.AsTypeReference());
    }

    private IEnumerable<ParameterModel> GetQueryParams()
    {
        return _azureFunctionModel.Parameters
            .Where(p => p.GetParameterSetting()?.Source().IsFromQuery() == true);
    }
}