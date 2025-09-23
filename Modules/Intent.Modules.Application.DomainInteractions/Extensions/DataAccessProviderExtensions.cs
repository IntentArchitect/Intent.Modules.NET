using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.DomainInteractions.DataAccessProviders;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Domain;

namespace Intent.Modules.Application.DomainInteractions.Extensions;

internal static class DataAccessProviderExtensions
{
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
                $"{aggregateAssociations.Last().Class.Name.ToLocalVariableName()}.{aggregateAssociations.Last().OtherEnd().Name}",
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
            //
            accessor: $"{aggregateAssociations.Last().Class.Name.ToLocalVariableName()}.{aggregateAssociations.Last().OtherEnd().Name}",
            explicitUpdateStatement: null,
            method: method);
        return true;
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
}