using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates;
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
    [IntentManaged(Mode.Ignore)]
    public class HttpClientTemplate : HttpClientTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Dapr.AspNetCore.ServiceInvocation.HttpClient";

        public HttpClientTemplate(IOutputTarget outputTarget, ServiceProxyModel model) : base(
            templateId: TemplateId,
            outputTarget: outputTarget,
            model: model,
            httpClientRequestExceptionTemplateId: HttpClientRequestExceptionTemplate.TemplateId,
            jsonResponseTemplateId: JsonResponseTemplate.TemplateId)
        { }

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