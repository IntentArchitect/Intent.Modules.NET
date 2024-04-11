using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.WebClient.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Modules.Modelers.Types.ServiceProxies;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Dtos.FluentValidation.Templates.DtoValidator
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DtoValidatorTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public DtoValidatorTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => DtoValidatorTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            var dtoFields = _metadataManager.Services(applicationManager).GetElementsOfType("")
                .SelectMany(s => s.ChildElements.Where(p => p.SpecializationTypeId == DTOFieldModel.SpecializationTypeId));

            var models = _metadataManager.WebClient(applicationManager).GetMappedServiceProxyInboundDTOModels()
                .Where(x =>
                {
                    if (x.InternalElement.IsCommandModel() || x.InternalElement.IsQueryModel())
                    {
                        return HttpEndpointModelFactory.GetEndpoint(x.InternalElement)?.Inputs.Any(i => i.Id == x.Id) == true;
                    }

                    return true;
                })
                .ToList();

            foreach (var model in models)
            {
                IElement advancedMappingSource = GetAdvancedMappings(dtoFields, model);

                registry.RegisterTemplate(TemplateId,
                    project => new DtoValidatorTemplate(
                        project,
                        model,
                        advancedMappingSource?.AssociatedElements));
            }
        }

        private static IElement GetAdvancedMappings(IEnumerable<IElement> dtoFields, DTOModel model)
        {
            // check to see if parent has advanced mapping
            var matchingReferenceFields = dtoFields.Where(f => f.TypeReference?.Element.Id == model.Id);

            var commandsOrQueries = matchingReferenceFields
                .Select(f => f.ParentElement)
                .Distinct()
                .ToArray();

            var advancedMappingSource = commandsOrQueries.Length switch
            {
                0 => null,
                1 => commandsOrQueries[0],
                _ => null,
            };

            return advancedMappingSource;
        }
    }
}