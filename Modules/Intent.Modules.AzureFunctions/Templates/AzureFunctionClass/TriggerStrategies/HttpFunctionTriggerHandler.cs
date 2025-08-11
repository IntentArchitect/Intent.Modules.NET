using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.AzureFunctions.Api;
using Intent.Modules.AzureFunctions.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Utils;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

internal class HttpFunctionTriggerHandler : IFunctionTriggerHandler
{
    private readonly ICSharpFileBuilderTemplate _template;
    private readonly IAzureFunctionModel _azureFunctionModel;
    private readonly IHttpEndpointModel _endpointModel;

    public HttpFunctionTriggerHandler(ICSharpFileBuilderTemplate template, IAzureFunctionModel azureFunctionModel)
    {
        _template = template;
        _azureFunctionModel = azureFunctionModel;
        _endpointModel = HttpEndpointModelFactory.GetEndpoint(_azureFunctionModel.InternalElement, "");
        if (_endpointModel == null)
        {
            Logging.Log.Warning($"Http Settings could not be found on Azure Function [{_azureFunctionModel.Name}] that is Http triggered");
        }
    }

    public void ApplyMethodParameters(CSharpClassMethod method)
    {
        method.AddParameter(_template.UseType("Microsoft.AspNetCore.Http.HttpRequest"), "req", param =>
        {
            param.AddAttribute("HttpTrigger", attr =>
            {
                var route = !string.IsNullOrWhiteSpace(_endpointModel?.Route)
                    ? $@"""{_endpointModel.Route}"""
                    : @"""""";

                if (route.Contains("{"))
                {
                    //Asp.Net Core is happy with it all lowercase but Azure functions need the correct casing for parameters
                    route = FixParamters(GetRouteParams().Select(p => p.Name.ToParameterName()), route);
                }
                
                var authorizationLevel = AzureFunctionsHelper.GetAzureFunctionsProcessType(_template.OutputTarget) == 
                                         AzureFunctionsHelper.AzureFunctionsProcessType.InProcess
                    ? _template.UseType("Microsoft.Azure.WebJobs.Extensions.Http.AuthorizationLevel")
                    : _template.UseType("Microsoft.Azure.Functions.Worker.AuthorizationLevel");
                
                attr.AddArgument($"{authorizationLevel}.{_azureFunctionModel.AuthorizationLevel}");
                attr.AddArgument(@$"""{_endpointModel?.Verb.ToString().ToLower() ?? "get"}""");
                attr.AddArgument($"Route = {route}");
            });
        });

        foreach (var parameterModel in GetRouteParams())
        {
            var paramName = parameterModel.Name.ToParameterName();
            var paramResult = parameterModel.TypeReference switch
            {
                var type when type.IsNullable && type.Element.IsEnumModel() => (Type: "string" + NullableSymbol, ReferalVar: $"{paramName}Enum"),
                var type when !type.IsNullable && type.Element.IsEnumModel() => (Type: "string", ReferalVar: $"{paramName}Enum"),
                var type => (Type: _template.GetTypeName(type), ReferalVar: null)
            };
            method.AddParameter(paramResult.Type, paramName, param =>
            {
                if (paramResult.ReferalVar is not null)
                {
                    param.AddMetadata("referralVar", paramResult.ReferalVar);
                }
                param.AddMetadata("model", parameterModel);
            });
        }

        method.AddParameter(_template.UseType("System.Threading.CancellationToken"), "cancellationToken");
    }

    private static readonly Regex RouteParameterRegex = new(@"\{(\w+)\}", RegexOptions.Compiled);
    private string FixParamters(IEnumerable<string> parameterNames, string route)
    {
        return RouteParameterRegex.Replace(route, match =>
        {
            string paramNameLower = match.Groups[1].Value;
            string correctParam = parameterNames.FirstOrDefault(p => p.Equals(paramNameLower, StringComparison.OrdinalIgnoreCase));
            return correctParam != null ? $"{{{correctParam}}}" : match.Value;
        });
    }

    private string NullableSymbol => _template.OutputTarget.GetProject().NullableEnabled ? "?" : string.Empty;

    public void ApplyMethodStatements(CSharpClassMethod method)
    {
        if (_template.ExecutionContext.Settings.GetAzureFunctionsSettings().UseGlobalExceptionMiddleware())
        {
            AddBodyStatements(method, method);
        }
        else
        {
            method.AddTryBlock(tryBlock =>
            {
                AddBodyStatements(tryBlock, method);
            }).AddCatchBlock("FormatException", "exception", catchBlock => { catchBlock.AddStatement($"return new BadRequestObjectResult(new {{ exception.Message }});"); });
        }
    }

