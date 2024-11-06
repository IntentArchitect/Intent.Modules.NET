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

            // this gets all the models which are referenced directly
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
                .ToList();

            var modelIds = models.Select(m => m.Id);

            // This block is to get the DTOs which are included in a hierachy of inheritence, but which themselves do not
            // have a validator (as there is nothing on the DTO to validate), but which do have a parent which requires validation
            // In this case, the validator should be generated, as it needs to be called by the child and call into its parent
            var hierarchyModels = _metadataManager.Services(applicationManager).GetDTOModels()
                .Where(x =>
                {
                    HashSet<string> inheritenceIds = [];
                    var currentDto = x;
                    var @continue = true;

                    // get a list of all ids in the hierarchy of DTOs
                    // until we reach the end of the hierarchy OR a DTO already flagged for inclusion
                    while (currentDto.ParentDto != null && @continue)
                    {
                        currentDto = currentDto.ParentDto;
                        inheritenceIds.Add(currentDto.Id);
                        if (modelIds.Contains(currentDto.Id))
                        { 
                            @continue = false;
                        }
                    }

                    // If any in the hierachy have already been flagged for inclusions
                    // then the current DTO should be included
                    if (modelIds.Intersect(inheritenceIds).Any())
                    {
                        return true;
                    }

                    return false;

                }).ToArray();

            models = models.Union(hierarchyModels).ToList();

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
        /// with some additional functionality added for parent DTOs
        /// </remarks>
        private static IEnumerable<IElement> DeepGetDistinctReferencedElements(IEnumerable<IElement> elements)
        {
            var referencedElements = new HashSet<IElement>();
            var parentElements = new HashSet<IElement>();
            var workingStack = new Stack<IElement>(elements);

            while (workingStack.Count != 0)
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
                    StackReferencedElements(referencedElement, workingStack, parentElements);
                }

                // If it is a parent entity
                if (isDataContract && parentElements.Contains(currentElement))
                {
                    StackReferencedElements(currentElement, workingStack, parentElements);
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

        private static void StackReferencedElements(IElement referencedElement, Stack<IElement> workingStack, HashSet<IElement> parentElements)
        {
            // add all the child items
            foreach (var childElement in referencedElement.ChildElements)
            {
                workingStack.Push(childElement);
            }

            // if the current reference is a DTO, and it has a parent DTO - add to the stack
            // and record that it was added as a parent of another entity
            if (referencedElement.AsDTOModel() is not null && referencedElement.AsDTOModel().ParentDto is not null)
            {
                workingStack.Push(referencedElement.AsDTOModel().ParentDto.InternalElement);
                parentElements.Add(referencedElement.AsDTOModel().ParentDto.InternalElement);
            }
        }
    }
}