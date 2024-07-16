using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.EnumContract;
using Intent.Modules.Application.Contracts.Clients.Templates.PagedResult;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClient;
using Intent.Modules.Integration.HttpClients.Templates.HttpClientRequestException;
using Intent.Modules.Integration.HttpClients.Templates.JsonResponse;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Templates.HttpClient
{
    [IntentManaged(Mode.Ignore)]
    public class HttpClientTemplate : HttpClientTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Integration.HttpClients.HttpClient";

        public HttpClientTemplate(IOutputTarget outputTarget, IServiceProxyModel model) : base(TemplateId, outputTarget, model,
            httpClientRequestExceptionTemplateId: HttpClientRequestExceptionTemplate.TemplateId,
            jsonResponseTemplateId: JsonResponseTemplate.TemplateId,
            serviceContractTemplateId: ServiceContractTemplate.TemplateId,
            dtoContractTemplateId: DtoContractTemplate.TemplateId,
            enumContractTemplateId: EnumContractTemplate.TemplateId,
            pagedResultTemplateId: PagedResultTemplate.TemplateId)
        {
        }
    }
}