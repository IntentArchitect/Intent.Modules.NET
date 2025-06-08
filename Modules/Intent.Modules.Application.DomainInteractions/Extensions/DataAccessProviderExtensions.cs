using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Templates;
using Intent.Utils;
using static Intent.Modules.Constants.TemplateRoles.Domain;

namespace Intent.Modules.Application.DomainInteractions.Extensions;

public static class DataAccessProviderExtensions
{

    public static IList<CSharpStatement> GetQueryStatements(this CSharpClassMethod method, IAssociationEnd interaction, QueryActionContext queryContext)
    {
        var queryMapping = interaction.Mappings.GetQueryEntityMapping();
        var foundEntity = interaction.TypeReference.Element.AsClassModel();
        if (queryContext.ActionType == ActionType.Update && foundEntity == null)
        {
            foundEntity = Intent.Modelers.Domain.Api.OperationModelExtensions.AsOperationModel(interaction.TypeReference.Element).ParentClass;
        }
        var _template = method.File.Template;
        var entityVariableName = interaction.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);

        var _csharpMapping = method.GetMappingManager();
        _csharpMapping.SetFromReplacement(foundEntity, entityVariableName);
        _csharpMapping.SetFromReplacement(interaction, entityVariableName);
        _csharpMapping.SetToReplacement(foundEntity, entityVariableName);
        _csharpMapping.SetToReplacement(interaction, entityVariableName);

