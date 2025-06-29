using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;
using Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.PagedResult
{
    [IntentManaged(Mode.Ignore)]
    public partial class PagedResultTemplate : PagedResultTemplateBase, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.RequestResponse.ClientContracts.PagedResult";

        public PagedResultTemplate(IOutputTarget outputTarget) : base(TemplateId, outputTarget)
        {
        }

        protected override IEnumerable<IServiceContractModel> GetServiceContractModels(IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.Services(applicationId).GetServiceProxyModels()
                .Select(x => new MassTransitServiceContractModel(x)); ;
        }
    }
}