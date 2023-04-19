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
                if (template.Model is not ServiceControllerModel)
                {
                    continue;
                }

                InstallContractDispatch(template);
                InstallValidation(template);
                InstallTransactionWithUnitOfWork(template, application);
                InstallMessageBus(template, application);
                InstallMongoDbUnitOfWork(template, application);
            }
        }

        private void InstallValidation(ControllerTemplate template)
        {
            if (!template.TryGetTypeName(TemplateFulfillingRoles.Application.Common.ValidationServiceInterface, out var validationProviderName))
            {
                return;
            }

            if (template.Model.Operations.All(o => o.Parameters.All(x => !template.TryGetTypeName(TemplateFulfillingRoles.Application.Validation.Dto, x.TypeReference.Element, out _))))
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                @class.Constructors.First().AddParameter(validationProviderName, "validationService", param => param.IntroduceReadonlyField((field, statement) =>
                {
                    statement.ThrowArgumentNullException();
                }));

                foreach (var method in @class.Methods)
                {
                    var fromBodyParam = method.Parameters.FirstOrDefault(p =>
                        p.Attributes.Any(p => p.GetText("")?.Contains("FromBody") == true));
                    if (fromBodyParam != null)
                    {
                        method.InsertStatement(0, $"await _validationService.Handle({fromBodyParam.Name}, cancellationToken);");
                    }
                }
            });
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
                    if (method.TryGetMetadata<IControllerOperationModel>("model", out var operationModel))
                    {
                        if (operationModel.ReturnType != null)
                        {
                            method.AddStatement($"var result = default({template.GetTypeName(operationModel)});");
                            method.AddStatement($@"result = await _appService.{operationModel.Name.ToPascalCase()}({template.GetArguments(operationModel.Parameters)});",
                                stmt => stmt.AddMetadata("service-contract-dispatch", true));
                        }
                        else
                        {
                            method.AddStatement($@"await _appService.{operationModel.Name.ToPascalCase()}({template.GetArguments(operationModel.Parameters)});",
                                stmt => stmt.AddMetadata("service-contract-dispatch", true));
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
                    if (!method.TryGetMetadata<IControllerOperationModel>("model", out var operation) ||
                        operation.Verb == HttpVerb.Get)
                    {
                        continue;
                    }

                    template.AddUsing("System.Transactions");

                    var dispatchStmt = method.FindStatement(stmt => stmt.HasMetadata("service-contract-dispatch"));
                    if (dispatchStmt == null)
                    {
                        continue;
                    }

                    var transactionScopeStmt = new CSharpStatementBlock($@"using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() {{ IsolationLevel = IsolationLevel.ReadCommitted }}, TransactionScopeAsyncFlowOption.Enabled))")
                        .AddStatement("await _unitOfWork.SaveChangesAsync(cancellationToken);")
                        .AddStatement("transaction.Complete();");
                    transactionScopeStmt.AddMetadata("transaction-scope", true);
                    dispatchStmt.InsertAbove(transactionScopeStmt);
                    dispatchStmt.Remove();
                    transactionScopeStmt.InsertStatement(0, dispatchStmt);
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
                        .InsertAbove("await _eventBus.FlushAllAsync(cancellationToken);", stmt => stmt.AddMetadata("eventbus-flush", true));
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
                    if (!method.TryGetMetadata<IControllerOperationModel>("model", out var operation) ||
                        operation.Verb == HttpVerb.Get)
                    {
                        continue;
                    }

                    method.Statements.LastOrDefault(x => x.ToString().StartsWith("return "))
                        ?.InsertAbove($"await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);");
                }
            }, -150);
        }

        private string GetReturnStatement(ControllerTemplate template, IControllerOperationModel operation)
        {
            switch (operation.Verb)
            {
                case HttpVerb.Get:
                    if (operation.ReturnType == null)
                    {
                        return "return NoContent();";
                    }

                    var resultExpression = GetResultExpression(template, operation);

                    if (operation.ReturnType.IsCollection)
                    {
                        return $"return Ok({resultExpression});";
                    }

                    return $@"return {resultExpression} != null ? Ok({resultExpression}) : NotFound();";
                case HttpVerb.Post:
                    return operation.ReturnType == null
                        ? @"return Created(string.Empty, null);"
                        : $@"return Created(string.Empty, {GetResultExpression(template, operation)});";
                case HttpVerb.Put:
                case HttpVerb.Patch:
                    return operation.ReturnType == null
                        ? @"return NoContent();"
                        : $@"return Ok({GetResultExpression(template, operation)});";
                case HttpVerb.Delete:
                    return operation.ReturnType == null
                        ? @"return Ok();"
                        : $@"return Ok({GetResultExpression(template, operation)});";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetResultExpression(ControllerTemplate template, IControllerOperationModel operationModel)
        {
            if (operationModel.ReturnType == null)
            {
                throw new ArgumentException($@"{nameof(operationModel.ReturnType)} is expected to be specified with a Type");
            }

            if (operationModel.MediaType == HttpMediaType.ApplicationJson
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