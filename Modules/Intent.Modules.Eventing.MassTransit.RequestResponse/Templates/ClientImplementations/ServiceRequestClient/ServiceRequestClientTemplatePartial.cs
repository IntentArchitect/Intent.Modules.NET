using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.DtoContract;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.ServiceContract;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperRequestMessage;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperResponseMessage;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientImplementations.ServiceRequestClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ServiceRequestClientTemplate : CSharpTemplateBase<ServiceProxyModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.RequestResponse.ClientImplementations.ServiceRequestClient";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceRequestClientTemplate(IOutputTarget outputTarget, ServiceProxyModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("System.Linq")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.ImplementsInterface(this.GetServiceContractName(Model));
                    @class.AddConstructor(ctor => ctor.AddParameter(UseType("System.IServiceProvider"), "serviceProvider", param => param.IntroduceReadonlyField()));

                    AddOperations(@class);

                    @class.AddMethod("void", "ConfigureClient", method =>
                    {
                        method.Private().Static();
                        method.AddGenericParameter("T", out var t);
                        method.AddGenericTypeConstraint(t, cfg => cfg.AddType("class"));
                        method.AddParameter($"IRequestPipeConfigurator<{t}>", "config");
                        method.AddStatement("config.UseRetry(retry => retry.None());");
                    });

                    @class.AddMethod("void", "Dispose");
                });
        }

        private void AddOperations(CSharpClass @class)
        {
            var serviceContract = new MassTransitServiceContractModel(Model);

            foreach (var operation in serviceContract.Operations)
            {
                var clientReturnType = operation.TypeReference?.Element is null
                    ? null
                    : operation.TypeReference.Element.SpecializationType == "DTO"
                        ? GetFullyQualifiedTypeExpression(DtoContractTemplate.TemplateId, operation)
                        : GetTypeName(operation.TypeReference);

                var mappedReturnType = operation.TypeReference?.Element is null
                    ? null
                    : operation.TypeReference.Element.SpecializationType == "DTO"
                        ? GetFullyQualifiedTypeExpression(MapperResponseMessageTemplate.TemplateId, operation)
                        : GetTypeName(operation.TypeReference);

                var firstParameter = operation.Parameters.First();
                var clientPrimaryInputType = GetFullyQualifiedTypeName(DtoContractTemplate.TemplateId, firstParameter.TypeReference.Element);
                var mapperRequestType = GetFullyQualifiedTypeName(MapperRequestMessageTemplate.TemplateId, firstParameter);

                var returnType = operation.TypeReference?.Element is null
                    ? UseType("System.Threading.Tasks.Task")
                    : $"{UseType("System.Threading.Tasks.Task")}<{clientReturnType}>";

                @class.AddMethod(returnType, $"{operation.Name}Async", method =>
                {
                    method.Async();
                    method.AddParameter(clientPrimaryInputType, firstParameter.Name);
                    method.AddOptionalCancellationTokenParameter(this);

                    method.AddIfStatement($"{firstParameter.Name} is null",
                        stmt => stmt.AddStatement($"throw new {UseType("System.ArgumentNullException")}(nameof({firstParameter.Name}));"));

                    var mappedVar = $"mapped{firstParameter.Name.ToPascalCase()}";

                    if (ServiceBusRequiresOwnTransaction())
                    {
                        method.AddStatement($"using var scope = new {UseType("System.Transactions.TransactionScope")}({UseType("System.Transactions.TransactionScopeOption")}.Suppress, {UseType("System.Transactions.TransactionScopeAsyncFlowOption")}.Enabled);");
                    }

                    method.AddStatement(
                        $"var client = _serviceProvider.GetRequiredService<{UseType("MassTransit.IRequestClient")}<{mapperRequestType}>>();");
                    method.AddStatement($"var {mappedVar} = new {mapperRequestType}({firstParameter.Name});");

                    var prefix = mappedReturnType is not null ? "var response = " : string.Empty;
                    var type = mappedReturnType is not null ? $"{this.GetRequestCompletedMessageName()}<{mappedReturnType}>" : this.GetRequestCompletedMessageName();
                    var invoke = new CSharpInvocationStatement($"{prefix}await client.GetResponse<{type}>")
                        .AddArgument(mappedVar)
                        .AddArgument("ConfigureClient")
                        .AddArgument("cancellationToken");
                    method.AddStatement(invoke);

                    if (mappedReturnType is not null)
                    {
                        var responsePayloadExpression = operation.TypeReference switch
                        {
                            null => string.Empty,
                            var rt when rt.Element?.SpecializationType != "DTO" => string.Empty,
                            var rt when rt.IsCollection => ".Select(x => x.ToDto()).ToList()",
                            _ => ".ToDto()"
                        };
                        method.AddStatement($"var mappedResponse = response.Message.Payload{responsePayloadExpression};");
                        if (ServiceBusRequiresOwnTransaction())
                        {
                            method.AddStatement("scope.Complete();");
                        }
                        method.AddStatement("return mappedResponse;");
                    }
                    else if (ServiceBusRequiresOwnTransaction())
                    {
                        method.AddStatement("scope.Complete();");
                    }
                });
            }
        }

        private bool ServiceBusRequiresOwnTransaction()
        {
            // Azure Service Bus requires a Transaction that is of type Serializable which is not how the
            // current transaction scope is setup, plus since the Request/Response is transient in nature
            // the risk of inconsistency is low since the messages are short lived as well as the receiver queues.
            return ExecutionContext.Settings.GetMassTransitMessageBusSettings().MessagingServiceProvider().IsAzureServiceBus();
        }

        private string GetFullyQualifiedTypeExpression(string templateId, IServiceContractOperationModel operation)
        {
            var type = GetFullyQualifiedTypeName(templateId, operation.TypeReference.Element);
            return operation.TypeReference switch
            {
                var returnType when returnType.IsCollection => $"{UseType("System.Collections.Generic.List")}<{type}>",
                var returnType when returnType.IsNullable => $"{type}?",
                _ => type
            };
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .ForInterface(this.GetServiceContractName(Model))
                .HasDependency(this.GetTemplate<ServiceContractTemplate>(ServiceContractTemplate.TemplateId, Model)));
        }

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