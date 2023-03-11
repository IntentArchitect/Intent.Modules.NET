using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.AspNetCore.Controllers.Templates;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

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
            var templates = application.FindTemplateInstances<ControllerTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Application.Services.Controllers));
            foreach (var template in templates)
            {
                InstallContractDispatch(template);
                InstallTransactionWithUnitOfWork(template, application);
                InstallMessageBus(template, application);
                InstallMongoDbUnitOfWork(template, application);
            }
        }

        private void InstallContractDispatch(ControllerTemplate template)
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
                    if (method.TryGetMetadata<OperationModel>("model", out var operationModel) && operationModel.HasHttpSettings())
                    {
                        if (operationModel.ReturnType != null)
                        {
                            method.AddStatement($"var result = default({template.GetTypeName(operationModel)});");
                            method.AddStatement($@"result = await _appService.{operationModel.Name.ToPascalCase()}({template.GetArguments(operationModel.Parameters)});");
                        }
                        else
                        {
                            method.AddStatement($@"await _appService.{operationModel.Name.ToPascalCase()}({template.GetArguments(operationModel.Parameters)});");
                        }
                        method.AddStatement(GetReturnStatement(template, operationModel));
                    }
                }
            });
        }
        
        private void InstallTransactionWithUnitOfWork(ControllerTemplate template, IApplication application)
        {
            if (!InteropCoordinator.ShouldInstallUnitOfWork(application))
            {
                return;
            }
            
            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(GetUnitOfWork(template), "unitOfWork", p =>
                {
                    p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                });

                foreach (var method in @class.Methods)
                {
                    if (method.TryGetMetadata<OperationModel>("model", out var operation) &&
                        operation.HasHttpSettings() && !operation.GetHttpSettings().Verb().IsGET())
                    {
                        template.AddUsing("System.Transactions");
                        method.Statements.FirstOrDefault(x => x.ToString().Contains("await "))?
                            .InsertAbove($@"using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() {{ IsolationLevel = IsolationLevel.ReadCommitted }}, TransactionScopeAsyncFlowOption.Enabled))
            {{")
                            .Indent()
                            .InsertBelow("await _unitOfWork.SaveChangesAsync(cancellationToken);", s => s
                                .InsertBelow("transaction.Complete();", s => s
                                    .InsertBelow("}", s => s.Outdent())));
                    }
                }
            }, order: 1);
        }
        
        private static void InstallMessageBus(ControllerTemplate template, IApplication application)
        {
            if (!InteropCoordinator.ShouldInstallMessageBus(application))
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(template.GetTypeName(TemplateFulfillingRoles.Application.Eventing.EventBusInterface), "eventBus",
                    p => { p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });

                foreach (var method in @class.Methods)
                {
                    method.Statements.LastOrDefault(x => x.ToString().Trim().StartsWith("return "))?
                        .InsertAbove("await _eventBus.FlushAllAsync(cancellationToken);");
                }
            }, order: -100);
        }
        
        private void InstallMongoDbUnitOfWork(ControllerTemplate template, IApplication application)
        {
            if (!InteropCoordinator.ShouldInstallMongoDbUnitOfWork(application))
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(template.GetTypeName("Domain.UnitOfWork.MongoDb"), "mongoDbUnitOfWork", p => { p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });

                foreach (var method in @class.Methods)
                {
                    if (method.TryGetMetadata<IHasStereotypes>("model", out var operation) &&
                        operation.HasStereotype("Http Settings") && operation.GetStereotype("Http Settings").GetProperty<string>("Verb") != "GET")
                    {
                        method.Statements.LastOrDefault(x => x.ToString().Contains("return "))
                            ?.InsertAbove($"await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);");
                    }
                }
            }, -150);
        }

        private string GetReturnStatement(ControllerTemplate template, OperationModel operationModel)
        {
            switch (template.GetHttpVerb(operationModel))
            {
                case ControllerTemplate.HttpVerb.Get:
                    if (operationModel.ReturnType == null)
                    {
                        return "return NoContent();";
                    }

                    var resultExpression = GetResultExpression(template, operationModel);

                    if (operationModel.ReturnType.IsCollection)
                    {
                        return $"return Ok({resultExpression});";
                    }

                    return $@"return {resultExpression} != null ? Ok({resultExpression}) : NotFound();";
                case ControllerTemplate.HttpVerb.Post:
                    return operationModel.ReturnType == null
                        ? @"return Created(string.Empty, null);"
                        : $@"return Created(string.Empty, {GetResultExpression(template, operationModel)});";
                case ControllerTemplate.HttpVerb.Put:
                case ControllerTemplate.HttpVerb.Patch:
                    return operationModel.ReturnType == null
                        ? @"return NoContent();"
                        : $@"return Ok({GetResultExpression(template, operationModel)});";
                case ControllerTemplate.HttpVerb.Delete:
                    return operationModel.ReturnType == null
                        ? @"return Ok();"
                        : $@"return Ok({GetResultExpression(template, operationModel)});";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetResultExpression(ControllerTemplate template, OperationModel operationModel)
        {
            if (operationModel.ReturnType == null)
            {
                throw new ArgumentException($@"{nameof(operationModel.ReturnType)} is expected to be specified with a Type");
            }

            if (operationModel.GetHttpSettings().ReturnTypeMediatype().IsApplicationJson()
                && (template.GetTypeInfo(operationModel.ReturnType).IsPrimitive || operationModel.ReturnType.HasStringType()))
            {
                return $@"new {template.GetJsonResponseName()}<{template.GetTypeName(operationModel.ReturnType)}>(result)";
            }

            return "result";
        }
        
        private string GetUnitOfWork(ControllerTemplate template)
        {
            if (template.TryGetTypeName(TemplateFulfillingRoles.Domain.UnitOfWork, out var unitOfWork) ||
                template.TryGetTypeName(TemplateFulfillingRoles.Application.Common.DbContextInterface, out unitOfWork) ||
                template.TryGetTypeName(TemplateFulfillingRoles.Infrastructure.Data.DbContext, out unitOfWork))
            {
                return unitOfWork;
            }
            throw new Exception($"A unit of work interface could not be resolved. Please ensure an interface with the role [{TemplateFulfillingRoles.Domain.UnitOfWork}] exists.");
        }
    }
}