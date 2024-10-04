using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.HttpClients.Templates.PagedResult;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.Contracts.Clients.Shared.Templates.DtoContract;
using Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Modules.Modelers.Types.ServiceProxies;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Templates.DtoContract
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DtoContractTemplateRegistration : FilePerModelTemplateRegistration<DTOModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DtoContractTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DtoContractTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, DTOModel model)
        {
            return new DtoContractTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<DTOModel> GetModels(IApplication application)
        {
            DtoContractTemplateBase.SetOutboundDtoElementIds(_metadataManager
                .UserInterface(application)
                .GetMappedServiceProxyInboundDTOModels()
                .Select(x => x.Id)
                .ToHashSet());

            return _metadataManager.UserInterface(application).GetMappedServiceProxyDTOModels()
                .Where(x =>
                {
                    if (x.InternalElement.IsCommandModel() || x.InternalElement.IsQueryModel())
                    {
                        // Excludes Commands / Queries and aren't actually payloads:
                        return HttpEndpointModelFactory.GetEndpoint(x.InternalElement)?.Inputs.Any(i => i.Id == x.Id) == true;
                    }

                    if (x.Id == PagedResultTemplateBase.TypeDefinitionElementId)
                    {
                        return false;
                    }

                    return true;
                })
                .ToList();
        }
    }
}