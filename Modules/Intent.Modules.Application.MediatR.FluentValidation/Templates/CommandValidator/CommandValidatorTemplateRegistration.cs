using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates.CommandValidator
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CommandValidatorTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        private const string CommandTypeId = "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9";

        public CommandValidatorTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => CommandValidatorTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            var dtoFields = _metadataManager.Services(applicationManager).GetElementsOfType(CommandTypeId)
                .SelectMany(s => s.ChildElements.Where(p => p.SpecializationTypeId == DTOFieldModel.SpecializationTypeId));

            var models = _metadataManager.Services(applicationManager).GetCommandModels();

            foreach (var model in models)
            {
                IElement advancedMappingSource = GetAdvancedMappings(dtoFields, model);

                registry.RegisterTemplate(TemplateId,
                    project => new CommandValidatorTemplate(
                        project,
                        model,
                        advancedMappingSource?.AssociatedElements));
            }
        }

        private static IElement GetAdvancedMappings(IEnumerable<IElement> dtoFields, CommandModel model)
        {
            // check to see if parent has advanced mapping
            var matchingReferenceFields = dtoFields.Where(f => f.ParentId == model.Id);

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