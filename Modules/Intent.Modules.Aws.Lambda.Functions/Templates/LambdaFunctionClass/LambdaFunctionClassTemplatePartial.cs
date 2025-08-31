using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Aws.Lambda.Functions.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Templates.LambdaFunctionClass;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public partial class LambdaFunctionClassTemplate : CSharpTemplateBase<ILambdaFunctionContainerModel>, ICSharpFileBuilderTemplate
{
    public const string TemplateId = "Intent.Aws.Lambda.Functions.LambdaFunctionClassTemplate";

    private readonly FunctionClassResponseMapper _responseMapper = new();

    [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
    public LambdaFunctionClassTemplate(IOutputTarget outputTarget, ILambdaFunctionContainerModel model) : base(TemplateId, outputTarget, model)
    {
        AddNugetDependency(NugetPackages.AmazonLambdaCore(outputTarget));
        AddNugetDependency(NugetPackages.AmazonLambdaAPIGatewayEvents(outputTarget));
        AddNugetDependency(NugetPackages.AmazonLambdaSerializationSystemTextJson(outputTarget));
        AddNugetDependency(NugetPackages.AmazonLambdaAnnotations(outputTarget));
        AddNugetDependency(NugetPackages.AmazonLambdaLoggingAspNetCore(outputTarget));

        AddFrameworkDependency("Microsoft.AspNetCore.App");

        AddTypeSource(TemplateRoles.Application.Contracts.Dto, "List<{0}>");
        AddTypeSource(TemplateRoles.Application.Command);
        AddTypeSource(TemplateRoles.Application.Query);
        AddTypeSource(TemplateRoles.Application.Contracts.Enum);

        CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
            .AddAssemblyAttribute("LambdaSerializer", attr => attr.AddArgument($"typeof(DefaultLambdaJsonSerializer)"))
            .AddUsing("System")
            .AddUsing("System.Collections.Generic")
            .AddUsing("System.Threading.Tasks")
            .AddUsing("Amazon.Lambda.Annotations")
            .AddUsing("Amazon.Lambda.Annotations.APIGateway")
            .AddUsing("Amazon.Lambda.Core")
            .AddUsing("Amazon.Lambda.Serialization.SystemTextJson")
            .AddClass($"{Model.Name}Functions", @class =>
            {
                @class.AddConstructor(ctor => { });
                @class.RepresentsModel(Model);

                foreach (var functionModel in Model.Endpoints)
                {
                    @class.AddMethod($"Task<IHttpResult>", functionModel.Name.EnsureSuffixedWith("Async"), method =>
                    {
                        method.Async();
                        method.RepresentsModel(functionModel);
                        method.TryAddXmlDocComments(functionModel.InternalElement);
                        method.AddAttribute("LambdaFunction");
                        method.AddAttribute("HttpApi", attr => attr
                            .AddArgument($"LambdaHttpMethod.{functionModel.Verb}")
                            .AddArgument($@"""{functionModel.Route}"""));
                        foreach (var parameterModel in functionModel.Parameters)
                        {
                            // Use string type for Guid route/query parameters
                            var methodParameterType = parameterModel.TypeReference.HasGuidType() ? "string" : GetTypeName(parameterModel.TypeReference);

                            method.AddParameter(methodParameterType, parameterModel.Name, param =>
                            {
                                param.RepresentsModel(parameterModel);
                                switch (parameterModel.Source)
                                {
                                    case HttpInputSource.FromRoute:
                                        break;
                                    case null:
                                    case HttpInputSource.FromQuery:
                                        param.AddAttribute("FromQuery", attr =>
                                        {
                                            if (!string.IsNullOrWhiteSpace(parameterModel.QueryStringName))
                                            {
                                                attr.AddArgument($@"Name = ""{parameterModel.QueryStringName}""");
                                            }
                                        });
                                        break;
                                    case HttpInputSource.FromBody:
                                        param.AddAttribute("FromBody");
                                        break;
                                    case HttpInputSource.FromForm:
                                        throw new ElementException(functionModel.InternalElement,
                                            $"Parameter {parameterModel.Name} source is FromForm which is not supported in AWS Lambda functions.");
                                    case HttpInputSource.FromHeader:
                                        param.AddAttribute("FromHeader", attr => attr.AddArgument($@"Name = ""{parameterModel.HeaderName}"""));
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException($"Source '{parameterModel.Source}' is not supported.");
                                }
                            });
                        }

                        // Add Guid parsing logic for route/query parameters
                        var guidRouteQueryParameters = functionModel.Parameters
                            .Where(p => p.TypeReference.HasGuidType())
                            .ToList();

                        if (guidRouteQueryParameters.Any())
                        {
                            method.AddStatement("""
                                                // AWS Lambda Function Annotations have issue accepting Guid parameter types due to how string is converted to Guid.
                                                // Workaround by accepting string parameters and converting to Guid here.
                                                """);
                        }

                        foreach (var guidParam in guidRouteQueryParameters)
                        {
                            var guidParamName = $"{guidParam.Name}Guid";

                            method.AddIfStatement($"!Guid.TryParse({guidParam.Name}, out var {guidParamName})", @if =>
                            {
                                @if.AddReturn($"HttpResults.BadRequest($\"Invalid format for {guidParam.Name}: {{{guidParam.Name}}}\")");
                            });
                        }
                        method.AddReturn("HttpResults.Ok()");
                    });
                }
            });
    }

    public override void BeforeTemplateExecution()
    {
        Project.GetProject().AddProperty("AWSProjectType", "Lambda");
        Project.GetProject().AddProperty("PublishReadyToRun", "true");
    }

    public CSharpStatement GetReturnStatement(ILambdaFunctionModel operationModel)
    {
        var resultExpression = default(string);
        if (operationModel.ReturnType != null)
        {
            resultExpression = ShouldBeJsonResponseWrapped(operationModel)
                ? $"new {this.GetJsonResponseName()}<{GetTypeName(operationModel.ReturnType)}>(result)"
                : "result";
        }

        string? returnExpression = null;
        if (operationModel.Verb == HttpVerb.Post)
        {
            switch (operationModel.Parameters.Count)
            {
                case 1: // Aggregate
                    {
                        var getByIdOperation = Model.Endpoints
                            .FirstOrDefault(x => x.Verb == HttpVerb.Get &&
                                                 !string.IsNullOrWhiteSpace(x.Route) &&
                                                 x is { ReturnType.IsCollection: false, Parameters.Count: 1 } &&
                                                 string.Equals(x.Parameters[0].Name, "id", StringComparison.OrdinalIgnoreCase));
                        if (getByIdOperation != null && operationModel.ReturnType?.Element.Name is "guid" or "long" or "int" or "string")
                        {
                            returnExpression = $@"HttpResults.Created($""{getByIdOperation.Route!.Replace("{id}", "{Uri.EscapeDataString(result.ToString())}")}"", {resultExpression})";
                        }
                    }
                    break;
                case 2: // Owned composite
                    {
                        var getByIdOperation = Model.Endpoints
                            .FirstOrDefault(x => x.Verb == HttpVerb.Get &&
                                                 !string.IsNullOrWhiteSpace(x.Route) &&
                                                 x is { ReturnType.IsCollection: false, Parameters.Count: 2 } &&
                                                 string.Equals(x.Parameters[0].Name, operationModel.Parameters[0].Name, StringComparison.OrdinalIgnoreCase) &&
                                                 string.Equals(x.Parameters[1].Name, "id", StringComparison.OrdinalIgnoreCase));
                        if (getByIdOperation != null && operationModel.ReturnType?.Element.Name is "guid" or "long" or "int" or "string")
                        {
                            var aggregateIdParameter = getByIdOperation.Parameters[0].Name.ToCamelCase();
                            returnExpression = $@"HttpResults.Created($""{getByIdOperation.Route!.Replace("{" + getByIdOperation.Parameters[0].Name + "}", $"{{Uri.EscapeDataString({aggregateIdParameter}.ToString())}}").Replace("{id}", "{Uri.EscapeDataString(result.ToString())}")}"", {resultExpression})";
                        }
                    }
                    break;
            }
        }

        if (returnExpression is null)
        {
            returnExpression = _responseMapper.GetSuccessResponseCodeOperation(
                template: this,
                operation: operationModel,
                resultExpression: resultExpression,
                defaultResultExpressionFunc:
                (hasValue, verb, resultExpression) =>
                {
                    return verb switch
                    {
                        HttpVerb.Get => hasValue ? $"HttpResults.Ok({resultExpression})" : $"HttpResults.NewResult({GetHttpStatusCode()}.NoContent)",
                        HttpVerb.Patch => hasValue ? $"HttpResults.Ok({resultExpression})" : $"HttpResults.NewResult({GetHttpStatusCode()}.NoContent)",
                        HttpVerb.Put => hasValue ? $"HttpResults.Ok({resultExpression})" : $"HttpResults.NewResult({GetHttpStatusCode()}.NoContent)",
                        HttpVerb.Delete => hasValue ? $"HttpResults.Ok({resultExpression})" : "HttpResults.Ok()",
                        HttpVerb.Post => hasValue ? $"HttpResults.Created(body: {resultExpression})" : "HttpResults.Created()",
                        _ => throw new ArgumentOutOfRangeException($"Http Verb '{verb}' is not supported.")
                    };
                });
        }

        if (operationModel.ReturnType != null &&
            CanReturnNotFound(operationModel) &&
            !GetTypeInfo(operationModel.ReturnType).IsPrimitive)
        {
            returnExpression = $"result == null ? HttpResults.NotFound() : {returnExpression}";
        }

        return new CSharpReturnStatement(returnExpression);
    }

    private string GetHttpStatusCode()
    {
        return this.UseType("System.Net.HttpStatusCode");
    }

    private bool ShouldBeJsonResponseWrapped(ILambdaFunctionModel operationModel)
    {
        var isWrappedReturnType = operationModel.MediaType == HttpMediaType.ApplicationJson;
        var returnsCollection = operationModel.ReturnType?.IsCollection == true;
        var returnsString = operationModel.ReturnType.HasStringType();
        var returnsPrimitive = GetTypeInfo(operationModel.ReturnType!).IsPrimitive &&
                               !returnsCollection;

        return isWrappedReturnType && (returnsPrimitive || returnsString);
    }

    private static bool CanReturnNotFound(ILambdaFunctionModel operation)
    {
        if (operation.ReturnType?.IsNullable == true)
        {
            return false;
        }

        if (operation.Verb == HttpVerb.Get &&
            operation.ReturnType?.IsCollection != true &&
            !CSharpTypeIsCollection(operation.ReturnType) &&
            operation.Parameters.Any())
        {
            return true;
        }

        return operation.Parameters.Any(x => x.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase) ||
                                             x.Name.StartsWith("id", StringComparison.OrdinalIgnoreCase));
    }

    private static bool CSharpTypeIsCollection(ITypeReference? typeReference)
    {
        if (typeReference?.Element is not IElement element)
        {
            return false;
        }

        var stereotype = element.GetStereotype("C#");
        if (stereotype == null)
        {
            return false;
        }

        return stereotype.GetProperty<bool>("Is Collection");
    }

    [IntentManaged(Mode.Fully)]
    public CSharpFile CSharpFile { get; }

    [IntentManaged(Mode.Fully)]
    protected override CSharpFileConfig DefineFileConfig()
    {
        return CSharpFile.GetConfig();
    }

    [IntentManaged(Mode.Fully)]
    public override string TransformText()
    {
        return CSharpFile.ToString();
    }
}