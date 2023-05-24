using System;
using System.Linq;
using System.Xml.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
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
                    var mappedOperation = _template.Model.InternalElement.AsOperationModel() ??
                        (_template.Model.IsMapped ? _template.Model.Mapping.Element.AsOperationModel() : null);

                    att.AddArgument($"\"{mappedOperation.Name}\"");

                    var grouping = GetOperationGrouping(endpointModel);
                    string serviceName = (endpointModel.InternalElement.ParentElement?.Name ?? "Default").RemoveSuffix("Controller", "Service");
                    att.AddArgument($"tags: new [] {{\"{grouping}\"}}");

                    att.AddArgument($"Description = \"{GetDescription(mappedOperation)}\"");
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

        private static string GetDescription(OperationModel mappedOperation)
        {
            return !string.IsNullOrEmpty(mappedOperation.InternalElement.Comment) ? mappedOperation.InternalElement.Comment : mappedOperation.Name.ToSentenceCase();
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