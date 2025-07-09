using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Intent.AspNetCore.Grpc.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Grpc.Templates.ServiceProtoFile;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.UnitOfWork.Persistence.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates.TraditionalService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TraditionalServiceTemplate : CSharpTemplateBase<ServiceModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Grpc.TraditionalService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TraditionalServiceTemplate(IOutputTarget outputTarget, ServiceModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Application.Command);
            AddTypeSource(TemplateRoles.Application.Query);
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

            var name = Model.Name.ToPascalCase();
            var installedModules = ExecutionContext.InstalledModules.Select(x => x.ModuleId).ToHashSet(StringComparer.OrdinalIgnoreCase);

            const string appContractTemplateId = "Intent.Application.Contracts.ServiceContract";

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}", @class =>
                {
                    var addValidation = TryGetTypeName(TemplateRoles.Application.Common.ValidationServiceInterface, out var validationServiceInterfaceTypeName);
                    var addEventBus = TryGetTypeName(TemplateRoles.Application.Eventing.EventBusInterface, out var eventBusInterfaceTypeName);
                    var addDispatch = installedModules.Contains("Intent.Application.Contracts");
                    var addUnitOfWork = this.SystemUsesPersistenceUnitOfWork();

                    var (serviceAuthAttributes, authAttributesByEndpoint) = this.GetAuthorizationAttributes(
                        container: Model.InternalElement,
                        endpointElements: Model.Operations.Where(x => x.HasExposeWithGRPC()).Select(x => x.InternalElement));
                    foreach (var attribute in serviceAuthAttributes)
                    {
                        @class.AddAttribute(attribute);
                    }

                    @class.WithBaseType($"{UseType($"{ServiceProtoFileTemplateInstance.CSharpNamespace}.{name}")}.{name}Base");
                    @class.AddConstructor(ctor =>
                    {
                        if (addDispatch)
                        {
                            AddUsing("System");
                            ctor.AddParameter(GetTypeName(appContractTemplateId, Model.Id), "appService", p => p.IntroduceReadonlyField((_, s) => s.ThrowArgumentNullException()));
                        }

                        if (addValidation &&
                            Model.Operations.Any(o => o.Parameters.Any(x => TryGetTypeName(TemplateRoles.Application.Validation.Dto, x.TypeReference.Element, out _))))
                        {
                            AddUsing("System");
                            ctor.AddParameter(validationServiceInterfaceTypeName, "validationService", p => p.IntroduceReadonlyField((_, s) => s.ThrowArgumentNullException()));
                        }

                        if (addEventBus)
                        {
                            AddUsing("System");
                            ctor.AddParameter(eventBusInterfaceTypeName, "eventBus", p => p.IntroduceReadonlyField((_, s) => s.ThrowArgumentNullException()));
                        }
                    });

                    foreach (var operation in Model.Operations)
                    {
                        if (!operation.HasExposeWithGRPC())
                        {
                            continue;
                        }

                        // Do this beforehand so that type disambiguation is adequately
                        // informed for the actual resolved type that gets used later
                        _ = GetTypeName(operation);
                        _ = this.MapToOperationReturnType(typeReference: operation.TypeReference);

                        var serviceName = Model.Name.ToPascalCase();
                        var operationName = operation.Name.ToPascalCase();

                        @class.AddMethod(this.MapToOperationReturnType(typeReference: operation.TypeReference), operationName, method =>
                        {
                            foreach (var attribute in authAttributesByEndpoint[operation.InternalElement])
                            {
                                method.AddAttribute(attribute);
                            }

                            method.Async();
                            method.Override();

                            var callArguments = new List<string>(operation.Parameters.Count + 1);
                            var hasResult = operation.ReturnType != null;
                            var result = string.Empty;

                            if (operation.Parameters.Count == 0)
                            {
                                method.AddParameter(UseType("Google.Protobuf.WellKnownTypes.Empty"), "request");
                            }
                            else
                            {
                                method.AddParameter(UseType($"{ServiceProtoFileTemplateInstance.CSharpNamespace}.{serviceName}{operationName}Request"), "request");
                                callArguments.AddRange(operation.Parameters.Select(x => this.MapFromMessage(x.TypeReference, $"request.{x.Name.ToPascalCase()}")));
                            }

                            method.AddParameter(UseType("Grpc.Core.ServerCallContext"), "context");

                            var await = string.Empty;
                            if (!operation.HasStereotype("Synchronous"))
                            {
                                await = "await ";
                                callArguments.Add("context.CancellationToken");
                            }

                            if (addValidation)
                            {
                                CSharpFile.OnBuild(_ =>
                                {
                                    foreach (var parameter in operation.Parameters.Where(x => x.TypeReference?.Element?.IsDTOModel() == true))
                                    {
                                        var dtoExpression = this.MapFromMessage(parameter.TypeReference, $"request.{parameter.Name.ToPascalCase()}");
                                        method.InsertStatement(0, $"await _validationService.Handle({dtoExpression}, context.CancellationToken);");
                                    }
                                });
                            }

                            if (addDispatch)
                            {
                                if (hasResult)
                                {
                                    if (addUnitOfWork)
                                    {
                                        method.AddStatement($"{GetTypeName(operation)} result;");
                                        result = "result = ";
                                    }
                                    else
                                    {
                                        result = "var result = ";
                                    }
                                }

                                method.AddInvocationStatement($"{result}{await}_appService.{operationName}", invocationStatement =>
                                {
                                    invocationStatement.AddMetadata("service-contract-dispatch", true);

                                    foreach (var argument in callArguments)
                                    {
                                        invocationStatement.AddArgument(argument);
                                    }

                                    if (addUnitOfWork)
                                    {
                                        CSharpFile.OnBuild(_ =>
                                        {
                                            invocationStatement.Remove();
                                            method.ApplyUnitOfWorkImplementations(
                                                template: this,
                                                constructor: @class.Constructors.First(),
                                                invocationStatement: invocationStatement,
                                                cancellationTokenExpression: "context.CancellationToken",
                                                fieldSuffix: "unitOfWork",
                                                includeComments: false);

                                            //Move return statement to the end
                                            var returnStatement = method.Statements.LastOrDefault(x => x.ToString()!.Trim().StartsWith("return "));
                                            if (returnStatement == null)
                                            {
                                                return;
                                            }

                                            returnStatement.Remove();
                                            method.AddStatement(returnStatement);
                                        }, order: 1);
                                    }
                                });

                                if (addEventBus)
                                {
                                    CSharpFile.AfterBuild(_ =>
                                    {
                                        method.Statements.LastOrDefault(x => x.ToString()!.Trim().StartsWith("return "))?
                                            .InsertAbove("await _eventBus.FlushAllAsync(context.CancellationToken);", stmt => stmt.AddMetadata("eventbus-flush", true));
                                    }, -100);
                                }

                                method.AddStatement($"return {this.MapToReturnStatement(operation.TypeReference, "result")};");
                            }
                            else
                            {
                                method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                                method.AddStatement("// IntentInitialGen");
                                method.AddStatement($"throw new {UseType("System.NotImplementedException")}();");
                            }
                        });
                    }
                });
        }

        public override void AfterTemplateRegistration()
        {
            ExecutionContext.EventDispatcher.Publish(new RegisterGrpcService(this));
        }

        private ServiceProtoFileTemplate ServiceProtoFileTemplateInstance => field ??= ExecutionContext.FindTemplateInstance<ServiceProtoFileTemplate>(ServiceProtoFileTemplate.TemplateId, Model.Id);

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}