        var dataAccess = method.InjectDataAccessProvider(foundEntity, queryContext);
        CSharpStatement queryInvocation = null;
        var prerequisiteStatement = new List<CSharpStatement>();
        if (dataAccess.MustAccessEntityThroughAggregate())
        {
            if (!method.TryGetFindAggregateStatements(queryMapping, foundEntity, out var findAggStatements))
            {
                return [];
            }

            prerequisiteStatement.AddRange(findAggStatements);

            if (interaction.TypeReference.IsCollection)
            {
                queryInvocation = dataAccess.FindAllAsync(queryMapping, out var requiredStatements);
                prerequisiteStatement.AddRange(requiredStatements);
            }
            else
            {
                queryInvocation = dataAccess.FindAsync(queryMapping, out var requiredStatements);
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
                    queryInvocation = dataAccess.FindByIdsAsync(idFields);
                }
                else
                {
                    queryInvocation = dataAccess.FindByIdAsync(idFields);
                }
            }
            // USE THE FindAllAsync/FindAsync METHODS WITH EXPRESSION:
            else
            {
                //var expression = CreateQueryFilterExpression(queryMapping, out var requiredStatements);

                if (TryGetPaginationValues(interaction, _csharpMapping, out var pageNo, out var pageSize, out var orderBy, out var orderByIsNUllable))
                {
                    queryInvocation = dataAccess.FindAllAsync(queryMapping, pageNo, pageSize, orderBy, orderByIsNUllable, out var requiredStatements);
                    prerequisiteStatement.AddRange(requiredStatements);
                }
                else if (interaction.TypeReference.IsCollection)
                {
                    queryInvocation = dataAccess.FindAllAsync(queryMapping, out var requiredStatements);
                    prerequisiteStatement.AddRange(requiredStatements);
                }
                else
                {
                    queryInvocation = dataAccess.FindAsync(queryMapping, out var requiredStatements);
                    prerequisiteStatement.AddRange(requiredStatements);
                }
            }
        }

        var statements = new List<CSharpStatement>();
        statements.AddRange(prerequisiteStatement);
        statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(entityVariableName), queryInvocation).SeparatedFromPrevious());

        if (!interaction.TypeReference.IsNullable && !interaction.TypeReference.IsCollection && !interaction.OtherEnd().TypeReference.Element.TypeReference.IsResultPaginated())
        {
            var queryFields = queryMapping.MappedEnds
                .Select(x => new CSharpStatement($"{{{_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}}}"))
                .ToList();
            if (queryFields.Count == 0)
            {
                throw new ElementException(interaction, "Query for single entity has no mappings specified. Either indicate mappings or set Is Collection to true if trying to fetch all entities as a collection.");
            }

            statements.Add(CreateIfNullThrowNotFoundStatement(
                template: _template,
                variable: entityVariableName,
                message: $"Could not find {foundEntity.Name} '{queryFields.AsSingleOrTuple()}'"));

        }

        method.TrackedEntities().Add(interaction.Id, new EntityDetails(foundEntity.InternalElement, entityVariableName, dataAccess, false, queryContext.ImplementWithProjections() && dataAccess.IsUsingProjections ? queryContext.GetDtoProjectionReturnType() : null, interaction.TypeReference.IsCollection));
        return statements;
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

    public static bool IsResultPaginated(this ITypeReference returnType)
    {
        return returnType.Element?.Name == "PagedResult";
    }

    private static bool IsPageNumberParam(IElement param)
    {
        if (param.TypeReference.Element.Name != "int")
        {
            return false;
        }

        switch (param.Name.ToLower())
        {
            case "page":
            case "pageno":
            case "pagenum":
            case "pagenumber":
                return true;
            default:
                break;
        }

        return false;
    }

    private static bool IsPageIndexParam(IElement param)
    {
        if (param.TypeReference.Element.Name != "int")
        {
            return false;
        }

        switch (param.Name.ToLower())
        {
            case "pageindex":
                return true;
            default:
                return false;
        }
    }

    private static bool IsPageSizeParam(IElement param)
    {
        if (param.TypeReference.Element.Name != "int")
        {
            return false;
        }

        switch (param.Name.ToLower())
        {
            case "size":
            case "pagesize":
                return true;
            default:
                break;
        }

        return false;
    }

    private static bool IsOrderByParam(IElement param)
    {
        if (param.TypeReference.Element.Name != "string")
        {
            return false;
        }

        switch (param.Name.ToLower())
        {
            case "orderby":
                return true;
            default:
                return false;
        }
    }
    public static IDataAccessProvider InjectDataAccessProvider(this CSharpClassMethod method, ClassModel foundEntity, QueryActionContext queryContext = null)
    {
        if (TryInjectRepositoryForEntity(method, foundEntity, queryContext, out var dataAccess))
        {
            return dataAccess;
        }
        if (TryInjectDataAccessForComposite(method, foundEntity, out dataAccess))
        {
            return dataAccess;
        }
        if (TryInjectDbContext(method, foundEntity, queryContext, out dataAccess))
        {
            return dataAccess;
        }
        throw new Exception("No CRUD Data Access Provider found. Please install a Module with a Repository Pattern or EF Core Module.");
    }

    private static bool TryInjectRepositoryForEntity(CSharpClassMethod method, ClassModel foundEntity, QueryActionContext context, out IDataAccessProvider dataAccessProvider)
    {
        var _template = method.File.Template;
        if (!_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, foundEntity, out var repositoryInterface))
        {
            dataAccessProvider = null;
            return false;
        }

        //This is being done for Dapper
        bool hasUnitOfWork = _template.TryGetTemplate<ITemplate>(UnitOfWork, out _);


        dataAccessProvider = new RepositoryDataAccessProvider(method.Class.InjectService(repositoryInterface), (ICSharpFileBuilderTemplate)_template, method.GetMappingManager(), hasUnitOfWork, context, foundEntity);
        return true;
    }

    private static bool TryInjectDbContext(CSharpClassMethod method, ClassModel entity, QueryActionContext queryContext, out IDataAccessProvider dataAccessProvider)
    {
        var handlerClass = method.Class;
        var _template = handlerClass.File.Template;
        if (!_template.TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out var dbContextInterface) ||
            !SettingGenerateDbContextInterface())
        {
            dataAccessProvider = null;
            return false;
        }

        if (queryContext?.ImplementWithProjections() == true)
        {
            handlerClass.InjectService(_template.UseType("AutoMapper.IMapper"));
        }

        var dbContextField = handlerClass.InjectService(dbContextInterface, "dbContext");
        dataAccessProvider = new DbContextDataAccessProvider(dbContextField, entity, _template, method.GetMappingManager(), queryContext);
        return true;
    }

    private static bool TryInjectDataAccessForComposite(CSharpClassMethod method, ClassModel foundEntity, out IDataAccessProvider dataAccessProvider)
    {
        if (!foundEntity.IsAggregateRoot())
        {
            var handlerClass = method.Class;
            var _template = handlerClass.File.Template;
            _template.AddUsing("System.Linq");
            //var aggregateAssociations = foundEntity.AssociatedClasses
            //    .Where(p => p.TypeReference?.Element?.AsClassModel()?.IsAggregateRoot() == true &&
            //                p.IsSourceEnd() && !p.IsCollection && !p.IsNullable)
            //    .Distinct()
            //    .ToList();
            var aggregateAssociations = foundEntity.GetAssociationsToAggregateRoot();
            var aggregateEntity = aggregateAssociations.First().Class;

            if (_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, aggregateEntity, out var repositoryInterface))
            {
                bool requiresExplicitUpdate = RepositoryRequiresExplicitUpdate(_template, aggregateEntity);
                var repositoryName = handlerClass.InjectService(repositoryInterface);
                dataAccessProvider = new CompositeDataAccessProvider(
                    saveChangesAccessor: $"{repositoryName}.UnitOfWork",
                    accessor: $"{aggregateAssociations.Last().Name.ToLocalVariableName()}.{aggregateAssociations.Last().OtherEnd().Name}",
                    explicitUpdateStatement: requiresExplicitUpdate ? $"{repositoryName}.Update({aggregateEntity.Name.ToLocalVariableName()});" : null,
                    method: method
                );

                return true;
            }
            else if (_template.TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out var dbContextInterface) &&
                     SettingGenerateDbContextInterface())
            {
                var dbContextField = handlerClass.InjectService(dbContextInterface, "dbContext");
                dataAccessProvider = new CompositeDataAccessProvider(
                    saveChangesAccessor: dbContextField,
                    accessor: $"{aggregateAssociations.Last().Name.ToLocalVariableName()}.{aggregateAssociations.Last().OtherEnd().Name}",
                    explicitUpdateStatement: null,
                    method: method
                );
                return true;
            }
        }
        dataAccessProvider = null;
        return false;
    }


    public static List<AssociationEndModel> GetAssociationsToAggregateRoot(this ClassModel entity)
    {
        var compositionalAssociations = entity.AssociatedClasses
            .Where(p => p.IsSourceEnd() && p is { IsCollection: false, IsNullable: false })
            .Distinct()
            .ToList();

        if (compositionalAssociations.Count == 1)
        {
            if (compositionalAssociations.Single().Class.IsAggregateRoot())
            {
                return compositionalAssociations;
            }

            var list = compositionalAssociations.Single().Class.GetAssociationsToAggregateRoot();
            list.AddRange(compositionalAssociations);
            return list;
        }
        if (compositionalAssociations.Count > 1)
        {
            Logging.Log.Warning($"{entity.Name} has multiple owning relationships.");
        }
        return [];
    }

    public static bool MustAccessEntityThroughAggregate(this IDataAccessProvider dataAccess)
    {
        return dataAccess is CompositeDataAccessProvider;
    }

    private static bool RepositoryRequiresExplicitUpdate(ICSharpTemplate _template, IMetadataModel forEntity)
    {
        return _template.TryGetTemplate<ICSharpFileBuilderTemplate>(
                   TemplateRoles.Repository.Interface.Entity,
                   forEntity,
                   out var repositoryInterfaceTemplate) &&
               repositoryInterfaceTemplate.CSharpFile.Interfaces[0].TryGetMetadata<bool>("requires-explicit-update", out var requiresUpdate) &&
               requiresUpdate;
    }

    // This is likely to cause bugs since it doesn't align exactly with the logic that "enabled/disables" the IApplicationDbContext template
    private static bool SettingGenerateDbContextInterface()
    {
        return true;
        //GetDatabaseSettings().GenerateDbContextInterface()
        //return bool.TryParse(_template.ExecutionContext.Settings.GetGroup("ac0a788e-d8b3-4eea-b56d-538608f1ded9").GetSetting("85dea0e8-8981-4c7b-908e-d99294fc37f1")?.Value.ToPascalCase(), out var result) && result;
    }


    public static bool TryGetFindAggregateStatements(this CSharpClassMethod method, IElementToElementMapping queryMapping, ClassModel foundEntity, out List<CSharpStatement> statements)
    {
        return method.TryGetFindAggregateStatements(queryMapping, (IElement)queryMapping.SourceElement, foundEntity, out statements);
    }

    public static bool TryGetFindAggregateStatements(this CSharpClassMethod method, IElement requestElement, ClassModel foundEntity, out List<CSharpStatement> statements)
    {
        return method.TryGetFindAggregateStatements(null, requestElement, foundEntity, out statements);
    }

    private static bool TryGetFindAggregateStatements(this CSharpClassMethod method, IElementToElementMapping queryMapping, IElement requestElement, ClassModel foundEntity, out List<CSharpStatement> statements)
    {
        statements = new List<CSharpStatement>();
        var aggregateEntity = foundEntity.GetAssociationsToAggregateRoot().First().Class;
        var aggregateVariableName = aggregateEntity.Name.ToLocalVariableName();
        var aggregateDataAccess = method.InjectDataAccessProvider(aggregateEntity);

        var idFields = GetAggregatePKFindCriteria(requestElement, aggregateEntity, foundEntity);
        if (!idFields.Any())
        {
            Logging.Log.Warning($"Unable to determine how to load Aggregate : {aggregateEntity.Name} for {requestElement.Name}. Try adding a '{aggregateEntity.Name}Id' property to your request.");
        }

        statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(aggregateVariableName), aggregateDataAccess.FindByIdAsync(idFields)));

        statements.Add(CreateIfNullThrowNotFoundStatement(
            template: method.File.Template,
            variable: aggregateVariableName,
            message: $"Could not find {aggregateEntity.Name} '{{{idFields.Select(x => x.ValueExpression).AsSingleOrTuple()}}}'"));

        // Traverse from aggregate root to target entity collection:
        var currentVariable = aggregateVariableName;
        foreach (var associationEndModel in foundEntity.GetAssociationsToAggregateRoot().SkipLast(1))
        {
            var targetEntity = associationEndModel.OtherEnd().Class;
            var primaryKeys = targetEntity.Attributes.Where(x => x.IsPrimaryKey());
            var requestProperties = primaryKeys.Select(x => (
                Property: x.Name,
                ValueExpression: new CSharpStatement($"request.{targetEntity.Name}{x.Name}")
            )).ToList();

            var expression = requestProperties.Count == 1
                ? $"x => x.{requestProperties[0].Property} == {requestProperties[0].ValueExpression}"
                : $"x => {string.Join(" && ", requestProperties.Select(pkMap => $"x.{pkMap.Property} == {pkMap.ValueExpression}"))}";

            statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(targetEntity.Name.ToLocalVariableName()),
                $"{currentVariable}.{associationEndModel.OtherEnd().Name.ToPropertyName()}.SingleOrDefault({expression})").WithSemicolon().SeparatedFromPrevious());

            currentVariable = targetEntity.Name.ToLocalVariableName();

            statements.Add(CreateIfNullThrowNotFoundStatement(
                template: method.File.Template,
                variable: currentVariable,
                message: $"Could not find {targetEntity.Name} '{{{requestProperties.Select(x => x.ValueExpression).AsSingleOrTuple()}}}'").SeparatedFromNext());
        }

        return true;
    }


    private static List<PrimaryKeyFilterMapping> GetAggregatePKFindCriteria(IElement requestElement, ClassModel aggregateEntity, ClassModel compositeEntity)
    {
        //There is no mapping to the aggregate's PK, try to match is heuristically
        var aggPks = aggregateEntity.GetTypesInHierarchy().SelectMany(c => c.Attributes).Where(x => x.IsPrimaryKey()).ToList();
        var keyMappings = new List<AggregateKeyMapping>();
        var aggregatePrefix = aggregateEntity.Name.ToPascalCase();
        for (int i = 0; i < aggPks.Count; i++)
        {
            var aggPk = aggPks[i];
            var names = new List<string>();
            if (!compositeEntity.Attributes.Any(c => c.Name == aggPk.Name))
            {
                names.Add(aggPk.Name);
            }
            names.Add($"{aggregatePrefix}{aggPk.Name}");
            //May have renamed the FK attribute and as such it maybe a valid name
            var fkAttributes = compositeEntity.Attributes.Where(a => a.IsForeignKey() == true && a.GetForeignKeyAssociation().OtherEnd().Class.Id == aggregateEntity.Id).ToList();
            if (fkAttributes.Count > i)
            {
                names.Add(fkAttributes[i].Name);
            }

            var match = requestElement.ChildElements.FirstOrDefault(f => names.Contains(f.Name))?.Name;
            keyMappings.Add(new AggregateKeyMapping(aggPk, match));
        }

        if (keyMappings.All(x => !string.IsNullOrEmpty(x.Match)))
        {
            return keyMappings.Select(x => new PrimaryKeyFilterMapping($"request.{x.Match}", $"{x.Key.Name}", new ElementToElementMappedEndStub(requestElement, aggregateEntity.InternalElement))).ToList();
        }
        return new List<PrimaryKeyFilterMapping>();
    }

    public static CSharpStatement CreateIfNullThrowNotFoundStatement(
        ICSharpTemplate template,
        string variable,
        string message)
    {
        var ifStatement = new CSharpIfStatement($"{variable} is null");
        ifStatement.SeparatedFromPrevious(false);
        ifStatement.AddStatement($@"throw new {template.GetNotFoundExceptionName()}($""{message}"");");

        return ifStatement;
    }
}