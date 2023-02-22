using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
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

        public HttpClientTemplate(IOutputTarget outputTarget, ServiceProxyModel model) : base(
            templateId: TemplateId,
            outputTarget: outputTarget,
            model: model,
            httpClientRequestExceptionTemplateId: HttpClientRequestExceptionTemplate.TemplateId,
            jsonResponseTemplateId: JsonResponseTemplate.TemplateId)
        { }
    }
}