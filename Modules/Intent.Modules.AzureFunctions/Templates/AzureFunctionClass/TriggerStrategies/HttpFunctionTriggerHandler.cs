using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

internal class HttpFunctionTriggerHandler : IFunctionTriggerHandler
{
    private readonly AzureFunctionClassTemplate _template;
    private readonly OperationModel _operationModel;

    public HttpFunctionTriggerHandler(AzureFunctionClassTemplate template, OperationModel operationModel)
    {
        _template = template;
        _operationModel = operationModel;
    }

    public IEnumerable<string> GetMethodParameterDefinitionList()
    {
        var paramList = new List<string>();

        var httpTriggersView = _operationModel.GetAzureFunction().GetHttpTriggerView();
        var method = @$"""{httpTriggersView.Method().Value.ToLower()}""";
        var route = !string.IsNullOrWhiteSpace(httpTriggersView.Route())
            ? $@"""{httpTriggersView.Route()}"""
            : @"""""";
        paramList.Add(
            @$"[HttpTrigger(AuthorizationLevel.{httpTriggersView.AuthorizationLevel().Value}, {method}, Route = {route})] HttpRequest req");

        foreach (var parameterModel in _operationModel.Parameters.Where(IsParameterRoute))
        {
            paramList.Add($@"{_template.GetTypeName(parameterModel.Type)} {parameterModel.Name.ToParameterName()}");
        }

        return paramList;
    }

    public IEnumerable<string> GetRunMethodEntryStatementList()
    {
        var statementList = new List<string>();
        
        foreach (var param in GetQueryParams())
        {
            if (_template.GetTypeName(param.Type) == "string")
            {
                statementList.Add(
                    $@"string {param.Name.ToParameterName()} = req.Query[""{param.Name.ToCamelCase()}""];");
                continue;
            }

            statementList.Add(
                $@"{_template.GetTypeName(param.Type)} {param.Name.ToParameterName()} = {_template.GetAzureFunctionClassHelperName()}.{(param.Type.IsNullable ? "GetQueryParamNullable" : "GetQueryParam")}(""{param.Name.ToParameterName()}"", req.Query, (string val, out {_template.GetTypeName(param.Type).Replace("?", string.Empty)} parsed) => {_template.GetTypeName(param.Type).Replace("?", string.Empty)}.TryParse(val, out parsed));");
        }

        if (!string.IsNullOrWhiteSpace(GetRequestDtoType()))
        {
            statementList.Add($@"var requestBody = await new StreamReader(req.Body).ReadToEndAsync();");
            statementList.Add($@"var {GetRequestDtoParameterName()} = JsonConvert.DeserializeObject<{GetRequestDtoType()}>(requestBody);");
        }

        return statementList;
    }

    public IEnumerable<INugetPackageInfo> GetNugetDependencies()
    {
        yield return NuGetPackages.MicrosoftExtensionsHttp;
    }

    public IEnumerable<ExceptionCatchBlock> GetExceptionCatchBlocks()
    {
        yield return new ExceptionCatchBlock("FormatException exception")
            .AddStatementLines("return new BadRequestObjectResult(new { Message = exception.Message });");
    }

    private string GetRequestDtoParameterName()
    {
        return _operationModel.GetRequestDtoParameter().Name.ToParameterName();
    }

    private static bool IsParameterRoute(ParameterModel parameterModel)
    {
        return parameterModel.GetParameterSetting()?.Source().IsFromRoute() == true
               || (parameterModel.GetParameterSetting()?.Source().IsDefault() == true &&
                   !parameterModel.TypeReference.Element.IsDTOModel());
    }

    public string GetRequestDtoType()
    {
        var dtoParameter = _operationModel.GetRequestDtoParameter();
        return dtoParameter == null
            ? null
            : _template.GetDtoModelName(dtoParameter.TypeReference.Element.AsDTOModel());
    }

    private IEnumerable<ParameterModel> GetQueryParams()
    {
        return _operationModel.Parameters
            .Where(p => p.GetParameterSetting()?.Source().IsFromQuery() == true);
    }
}