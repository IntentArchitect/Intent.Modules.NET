using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Blazor.HttpClients.Templates.DtoContract;
using Intent.Modules.Blazor.HttpClients.Templates.HttpClientRequestException;
using Intent.Modules.Blazor.HttpClients.Templates.JsonResponse;
using Intent.Modules.Blazor.HttpClients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClient;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Templates.HttpClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HttpClientTemplate : CSharpTemplateBase<ServiceProxyModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.HttpClients.HttpClient";

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
    }
}