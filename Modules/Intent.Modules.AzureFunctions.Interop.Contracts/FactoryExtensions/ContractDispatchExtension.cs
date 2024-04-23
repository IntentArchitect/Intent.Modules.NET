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
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using IHasCSharpStatements = Intent.Modules.Common.CSharp.Builder.IHasCSharpStatements;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Interop.Contracts.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ContractDispatchExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AzureFunctions.Interop.Contracts.ContractDispatchExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<AzureFunctionClassTemplate>(TemplateDependency.OnTemplate(AzureFunctionClassTemplate.TemplateId));
            foreach (var template in templates)
            {
                var mappedOperation = template.Model.InternalElement.AsOperationModel() ??
                                      (template.Model.IsMapped ? template.Model.Mapping.Element.AsOperationModel() : null);
                if (mappedOperation == null)
                {
                    continue;
                }

                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.Single();
                    @class.Constructors.First().AddParameter(template.GetServiceContractName(mappedOperation.ParentService), "appService",
                        param => { param.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });

                    var runMethod = FindServiceInvokePoint(@class, out var hostMethod)
                        ?.AddInvocationStatement($"{(template.Model.ReturnType != null ? "var result = " : "")}await _appService.{mappedOperation.Name.ToPascalCase()}",
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
                            })
                        .AddStatement(GetReturnStatement(template));
                });
            }
        }

        private IHasCSharpStatements FindServiceInvokePoint(CSharpClass @class, out CSharpClassMethod hostMethod)
        {
            var runMethod = @class.FindMethod("Run");
            hostMethod = runMethod;
            var explicitPoint = runMethod.FindStatement(s => s.HasMetadata("service-invoke")) as IHasCSharpStatements;
            if (explicitPoint != null) return explicitPoint;
            return ((IHasCSharpStatements)runMethod.FindStatement<CSharpTryBlock>(x => true) ?? runMethod);
        }

        private static CSharpStatement GetReturnStatement(AzureFunctionClassTemplate template)
        {
            var httpTriggersView = HttpEndpointModelFactory.GetEndpoint(template.Model.InternalElement, "");
            string result = httpTriggersView?.Verb switch
            {
                HttpVerb.Get => template.Model.ReturnType == null
                    ? $"return new NoContentResult();"
                    : $"return new OkObjectResult({GetResultExpression(template)});",
                HttpVerb.Post => template.Model.ReturnType == null
                    ? $"return new CreatedResult(string.Empty, null);"
                    : $"return new CreatedResult(string.Empty, {GetResultExpression(template)});",
                HttpVerb.Put or HttpVerb.Patch => template.Model.ReturnType == null
                    ? $"return new NoContentResult();"
                    : $"return new OkObjectResult({GetResultExpression(template)});",
                HttpVerb.Delete => template.Model.ReturnType == null
                    ? $"return new OkResult();"
                    : $"return new OkObjectResult({GetResultExpression(template)});",
                null => template.Model.ReturnType == null
                    ? string.Empty
                    : $"return result;",
                _ => throw new ArgumentOutOfRangeException()
            };

            return new CSharpStatement(result)
                .AddMetadata("return", true);
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