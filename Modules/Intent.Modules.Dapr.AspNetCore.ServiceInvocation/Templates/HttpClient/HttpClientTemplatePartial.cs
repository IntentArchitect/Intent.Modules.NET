using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.EnumContract;
using Intent.Modules.Application.Contracts.Clients.Templates.PagedResult;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.Events;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClientRequestException;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.JsonResponse;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
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

        public HttpClientTemplate(IOutputTarget outputTarget, IServiceProxyModel model)
            : base(
                templateId: TemplateId,
                outputTarget: outputTarget,
                model: model,
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