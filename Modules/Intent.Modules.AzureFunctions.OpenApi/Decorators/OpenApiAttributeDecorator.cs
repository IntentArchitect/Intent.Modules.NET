using System;
using System.Linq;
using System.Xml.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using Microsoft.VisualBasic;

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

            _template.AddNugetDependency(NuGetPackages.MicrosoftAzureWebJobsExtensionsOpenApi);


            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("System.Net");
                file.AddUsing("Microsoft.OpenApi.Models");
                file.AddUsing("Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes");
                var @class = file.Classes.First();
                var runMethod = @class.FindMethod("Run");
                runMethod.AddAttribute("OpenApiOperation", att =>
                {
                    var operationInfo = GetOperatiopnInfo();

                    att.AddArgument($"\"{operationInfo.Name}\"");
                    var grouping = GetOperationGrouping(endpointModel);
                    att.AddArgument($"tags: new [] {{\"{grouping}\"}}");
                    att.AddArgument($"Description = \"{GetDescription(operationInfo)}\"");
                });

                var requestDtoTypeName = template.Model.GetRequestDtoParameter() != null
                    ? template.GetTypeName(template.Model.GetRequestDtoParameter().TypeReference)
                    : null;
                if (!string.IsNullOrEmpty(requestDtoTypeName))
                {
                    runMethod.AddAttribute("OpenApiRequestBody", att =>
                    {

                        att.AddArgument("contentType: \"application/json\"");
                        att.AddArgument($"bodyType: typeof({requestDtoTypeName})");
                    });
                }

                if (endpointModel.Inputs.Any())
                {
                    foreach (var parameterModel in endpointModel.Inputs.Where(x => x.Source is HttpInputSource.FromRoute || x.Source is HttpInputSource.FromHeader || x.Source is HttpInputSource.FromQuery))
                    {
                        runMethod.AddAttribute("OpenApiParameter", att =>
                        {
                            att.AddArgument($"name: \"{parameterModel.Name.ToParameterName()}\"");
                            att.AddArgument($"In = ParameterLocation.{GetParameterLocation(parameterModel.Source)}");
                            att.AddArgument("Required = true");
                            att.AddArgument($"Type = typeof({_template.GetTypeName(parameterModel.TypeReference)})");
                        });
                    }
                }

                if (_template.Model.ReturnType != null)
                {
                    runMethod.AddAttribute("OpenApiResponseWithBody", att =>
                    {
                        att.AddArgument("statusCode: HttpStatusCode.OK");
                        att.AddArgument($"contentType: \"application/json\"");
                        att.AddArgument($"bodyType: typeof({_template.GetTypeName(_template.Model.ReturnType)})");
                    });

                }
                runMethod.AddAttribute("OpenApiResponseWithBody", att =>
                {

                    att.AddArgument("statusCode: HttpStatusCode.BadRequest");
                    att.AddArgument($"contentType: \"application/json\"");
                    att.AddArgument($"bodyType: typeof(object)");
                });

            }, 10);
        }

        private OperationInfo GetOperatiopnInfo()
        {
            var mappedOperation = _template.Model.InternalElement.AsOperationModel() ??
                (_template.Model.IsMapped ? _template.Model.Mapping.Element.AsOperationModel() : null);

            if (mappedOperation != null)
            {
                return new OperationInfo(mappedOperation.Name, mappedOperation.InternalElement);
            }

            var commandModel = _template.Model.InternalElement.AsCommandModel() ??
                (_template.Model.IsMapped ? _template.Model.Mapping.Element.AsCommandModel() : null);

            if (commandModel != null)
            {
                return new OperationInfo(commandModel.Name, commandModel.InternalElement);
            }

            var queryModel = _template.Model.InternalElement.AsQueryModel() ??
                (_template.Model.IsMapped ? _template.Model.Mapping.Element.AsQueryModel() : null);

            if (queryModel != null)
            {
                return new OperationInfo(queryModel.Name, queryModel.InternalElement);
            }
            throw new Exception("Unknow operation type");
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

        private static string GetDescription(OperationInfo operationInfo)
        {
            return !string.IsNullOrEmpty(operationInfo.Element.Comment) ? operationInfo.Element.Comment : operationInfo.Name.ToSentenceCase();
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

        private class OperationInfo
        {
            public OperationInfo(string name, IElement element)
            {
                Name = name;
                Element = element;
            }

            internal string Name { get; }
            internal IElement Element { get; }
        }



    }
}