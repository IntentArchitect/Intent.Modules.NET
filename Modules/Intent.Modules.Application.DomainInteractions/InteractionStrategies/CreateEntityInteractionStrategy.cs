using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions.DataAccessProviders;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;

namespace Intent.Modules.Application.DomainInteractions.InteractionStrategies
{
    public class CreateEntityInteractionStrategy : IInteractionStrategy
    {
        public bool IsMatch(IElement interaction)
        {
            return interaction.IsCreateEntityActionTargetEndModel();
        }

        public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
        {
            ArgumentNullException.ThrowIfNull(method);
            var interaction = (IAssociationEnd)interactionElement;

            try
            {
                var createAction = interaction.AsCreateEntityActionTargetEndModel();
                var entity = createAction.TypeReference.Element.AsClassModel() ??
                             createAction.TypeReference.Element.AsClassConstructorModel()?.ParentClass ??
                             OperationModelExtensions.AsOperationModel(createAction.TypeReference.Element)?.ParentClass ??
                             throw new InvalidOperationException("Could not determine entity");
                var handlerClass = method.Class;
                var entityVariableName = createAction.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);
                var dataAccess = method.InjectDataAccessProvider(entity);

                var csharpMapping = method.GetMappingManager();
                csharpMapping.SetFromReplacement(interaction, entityVariableName);
                csharpMapping.SetFromReplacement(entity, entityVariableName);
                csharpMapping.SetToReplacement(interaction, entityVariableName);
                csharpMapping.SetToReplacement(entity, entityVariableName);

                method.TrackedEntities().Add(createAction.Id, new EntityDetails(entity.InternalElement, entityVariableName, dataAccess, true));

                var mapping = createAction.Mappings.Single();
                var statements = new List<CSharpStatement>();

                if (dataAccess.MustAccessEntityThroughAggregate())
                {
                    statements = method.GetFindAggregateStatements(
                        dataAccessProviderInjector: DataAccessProviderInjector.Instance,
                        requestElement: (IElement)mapping.SourceElement,
                        foundEntity: entity,
                        aggregateDetails: out _);
                }

                // Handle Lookup IDs mappings (surrogate IDs to entity collections)
                dataAccess.ProcessLookupIdsMappings(method, mapping, csharpMapping, statements);

                if (mapping != null)
                {
                    var constructionStatement = csharpMapping.GenerateCreationStatement(mapping);

                    handlerClass.WireUpDomainServicesForConstructors(createAction, constructionStatement);

                    statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(entityVariableName), constructionStatement).WithSemicolon());
                }
                else
                {
                    statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(entityVariableName), $"new {entity.Name}();"));
                }

                method.AddStatements(statements);
                method.AddStatement(ExecutionPhases.Persistence, dataAccess.AddEntity(entityVariableName).SeparatedFromPrevious());

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


        private static void AddSaveChangesStatements(ICSharpClassMethodDeclaration method, ITypeReference returnType)
        {

            // TODO: GCB - This needs to be re-looked at. It's using the tracked entities, which doesn't make sense.
            var nonUserSuppliedEntitiesReturningPks = GetEntitiesReturningPk(method, returnType, isUserSupplied: false);

            foreach (var entity in nonUserSuppliedEntitiesReturningPks.Where(x => x.IsNew).GroupBy(x => x.ElementModel.Id).Select(x => x.First()))
            {
                if (entity.ElementModel.IsClassModel())
                {
                    var primaryKeys = entity.ElementModel.AsClassModel().GetTypesInHierarchy().SelectMany(c => c.Attributes.Where(a => a.IsPrimaryKey()));
                    if (primaryKeys.Any(p => HasDBGeneratedPk(entity.ElementModel, p)))
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


        private static List<EntityDetails> GetEntitiesReturningPk(ICSharpClassMethodDeclaration method, ITypeReference returnType, bool? isUserSupplied = null)
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

        private class DataSources
        {
            /// <summary>
            /// Explicitly Stating the Keys are Auto-generated 
            /// DB Tech may have options for how ths is done, e.g. EF has options here 
            /// For EF this typically means assigned on Save Changes
            /// For Cosmos this would typically be set on object construction
            /// </summary>
            public const string AutoGenerated = "Auto-generated";
            /// <summary>
            /// What ever the default is for the technology.
            /// e.g. EF Auto-generated PKs, Guids EF assigned in Memory, Oridinals are DB Assigned
            /// </summary>
            public const string Default = "Default";
            /// <summary>
            /// PKS are set but the explicitly (Business keys, composite keys, these are typcially not auto assigned)
            /// </summary>            
            public const string UserSupplied = "User supplied";
        }

        private static bool HasDBGeneratedPk(IElement entity, AttributeModel attribute)
        {
            if (entity.Package.AsDomainPackageModel()?.HasStereotype("Relational Database") == true)
            {
                return attribute.IsPrimaryKey() && attribute.GetStereotypeProperty("Primary Key", "Data source", "Default") is DataSources.Default or DataSources.AutoGenerated;
            }
            // For document DBs User Supplied and Auto-generated is assumed to not be set by the DB
            return attribute.IsPrimaryKey() && attribute.GetStereotypeProperty("Primary Key", "Data source", "Default") == DataSources.Default;
        }
    }
}