using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Blazor.HttpClients.Templates.EnumContract;
using Intent.Modules.Blazor.HttpClients.Templates.PagedResult;
using Intent.Modules.Constants;
using Intent.Modules.Contracts.Clients.Shared.FileNamespaceProviders;
using Intent.Modules.Contracts.Clients.Shared.Templates.DtoContract;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Templates.DtoContract
{
    [IntentManaged(Mode.Ignore)]
    public class DtoContractTemplate : DtoContractTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.HttpClients.DtoContract";

        public DtoContractTemplate(IOutputTarget outputTarget, DTOModel model)
            : base(
                templateId: TemplateId,
                outputTarget: outputTarget,
                model: model,
                config: new DtoContractTemplateConfig(EnumContractTemplate.TemplateId, PagedResultTemplate.TemplateId, new OutputTargetFileNamespaceProvider())
                {
                    InstantiateNonNullableDtoProperties = true
                })
        {
            FulfillsRole(TemplateRoles.Blazor.Client.Model.Definition);
        }
    }
}
