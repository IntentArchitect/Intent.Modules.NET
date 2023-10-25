using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.EnumContract;
using Intent.Modules.Application.Contracts.Clients.Templates.PagedResult;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract
{
    [IntentManaged(Mode.Ignore)]
    public class ServiceContractTemplate : ServiceContractTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Contracts.Clients.ServiceContract";

        public ServiceContractTemplate(IOutputTarget outputTarget, ServiceProxyModel model)
            : base(
                templateId: TemplateId,
                outputTarget: outputTarget,
                model: model,
                dtoContractTemplateId: DtoContractTemplate.TemplateId,
                enumContractTemplateId: EnumContractTemplate.TemplateId,
                pagedResultTemplateId: PagedResultTemplate.TemplateId)
        {
        }
    }
}