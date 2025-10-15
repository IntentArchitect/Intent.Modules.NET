using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Application.DomainInteractions.Extensions;

// Disambiguation
using DataAccessProviders;
using Exceptions;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.DomainInteractions.Strategies;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Utils;

public static class DomainInteractionExtensions
{
    public static List<AssociationEndModel> GetAssociationsToAggregateRoot(this ClassModel entity)
    {
        var compositionalAssociations = entity.AssociatedClasses
            .Where(p => p.IsSourceEnd() && p is { IsCollection: false, IsNullable: false })
            .Distinct()
            .ToList();

        switch (compositionalAssociations.Count)
        {
            case 0:
                break;
            case 1:
                {
                    if (compositionalAssociations.Single().Class.IsAggregateRoot())
                    {
                        return compositionalAssociations;
                    }

                    var list = compositionalAssociations.Single().Class.GetAssociationsToAggregateRoot();
                    list.AddRange(compositionalAssociations);
                    return list;
                }
            default:
                Logging.Log.Warning($"{entity.Name} has multiple owning relationships.");
                break;
        }

        return [];
    }

    public static List<CSharpStatement> GetFindAggregateStatements(
        this ICSharpClassMethodDeclaration method,
        IDataAccessProviderInjector dataAccessProviderInjector,
        IElement requestElement,
        ClassModel foundEntity,
        out EntityDetails aggregateDetails)
    {
        var statements = new List<CSharpStatement>();
        var aggregateEntity = foundEntity.GetAssociationsToAggregateRoot().First().Class;
        var aggregateVariableName = aggregateEntity.Name.ToLocalVariableName();
        var aggregateDataAccess = dataAccessProviderInjector.Inject(method, aggregateEntity);

        var idFields = GetAggregatePkFindCriteria(requestElement, aggregateEntity, foundEntity);
        if (!idFields.Any())
        {
            Logging.Log.Warning($"Unable to determine how to load Aggregate : {aggregateEntity.Name} for {requestElement.Name}. Try adding a '{aggregateEntity.Name}Id' property to your request.");
        }

        statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(aggregateVariableName), aggregateDataAccess.FindByIdAsync(idFields)));

        statements.Add(method.File.Template.CreateIfNullThrowNotFoundStatement(
            variable: aggregateVariableName,
            message: $"Could not find {aggregateEntity.Name} '{{{idFields.Select(x => x.ValueExpression).AsSingleOrTuple()}}}'"));

        aggregateDetails = new EntityDetails(
            ElementModel: aggregateEntity.InternalElement,
            VariableName: aggregateVariableName,
            DataAccessProvider: aggregateDataAccess,
            IsNew: false,
            ProjectedType: null,
            IsCollection: false);

        // Traverse from aggregate root to target entity collection:
        var currentVariable = aggregateVariableName;
        foreach (var associationEndModel in foundEntity.GetAssociationsToAggregateRoot().SkipLast(1))
        {
            Func<string, string> formatter = requestElement.SpecializationType == "Operation" ? (x) => x.ToCamelCase() : (x) => $"request.{x}";

            var targetEntity = associationEndModel.OtherEnd().Class;
            var primaryKeys = targetEntity.Attributes.Where(x => x.IsPrimaryKey());
            var requestProperties = primaryKeys.Select(x =>
            {
                var simplifiedPk = x.Name.StartsWith(targetEntity.Name) ? x.Name.RemovePrefix(targetEntity.Name) : x.Name;
                var accessorExpression = $"{formatter(targetEntity.Name)}{simplifiedPk}";
                return (
                    Property: x.Name,
                    ValueExpression: new CSharpStatement(accessorExpression)
                );
            }).ToList();

            var expression = requestProperties.Count == 1
                ? $"x => x.{requestProperties[0].Property} == {requestProperties[0].ValueExpression}"
                : $"x => {string.Join(" && ", requestProperties.Select(pkMap => $"x.{pkMap.Property} == {pkMap.ValueExpression}"))}";

            statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(targetEntity.Name.ToLocalVariableName()),
                $"{currentVariable}.{associationEndModel.OtherEnd().Name.ToPropertyName()}.SingleOrDefault({expression})").WithSemicolon().SeparatedFromPrevious());

            currentVariable = targetEntity.Name.ToLocalVariableName();

            statements.Add(method.File.Template.CreateIfNullThrowNotFoundStatement(
                variable: currentVariable,
                message: $"Could not find {targetEntity.Name} '{{{requestProperties.Select(x => x.ValueExpression).AsSingleOrTuple()}}}'").SeparatedFromNext());
        }

        return statements;
    }

    public static IList<CSharpStatement> GetQueryStatements(
        this ICSharpClassMethodDeclaration method,
        IDataAccessProviderInjector dataAccessProviderInjector,
        IDataAccessProvider dataAccessProvider,
        IAssociationEnd interaction,
        ClassModel foundEntity,
        string? projectedType,
        bool mustAccessEntityThroughAggregate,
        out EntityDetails? aggregateDetails)
    {
        var queryMapping = interaction.Mappings.GetQueryEntityMapping();
        if (queryMapping == null)
        {
            throw new Exception($"{nameof(queryMapping)} is null");
        }

        var template = method.File.Template;
        var entityVariableName = interaction.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);

        var csharpMapping = method.GetMappingManager();
        csharpMapping.SetFromReplacement(foundEntity, entityVariableName);
        csharpMapping.SetFromReplacement(interaction, entityVariableName);
        csharpMapping.SetToReplacement(foundEntity, entityVariableName);
        csharpMapping.SetToReplacement(interaction, entityVariableName);

        CSharpStatement queryInvocation;
        var prerequisiteStatement = new List<CSharpStatement>();
        if (mustAccessEntityThroughAggregate)
        {
            prerequisiteStatement.AddRange(method.GetFindAggregateStatements(
                dataAccessProviderInjector: dataAccessProviderInjector,
                queryMapping: queryMapping,
                foundEntity: foundEntity,
                aggregateDetails: out aggregateDetails));

            if (interaction.TypeReference.IsCollection)
            {
                queryInvocation = dataAccessProvider.FindAllAsync(queryMapping, out var requiredStatements);
                prerequisiteStatement.AddRange(requiredStatements);
            }
            else
            {
                queryInvocation = dataAccessProvider.FindAsync(queryMapping, out var requiredStatements);
                prerequisiteStatement.AddRange(requiredStatements);
            }
        }
        else
        {
            // USE THE FindByIdAsync/FindByIdsAsync METHODS:
            if (queryMapping.MappedEnds.Any() && queryMapping.MappedEnds.All(x => x.TargetElement.AsAttributeModel()?.IsPrimaryKey() == true)
                                              && foundEntity.GetTypesInHierarchy().SelectMany(c => c.Attributes).Count(x => x.IsPrimaryKey()) == queryMapping.MappedEnds.Count)
            {
                var idFields = queryMapping.MappedEnds
                    .OrderBy(x => ((IElement)x.TargetElement).Order)
                    .Select(x => new PrimaryKeyFilterMapping(
                        method.GetMappingManager().GenerateSourceStatementForMapping(queryMapping, x),
                        x.TargetElement.AsAttributeModel().Name.ToPropertyName(),
                        x))
                    .ToList();

                if (interaction.TypeReference.IsCollection && idFields.All(x => x.Mapping.SourceElement.TypeReference.IsCollection))
                {
                    queryInvocation = dataAccessProvider.FindByIdsAsync(idFields);
                }
                else
                {
                    queryInvocation = dataAccessProvider.FindByIdAsync(idFields);
                }
            }
            // USE THE FindAllAsync/FindAsync METHODS WITH EXPRESSION:
            else
            {
                if (TryGetPaginationValues(interaction, csharpMapping, out var pageNo, out var pageSize, out var orderBy, out var orderByIsNullable))
                {
                    queryInvocation = dataAccessProvider.FindAllAsync(queryMapping, pageNo, pageSize, orderBy, orderByIsNullable, out var requiredStatements);
                    prerequisiteStatement.AddRange(requiredStatements);
                }
                else if (TryGetCursorPaginationValues(interaction, csharpMapping, out var cursorPageSize, out var cursorToken))
                {
                    queryInvocation = dataAccessProvider.FindAllAsync(queryMapping, cursorPageSize, cursorToken, out var requiredStatements);
                    prerequisiteStatement.AddRange(requiredStatements);
                }
                else if (interaction.TypeReference.IsCollection)
                {
                    queryInvocation = dataAccessProvider.FindAllAsync(queryMapping, out var requiredStatements);
                    prerequisiteStatement.AddRange(requiredStatements);
                }
                else
                {
                    queryInvocation = dataAccessProvider.FindAsync(queryMapping, out var requiredStatements);
                    prerequisiteStatement.AddRange(requiredStatements);
                }
            }
        }

        var statements = new List<CSharpStatement>();
        statements.AddRange(prerequisiteStatement);
        statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(entityVariableName), queryInvocation).SeparatedFromPrevious());

        if (!interaction.TypeReference.IsNullable &&
            !interaction.TypeReference.IsCollection &&
            !interaction.OtherEnd().TypeReference.Element.TypeReference.IsResultPaginated() &&
            !interaction.OtherEnd().TypeReference.Element.TypeReference.IsResultCursorPaginated())
        {
            var queryFields = queryMapping.MappedEnds
                .Select(x => new CSharpStatement($"{{{csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}}}"))
                .ToList();
            if (queryFields.Count == 0)
            {
                throw new ElementException(interaction, "Query for single entity has no mappings specified. Either indicate mappings or set Is Collection to true if trying to fetch all entities as a collection.");
            }

            statements.Add(CreateIfNullThrowNotFoundStatement(
                template: template,
                variable: entityVariableName,
                message: $"Could not find {foundEntity.Name} '{queryFields.AsSingleOrTuple()}'"));

        }

        method.TrackedEntities().Add(interaction.Id, new EntityDetails(
            ElementModel: foundEntity.InternalElement,
            VariableName: entityVariableName,
            DataAccessProvider: dataAccessProvider,
            IsNew: false,
            ProjectedType: projectedType,
            IsCollection: interaction.TypeReference.IsCollection));

        aggregateDetails = null;
        return statements;
    }

    public static IEnumerable<CSharpStatement> GetReturnStatements(this CSharpClassMethod method, ITypeReference returnType)
    {
        if (returnType.Element == null)
        {
            throw new Exception("No return type specified");
        }
        var statements = new List<CSharpStatement>();

        var mapperIsInstalled = method.File.Template.ExecutionContext.InstalledModules.Any(x => x.ModuleId == "Intent.Application.Dtos.AutoMapper"
        || x.ModuleId == "Intent.Application.Dtos.Mapperly");

        var template = method.File.Template;
        var entitiesReturningPk = GetEntitiesReturningPk(method, returnType);
        var nonUserSuppliedEntitiesReturningPks = GetEntitiesReturningPk(method, returnType, isUserSupplied: false);

        if (mapperIsInstalled &&
            returnType.Element.AsDTOModel()?.IsMapped == true &&
            method.TrackedEntities().Values.Any(x => x.ElementModel?.Id == returnType.Element.AsDTOModel().Mapping.ElementId) &&
            template.TryGetTypeName("Application.Contract.Dto", returnType.Element, out var returnDto))
        {
            var entityDetails = method.TrackedEntities().Values.First(x => x.ElementModel?.Id == returnType.Element.AsDTOModel().Mapping.ElementId);
            if (entityDetails.ProjectedType == returnDto)
            {
                statements.Add($"return {entityDetails.VariableName};");
            }
            else
            {
                var mapper = MappingStrategyProvider.Instance.GetMappingStrategy(method);
                mapper.ImplementMappingStatement(method, statements, entityDetails, template, returnType, returnDto);
            }
        }
        else if (mapperIsInstalled &&
                 (returnType.IsResultPaginated() || returnType.IsResultCursorPaginated()) &&
                 returnType.GenericTypeParameters.FirstOrDefault()?.Element.AsDTOModel()?.IsMapped == true &&
                 method.TrackedEntities().Values.Any(x => x.ElementModel?.Id == returnType.GenericTypeParameters.First().Element.AsDTOModel().Mapping.ElementId) &&
                 template.TryGetTypeName("Application.Contract.Dto", returnType.GenericTypeParameters.First().Element, out returnDto))
        {
            var mappingMethod = returnType.IsResultPaginated() ? "MapToPagedResult" : "MapToCursorPagedResult";

            var entityDetails = method.TrackedEntities().Values.First(x => x.ElementModel.Id == returnType.GenericTypeParameters.First().Element.AsDTOModel().Mapping.ElementId);
            if (entityDetails.ProjectedType == returnDto)
            {
                statements.Add($"return {entityDetails.VariableName}.{mappingMethod}();");
            }
            else
            {
                
                var mapper = MappingStrategyProvider.Instance.GetMappingStrategy(method);
                mapper.ImplementPagedMappingStatement(method, statements, entityDetails, template, returnType, returnDto, mappingMethod);
                //statements.Add($"return {entityDetails.VariableName}.{mappingMethod}(x => x.MapTo{returnDto}({autoMapperFieldName}));");
            }
        }
        else if (returnType.Element.IsTypeDefinitionModel() && (nonUserSuppliedEntitiesReturningPks.Count == 1 || entitiesReturningPk.Count == 1)) // No need for TrackedEntities thus no check for it
        {
            var entityDetails = nonUserSuppliedEntitiesReturningPks.Count == 1
                ? nonUserSuppliedEntitiesReturningPks[0]
                : entitiesReturningPk[0];
            var entity = entityDetails.ElementModel.AsClassModel();
            statements.Add($"return {entityDetails.VariableName}.{entity.GetTypesInHierarchy().SelectMany(x => x.Attributes).FirstOrDefault(x => x.IsPrimaryKey(isUserSupplied: false))?.Name ?? "Id"};");
        }
        else if (method.TrackedEntities().Values.Any(x => returnType.Element.Id == x.ElementModel.Id))
        {
            var entityDetails = method.TrackedEntities().Values.First(x => returnType.Element.Id == x.ElementModel.Id);
            statements.Add($"return {entityDetails.VariableName};");
        }
        else
        {
            statements.Add(new CSharpStatement("// IntentInitialGen").SeparatedFromPrevious());
            statements.Add("// TODO: Implement return type mapping...");
            statements.Add("""throw new NotImplementedException("Implement return type mapping...");""");
        }

        return statements;
    }

    public static bool HasDomainInteractions(this IProcessingHandlerModel model)
    {
        return model.CreateEntityActions().Any() ||
               model.QueryEntityActions().Any() ||
               model.UpdateEntityActions().Any() ||
               model.DeleteEntityActions().Any() ||
               model.PerformInvocationActions().Any() ||
               model.CallServiceOperationActions().Any();
    }

    public static bool MustAccessEntityThroughAggregate(this IDataAccessProvider dataAccess)
    {
        return dataAccess is CompositeDataAccessProvider;
    }

    private static CSharpIfStatement CreateIfNullThrowNotFoundStatement(
        this ICSharpTemplate template,
        string variable,
        string message)
    {
        var ifStatement = new CSharpIfStatement($"{variable} is null");
        ifStatement.SeparatedFromPrevious(false);
        ifStatement.AddStatement($@"throw new {template.GetNotFoundExceptionName()}($""{message}"");");

        return ifStatement;
    }

    private static List<PrimaryKeyFilterMapping> GetAggregatePkFindCriteria(IElement requestElement, ClassModel aggregateEntity, ClassModel compositeEntity)
    {
        //There is no mapping to the aggregate's PK, try to match is heuristically
        var aggPks = aggregateEntity.GetTypesInHierarchy().SelectMany(c => c.Attributes).Where(x => x.IsPrimaryKey()).ToList();
        var keyMappings = new List<AggregateKeyMapping>();
        var aggregatePrefix = aggregateEntity.Name.ToPascalCase();
        for (var i = 0; i < aggPks.Count; i++)
        {
            var aggPk = aggPks[i];
            var names = new List<string>();
            if (!compositeEntity.Attributes.Any(c => string.Equals(c.Name, aggPk.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                names.Add(aggPk.Name);
            }
            names.Add($"{aggregatePrefix}{aggPk.Name}");
            //May have renamed the FK attribute and as such it maybe a valid name
            var fkAttributes = compositeEntity.Attributes.Where(a => a.IsForeignKey() && a.GetForeignKeyAssociation()!.OtherEnd().Class.Id == aggregateEntity.Id).ToList();
            if (fkAttributes.Count > i)
            {
                names.Add(fkAttributes[i].Name);
            }

            var match = requestElement.ChildElements.FirstOrDefault(f => names.Contains(f.Name, StringComparer.OrdinalIgnoreCase))?.Name;
            keyMappings.Add(new AggregateKeyMapping(aggPk, match));
        }

        if (keyMappings.All(x => !string.IsNullOrEmpty(x.Match)))
        {
            var prefix = requestElement.SpecializationType == "Operation" ? "" : "request.";
            return keyMappings.Select(x => new PrimaryKeyFilterMapping($"{prefix}{x.Match}", $"{x.Key.Name}", new ElementToElementMappedEndStub(requestElement, aggregateEntity.InternalElement))).ToList();
        }
        return new List<PrimaryKeyFilterMapping>();
    }

    private static List<EntityDetails> GetEntitiesReturningPk(CSharpClassMethod method, ITypeReference returnType, bool? isUserSupplied = null)
    {
        if (returnType.Element.IsDTOModel())
        {
            var dto = returnType.Element.AsDTOModel();

            var mappedPks = dto.Fields
                .Where(x => x.Mapping != null && x.Mapping.Element.IsAttributeModel() && x.Mapping.Element.AsAttributeModel().IsPrimaryKey(isUserSupplied))
                .Select(x => x.Mapping.Element.AsAttributeModel().InternalElement.ParentElement.Id)
                .Distinct()
                .ToArray();

            if (mappedPks.Length == 0)
            {
                return [];
            }

            return method.TrackedEntities().Values
                .Where(x => x.ElementModel.IsClassModel() && mappedPks.Contains(x.ElementModel.Id))
                .ToList();

        }

        return method.TrackedEntities().Values
            .Where(x => x.ElementModel.AsClassModel()?.GetTypesInHierarchy()
                .SelectMany(c => c.Attributes)
                .Count(a => a.IsPrimaryKey(isUserSupplied) && a.TypeReference.Element.Id == returnType.Element.Id) == 1)
            .ToList();
    }

    private static List<CSharpStatement> GetFindAggregateStatements(
        this ICSharpClassMethodDeclaration method,
        IDataAccessProviderInjector dataAccessProviderInjector,
        IElementToElementMapping queryMapping,
        ClassModel foundEntity,
        out EntityDetails aggregateDetails)
    {
        return method.GetFindAggregateStatements(dataAccessProviderInjector, (IElement)queryMapping.SourceElement, foundEntity, out aggregateDetails);
    }

    private static bool IsOrderByParam(IElement param)
    {
        if (param.TypeReference.Element.Name != "string")
        {
            return false;
        }

        return param.Name.ToLower() switch
        {
            "orderby" => true,
            _ => false
        };
    }

    private static bool IsPageIndexParam(IElement param)
    {
        if (param.TypeReference.Element.Name != "int")
        {
            return false;
        }

        return param.Name.ToLower() switch
        {
            "pageindex" => true,
            _ => false
        };
    }

    private static bool IsPageNumberParam(IElement param)
    {
        if (param.TypeReference.Element.Name != "int")
        {
            return false;
        }

        return param.Name.ToLower() switch
        {
            "page" => true,
            "pageno" => true,
            "pagenum" => true,
            "pagenumber" => true,
            _ => false
        };
    }

    private static bool IsPageSizeParam(IElement param)
    {
        if (param.TypeReference.Element.Name != "int")
        {
            return false;
        }

        return param.Name.ToLower() switch
        {
            "size" => true,
            "pagesize" => true,
            _ => false
        };
    }

    private static bool IsResultCursorPaginated(this ITypeReference returnType)
    {
        return returnType.Element?.Name == "CursorPagedResult";
    }

    private static bool IsResultPaginated(this ITypeReference returnType)
    {
        return returnType.Element?.Name == "PagedResult";
    }

    private static bool IsTokenParam(IElement param)
    {
        if (param.TypeReference.Element.Name != "string")
        {
            return false;
        }

        return param.Name.ToLower() switch
        {
            "token" => true,
            "cursortoken" => true,
            _ => false
        };
    }

    private static bool TryGetCursorPaginationValues(IAssociationEnd associationEnd, CSharpClassMappingManager mappingManager, out string pageSize, out string? cursorToken)
    {
        var handler = (IElement)associationEnd.OtherEnd().TypeReference.Element;
        var returnsPagedResult = handler.TypeReference.IsResultCursorPaginated();

        var pageSizeVar = handler.ChildElements.SingleOrDefault(IsPageSizeParam)?.Name;
        var cursorTokenVar = handler.ChildElements.SingleOrDefault(IsTokenParam)?.Name;

        var accessVariable = mappingManager.GetFromReplacement(handler);

        if (!returnsPagedResult)
        {
            pageSize = "";
            cursorToken = "";
            return false;
        }

        if (string.IsNullOrEmpty(pageSizeVar))
        {
            throw new ElementException(handler, "Paged endpoints require a 'PageSize' property");
        }

        if (string.IsNullOrEmpty(cursorTokenVar))
        {
            throw new ElementException(handler, "Paged endpoints require a 'CursorToken' property");
        }

        pageSize = $"{(accessVariable != null ? $"{accessVariable}." : "")}{pageSizeVar}";
        cursorToken = $"{(accessVariable != null ? $"{accessVariable}." : "")}{cursorTokenVar}";

        return returnsPagedResult;
    }

    private static bool TryGetPaginationValues(IAssociationEnd associationEnd, CSharpClassMappingManager mappingManager, out string pageNo, out string pageSize, out string? orderBy, out bool orderByIsNullable)
    {
        orderByIsNullable = false;
        var handler = (IElement)associationEnd.OtherEnd().TypeReference.Element;
        var returnsPagedResult = handler.TypeReference.IsResultPaginated();

        var pageIndexVar = handler.ChildElements.SingleOrDefault(IsPageIndexParam)?.Name;
        var pageNoVar = handler.ChildElements.SingleOrDefault(IsPageNumberParam)?.Name;
        var pageSizeVar = handler.ChildElements.SingleOrDefault(IsPageSizeParam)?.Name;
        var accessVariable = mappingManager.GetFromReplacement(handler);

        if (!returnsPagedResult)
        {
            pageNo = "";
            pageSize = "";
            orderBy = null;
            return false;
        }

        if (string.IsNullOrEmpty(pageNoVar) && string.IsNullOrEmpty(pageIndexVar))
        {
            throw new ElementException(handler, "Paged endpoints require a 'PageNo' or 'PageIndex' property");
        }
        if (string.IsNullOrEmpty(pageSizeVar))
        {
            throw new ElementException(handler, "Paged endpoints require a 'PageSize' property");
        }

        pageNo = $"{(accessVariable != null ? $"{accessVariable}." : "")}{pageNoVar ?? $"{pageIndexVar} + 1"}";
        pageSize = $"{(accessVariable != null ? $"{accessVariable}." : "")}{pageSizeVar}";

        var orderByVar = handler.ChildElements.SingleOrDefault(IsOrderByParam);
        if (orderByVar == null)
        {
            orderBy = null;
        }
        else
        {
            orderByIsNullable = orderByVar.TypeReference.IsNullable;
            orderBy = $"{(accessVariable != null ? $"{accessVariable}." : "")}{handler.ChildElements.Single(IsOrderByParam)?.Name}";
        }

        return returnsPagedResult;
    }
}