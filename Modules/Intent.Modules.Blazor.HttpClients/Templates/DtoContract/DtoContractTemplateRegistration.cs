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
using Intent.Modules.Contracts.Clients.Http.Shared;
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
                .GetServiceProxyReferencedDtos(
                    applicationId: application.Id,
                    includeReturnTypes: false,
                    stereotypeNames: null,
                    getDesigners: [_metadataManager.UserInterface])
                .Select(x => x.Id)
                .ToHashSet());

            var referencedElementIds = _metadataManager
                .GetServiceContractModels(
                    application.Id,
                    _metadataManager.UserInterface)
                .SelectMany(x => x.Operations)
                .SelectMany(x => x.Parameters)
                .Select(x => x.TypeReference.ElementId)
                .ToHashSet();

            var results = _metadataManager
                .GetServiceProxyReferencedDtos(
                    applicationId: application.Id,
                    includeReturnTypes: true,
                    stereotypeNames: null,
                    getDesigners: [_metadataManager.UserInterface])
                .Where(dto =>
                {
                    if (dto.InternalElement.IsCommandModel() || dto.InternalElement.IsQueryModel())
                    {
                        // Only generate "used" DTOs. The HttpEndpointFactory in cases will create
                        // some endpoints which don't actually use commands, e.g. for GetById, the
                        // parameter is just the id field, not the containing query. So we don't
                        // want to generate them.

                        // Additionally, when there are no fields on a command or DTO then we don't
                        // need it either.

                        return
                            referencedElementIds.Contains(dto.InternalElement.Id) &&
                            dto.InternalElement.ChildElements.Any(y => y.IsDTOFieldModel());
                    }

                    if (dto.Id == PagedResultTemplateBase.TypeDefinitionElementId)
                    {
                        return false;
                    }

                    return true;
                })
                .ToArray();

            return results;
        }
    }
}