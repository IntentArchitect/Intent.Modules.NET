using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Eventing.Contracts.InteractionStrategies
{
    public class QueryInteractionStrategy : IInteractionStrategy
    {
        //public Dictionary<string, EntityDetails> TrackedEntities { get; set; } = new();
        public bool IsMatch(IAssociationEnd interaction)
        {
            return interaction.IsQueryEntityActionTargetEndModel();
        }

        public void ImplementInteraction(CSharpClassMethod method, IAssociationEnd interaction)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            try
            {
                var associationEnd = interaction;
                var foundEntity = interaction.TypeReference.Element.AsClassModel();
                var queryMapping = interaction.Mappings.GetQueryEntityMapping();
                if (queryMapping == null)
                {
                    throw new ElementException(interaction, "Query Entity Mapping has not been specified.");
                }

                var entityVariableName = associationEnd.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);

                var _template = method.File.Template;
                var _csharpMapping = method.GetMappingManager();
                var queryContext = new QueryActionContext(method, ActionType.Query, interaction);
                _csharpMapping.SetFromReplacement(foundEntity, entityVariableName);
                _csharpMapping.SetFromReplacement(associationEnd, entityVariableName);
                _csharpMapping.SetToReplacement(foundEntity, entityVariableName);
                _csharpMapping.SetToReplacement(associationEnd, entityVariableName);

                var dataAccess = method.InjectDataAccessProvider(foundEntity, queryContext);
                var queryStatements = method.GetQueryStatements(interaction, queryContext);
                //CSharpStatement queryInvocation = null;
                //var prerequisiteStatement = new List<CSharpStatement>();
                //if (dataAccess.MustAccessEntityThroughAggregate())
                //{
                //    if (!method.TryGetFindAggregateStatements(queryMapping, foundEntity, out var findAggStatements))
                //    {
                //        return;
                //    }

                //    prerequisiteStatement.AddRange(findAggStatements);

                //    if (associationEnd.TypeReference.IsCollection)
                //    {
                //        queryInvocation = dataAccess.FindAllAsync(queryMapping, out var requiredStatements);
                //        prerequisiteStatement.AddRange(requiredStatements);
                //    }
                //    else
                //    {
                //        queryInvocation = dataAccess.FindAsync(queryMapping, out var requiredStatements);
                //        prerequisiteStatement.AddRange(requiredStatements);
                //    }
                //}
                //else
                //{
                //    // USE THE FindByIdAsync/FindByIdsAsync METHODS:
                //    if (queryMapping.MappedEnds.Any() && queryMapping.MappedEnds.All(x => Intent.Modelers.Domain.Api.AttributeModelExtensions.AsAttributeModel(x.TargetElement)?.IsPrimaryKey() == true)
                //                                      && foundEntity.GetTypesInHierarchy().SelectMany(c => c.Attributes).Count(x => x.IsPrimaryKey()) == queryMapping.MappedEnds.Count)
                //    {
                //        var idFields = queryMapping.MappedEnds
                //        .OrderBy(x => ((IElement)x.TargetElement).Order)
                //            .Select(x => new PrimaryKeyFilterMapping(
                //                _csharpMapping.GenerateSourceStatementForMapping(queryMapping, x),
                //                Intent.Modelers.Domain.Api.AttributeModelExtensions.AsAttributeModel(x.TargetElement).Name.ToPropertyName(),
                //                x))
                //            .ToList();

                //        if (associationEnd.TypeReference.IsCollection && idFields.All(x => x.Mapping.SourceElement.TypeReference.IsCollection))
                //        {
                //            queryInvocation = dataAccess.FindByIdsAsync(idFields);
                //        }
                //        else
                //        {
                //            queryInvocation = dataAccess.FindByIdAsync(idFields);
                //        }
                //    }
                //    // USE THE FindAllAsync/FindAsync METHODS WITH EXPRESSION:
                //    else
                //    {
                //        //var expression = CreateQueryFilterExpression(queryMapping, out var requiredStatements);

                //        if (TryGetPaginationValues(associationEnd, _csharpMapping, out var pageNo, out var pageSize, out var orderBy, out var orderByIsNUllable))
                //        {
                //            queryInvocation = dataAccess.FindAllAsync(queryMapping, pageNo, pageSize, orderBy, orderByIsNUllable, out var requiredStatements);
                //            prerequisiteStatement.AddRange(requiredStatements);
                //        }
                //        else if (associationEnd.TypeReference.IsCollection)
                //        {
                //            queryInvocation = dataAccess.FindAllAsync(queryMapping, out var requiredStatements);
                //            prerequisiteStatement.AddRange(requiredStatements);
                //        }
                //        else
                //        {
                //            queryInvocation = dataAccess.FindAsync(queryMapping, out var requiredStatements);
                //            prerequisiteStatement.AddRange(requiredStatements);
                //        }
                //    }
                //}

                //var statements = new List<CSharpStatement>();
                //statements.AddRange(prerequisiteStatement);

                //if (!associationEnd.TypeReference.IsNullable && !associationEnd.TypeReference.IsCollection && !IsResultPaginated(associationEnd.OtherEnd().TypeReference.Element.TypeReference))
                //{
                //    var queryFields = queryMapping.MappedEnds
                //        .Select(x => new CSharpStatement($"{{{_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}}}"))
                //        .ToList();
                //    if (queryFields.Count == 0)
                //    {
                //        throw new ElementException(associationEnd, "Query for single entity has no mappings specified. Either indicate mappings or set Is Collection to true if trying to fetch all entities as a collection.");
                //    }

                //    statements.Add(DataAccessProviderExtensions.CreateIfNullThrowNotFoundStatement(
                //        template: _template,
                //        variable: entityVariableName,
                //        message: $"Could not find {foundEntity.Name} '{queryFields.AsSingleOrTuple()}'"));

                //}

                //method.TrackedEntities().Add(associationEnd.Id, new EntityDetails(foundEntity.InternalElement, entityVariableName, dataAccess, false, queryContext.ImplementWithProjections() && dataAccess.IsUsingProjections ? queryContext.GetDtoProjectionReturnType() : null, associationEnd.TypeReference.IsCollection));

                method.AddStatements(queryStatements);
            }
            catch (ElementException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ElementException(interaction, "An error occurred while generating the domain interactions logic", ex);
            }
        }
    }
}