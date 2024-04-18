using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.FluentValidation.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Constants;
using Intent.Modules.FluentValidation.Shared;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Dtos.Templates.DTOValidator
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DTOValidatorTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        private const string CommandTypeId = "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9";
        private const string QueryTypeId = "e71b0662-e29d-4db2-868b-8a12464b25d0";

        public DTOValidatorTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => DTOValidatorTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            var commandElements = _metadataManager.Services(applicationManager).GetElementsOfType(CommandTypeId);
            var queryElements = _metadataManager.Services(applicationManager).GetElementsOfType(QueryTypeId);
            var dtoFields = commandElements.Union(queryElements)
                .SelectMany(s => s.ChildElements.Where(p => p.SpecializationTypeId == DTOFieldModel.SpecializationTypeId));

            var operations = _metadataManager.Services(applicationManager).GetServiceModels()
                .SelectMany(x => x.Operations.Select(o => o.InternalElement));
            var parameters = operations
                .SelectMany(s => s.ChildElements.Where(p => p.SpecializationTypeId == ParameterModel.SpecializationTypeId));

            var referencedElements = DeepGetDistinctReferencedElements(dtoFields.Union(parameters));
            var referencedElementIds = referencedElements.Select(x => x.Id).ToHashSet();

            var models = _metadataManager.Services(applicationManager).GetDTOModels()
                .Where(x =>
                {
                    IElement advancedMappingSource = GetAdvancedMappings(dtoFields, x);

                    return referencedElementIds.Contains(x.Id) &&
                    ValidationRulesExtensions.HasValidationRules(
                        dtoModel: x,
                        dtoTemplateId: TemplateRoles.Application.Contracts.Dto,
                        dtoValidatorTemplateId: TemplateRoles.Application.Validation.Dto,
                        uniqueConstraintValidationEnabled: applicationManager.Settings.GetFluentValidationApplicationLayer().UniqueConstraintValidation().IsDefaultEnabled(),
                        customValidationEnabled: true,
                        associationedElements: advancedMappingSource?.AssociatedElements);
                })
                .ToArray();

            foreach (var model in models)
            {
                IElement advancedMappingSource = GetAdvancedMappings(dtoFields, model);

                registry.RegisterTemplate(TemplateId,
                    project => new DTOValidatorTemplate(
                        project,
                        model,
                        advancedMappingSource?.AssociatedElements));
            }
        }

        private static IElement GetAdvancedMappings(IEnumerable<IElement> dtoFields, IMetadataModel model)
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

        /// <remarks>
        /// This method is essentially a copy/paste of
        /// <see href="https://github.com/IntentArchitect/Intent.Modules/blob/150ac6a4535d4d28fa2ca384c4e7866f613da51a/Modules/Intent.Modules.Modelers.Types.ServiceProxies/ExtensionMethods.cs#L62">this</see>.
        /// </remarks>
        private static IEnumerable<IElement> DeepGetDistinctReferencedElements(IEnumerable<IElement> elements)
        {
            var referencedElements = new HashSet<IElement>();
            var workingStack = new Stack<IElement>(elements);

            while (workingStack.Any())
            {
                var currentElement = workingStack.Pop();
                var isDataContract = currentElement.SpecializationTypeId
                    is DTOModel.SpecializationTypeId
                    or CommandTypeId
                    or QueryTypeId;

                if (!isDataContract &&
                    currentElement.TypeReference?.Element is IElement referencedElement &&
                    referencedElements.Add(referencedElement)) // Avoid infinite loops due to cyclic references
                {
                    foreach (var childElement in referencedElement.ChildElements)
                    {
                        workingStack.Push(childElement);
                    }
                }

                foreach (var genericArgument in currentElement.TypeReference?.GenericTypeParameters ?? new List<ITypeReference>())
                {
                    if (genericArgument?.Element is not IElement genericArgumentType ||
                        !referencedElements.Add(genericArgumentType))
                    {
                        continue; // Avoid infinite loops due to cyclic references
                    }

                    foreach (var childElement in genericArgumentType.ChildElements)
                    {
                        workingStack.Push(childElement);
                    }
                }


                if (isDataContract)
                {
                    referencedElements.Add(currentElement);
                }

                foreach (var childElement in currentElement.ChildElements)
                {
                    workingStack.Push(childElement);
                }
            }

            return referencedElements;
        }
    }
}