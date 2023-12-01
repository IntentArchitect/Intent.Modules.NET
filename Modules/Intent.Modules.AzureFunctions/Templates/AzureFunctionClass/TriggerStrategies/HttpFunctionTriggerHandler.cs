using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
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
                attr.AddArgument($"{_template.UseType("Microsoft.Azure.WebJobs.Extensions.Http.AuthorizationLevel")}.{_azureFunctionModel.AuthorizationLevel}");
                attr.AddArgument(@$"""{_endpointModel?.Verb.ToString().ToLower() ?? "get"}""");
                attr.AddArgument($"Route = {route}");
            });
        });

        foreach (var parameterModel in _endpointModel.Inputs.Where(x => x.Source is HttpInputSource.FromRoute))
        {
            method.AddParameter(_template.GetTypeName(parameterModel.TypeReference), parameterModel.Name.ToParameterName());
        }

        method.AddParameter(_template.UseType("System.Threading.CancellationToken"), "cancellationToken");
    }

    public void ApplyMethodStatements(CSharpClassMethod method)
    {
        method.AddTryBlock(tryBlock =>
        {
            foreach (var param in GetQueryParams())
            {
                ConvertParamToLocalVariable(tryBlock, param, "Query");
            }

            foreach (var param in GetHeaderParams())
            {
                ConvertParamToLocalVariable(tryBlock, param, "Headers");
            }


            if (!string.IsNullOrWhiteSpace(GetRequestDtoType()))
            {
                tryBlock.AddStatement($@"var requestBody = await new StreamReader(req.Body).ReadToEndAsync();");
                tryBlock.AddStatement($@"var {GetRequestInput().Name} = {_template.UseType("System.Text.Json.JsonSerializer")}.Deserialize<{GetRequestDtoType()}>(requestBody, new JsonSerializerOptions {{PropertyNameCaseInsensitive = true}})!;");
            }
        }).AddCatchBlock("FormatException", "exception", catchBlock =>
        {
            catchBlock.AddStatement($"return new BadRequestObjectResult(new {{ Message = exception.Message }});");
        });
    }

    private void ConvertParamToLocalVariable(CSharpTryBlock tryBlock, IHttpEndpointInputModel param, string paramType)
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
            tryBlock.AddStatement($@"var {param.Name.ToParameterName()} = {_template.GetAzureFunctionClassHelperName()}.Get{paramType}ParamCollection(""{param.Name.ToParameterName()}""
                    , req.{paramType}
                    , (string val, out {itemType} parsed) => {itemType}.TryParse(val, out parsed)).ToList();
");
        }
        else
        {
            tryBlock.AddStatement($@"{_template.GetTypeName(param.TypeReference)} {param.Name.ToParameterName()} = {_template.GetAzureFunctionClassHelperName()}.{(param.TypeReference.IsNullable ? $"Get{paramType}ParamNullable" : $"Get{paramType}Param")}(""{param.Name.ToParameterName()}"", req.{paramType}, (string val, out {_template.GetTypeName(param.TypeReference).Replace("?", string.Empty)} parsed) => {_template.GetTypeName(param.TypeReference).Replace("?", string.Empty)}.TryParse(val, out parsed));");
        }
    }

    public IEnumerable<INugetPackageInfo> GetNugetDependencies()
    {
        yield return NuGetPackages.MicrosoftExtensionsHttp;
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
}