using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.Events;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClientRequestException;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.JsonResponse;
using Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClient;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HttpClientTemplate : CSharpTemplateBase<ServiceProxyModel>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Dapr.AspNetCore.ServiceInvocation.HttpClient";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientTemplate(IOutputTarget outputTarget, ServiceProxyModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = HttpClientGenerator.CreateCSharpFile(
                template: this,
                httpClientRequestExceptionTemplateId: HttpClientRequestExceptionTemplate.TemplateId,
                jsonResponseTemplateId: JsonResponseTemplate.TemplateId,
                serviceContractTemplateId: ServiceContractTemplate.TemplateId,
                dtoContractTemplateId: DtoContractTemplate.TemplateId);
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

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            var applicationName = ExecutionContext.GetDaprApplicationName(Model.InternalElement.MappedElement.ApplicationId);

            ExecutionContext.EventDispatcher.Publish(new DaprServiceRegistration(
                serviceTypeResolver: template => template.GetServiceContractName(Model),
                implementationTypeResolver: template => template.GetHttpClientName(Model),
                implementationFactoryResolver: template => $"_ => new {ClassName}({template.UseType("Dapr.Client.DaprClient")}.CreateInvokeHttpClient(\"{applicationName}\"))"));
        }
    }
}