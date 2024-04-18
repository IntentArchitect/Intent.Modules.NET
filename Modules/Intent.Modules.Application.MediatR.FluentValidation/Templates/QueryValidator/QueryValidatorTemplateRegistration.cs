using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.FluentValidation.Templates.CommandValidator;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates.QueryValidator
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class QueryValidatorTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        private const string QueryTypeId = "e71b0662-e29d-4db2-868b-8a12464b25d0";

        public QueryValidatorTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => QueryValidatorTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            var dtoFields = _metadataManager.Services(applicationManager).GetElementsOfType(QueryTypeId)
                .SelectMany(s => s.ChildElements.Where(p => p.SpecializationTypeId == DTOFieldModel.SpecializationTypeId));

            var models = _metadataManager.Services(applicationManager).GetQueryModels();

            foreach (var model in models)
            {
                IElement advancedMappingSource = GetAdvancedMappings(dtoFields, model);

                registry.RegisterTemplate(TemplateId,
                    project => new QueryValidatorTemplate(
                        project,
                        model,
                        advancedMappingSource?.AssociatedElements));
            }
        }

        private static IElement GetAdvancedMappings(IEnumerable<IElement> dtoFields, QueryModel model)
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