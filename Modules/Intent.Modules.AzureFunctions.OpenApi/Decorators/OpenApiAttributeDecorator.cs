using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.AzureFunctions.Settings;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
using Microsoft.CodeAnalysis.CSharp;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.OpenApi.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class OpenApiAttributeDecorator : AzureFunctionClassDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AzureFunctions.OpenApi.OpenApiAttributeDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly AzureFunctionClassTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public OpenApiAttributeDecorator(AzureFunctionClassTemplate template, IApplication application)
        {
            _template = template;
            _application = application;

            if (template.Model.TriggerType != TriggerType.HttpTrigger)
            {
                return;
            }
            var endpointModel = HttpEndpointModelFactory.GetEndpoint(template.Model.InternalElement, "");
            if (endpointModel == null)
            {
                Logging.Log.Warning($"Http Settings could not be found on Azure Function [{template.Model.Name}] that is Http triggered");
            }

            _template.AddNugetDependency(NugetPackages.MicrosoftAzureWebJobsExtensionsOpenApi(_template.OutputTarget));


            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("System.Net");
                file.AddUsing("Microsoft.OpenApi.Models");
                file.AddUsing("Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes");
                var @class = file.Classes.First();
                var runMethod = @class.FindMethod("Run");
                var endpointElement = _template.Model.InternalElement;
                var openApiSettings = endpointElement.GetStereotype("OpenAPI Settings");

                runMethod.AddAttribute("OpenApiOperation", att =>
                {
                    var operationId = endpointElement.Name;

                    var openApiSettingsOperationId = (openApiSettings?.GetProperty<string>("OperationId") ?? string.Empty)
                        .Replace("{ServiceName}", string.Empty)
                        .Replace("{MethodName}", endpointElement.Name);
                    if (!string.IsNullOrWhiteSpace(openApiSettingsOperationId))
                    {
                        operationId = openApiSettingsOperationId;
                    }

                    att.AddArgument($"\"{operationId}\"");
                    var grouping = GetOperationGrouping(endpointModel);
                    att.AddArgument($"tags: new [] {{\"{grouping}\"}}");
                    att.AddArgument($"Description = \"{GetDescription(endpointElement)}\"");
                });

                if (openApiSettings?.GetProperty<bool>("Ignore") == true)
                {
                    runMethod.AddAttribute("OpenApiIgnoreAttribute");
                }

                var requestDtoTypeName = template.Model.GetRequestDtoParameter() != null
                    ? template.UseType(template.GetTypeInfo(template.Model.GetRequestDtoParameter().TypeReference).WithIsNullable(false))
                    : null;
                if (!string.IsNullOrEmpty(requestDtoTypeName))
                {
                    runMethod.AddAttribute("OpenApiRequestBody", att =>
                    {

                        att.AddArgument("contentType: \"application/json\"");
                        att.AddArgument($"bodyType: typeof({requestDtoTypeName})");
                    });
                }

                if (endpointModel?.Inputs.Any() == true)
                {
                    foreach (var parameterModel in endpointModel.Inputs
                                 .Where(x => x.Source is HttpInputSource.FromRoute or HttpInputSource.FromHeader or HttpInputSource.FromQuery))
                    {
                        runMethod.AddAttribute("OpenApiParameter", att =>
                        {
                            att.AddArgument($"name: \"{parameterModel.Name.ToParameterName()}\"");
                            att.AddArgument($"In = ParameterLocation.{GetParameterLocation(parameterModel.Source)}");
                            att.AddArgument($"Required = {(!parameterModel.TypeReference.IsNullable).ToString().ToLower()}");
                            att.AddArgument($"Type = typeof({_template.UseType(template.GetTypeInfo(parameterModel.TypeReference).WithIsNullable(false))})");
                        });
                    }
                }

                var responseStatusCode = GetResponseStatusCodes(runMethod);
                
                if (_template.Model.ReturnType != null)
                {
                    foreach (var response in responseStatusCode)
                    {
                        runMethod.AddAttribute("OpenApiResponseWithBody", att =>
                        {
                            att.AddArgument($"statusCode: HttpStatusCode.{response}");
                            att.AddArgument($"contentType: \"application/json\"");
                            att.AddArgument($"bodyType: typeof({_template.UseType(template.GetTypeInfo(_template.Model.ReturnType).WithIsNullable(false))})");
                        });
                    }
                }
                else
                {
                    foreach (var response in responseStatusCode)
                    {
                        runMethod.AddAttribute("OpenApiResponseWithoutBody", att =>
                        {
                            att.AddArgument($"statusCode: HttpStatusCode.{response}");
                        });
                    }
                }

                runMethod.AddAttribute("OpenApiResponseWithBody", att =>
                {
                    att.AddArgument("statusCode: HttpStatusCode.BadRequest");
                    att.AddArgument($"contentType: \"application/json\"");
                    att.AddArgument($"bodyType: typeof(object)");
                });

                if (_template.ExecutionContext.Settings.GetAzureFunctionsSettings().UseGlobalExceptionMiddleware())
                {
                    runMethod.AddAttribute("OpenApiResponseWithBody", att =>
                    {
                        att.AddArgument($"statusCode: HttpStatusCode.NotFound");
                        att.AddArgument($"contentType: \"application/json\"");
                        att.AddArgument($"bodyType: typeof(object)");
                    });
                }

                AddExceptionResponseWithBodyAttribute(runMethod);

            }, 10);
        }

        private static void AddExceptionResponseWithBodyAttribute(CSharpClassMethod runMethod)
        {
            foreach (CSharpStatement statement in runMethod.Statements)
            {
                if (statement is CSharpCatchBlock catchBlock && catchBlock.HasMetadata("exception-response-type"))
                {
                    runMethod.AddAttribute("OpenApiResponseWithBody", att =>
                    {
                        att.AddArgument($"statusCode: HttpStatusCode.{catchBlock.GetMetadata<string>("exception-response-type")}");
                        att.AddArgument($"contentType: \"application/json\"");
                        att.AddArgument($"bodyType: typeof(object)");
                    });
                }
            }
            
        }

        private List<string> GetResponseStatusCodes(CSharpClassMethod runMethod)
        {
            var responseCodes = new List<string>();

            foreach (CSharpStatement statement in runMethod.Statements)
            {
                if (statement.HasMetadata("return-response-type"))
                {
                    responseCodes.Add(statement.GetMetadata<string>("return-response-type"));
                }
                if (statement is CSharpTryBlock tryBlock)
                {
                    responseCodes.AddRange(tryBlock.Statements.Where(s => s.HasMetadata("return-response-type"))
                        .Select(s => s.GetMetadata<string>("return-response-type")));
                }
            }
            
            return responseCodes;
        }

        private string GetParameterLocation(HttpInputSource? source)
        {
            return source switch
            {
                HttpInputSource.FromRoute => "Path",
                HttpInputSource.FromQuery => "Query",
                HttpInputSource.FromHeader => "Header",
                _ => "Path"
            };
        }

        private static string GetDescription(IElement endpointElement)
        {
            if (!string.IsNullOrWhiteSpace(endpointElement.Comment))
            {
                return SymbolDisplay.FormatLiteral(endpointElement.Comment, false);
            }

            var mappedOperation = endpointElement.MappedElement?.Element.AsOperationModel();
            if (!string.IsNullOrWhiteSpace(mappedOperation?.Comment))
            {
                return SymbolDisplay.FormatLiteral(mappedOperation.Comment, false);
            }

            return endpointElement.Name.ToSentenceCase();
        }

        private string GetOperationGrouping(IHttpEndpointModel endpointModel)
        {
            string result = "Default";
            if (!string.IsNullOrEmpty(endpointModel?.Route))
            {
                if (endpointModel.Route.IndexOf('/') > -1)
                {
                    result = endpointModel.Route.Substring(0, endpointModel.Route.IndexOf('/'));
                }
                else
                {
                    result = endpointModel.Route;
                }
            }
            return result.ToPascalCase();
        }
    }
}