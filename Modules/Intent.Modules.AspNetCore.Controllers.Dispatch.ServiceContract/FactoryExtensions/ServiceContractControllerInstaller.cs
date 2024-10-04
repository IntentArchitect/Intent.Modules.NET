using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.AspNetCore.Controllers.Templates;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Modules.UnitOfWork.Persistence.Shared;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Build.Evaluation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Dispatch.ServiceContract.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ServiceContractControllerInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Controllers.Dispatch.ServiceContract.ServiceContractControllerInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {

            var templates = application.FindTemplateInstances<IControllerTemplate<IControllerModel>>(TemplateDependency.OnTemplate(TemplateRoles.Distribution.Custom.Dispatcher));
            if (!templates.Any())
            {
                templates = application.FindTemplateInstances<IControllerTemplate<IControllerModel>>(TemplateDependency.OnTemplate(TemplateRoles.Distribution.WebApi.Controller));
            }
            foreach (var template in templates)
            {
                if (template.Model is not ServiceControllerModel)
                {
                    continue;
                }

                InstallServiceContractDispatch(template);
                InstallValidation(template);
                InstallTransactionWithUnitOfWork(template, application);
                InstallMessageBus(template, application);
            }
        }

        private static void InstallValidation(IControllerTemplate<IControllerModel> template)
        {
            if (!template.TryGetTypeName(TemplateRoles.Application.Common.ValidationServiceInterface, out var validationProviderName))
            {
                return;
            }

            if (template.Model.Operations.All(o => o.Parameters.All(x => !template.TryGetTypeName(TemplateRoles.Application.Validation.Dto, x.TypeReference.Element, out _))))
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
        private void InstallServiceContractDispatch(IControllerTemplate<IControllerModel> template)
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
                    if (method.TryGetMetadata<IControllerOperationModel>("model", out var operationModel))
                    {
                        var awaitModifier = string.Empty;
                        var arguments = string.Join(", ", operationModel.Parameters.Select((x) => x.Name ?? ""));
                        if (FileTransferHelper.IsFileUploadOperation(operationModel))
                        {

                            FileTransferHelper.AddControllerStreamLogic(template, method, operationModel);


                            arguments = string.Join(", ", operationModel.Parameters.Select((x) => FileTransferHelper.IsStreamType(x.TypeReference) ? $"stream" : x.Name ?? ""));
                        }

                        if (!operationModel.InternalElement.HasStereotype("Synchronous"))
                        {
                            awaitModifier = "await ";
                            arguments = string.IsNullOrEmpty(arguments)
                                ? "cancellationToken"
                                : $"{arguments}, cancellationToken";
                        }

                        if (operationModel.ReturnType != null)
                        {
                            method.AddStatement($"var result = default({template.GetTypeName(operationModel)});");
                            method.AddStatement($"result = {awaitModifier}_appService.{operationModel.Name.ToPascalCase()}({arguments});",
                                stmt => stmt.AddMetadata("service-contract-dispatch", true));
                        }
                        else
                        {
                            method.AddStatement($"{awaitModifier}_appService.{operationModel.Name.ToPascalCase()}({arguments});",
                                stmt => stmt.AddMetadata("service-contract-dispatch", true));
                        }

                        var returnStatement = template.GetReturnStatement(operationModel);
                        if (returnStatement != null)
                        {
                            method.AddStatement(returnStatement);
                        }
                    }
                }
            });
        }

        private static void InstallTransactionWithUnitOfWork(IControllerTemplate<IControllerModel> template, IApplication application)
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
                    if (!method.TryGetMetadata<IControllerOperationModel>("model", out var operation) ||
                        operation.Verb == HttpVerb.Get)
                    {
                        continue;
                    }

                    var dispatchStmt = method.FindStatement(stmt => stmt.HasMetadata("service-contract-dispatch"));
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

        private static void InstallMessageBus(IControllerTemplate<IControllerModel> template, IApplication application)
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

                foreach (var method in @class.Methods.Where(x => x.Attributes.All(a => !a.ToString()!.StartsWith("[HttpGet"))))
                {
                    method.Statements.LastOrDefault(x => x.ToString()!.Trim().StartsWith("return "))?
                        .InsertAbove("await _eventBus.FlushAllAsync(cancellationToken);", stmt => stmt.AddMetadata("eventbus-flush", true));
                }
            }, order: -100);
        }

        private static string GetUnitOfWork(IControllerTemplate<IControllerModel> template)
        {
            if (template.TryGetTypeName(TemplateRoles.Domain.UnitOfWork, out var unitOfWork) ||
                template.TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out unitOfWork) ||
                template.TryGetTypeName(TemplateRoles.Infrastructure.Data.DbContext, out unitOfWork))
            {
                return unitOfWork;
            }
            throw new Exception($"A unit of work interface could not be resolved. Please ensure an interface with the role [{TemplateRoles.Domain.UnitOfWork}] exists.");
        }
    }
}