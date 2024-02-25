using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.EnumContract;
using Intent.Modules.Application.Contracts.Clients.Templates.PagedResult;
using Intent.Modules.Constants;
using Intent.Modules.Contracts.Clients.Http.Shared;
using Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;
using Intent.RoslynWeaver.Attributes;

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
                pagedResultTemplateId: PagedResultTemplate.TemplateId,
                serviceProxyMappedService: new HttpServiceProxyMappedService())
        {
            // So that this service (which is an application layer interface) is discoverable
            FulfillsRole(TemplateRoles.Application.Services.Interface);
        }
    }
}