    private void AddBodyStatements(IHasCSharpStatements statementBlock, CSharpClassMethod method)
    {
        foreach (var param in GetQueryParams())
        {
            ConvertParamToLocalVariable(statementBlock, param, "Query");
        }

        foreach (var param in GetHeaderParams())
        {
            ConvertParamToLocalVariable(statementBlock, param, "Headers");
        }

        foreach (var param in GetRouteParams())
        {
            if (param.TypeReference.Element.IsEnumModel())
            {
                statementBlock.AddStatement(
                    $"var {param.Name.ToParameterName()}Enum = {_template.GetAzureFunctionClassHelperName()}.{(param.TypeReference.IsNullable ? "GetEnumParamNullable" : "GetEnumParam")}<{_template.GetTypeName(param.TypeReference.Element.AsTypeReference())}>(nameof({param.Name.ToParameterName()}), {param.Name.ToParameterName()});");
            }
        }

        if (!string.IsNullOrWhiteSpace(GetRequestDtoType()))
        {
            statementBlock.AddInvocationStatement(
                $"var {GetRequestInput().Name} = await {_template.GetAzureFunctionClassHelperName()}.DeserializeJsonContentAsync<{GetRequestDtoType()}>", inv => inv
                    .AddArgument("req.Body")
                    .AddArgument("cancellationToken"));

            if (statementBlock != method)
            {
                method.AddCatchBlock(_template.UseType("System.Text.Json.JsonException"), "exception", catchBlock =>
                {
                    catchBlock.AddStatement($"return new BadRequestObjectResult(new {{ exception.Message }});");
                });
            }
        }
    }

    private void ConvertParamToLocalVariable(IHasCSharpStatements tryBlock, IHttpEndpointInputModel param, string paramType)
    {
        if (param.TypeReference.HasStringType())
        {
            if (param.TypeReference.IsCollection)
            {
                _template.AddUsing("System.Linq");
                tryBlock.AddStatement($@"var {param.Name.ToParameterName()} = ((string)req.{paramType}[""{param.Name.ToCamelCase()}""]).Split(',').ToList();");
            }
            else
            {
                tryBlock.AddStatement($@"string {param.Name.ToParameterName()} = req.{paramType}[""{param.Name.ToCamelCase()}""];");
            }

            return;
        }

        if (param.TypeReference.IsCollection)
        {
            _template.AddUsing("System.Linq");

            var itemType = _template.GetTypeName(param.TypeReference, "{0}");
            tryBlock.AddStatement(
                $@"var {param.Name.ToParameterName()} = {_template.GetAzureFunctionClassHelperName()}.Get{paramType}ParamCollection(""{param.Name.ToParameterName()}""
                    , req.{paramType}
                    , (string val, out {itemType} parsed) => {itemType}.TryParse(val, out parsed)).ToList();
");
            return;
        }

        tryBlock.AddStatement(
            $@"{_template.GetTypeName(param.TypeReference)} {param.Name.ToParameterName()} = {_template.GetAzureFunctionClassHelperName()}.{(param.TypeReference.IsNullable ? $"Get{paramType}ParamNullable" : $"Get{paramType}Param")}(""{param.Name.ToParameterName()}"", req.{paramType}, (string val, out {_template.GetTypeName(param.TypeReference).Replace("?", string.Empty)} parsed) => {_template.GetTypeName(param.TypeReference).Replace("?", string.Empty)}.TryParse(val, out parsed));");
    }

    public IEnumerable<INugetPackageInfo> GetNugetDependencies()
    {
        yield return NugetPackages.MicrosoftExtensionsHttp(_template.OutputTarget);
    }

    public IEnumerable<INugetPackageInfo> GetNugetRedundantDependencies()
    {
        return [];
    }

    private IAzureFunctionParameterModel GetRequestInput()
    {
        return _azureFunctionModel.Parameters.FirstOrDefault(x => x.InputSource == HttpInputSource.FromBody);
    }

    public string GetRequestDtoType()
    {
        var dtoParameter = GetRequestInput();
        return dtoParameter == null
            ? null
            : _template.GetTypeName(dtoParameter.TypeReference, "System.Collections.Generic.List<{0}>");
    }

    private IEnumerable<IHttpEndpointInputModel> GetQueryParams()
    {
        return _endpointModel.Inputs.Where(x => x.Source == HttpInputSource.FromQuery);
    }

    private IEnumerable<IHttpEndpointInputModel> GetHeaderParams()
    {
        return _endpointModel.Inputs.Where(x => x.Source == HttpInputSource.FromHeader);
    }

    private IEnumerable<IHttpEndpointInputModel> GetRouteParams()
    {
        return _endpointModel.Inputs.Where(x => x.Source is HttpInputSource.FromRoute);
    }
}