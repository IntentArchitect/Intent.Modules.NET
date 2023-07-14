using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClient;
using Intent.Modules.Integration.HttpClients.Templates.HttpClientRequestException;
using Intent.Modules.Integration.HttpClients.Templates.JsonResponse;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Templates.HttpClient
{
    [IntentManaged(Mode.Ignore)]
    public class HttpClientTemplate : HttpClientTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Integration.HttpClients.HttpClient";

        public HttpClientTemplate(IOutputTarget outputTarget, ServiceProxyModel model) : base(TemplateId, outputTarget, model,
            httpClientRequestExceptionTemplateId: HttpClientRequestExceptionTemplate.TemplateId,
            jsonResponseTemplateId: JsonResponseTemplate.TemplateId,
            serviceContractTemplateId: ServiceContractTemplate.TemplateId,
            dtoContractTemplateId: DtoContractTemplate.TemplateId)
        {
        }
    }
}