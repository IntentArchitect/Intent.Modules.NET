using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Http.Shared;
using Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;
using Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Templates.PagedResult
{
    [IntentManaged(Mode.Ignore)]
    public partial class PagedResultTemplate : PagedResultTemplateBase, ICSharpFileBuilderTemplate, ITemplateWithModel
    {
        public const string TemplateId = "Intent.Blazor.HttpClients.PagedResult";

        public PagedResultTemplate(IOutputTarget outputTarget) : base(TemplateId, outputTarget)
        {
            FulfillsRole("Blazor.HttpClient.Contracts.Dto");
        }

        protected override IEnumerable<IServiceContractModel> GetServiceContractModels(IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.GetServiceContractModels(
                applicationId,
                metadataManager.UserInterface);
        }
    }
}