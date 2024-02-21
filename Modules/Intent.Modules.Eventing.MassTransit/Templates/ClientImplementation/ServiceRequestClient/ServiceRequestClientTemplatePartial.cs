using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Eventing.MassTransit.Templates.ClientContracts;
using Intent.Modules.Eventing.MassTransit.Templates.ClientContracts.DtoContract;
using Intent.Modules.Eventing.MassTransit.Templates.ClientContracts.EnumContract;
using Intent.Modules.Eventing.MassTransit.Templates.ClientContracts.ServiceContract;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.ClientImplementation.ServiceRequestClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ServiceRequestClientTemplate : CSharpTemplateBase<ServiceProxyModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.ClientImplementation.ServiceRequestClient";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceRequestClientTemplate(IOutputTarget outputTarget, ServiceProxyModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(DtoContractTemplate.TemplateId);
            AddTypeSource(EnumContractTemplate.TemplateId);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.ImplementsInterface(this.GetServiceContractName(Model));
                    @class.AddConstructor(ctor => ctor.AddParameter(UseType("System.IServiceProvider"), "serviceProvider", param => param.IntroduceReadonlyField()));

                    AddOperations(@class);

                    @class.AddMethod("void", "Dispose");
                });
        }

        private void AddOperations(CSharpClass @class)
        {
            var proxyMappedService = new MassTransitServiceProxyMappedService();
            
            var mappedEndpoints = proxyMappedService.GetMappedEndpoints(Model);
            foreach (var mappedEndpoint in mappedEndpoints)
            {
                var baseReturnType = mappedEndpoint.ReturnType?.Element is null
                    ? null
                    : GetTypeName(mappedEndpoint.ReturnType);

                var returnType = mappedEndpoint.ReturnType?.Element is null
                    ? UseType("System.Threading.Tasks.Task")
                    : $"{UseType("System.Threading.Tasks.Task")}<{baseReturnType}>";

                @class.AddMethod(returnType, $"{mappedEndpoint.Name}Async", method =>
                {
                    method.Async();
                    foreach (var input in mappedEndpoint.Inputs)
                    {
                        method.AddParameter(GetTypeName(input.TypeReference), input.Name);
                    }
                    method.AddOptionalCancellationTokenParameter(this);

                    method.AddStatement($"var client = _serviceProvider.GetRequiredService<{UseType("MassTransit.IRequestClient")}<{GetTypeName(mappedEndpoint.Inputs.First())}>>();");

                    if (baseReturnType is null)
                    {
                        method.AddStatement($"await client.GetResponse<{this.GetRequestCompletedMessageName()}>({mappedEndpoint.Inputs.First().Name}, cancellationToken);");
                    }
                    else
                    {
                        method.AddStatement($"var response = await client.GetResponse<{this.GetRequestCompletedMessageName()}<{baseReturnType}>>({mappedEndpoint.Inputs.First().Name}, cancellationToken);");
                        method.AddStatement("return response.Message.Payload;");
                    }
                });
            }
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