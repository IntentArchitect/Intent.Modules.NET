using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates;
using Intent.Modules.AzureFunctions.Api;
using Intent.Modules.AzureFunctions.Templates;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Dispatch.Services.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ContractDispatchExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AzureFunctions.Dispatch.Services.ContractDispatchExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        // Please don't use this as an example for how to inject dispatcher code into your tech solution.
        // Rather look at ASP.NET Core or FastEndpoints for more robust examples.
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<AzureFunctionClassTemplate>(TemplateDependency.OnTemplate(AzureFunctionClassTemplate.TemplateId));
            foreach (var template in templates)
            {
                var mappedOperation = OperationModelExtensions.AsOperationModel(template.Model.InternalElement) ??
                                      (template.Model.IsMapped ? OperationModelExtensions.AsOperationModel(template.Model.Mapping.Element) : null);
                if (mappedOperation is null)
                {
                    continue;
                }

                var parentService = mappedOperation.InternalElement.ParentElement.AsServiceModel();
                if (parentService == null)
                {
                    continue;
                }
                
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.Single();
                    @class.Constructors.First().AddParameter(template.GetServiceContractName(parentService), "appService",
                        param => { param.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });

                    var invocationStatement = FindServiceInvokePoint(@class, out var hostMethod);
                    if (invocationStatement is null)
                    {
                        return;
                    }

                    invocationStatement.AddInvocationStatement(
                        $"{(template.Model.ReturnType != null ? "var result = " : "")}await _appService.{mappedOperation.Name.ToPascalCase()}",
                        dispatch =>
                        {
                            foreach (var parameter in template.Model.Parameters)
                            {
                                var matchedMethodParam =
                                    hostMethod.Parameters.FirstOrDefault(p => p.TryGetMetadata("model", out IMetadataModel model) && model.Id == parameter.Id);
                                var paramName = parameter.Name;
                                if (matchedMethodParam?.TryGetMetadata("referralVar", out string referralVar) == true)
                                {
                                    paramName = referralVar;
                                }

                                dispatch.AddArgument(paramName);
                            }

                            dispatch.AddArgument("cancellationToken");
                            dispatch.AddMetadata("service-dispatch-statement", true);
                        });
                    if (ShouldInstallMessageBus(application))
                    {
                        var busVariableName = GetBusVariableName(template);
                        @class.Constructors.First().AddParameter(GetMessageBusInterfaceName(template), busVariableName, param => param.IntroduceReadonlyField());
                        invocationStatement.AddStatement($"await _{busVariableName}.FlushAllAsync(cancellationToken);", stmt => stmt.AddMetadata("eventBus", true));
                    }
                    invocationStatement.AddStatement(GetReturnStatement(template));
                });
            }
        }
        
        private static bool ShouldInstallMessageBus(IApplication application)
        {
                // Support newer MessageBusInterface and legacy EventBusInterface roles.
                return application.FindTemplateInstances<IClassProvider>(TemplateRoles.Application.Eventing.MessageBusInterface).Any() ||
                       application.FindTemplateInstances<IClassProvider>(TemplateRoles.Application.Eventing.EventBusInterface).Any();
        }

        private static string GetBusVariableName(IIntentTemplate template)
        {
            // Legacy support first since both interfaces have the MessageBusInterface role assigned
            if (template.TryGetTypeName(TemplateRoles.Application.Eventing.EventBusInterface, out _))
            {
                return "eventBus";
            }
            
            if (template.TryGetTypeName(TemplateRoles.Application.Eventing.MessageBusInterface, out _))
            {
                return "messageBus";
            }

            throw new InvalidOperationException(
                $"Could not find Message Bus interface with template role '{TemplateRoles.Application.Eventing.MessageBusInterface}' (or older legacy template with role '{TemplateRoles.Application.Eventing.EventBusInterface}').");
        }
        
        private static string GetMessageBusInterfaceName(IIntentTemplate template)
        {
            // Legacy support first since both interfaces have the MessageBusInterface role assigned
            if (template.TryGetTypeName(TemplateRoles.Application.Eventing.EventBusInterface, out var typeName))
            {
                return typeName;
            }
            
            if (template.TryGetTypeName(TemplateRoles.Application.Eventing.MessageBusInterface, out typeName))
            {
                return typeName;
            }

            throw new InvalidOperationException(
                $"Could not find Message Bus interface with template role '{TemplateRoles.Application.Eventing.MessageBusInterface}' (or older legacy template with role '{TemplateRoles.Application.Eventing.EventBusInterface}').");
        }

        private static IHasCSharpStatements? FindServiceInvokePoint(CSharpClass @class, out CSharpClassMethod? hostMethod)
        {
            var runMethod = @class.FindMethod("Run");
            hostMethod = runMethod;
            if (runMethod is null)
            {
                return null;
            }
            
            if (runMethod.FindStatement(s => s.HasMetadata("service-invoke")) is IHasCSharpStatements explicitPoint)
            {
                return explicitPoint;
            }

            return (IHasCSharpStatements)runMethod.FindStatement<CSharpTryBlock>(_ => true) ?? runMethod;
        }

        private static HttpResponseMapper _httpResponseMapper = new HttpResponseMapper();

        private static string GetResponse(IHttpEndpointModel endpoint, string defautlResult)
        {
            return $"new {_httpResponseMapper.GetSuccessResponseCodeOperation(endpoint.InternalElement, defautlResult, null)}";
        }
        private static string GetResponse(IHttpEndpointModel endpoint, string defautlResult, string response)
        {
            return $"new {_httpResponseMapper.GetSuccessResponseCodeOperation(endpoint.InternalElement, defautlResult, response)}";
        }

        public static string GetEnum(IHttpEndpointModel endpoint, string defaultValue)
        {
            return _httpResponseMapper.GetResponseStatusCodeEnum(endpoint.InternalElement, defaultValue);
        }

        private static CSharpStatement GetReturnStatement(AzureFunctionClassTemplate template)
        {
            var httpTriggersView = HttpEndpointModelFactory.GetEndpoint(template.Model.InternalElement, "");
            var (result, statusCode) = httpTriggersView?.Verb switch
            {
                HttpVerb.Get => template.Model.ReturnType == null
                    ? (GetResponse(httpTriggersView, "NoContentResult()")
                        , GetEnum(httpTriggersView, "NoContent"))
                    : (GetResponse(httpTriggersView, $"OkObjectResult({GetResultExpression(template)})", GetResultExpression(template))
                        , GetEnum(httpTriggersView, "OK")),
                HttpVerb.Post => template.Model.ReturnType == null
                    ? (GetResponse(httpTriggersView, "CreatedResult(string.Empty, null)")
                        , GetEnum(httpTriggersView, "Created"))
                    : (GetResponse(httpTriggersView, $"CreatedResult(string.Empty, {GetResultExpression(template)})", GetResultExpression(template))
                        , GetEnum(httpTriggersView, "Created")),
                HttpVerb.Put or HttpVerb.Patch => template.Model.ReturnType == null
                    ? (GetResponse(httpTriggersView, "NoContentResult()")
                        , GetEnum(httpTriggersView, "NoContent"))
                    : (GetResponse(httpTriggersView, $"OkObjectResult({GetResultExpression(template)})", GetResultExpression(template))
                        , GetEnum(httpTriggersView, "OK")),
                HttpVerb.Delete => template.Model.ReturnType == null
                    ? (GetResponse(httpTriggersView, "OkResult()")
                        , GetEnum(httpTriggersView, "OK"))
                    : (GetResponse(httpTriggersView, $"OkObjectResult({GetResultExpression(template)})", GetResultExpression(template))
                        , GetEnum(httpTriggersView, "OK")),
                null => template.Model.ReturnType == null
                    ? (string.Empty, "OK")
                    : ($"result", "OK"),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (string.IsNullOrEmpty(result))
            {
                return new CSharpStatement()
                    .AddMetadata("return", true)
                    .AddMetadata("return-response-type", statusCode);
            }
            
            return new CSharpReturnStatement(result)
                .AddMetadata("return", true)
                .AddMetadata("return-response-type", statusCode);
        }

        private static string GetResultExpression(AzureFunctionClassTemplate template)
        {
            if (template.Model.ReturnType == null)
            {
                throw new ArgumentException($@"{nameof(template.Model.ReturnType)} is expected to be specified with a Type");
            }

            if (HttpEndpointModelFactory.GetEndpoint(template.Model.InternalElement)?.MediaType == HttpMediaType.ApplicationJson
                && template.GetTypeInfo(template.Model.ReturnType).IsPrimitive)
            {
                return $@"new {template.GetJsonResponseName()}<{template.GetTypeName(template.Model.ReturnType)}>(result)";
            }

            return "result";
        }
    }
}