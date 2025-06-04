using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Templates;
using Intent.Utils;

namespace Intent.Modules.Eventing.Contracts.InteractionStrategies;

public static class DataAccessProviderExtensions
{
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
        bool hasUnitOfWork = _template.TryGetTemplate<ITemplate>(TemplateRoles.Domain.UnitOfWork, out _);


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
            var aggregateAssociations = GetAssociationsToAggregateRoot(foundEntity);
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

            var list = GetAssociationsToAggregateRoot(compositionalAssociations.Single().Class);
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
    public static bool SettingGenerateDbContextInterface()
    {
        return true;
        //GetDatabaseSettings().GenerateDbContextInterface()
        //return bool.TryParse(_template.ExecutionContext.Settings.GetGroup("ac0a788e-d8b3-4eea-b56d-538608f1ded9").GetSetting("85dea0e8-8981-4c7b-908e-d99294fc37f1")?.Value.ToPascalCase(), out var result) && result;
    }


    public static bool TryGetFindAggregateStatements(this CSharpClassMethod method, IElementToElementMapping queryMapping, ClassModel foundEntity, out List<CSharpStatement> statements)
    {
        return TryGetFindAggregateStatements(method, queryMapping, (IElement)queryMapping.SourceElement, foundEntity, out statements);
    }

    public static bool TryGetFindAggregateStatements(this CSharpClassMethod method, IElement requestElement, ClassModel foundEntity, out List<CSharpStatement> statements)
    {
        return TryGetFindAggregateStatements(method, null, requestElement, foundEntity, out statements);
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