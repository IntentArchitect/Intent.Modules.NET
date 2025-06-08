using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Utils;

namespace Intent.Modules.Application.DomainInteractions.InteractionStrategies
{
    public class CreateEntityInteractionStrategy : IInteractionStrategy
    {
        //public Dictionary<string, EntityDetails> TrackedEntities { get; set; } = new();
        public bool IsMatch(IElement interaction)
        {
            return interaction.IsCreateEntityActionTargetEndModel();
        }

        public void ImplementInteraction(CSharpClassMethod method, IElement interactionElement)
        {
            var interaction = (IAssociationEnd)interactionElement;
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            try
            {
                var createAction = interaction.AsCreateEntityActionTargetEndModel();
                var handlerClass = method.Class;
                var _csharpMapping = method.GetMappingManager();
                var entity = createAction.TypeReference.Element.AsClassModel() ?? createAction.TypeReference.Element.AsClassConstructorModel().ParentClass;

                var entityVariableName = createAction.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);
                var dataAccess = method.InjectDataAccessProvider(entity);

                method.TrackedEntities().Add(createAction.Id, new EntityDetails(entity.InternalElement, entityVariableName, dataAccess, true));

                var mapping = createAction.Mappings.SingleOrDefault();
                var statements = new List<CSharpStatement>();

                if (dataAccess.MustAccessEntityThroughAggregate())
                {
                    if (!method.TryGetFindAggregateStatements(mapping.SourceElement as IElement, entity, out statements))
                    {
                        Logging.Log.Warning($"Unable to implement creation logic for handler '{handlerClass.Name}'. See earlier warnings for more information.");
                        return;
                    }
                }

                if (mapping != null)
                {
                    var constructionStatement = _csharpMapping.GenerateCreationStatement(mapping);

                    handlerClass.WireupDomainServicesForConstructors(createAction, constructionStatement);

                    statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(entityVariableName), constructionStatement).WithSemicolon());
                }
                else
                {
                    statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(entityVariableName), $"new {entity.Name}();"));
                }

                method.AddStatements(statements);
                method.AddStatement(ExecutionPhases.Persistence, dataAccess.AddEntity(entityVariableName).SeparatedFromPrevious());

                _csharpMapping.SetFromReplacement(interaction, entityVariableName);
                _csharpMapping.SetFromReplacement(entity, entityVariableName);
                _csharpMapping.SetToReplacement(interaction, entityVariableName);
                _csharpMapping.SetToReplacement(entity, entityVariableName);

                if (interaction.OtherEnd().TypeReference.Element.TypeReference.Element != null)
                {
                    AddSaveChangesStatements(method, interaction.OtherEnd().TypeReference.Element.TypeReference);
                }
            }
            catch (Exception ex)
            {
                throw new ElementException(interaction, "An error occurred while generating the domain interactions logic", ex);
            }
        }


        private static void AddSaveChangesStatements(CSharpClassMethod method, ITypeReference returnType)
        {
            var nonUserSuppliedEntitiesReturningPks = GetEntitiesReturningPK(method, returnType, isUserSupplied: false);

            foreach (var entity in nonUserSuppliedEntitiesReturningPks.Where(x => x.IsNew).GroupBy(x => x.ElementModel.Id).Select(x => x.First()))
            {
                if (entity.ElementModel.IsClassModel())
                {
                    var primaryKeys = entity.ElementModel.AsClassModel().GetTypesInHierarchy().SelectMany(c => c.Attributes.Where(a => a.IsPrimaryKey()));
                    if (primaryKeys.Any(p => HasDBGeneratedPk(p)))
                    {
                        method.AddStatement(ExecutionPhases.Persistence, new CSharpStatement($"{entity.DataAccessProvider.SaveChangesAsync()}"));
                    }
                }
                else
                {
                    method.AddStatement(ExecutionPhases.Persistence, new CSharpStatement($"{entity.DataAccessProvider.SaveChangesAsync()}"));
                }
            }
        }


        private static List<EntityDetails> GetEntitiesReturningPK(CSharpClassMethod method, ITypeReference returnType, bool? isUserSupplied = null)
        {
            if (returnType.Element.IsDTOModel())
            {
                var dto = returnType.Element.AsDTOModel();

                var mappedPks = dto.Fields
                    .Where(x => x.Mapping != null && x.Mapping.Element.IsAttributeModel() && x.Mapping.Element.AsAttributeModel().IsPrimaryKey(isUserSupplied))
                    .Select(x => x.Mapping.Element.AsAttributeModel().InternalElement.ParentElement.Id)
                    .Distinct()
                    .ToList();
                if (mappedPks.Any())
                {
                    return method.TrackedEntities().Values
                        .Where(x => x.ElementModel.IsClassModel() && mappedPks.Contains(x.ElementModel.Id))
                        .ToList();
                }
                return new List<EntityDetails>();
            }
            return method.TrackedEntities().Values
                .Where(x => x.ElementModel.AsClassModel()?.GetTypesInHierarchy()
                    .SelectMany(c => c.Attributes)
                    .Count(a => a.IsPrimaryKey(isUserSupplied) && a.TypeReference.Element.Id == returnType.Element.Id) == 1)
                .ToList();
        }

        private static bool HasDBGeneratedPk(AttributeModel attribute)
        {
            return attribute.IsPrimaryKey() && attribute.GetStereotypeProperty("Primary Key", "Data source", "Default") == "Default";
        }
    }
}