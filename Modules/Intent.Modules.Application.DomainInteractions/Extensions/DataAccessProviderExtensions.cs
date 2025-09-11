using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.DomainInteractions.DataAccessProviders;
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
    public static IList<CSharpStatement> GetQueryStatements(this ICSharpClassMethodDeclaration method, IAssociationEnd interaction, QueryActionContext queryContext)
    {
        var queryMapping = interaction.Mappings.GetQueryEntityMapping();
        if (queryMapping == null)
        {
            throw new Exception($"{nameof(queryMapping)} is null");
        }

        var foundEntity = interaction.TypeReference.Element.AsClassModel();
        if (queryContext.ActionType == ActionType.Update && foundEntity == null)
        {
            foundEntity = interaction.TypeReference.Element.AsOperationModel().ParentClass;
        }

        var template = method.File.Template;
        var entityVariableName = interaction.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);

        var csharpMapping = method.GetMappingManager();
        csharpMapping.SetFromReplacement(foundEntity, entityVariableName);
        csharpMapping.SetFromReplacement(interaction, entityVariableName);
        csharpMapping.SetToReplacement(foundEntity, entityVariableName);
        csharpMapping.SetToReplacement(interaction, entityVariableName);

        var dataAccess = method.InjectDataAccessProvider(foundEntity, queryContext);
        CSharpStatement queryInvocation;
        var prerequisiteStatement = new List<CSharpStatement>();
        if (dataAccess.MustAccessEntityThroughAggregate())
        {
            prerequisiteStatement.AddRange(method.GetFindAggregateStatements(queryMapping, foundEntity));

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

                if (TryGetPaginationValues(interaction, csharpMapping, out var pageNo, out var pageSize, out var orderBy, out var orderByIsNullable))
                {
                    queryInvocation = dataAccess.FindAllAsync(queryMapping, pageNo, pageSize, orderBy, orderByIsNullable, out var requiredStatements);
                    prerequisiteStatement.AddRange(requiredStatements);
                }
                else if (TryGetCursorPaginationValues(interaction, csharpMapping, out var cursorPageSize, out var cursorToken))
                {
                    queryInvocation = dataAccess.FindAllAsync(queryMapping, cursorPageSize, cursorToken, out var requiredStatements);
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

        if (!interaction.TypeReference.IsNullable && !interaction.TypeReference.IsCollection
            && !interaction.OtherEnd().TypeReference.Element.TypeReference.IsResultPaginated()
            && !interaction.OtherEnd().TypeReference.Element.TypeReference.IsResultCursorPaginated())
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

    public static bool IsResultPaginated(this ITypeReference returnType)
    {
        return returnType.Element?.Name == "PagedResult";
    }

    public static bool IsResultCursorPaginated(this ITypeReference returnType)
    {
        return returnType.Element?.Name == "CursorPagedResult";
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
    public static IDataAccessProvider InjectDataAccessProvider(this ICSharpClassMethodDeclaration method, ClassModel foundEntity, QueryActionContext? queryContext = null)
    {
        if (TryInjectRepositoryForEntity(method, foundEntity, queryContext, out var dataAccess) ||
            TryInjectDataAccessForComposite(method, foundEntity, out dataAccess) ||
            TryInjectDbContext(method, foundEntity, queryContext, out dataAccess))
        {
            return dataAccess;
        }

        throw new Exception("No CRUD Data Access Provider found. Please install a Module with a Repository Pattern or EF Core Module.");
    }

    private static bool TryInjectRepositoryForEntity(
        ICSharpClassMethodDeclaration method,
        ClassModel foundEntity,
        QueryActionContext? context,
        [NotNullWhen(true)] out IDataAccessProvider? dataAccessProvider)
    {
        var template = method.File.Template;
        if (!template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, foundEntity, out var repositoryInterface))
        {
            dataAccessProvider = null;
            return false;
        }

        //This is being done for Dapper
        var hasUnitOfWork = template.TryGetTemplate<ITemplate>(UnitOfWork, out _);

        dataAccessProvider = new RepositoryDataAccessProvider(
            repositoryFieldName: method.Class.InjectService(repositoryInterface),
            template: (ICSharpFileBuilderTemplate)template,
            mappingManager: method.GetMappingManager(),
            hasUnitOfWork: hasUnitOfWork,
            queryContext: context,
            entity: foundEntity);
        return true;
    }

    private static bool TryInjectDbContext(
        ICSharpClassMethodDeclaration method,
        ClassModel entity,
        QueryActionContext? queryContext,
        [NotNullWhen(true)] out IDataAccessProvider? dataAccessProvider)
    {
        var handlerClass = method.Class;
        var template = handlerClass.File.Template;

        var dbContextInterfaceTemplate = template.ExecutionContext.FindTemplateInstance<ICSharpTemplate>(TemplateRoles.Application.Common.DbContextInterface);
        if (dbContextInterfaceTemplate?.CanRunTemplate() != true)
        {
            dataAccessProvider = null;
            return false;
        }

        if (queryContext?.ImplementWithProjections() == true)
        {
            handlerClass.InjectService(template.UseType("AutoMapper.IMapper"));
        }

        var dbContextField = handlerClass.InjectService(template.GetTypeName(dbContextInterfaceTemplate), "dbContext");
        dataAccessProvider = new DbContextDataAccessProvider(dbContextField, entity, template, method.GetMappingManager(), queryContext);
        return true;
    }

    private static bool TryInjectDataAccessForComposite(
        ICSharpClassMethodDeclaration method,
        ClassModel foundEntity,
        [NotNullWhen(true)] out IDataAccessProvider? dataAccessProvider)
    {
        if (foundEntity.IsAggregateRoot())
        {
            dataAccessProvider = null;
            return false;
        }

        var handlerClass = method.Class;
        var template = handlerClass.File.Template;
        template.AddUsing("System.Linq");
        var aggregateAssociations = foundEntity.GetAssociationsToAggregateRoot();
        var aggregateEntity = aggregateAssociations.First().Class;

        if (template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, aggregateEntity,
                out var repositoryInterface))
        {
            var requiresExplicitUpdate = RepositoryRequiresExplicitUpdate(template, aggregateEntity);
            var repositoryName = handlerClass.InjectService(repositoryInterface);
            dataAccessProvider = new CompositeDataAccessProvider(
                saveChangesAccessor: $"{repositoryName}.UnitOfWork",
                accessor:
                $"{aggregateAssociations.Last().Name.ToLocalVariableName()}.{aggregateAssociations.Last().OtherEnd().Name}",
                explicitUpdateStatement: requiresExplicitUpdate
                    ? $"{repositoryName}.Update({aggregateEntity.Name.ToLocalVariableName()});"
                    : null,
                method: method
            );

            return true;
        }

        var dbContextInterfaceTemplate = template.ExecutionContext.FindTemplateInstance<ICSharpTemplate>(TemplateRoles.Application.Common.DbContextInterface);
        if (dbContextInterfaceTemplate?.CanRunTemplate() != true)
        {
            dataAccessProvider = null;
            return false;
        }

        var dbContextField = handlerClass.InjectService(template.GetTypeName(dbContextInterfaceTemplate), "dbContext");
        dataAccessProvider = new CompositeDataAccessProvider(
            saveChangesAccessor: dbContextField,
            accessor: $"{aggregateAssociations.Last().Name.ToLocalVariableName()}.{aggregateAssociations.Last().OtherEnd().Name}",
            explicitUpdateStatement: null,
            method: method);
        return true;
    }

    public static List<AssociationEndModel> GetAssociationsToAggregateRoot(this ClassModel entity)
    {
        var compositionalAssociations = entity.AssociatedClasses
            .Where(p => p.IsSourceEnd() && p is { IsCollection: false, IsNullable: false })
            .Distinct()
            .ToList();

        switch (compositionalAssociations.Count)
        {
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
            case > 1:
                Logging.Log.Warning($"{entity.Name} has multiple owning relationships.");
                break;
        }

        return [];
    }

    public static bool MustAccessEntityThroughAggregate(this IDataAccessProvider dataAccess)
    {
        return dataAccess is CompositeDataAccessProvider;
    }

    private static bool RepositoryRequiresExplicitUpdate(ICSharpTemplate template, IMetadataModel forEntity)
    {
        return template.TryGetTemplate<ICSharpFileBuilderTemplate>(
                   TemplateRoles.Repository.Interface.Entity,
                   forEntity,
                   out var repositoryInterfaceTemplate) &&
               repositoryInterfaceTemplate.CSharpFile.Interfaces[0].TryGetMetadata<bool>("requires-explicit-update", out var requiresUpdate) &&
               requiresUpdate;
    }

    public static List<CSharpStatement> GetFindAggregateStatements(this ICSharpClassMethodDeclaration method, IElementToElementMapping queryMapping, ClassModel foundEntity)
    {
        return method.GetFindAggregateStatements((IElement)queryMapping.SourceElement, foundEntity);
    }

    public static List<CSharpStatement> GetFindAggregateStatements(this ICSharpClassMethodDeclaration method, IElement requestElement, ClassModel foundEntity)
    {
        var statements = new List<CSharpStatement>();
        var aggregateEntity = foundEntity.GetAssociationsToAggregateRoot().First().Class;
        var aggregateVariableName = aggregateEntity.Name.ToLocalVariableName();
        var aggregateDataAccess = method.InjectDataAccessProvider(aggregateEntity);

        var idFields = GetAggregatePkFindCriteria(requestElement, aggregateEntity, foundEntity);
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

            statements.Add(CreateIfNullThrowNotFoundStatement(
                template: method.File.Template,
                variable: currentVariable,
                message: $"Could not find {targetEntity.Name} '{{{requestProperties.Select(x => x.ValueExpression).AsSingleOrTuple()}}}'").SeparatedFromNext());
        }

        return statements;
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