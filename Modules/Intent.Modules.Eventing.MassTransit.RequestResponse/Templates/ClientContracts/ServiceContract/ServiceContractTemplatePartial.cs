using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.DtoContract;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.EnumContract;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.PagedResult;
using Intent.Modules.Eventing.MassTransit.Templates.ClientContracts;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.ServiceContract
{
    [IntentManaged(Mode.Ignore)]
    public partial class ServiceContractTemplate : ServiceContractTemplateBase
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.RequestResponse.ClientContracts.ServiceContract";

        public ServiceContractTemplate(IOutputTarget outputTarget, ServiceProxyModel model)
            : base(
                templateId: TemplateId,
                outputTarget: outputTarget,
                model: model,
                dtoContractTemplateId: DtoContractTemplate.TemplateId,
                enumContractTemplateId: EnumContractTemplate.TemplateId,
                pagedResultTemplateId: PagedResultTemplate.TemplateId,
                serviceProxyMappedService: new MassTransitServiceProxyMappedService())
        {
            // So that this service (which is an application layer interface) is discoverable
            FulfillsRole(TemplateRoles.Application.Services.Interface);
        }
    }
}