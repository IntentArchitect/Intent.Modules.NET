using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.FluentValidation.Settings;
using Intent.Modules.Application.MediatR.FluentValidation.Templates.CommandValidator;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Constants;
using Intent.Modules.FluentValidation.Shared;
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

            // only if it could parse the setting and the outcome was false (i.e. don't create stubs) do we do the validation rules check
            if (bool.TryParse(applicationManager.Settings.GetSetting("459a4008-350c-42ec-b43d-9c85000babc0", "c467d8e6-1aaf-4b19-9e76-9d419e1c0b74")?.Value, out var result) && result == false)
            {
                models = models.Where(x =>
                {
                    IElement advancedMappingSource = GetAdvancedMappings(dtoFields, x);

                    return ValidationRulesExtensions.HasValidationRules(
                        dtoModel: new DTOModel(x.InternalElement),
                        dtoTemplateId: QueryModelsTemplate.TemplateId,
                        dtoValidatorTemplateId: TemplateRoles.Application.Validation.Dto,
                        uniqueConstraintValidationEnabled: applicationManager.Settings.GetFluentValidationApplicationLayer().UniqueConstraintValidation().IsDefaultEnabled(),
                        customValidationEnabled: true,
                        sourceElementAdvancedMappings: advancedMappingSource?.AssociatedElements);
                }).ToList();
            }

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