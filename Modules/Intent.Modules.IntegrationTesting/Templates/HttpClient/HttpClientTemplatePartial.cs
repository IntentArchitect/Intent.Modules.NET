using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClient;
using Intent.Modules.IntegrationTesting.Templates.DtoContract;
using Intent.Modules.IntegrationTesting.Templates.EnumContract;
using Intent.Modules.IntegrationTesting.Templates.HttpClientRequestException;
using Intent.Modules.IntegrationTesting.Templates.JsonResponse;
using Intent.Modules.IntegrationTesting.Templates.PagedResult;
using Intent.Modules.IntegrationTesting.Templates.ProxyServiceContract;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.IntegrationTesting.Templates.HttpClient
{
    [IntentManaged(Mode.Ignore)]
    public partial class HttpClientTemplate : HttpClientTemplateBase
    {
        public const string TemplateId = "Intent.IntegrationTesting.HttpClient";

        public HttpClientTemplate(IOutputTarget outputTarget, IServiceProxyModel model) : base(TemplateId, outputTarget, model,
            httpClientRequestExceptionTemplateId: HttpClientRequestExceptionTemplate.TemplateId,
            jsonResponseTemplateId: JsonResponseTemplate.TemplateId,
            serviceContractTemplateId: ProxyServiceContractTemplate.TemplateId,
            dtoContractTemplateId: DtoContractTemplate.TemplateId,
            enumContractTemplateId: EnumContractTemplate.TemplateId,
            pagedResultTemplateId: PagedResultTemplate.TemplateId)
        {
        }
    }
}