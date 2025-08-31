using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Aws.Lambda.Functions.Api;
using Intent.Modules.Aws.Lambda.Functions.Templates.LambdaFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Modules.UnitOfWork.Persistence.Shared;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Dispatch.Services.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ServiceContractEndpointExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Aws.Lambda.Functions.Dispatch.Services.ServiceContractEndpointExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<LambdaFunctionClassTemplate>(TemplateDependency.OnTemplate(LambdaFunctionClassTemplate.TemplateId));
            foreach (var template in templates)
            {
                if (template.Model is not TraditionalServiceLambdaFunctionContainerModel)
                {
                    continue;
                }

                InstallServiceContractDispatch(template);
                InstallValidation(template);
                InstallTransactionWithUnitOfWork(template, application);
                InstallMessageBus(template, application);
            }
        }

        private static void InstallValidation(LambdaFunctionClassTemplate template)
        {
            if (!template.TryGetTypeName(TemplateRoles.Application.Common.ValidationServiceInterface, out var validationProviderName))
            {
                return;
            }

            if (template.Model.Endpoints.All(o => o.Parameters.All(x => !template.TryGetTypeName(TemplateRoles.Application.Validation.Dto, x.TypeReference.Element, out _))))
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                @class.Constructors.First().AddParameter(validationProviderName, "validationService", param => param.IntroduceReadonlyField((_, statement) =>
                {
                    statement.ThrowArgumentNullException();
                }));

                foreach (var method in @class.Methods)
                {
                    var fromBodyParam = method.Parameters.FirstOrDefault(p =>
                        p.Attributes.Any(static parameter => parameter.GetText("")?.Contains("FromBody") == true));
                    if (fromBodyParam != null)
                    {
                        method.InsertStatement(0, $"await _validationService.Handle({fromBodyParam.Name}, cancellationToken);");
                    }
                }
            });
        }

        // Please look at FastEndpoints to see how this can be refactored towards
        private static void InstallServiceContractDispatch(LambdaFunctionClassTemplate template)
        {
            template.AddTypeSource(DtoModelTemplate.TemplateId, "List<{0}>");
            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(template.GetTypeName(ServiceContractTemplate.TemplateId, template.Model), "appService", p =>
                {
                    p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                });

                foreach (var method in @class.Methods)
                {
                    if (method.RepresentedModel is not ILambdaFunctionModel operationModel)
                    {
                        continue;
                    }

                    method.InsertStatement(0, $"""
                                               // AWSLambda0107: can parameter of type System.Threading.CancellationToken passing is not supported.
                                               var cancellationToken = {template.UseType("System.Threading.CancellationToken")}.None;
                                               """);

                    var awaitModifier = string.Empty;
                    var arguments = string.Join(", ", operationModel.Parameters
                        .Select(param => param.TypeReference.HasGuidType() ? $"{param.Name}Guid" : param.Name ?? ""));

                    if (!operationModel.InternalElement.HasStereotype("Synchronous"))
                    {
                        awaitModifier = "await ";
                        arguments = string.IsNullOrEmpty(arguments)
                            ? "cancellationToken"
                            : $"{arguments}, cancellationToken";
                    }

                    if (operationModel.ReturnType != null)
                    {
                        var defaultResultValue = GetDefaultValue(template.GetTypeName(operationModel));

                        method.AddStatement($"var result = {defaultResultValue};");
                        method.AddStatement($"result = {awaitModifier}_appService.{operationModel.Name}({arguments});",
                            stmt => stmt.AddMetadata("service-contract-dispatch", true));
                    }
                    else
                    {
                        method.AddStatement($"{awaitModifier}_appService.{operationModel.Name}({arguments});",
                            stmt => stmt.AddMetadata("service-contract-dispatch", true));
                    }

                    var returnStatement = method.Statements.LastOrDefault(x => x.ToString()!.Trim().StartsWith("return "));
                    if (returnStatement != null)
                    {
                        returnStatement.Remove();
                        returnStatement = template.GetReturnStatement(operationModel);
                        method.AddStatement(returnStatement);
                    }
                }
            });
        }

        private static string GetDefaultValue(string type) => type switch
        {
            "Guid" => "Guid.Empty",
            _ => $"default({type})"
        };

        private static void InstallTransactionWithUnitOfWork(LambdaFunctionClassTemplate template, IApplication application)
        {
            if (!InteropCoordinator.ShouldInstallUnitOfWork(template))
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {

                var @class = file.Classes.First();

                foreach (var method in @class.Methods)
                {
                    if (method.RepresentedModel is not ILambdaFunctionModel operationModel)
                    {
                        continue;
                    }

                    if (operationModel.Verb == HttpVerb.Get)
                    {
                        continue;
                    }

                    var dispatchStmt = (CSharpStatement)method.FindStatement(stmt => stmt.HasMetadata("service-contract-dispatch"));
                    if (dispatchStmt == null)
                    {
                        continue;
                    }

                    //remove current dispatch statement (UOW implementation replaces it)
                    dispatchStmt.Remove();
                    method.ApplyUnitOfWorkImplementations(
                        template: template,
                        constructor: @class.Constructors.First(),
                        invocationStatement: dispatchStmt,
                        returnType: null,
                        resultVariableName: "result",
                        fieldSuffix: "unitOfWork",
                        includeComments: false);

                    //Move return statement to the end
                    var returnStatement = method.Statements.LastOrDefault(x => x.ToString()!.Trim().StartsWith("return "));
                    if (returnStatement != null)
                    {
                        returnStatement.Remove();
                        method.AddStatement(returnStatement);
                    }
                }
            }, order: 1);
        }

        private static void InstallMessageBus(LambdaFunctionClassTemplate template, IApplication application)
        {
            if (!InteropCoordinator.ShouldInstallMessageBus(application))
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(template.GetTypeName(TemplateRoles.Application.Eventing.EventBusInterface), "eventBus",
                    p => { p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });

                foreach (var method in @class.Methods.Where(method => (method.RepresentedModel as ILambdaFunctionModel)?.Verb != HttpVerb.Get))
                {
                    method.Statements.LastOrDefault(x => x.ToString()!.Trim().StartsWith("return "))?
                        .InsertAbove("await _eventBus.FlushAllAsync(cancellationToken);", stmt => stmt.AddMetadata("eventbus-flush", true));
                }
            }, order: -100);
        }
    }
}