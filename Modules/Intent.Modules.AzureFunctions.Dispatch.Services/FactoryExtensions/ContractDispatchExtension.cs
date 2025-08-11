using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates;
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
                        @class.Constructors.First().AddParameter(template.GetTypeName(TemplateRoles.Application.Eventing.EventBusInterface), "eventBus", param => param.IntroduceReadonlyField());
                        invocationStatement.AddStatement($@"await _eventBus.FlushAllAsync(cancellationToken);", stmt => stmt.AddMetadata("eventBus", true));
                    }
                    invocationStatement.AddStatement(GetReturnStatement(template));
                });
            }
        }
        
        private static bool ShouldInstallMessageBus(IApplication application)
        {
            return application.FindTemplateInstance<IClassProvider>(TemplateRoles.Application.Eventing.EventBusInterface) is not null;
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

        private static CSharpStatement GetReturnStatement(AzureFunctionClassTemplate template)
        {
            var httpTriggersView = HttpEndpointModelFactory.GetEndpoint(template.Model.InternalElement, "");
            var (result, statusCode) = httpTriggersView?.Verb switch
            {
                HttpVerb.Get => template.Model.ReturnType == null
                    ? ($"new NoContentResult()", "NoContent")
                    : ($"new OkObjectResult({GetResultExpression(template)})", "OK"),
                HttpVerb.Post => template.Model.ReturnType == null
                    ? ($"new CreatedResult(string.Empty, null)", "Created")
                    : ($"new CreatedResult(string.Empty, {GetResultExpression(template)})", "Created"),
                HttpVerb.Put or HttpVerb.Patch => template.Model.ReturnType == null
                    ? ($"new NoContentResult()", "NoContent")
                    : ($"new OkObjectResult({GetResultExpression(template)})", "OK"),
                HttpVerb.Delete => template.Model.ReturnType == null
                    ? ($"new OkResult()", "OK")
                    : ($"new OkObjectResult({GetResultExpression(template)})", "OK"),
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