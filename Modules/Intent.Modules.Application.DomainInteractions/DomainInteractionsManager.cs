using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Templates;
using Intent.Utils;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;

namespace Intent.Modules.Application.DomainInteractions;

public static class DomainInteractionExtensions
{

    public static bool HasDomainInteractions(this IProcessingHandlerModel model)
    {
        return model.CreateEntityActions().Any()
               || model.QueryEntityActions().Any()
               || model.UpdateEntityActions().Any()
               || model.DeleteEntityActions().Any()
               || model.CallServiceOperationActions().Any();
    }
}

public class DomainInteractionsManager
{
    private readonly ICSharpFileBuilderTemplate _template;
    private readonly CSharpClassMappingManager _csharpMapping;

    public DomainInteractionsManager(ICSharpFileBuilderTemplate template, CSharpClassMappingManager csharpMapping)
    {
        _template = template;
        _csharpMapping = csharpMapping;
    }

    public Dictionary<string, EntityDetails> TrackedEntities { get; set; } = new();

    public IEnumerable<CSharpStatement> CreateInteractionStatements(IProcessingHandlerModel model)
    {
        var domainInteractionManager = this;
        var statements = new List<CSharpStatement>();
        try
        {
            foreach (var queryAction in model.QueryEntityActions())
            {
                var foundEntity = queryAction.Element.AsClassModel();
                if (foundEntity != null && queryAction.Mappings.GetQueryEntityMapping() != null)
                {
                    statements.AddRange(domainInteractionManager.QueryEntity(foundEntity, queryAction.InternalAssociationEnd));
                }
            }

            foreach (var createAction in model.CreateEntityActions())
            {
                statements.AddRange(domainInteractionManager.CreateEntity(createAction));
            }

            foreach (var updateAction in model.UpdateEntityActions())
            {
                var entity = updateAction.Element.AsClassModel() ?? OperationModelExtensions.AsOperationModel(updateAction.Element).ParentClass;

                statements.AddRange(domainInteractionManager.QueryEntity(entity, updateAction.InternalAssociationEnd));

                statements.Add(string.Empty);
                statements.AddRange(domainInteractionManager.UpdateEntity(updateAction));
            }

            foreach (var callAction in model.CallServiceOperationActions())
            {
                statements.AddRange(domainInteractionManager.CallServiceOperation(callAction));
            }

            foreach (var deleteAction in model.DeleteEntityActions())
            {
                var foundEntity = deleteAction.Element.AsClassModel();
                statements.AddRange(domainInteractionManager.QueryEntity(foundEntity, deleteAction.InternalAssociationEnd));
                statements.AddRange(domainInteractionManager.DeleteEntity(deleteAction));
            }

            foreach (var actions in model.ProcessingActions().Where(x => x.InternalElement.Mappings.Count() == 1))
            {
                try
                {
                    var processingStatements = _csharpMapping.GenerateUpdateStatements(actions.InternalElement.Mappings.Single())
                    .Select(x =>
                    {
                        if (x is CSharpAssignmentStatement)
                        {
                            x.WithSemicolon();
                        }

                        return x;
                    }).ToList();
                    processingStatements.FirstOrDefault()?.SeparatedFromPrevious();
                    statements.AddRange(processingStatements);

                }
                catch (Exception ex)
                {
                    throw new ElementException(actions.InternalElement, "An error occurred while generating processing action logic", ex);
                }
            }

            foreach (var entity in domainInteractionManager.TrackedEntities.Values.Where(x => x.IsNew))
            {
                statements.Add(entity.DataAccessProvider.AddEntity(entity.VariableName).SeparatedFromPrevious());
            }

            return statements;
        }
        catch (ElementException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ElementException(model.InternalElement, "An error occurred while generating the domain interactions logic. See inner exception for more details", ex);
        }
    }

    public List<CSharpStatement> QueryEntity(ClassModel foundEntity, IAssociationEnd associationEnd)
    {
        try
        {
            var queryMapping = associationEnd.Mappings.GetQueryEntityMapping();
            if (queryMapping == null)
            {
                throw new ElementException(associationEnd, "Query Entity Mapping has not been specified.");
            }

            var entityVariableName = associationEnd.Name;

            _csharpMapping.SetFromReplacement(foundEntity, entityVariableName);
            _csharpMapping.SetFromReplacement(associationEnd, entityVariableName);
            _csharpMapping.SetToReplacement(foundEntity, entityVariableName);
            _csharpMapping.SetToReplacement(associationEnd, entityVariableName);

            var dataAccess = InjectDataAccessProvider(foundEntity);
            var statements = new List<CSharpStatement>();

            if (MustAccessEntityThroughAggregate(dataAccess))
            {
                if (!TryGetFindAggregateStatements(queryMapping, foundEntity, out var findAggStatements))
                {
                    return new List<CSharpStatement>();
                }

                statements.AddRange(findAggStatements);

                var expression = CreateQueryFilterExpression(queryMapping, out var requiredStatements);
                statements.AddRange(requiredStatements);

                if (associationEnd.TypeReference.IsCollection)
                {
                    statements.Add(new CSharpAssignmentStatement($"var {entityVariableName}", dataAccess.FindAllAsync(expression)).SeparatedFromPrevious());
                }
                else
                {
                    statements.Add(new CSharpAssignmentStatement($"var {entityVariableName}", dataAccess.FindAsync(expression)).SeparatedFromPrevious());
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
                            _csharpMapping.GenerateSourceStatementForMapping(queryMapping, x),
                            x.TargetElement.AsAttributeModel().Name.ToPropertyName(),
                            x))
                        .ToList();

                    if (associationEnd.TypeReference.IsCollection && idFields.All(x => x.Mapping.SourceElement.TypeReference.IsCollection))
                    {
                        statements.Add(new CSharpAssignmentStatement($"var {entityVariableName}", dataAccess.FindByIdsAsync(idFields)));
                    }
                    else
                    {
                        statements.Add(new CSharpAssignmentStatement($"var {entityVariableName}", dataAccess.FindByIdAsync(idFields)));
                    }
                }
                // USE THE FindAllAsync/FindAsync METHODS WITH EXPRESSION:
                else
                {
                    var expression = CreateQueryFilterExpression(queryMapping, out var requiredStatements);
                    statements.AddRange(requiredStatements);

                    if (associationEnd.TypeReference.IsCollection)
                    {
                        statements.Add(new CSharpAssignmentStatement($"var {entityVariableName}", dataAccess.FindAllAsync(expression)));
                    }
                    else if (TryGetPaginationValues(associationEnd, _csharpMapping, out var pageNo, out var pageSize))
                    {
                        statements.Add(new CSharpAssignmentStatement($"var {entityVariableName}", dataAccess.FindAllAsync(expression, pageNo, pageSize)));
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(expression))
                        {
                            throw new ElementException(associationEnd, "No query fields have been mapped for this Query Entity Action, which signifies a single return value. Either specify this action returns a collection or map at least one field.");
                        }

                        statements.Add(new CSharpAssignmentStatement($"var {entityVariableName}", dataAccess.FindAsync(expression)));
                    }
                }
            }

            if (!associationEnd.TypeReference.IsNullable && !associationEnd.TypeReference.IsCollection && !IsResultPaginated(associationEnd.OtherEnd().TypeReference.Element.TypeReference))
            {
                var queryFields = queryMapping.MappedEnds
                    .Select(x => new CSharpStatement($"{{{_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}}}"))
                    .ToList();
                statements.Add(CreateIfNullThrowNotFoundStatement(
                    template: _template,
                    variable: entityVariableName,
                    message: $"Could not find {foundEntity.Name.ToPascalCase()} '{queryFields.AsSingleOrTuple()}'"));

            }

            TrackedEntities.Add(associationEnd.Id, new EntityDetails(foundEntity, entityVariableName, dataAccess, false, associationEnd.TypeReference.IsCollection));

            return statements;
        }
        catch (Exception ex)
        {
            throw new ElementException(associationEnd, "An error occurred while generating the domain interactions logic", ex);
        }
    }

    private string CreateQueryFilterExpression(IElementToElementMapping queryMapping, out IList<CSharpStatement> requiredStatements)
    {
        requiredStatements = new List<CSharpStatement>();

        var queryFields = queryMapping.MappedEnds.Where(x => !x.SourceElement.TypeReference.IsNullable)
            .Select(x => x.IsOneToOne()
                ? $"x.{x.TargetElement.Name} == {_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}"
                : $"x.{x.TargetElement.Name}.{_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}")
            .ToList();

        var expression = queryFields.Any() ? $"x => {string.Join(" && ", queryFields)}" : "";

        if (queryMapping.MappedEnds.All(x => !x.SourceElement.TypeReference.IsNullable))
        {
            return expression;
        }

        var typeName = _template.GetTypeName((IElement)queryMapping.TargetElement);
        var filterName = $"Filter{typeName.Pluralize()}";
        var block = new CSharpStatementBlock($"IQueryable<{typeName}> {filterName}(IQueryable<{typeName}> queryable)");

        if (!string.IsNullOrWhiteSpace(expression))
        {
            block.AddStatement($"queryable = queryable.Where({expression})", x => x.WithSemicolon());
        }

        foreach (var mappedEnd in queryMapping.MappedEnds.Where(x => x.SourceElement.TypeReference.IsNullable))
        {
            block.AddIfStatement(_csharpMapping.GenerateSourceStatementForMapping(queryMapping, mappedEnd) + " != null", inside =>
            {
                inside.AddStatement($"queryable = queryable.Where(x => x.{mappedEnd.TargetElement.Name} == {_csharpMapping.GenerateSourceStatementForMapping(queryMapping, mappedEnd)})", x => x.WithSemicolon());
            });
        }

        block.AddStatement("return queryable;");
        block.SeparatedFromNext();
        requiredStatements.Add(block);

        return filterName;
    }

    public CSharpStatement CreateIfNullThrowNotFoundStatement(
        ICSharpTemplate template,
        string variable,
        string message)
    {
        var ifStatement = new CSharpIfStatement($"{variable} is null");
        ifStatement.SeparatedFromPrevious(false);
        ifStatement.AddStatement($@"throw new {template.GetNotFoundExceptionName()}($""{message}"");");

        return ifStatement;
    }

    public bool TryInjectDataAccessForComposite(ClassModel foundEntity, out IDataAccessProvider dataAccessProvider)
    {
        if (!foundEntity.IsAggregateRoot())
        {
            _template.AddUsing("System.Linq");
            var aggregateAssociations = foundEntity.AssociatedClasses
                .Where(p => p.TypeReference?.Element?.AsClassModel()?.IsAggregateRoot() == true &&
                            p.IsSourceEnd() && !p.IsCollection && !p.IsNullable)
                .Distinct()
                .ToList();
            if (aggregateAssociations.Count == 1)
            {
                var aggregateEntity = aggregateAssociations.Single().Class;

                if (_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, aggregateEntity, out var repositoryInterface))
                {
                    bool requiresExplicitUpate = RepositoryRequiresExplicitUpdate(aggregateEntity);
                    var repositoryName = InjectService(repositoryInterface);
                    dataAccessProvider = new CompositeDataAccessProvider(
                        $"{repositoryName}.UnitOfWork",
                        $"{aggregateEntity.Name.ToLocalVariableName()}.{aggregateAssociations.Single().OtherEnd().Name}",
                        requiresExplicitUpate ? $"{repositoryName}.Update({aggregateEntity.Name.ToLocalVariableName()});" : null
                        );

                    return true;
                }
                else if (_template.TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out var dbContextInterface) &&
                    SettingGenerateDbContextInterface())
                {
                    var dbContextField = InjectService(dbContextInterface, "dbContext");
                    dataAccessProvider = new CompositeDataAccessProvider(
                        dbContextField,
                        $"{aggregateEntity.Name.ToLocalVariableName()}.{aggregateAssociations.Single().OtherEnd().Name}");
                    return true;
                }
            }
        }
        dataAccessProvider = null;
        return false;
    }

    public bool TryInjectRepositoryForEntity(ClassModel foundEntity, out IDataAccessProvider dataAccessProvider)
    {

        if (!_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, foundEntity, out var repositoryInterface))
        {
            dataAccessProvider = null;
            return false;
        }
        dataAccessProvider = new RepositoryDataAccessProvider(InjectService(repositoryInterface));
        return true;
    }

    //private string InjectRepositoryService(string repositoryInterface)
    //{
    //    var repositoryName = repositoryInterface[1..].ToCamelCase();
    //    var repositoryFieldName = default(string);

    //    var ctor = _template.CSharpFile.Classes.First().Constructors.First();
    //    if (ctor.Parameters.All(x => x.Type != repositoryInterface))
    //    {
    //        ctor.AddParameter(repositoryInterface, repositoryName.ToParameterName(),
    //            param => param.IntroduceReadonlyField(field => repositoryFieldName = field.Name));
    //    }
    //    else
    //    {
    //        repositoryFieldName = ctor.Parameters.First(x => x.Type == repositoryInterface).Name.ToPrivateMemberName();
    //    }

    //    return repositoryFieldName;
    //}

    // This is likely to cause bugs since it doesn't align exactly with the logic that "enabled/disables" the IApplicationDbContext template
    public bool SettingGenerateDbContextInterface()
    {
        return true;
        //GetDatabaseSettings().GenerateDbContextInterface()
        return bool.TryParse(_template.ExecutionContext.Settings.GetGroup("ac0a788e-d8b3-4eea-b56d-538608f1ded9").GetSetting("85dea0e8-8981-4c7b-908e-d99294fc37f1")?.Value.ToPascalCase(), out var result) && result;
    }

    public IEnumerable<CSharpStatement> GetReturnStatements(ITypeReference returnType)
    {
        if (returnType.Element == null)
        {
            throw new Exception("No return type specified");
        }
        var statements = new List<CSharpStatement>();
        var entitiesReturningPk = TrackedEntities.Values
            .Where(x => x.Model.GetTypesInHierarchy().SelectMany(c => c.Attributes).Count(a => a.IsPrimaryKey() && a.TypeReference.Element.Id == returnType.Element.Id) == 1)
            .ToList();
        foreach (var entity in entitiesReturningPk.Where(x => x.IsNew).GroupBy(x => x.Model.Id).Select(x => x.First()))
        {
            statements.Add($"await {entity.DataAccessProvider.SaveChangesAsync()}");
        }

        if (returnType.Element.AsDTOModel()?.IsMapped == true && _template.TryGetTypeName("Application.Contract.Dto", returnType.Element, out var returnDto))
        {
            var entityDetails = TrackedEntities.Values.First(x => x.Model.Id == returnType.Element.AsDTOModel().Mapping.ElementId);
            var autoMapperFieldName = InjectService(_template.UseType("AutoMapper.IMapper"));
            statements.Add($"return {entityDetails.VariableName}.MapTo{returnDto}{(returnType.IsCollection ? "List" : "")}({autoMapperFieldName});");
        }
        else if (IsResultPaginated(returnType) && returnType.GenericTypeParameters.FirstOrDefault()?.Element.AsDTOModel()?.IsMapped == true && _template.TryGetTypeName("Application.Contract.Dto", returnType.GenericTypeParameters.First().Element, out returnDto))
        {
            var entityDetails = TrackedEntities.Values.First(x => x.Model.Id == returnType.GenericTypeParameters.First().Element.AsDTOModel().Mapping.ElementId);
            var autoMapperFieldName = InjectService(_template.UseType("AutoMapper.IMapper"));
            statements.Add($"return {entityDetails.VariableName}.MapToPagedResult(x => x.MapTo{returnDto}({autoMapperFieldName}));");
        }
        else if (returnType.Element.IsTypeDefinitionModel() && entitiesReturningPk.Count == 1)
        {
            var entityDetails = entitiesReturningPk.Single();
            var entity = entityDetails.Model;
            statements.Add($"return {entityDetails.VariableName}.{entity.GetTypesInHierarchy().SelectMany(x => x.Attributes).FirstOrDefault(x => x.IsPrimaryKey())?.Name.ToPascalCase() ?? "Id"};");
        }
        else
        {
            statements.Add(string.Empty);
            statements.Add("throw new NotImplementedException(\"Implement return type mapping...\");");
        }

        return statements;
    }

    public IEnumerable<CSharpStatement> CreateEntity(CreateEntityActionTargetEndModel createAction)
    {
        try
        {
            var entity = createAction.Element.AsClassModel() ?? createAction.Element.AsClassConstructorModel().ParentClass;
            var entityVariableName = createAction.Name;
            var dataAccess = InjectDataAccessProvider(entity);

            TrackedEntities.Add(createAction.Id, new EntityDetails(entity, createAction.Name, dataAccess, true));

            var mapping = createAction.Mappings.SingleOrDefault();
            var statements = new List<CSharpStatement>();

            if (MustAccessEntityThroughAggregate(dataAccess))
            {
                if (!TryGetFindAggregateStatements(mapping.SourceElement as IElement, entity, out statements))
                {
                    return new List<CSharpStatement>();
                }
            }

            if (mapping != null)
            {
                statements.Add(new CSharpAssignmentStatement($"var {entityVariableName}", _csharpMapping.GenerateCreationStatement(mapping)).WithSemicolon());
            }
            else
            {
                statements.Add(new CSharpAssignmentStatement($"var {entityVariableName}", $"new {entity.Name}();"));
            }

            _csharpMapping.SetFromReplacement(createAction.InternalAssociationEnd, entityVariableName);
            _csharpMapping.SetFromReplacement(entity, entityVariableName);
            _csharpMapping.SetToReplacement(createAction.InternalAssociationEnd, entityVariableName);
            _csharpMapping.SetToReplacement(entity, entityVariableName);
            return statements;
        }
        catch (Exception ex)
        {
            throw new ElementException(createAction.InternalAssociationEnd, "An error occurred while generating the domain interactions logic", ex);
        }
    }

    public IEnumerable<CSharpStatement> UpdateEntity(UpdateEntityActionTargetEndModel updateAction)
    {
        try
        {
            var entityDetails = TrackedEntities[updateAction.Id];
            var entity = entityDetails.Model;
            var updateMapping = updateAction.Mappings.GetUpdateEntityMapping();

            var statements = new List<CSharpStatement>();

            if (entityDetails.IsCollection)
            {
                _csharpMapping.SetToReplacement(entity, entityDetails.VariableName.Singularize());
                if (updateMapping != null)
                {
                    statements.Add(new CSharpForEachStatement(entityDetails.VariableName.Singularize(), entityDetails.VariableName)
                        .AddStatements(_csharpMapping.GenerateUpdateStatements(updateMapping)));
                }

                if (RepositoryRequiresExplicitUpdate(entity))
                {
                    statements.Add(entityDetails.DataAccessProvider.Update(entityDetails.VariableName.Singularize())
                        .SeparatedFromPrevious());
                }
            }
            else
            {
                if (updateMapping != null)
                {
                    statements.AddRange(_csharpMapping.GenerateUpdateStatements(updateMapping));
                }

                if (RepositoryRequiresExplicitUpdate(entity))
                {
                    statements.Add(entityDetails.DataAccessProvider.Update(entityDetails.VariableName)
                        .SeparatedFromPrevious());
                }
            }

            if (RequiresAggegateExplicitUpdate(entityDetails))
            {
                statements.Add(entityDetails.DataAccessProvider.Update(entityDetails.VariableName)
                    .SeparatedFromPrevious());
            }

            return statements;
        }
        catch (Exception ex)
        {
            throw new ElementException(updateAction.InternalAssociationEnd, "An error occurred while generating the domain interactions logic", ex);
        }
    }

    public IEnumerable<CSharpStatement> DeleteEntity(DeleteEntityActionTargetEndModel deleteAction)
    {
        try
        {
            var entityDetails = TrackedEntities[deleteAction.Id];
            var statements = new List<CSharpStatement>();
            if (entityDetails.IsCollection)
            {
                statements.Add(new CSharpForEachStatement(entityDetails.VariableName.Singularize(), entityDetails.VariableName)
                    .AddStatement(entityDetails.DataAccessProvider.Remove(entityDetails.VariableName.Singularize()))
                        .SeparatedFromPrevious());
            }
            else
            {
                statements.Add(entityDetails.DataAccessProvider.Remove(entityDetails.VariableName)
                    .SeparatedFromPrevious());
            }
            return statements;
        }
        catch (Exception ex)
        {
            throw new ElementException(deleteAction.InternalAssociationEnd, "An error occurred while generating the domain interactions logic", ex);
        }
    }

    public IEnumerable<CSharpStatement> CallServiceOperation(CallServiceOperationTargetEndModel callServiceOperation)
    {
        try
        {
            var statements = new List<CSharpStatement>();
            var operationModel = (IElement)callServiceOperation.Element;
            var serviceModel = operationModel.ParentElement;
            if ((!_template.TryGetTypeName(TemplateRoles.Domain.DomainServices.Interface, serviceModel, out var serviceInterface)
                && !_template.TryGetTypeName(TemplateRoles.Application.Services.Interface, serviceModel, out serviceInterface))
                || callServiceOperation.Mappings.Any() is false)
            {
                return Array.Empty<CSharpStatement>();
            }

            var serviceField = InjectService(serviceInterface);
            var isAsync = serviceModel.IsServiceModel() || operationModel.Name.EndsWith("Async", StringComparison.InvariantCultureIgnoreCase);
            var methodInvocation = _csharpMapping.GenerateCreationStatement(callServiceOperation.Mappings.First());
            if (isAsync && methodInvocation is CSharpInvocationStatement s)
            {
                s.AddArgument("cancellationToken");
            }
            var invoke = new CSharpAccessMemberStatement($"{(isAsync ? "await " : "")}{serviceField}", methodInvocation);
            if (operationModel.TypeReference.Element != null)
            {
                string variableName = callServiceOperation.Name.ToLocalVariableName();
                _csharpMapping.SetFromReplacement(callServiceOperation, variableName);
                _csharpMapping.SetToReplacement(callServiceOperation, variableName);
                statements.Add(new CSharpAssignmentStatement($"var {variableName}", invoke));

                if (operationModel.TypeReference.Element.AsClassModel() is { } entityModel)
                {
                    TrackedEntities.Add(callServiceOperation.Id, new EntityDetails(entityModel, variableName, null, false, operationModel.TypeReference.IsCollection));
                }
            }
            else
            {
                statements.Add(invoke);
            }

            return statements;
        }
        catch (Exception ex)
        {
            throw new ElementException(callServiceOperation.InternalAssociationEnd, "An error occurred while generating the domain interactions logic", ex);
        }
    }

    private IDataAccessProvider InjectDataAccessProvider(ClassModel foundEntity)
    {
        if (TryInjectRepositoryForEntity(foundEntity, out var dataAccess))
        {
            return dataAccess;
        }
        if (TryInjectDataAccessForComposite(foundEntity, out dataAccess))
        {
            return dataAccess;
        }
        if (TryInjectDbContext(foundEntity, out dataAccess))
        {
            return dataAccess;
        }
        throw new Exception("No CRUD Data Access Provider found. Please install a Module with a Repository Pattern or EF Core Module.");
    }

    private bool TryInjectDbContext(ClassModel entity, out IDataAccessProvider dataAccessProvider)
    {
        if (!_template.TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out var dbContextInterface) ||
            !SettingGenerateDbContextInterface())
        {
            dataAccessProvider = null;
            return false;
        }
        var dbContextField = InjectService(dbContextInterface, "dbContext");
        dataAccessProvider = new DbContextDataAccessProvider(dbContextField, entity, _template);
        return true;
    }


    private string InjectService(string interfaceName, string parameterName = null)
    {
        var fieldName = default(string);

        var ctor = _template.CSharpFile.Classes.First().Constructors.First();
        if (ctor.Parameters.All(x => x.Type != interfaceName))
        {
            ctor.AddParameter(interfaceName, (parameterName ?? interfaceName.RemovePrefix("I")).ToParameterName(),
                param => param.IntroduceReadonlyField(field => fieldName = field.Name));
        }
        else
        {
            fieldName = ctor.Parameters.First(x => x.Type == interfaceName).Name.ToPrivateMemberName();
        }
        return fieldName;
    }

    //private void InjectAutoMapper(out string fieldName)
    //{
    //    var temp = default(string);
    //    var ctor = _template.CSharpFile.Classes.First().Constructors.First();
    //    if (ctor.Parameters.All(x => x.Type != _template.UseType("AutoMapper.IMapper")))
    //    {
    //        ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper",
    //            param => param.IntroduceReadonlyField(field => temp = field.Name));
    //        fieldName = temp;
    //    }
    //    else
    //    {
    //        fieldName = ctor.Parameters.First(x => x.Type == _template.UseType("AutoMapper.IMapper")).Name.ToPrivateMemberName();
    //    }
    //}

    private bool RequiresAggegateExplicitUpdate(EntityDetails entityDetails)
    {
        if (entityDetails.DataAccessProvider is CompositeDataAccessProvider cda)
        {
            return cda.RequiresExplicitUpdate();
        }
        return false;
    }


    private bool MustAccessEntityThroughAggregate(IDataAccessProvider dataAccess)
    {
        return dataAccess is CompositeDataAccessProvider;
    }

    private bool RepositoryRequiresExplicitUpdate(IMetadataModel forEntity)
    {
        return _template.TryGetTemplate<ICSharpFileBuilderTemplate>(
                   TemplateRoles.Repository.Interface.Entity,
                   forEntity,
                   out var repositoryInterfaceTemplate) &&
               repositoryInterfaceTemplate.CSharpFile.Interfaces[0].TryGetMetadata<bool>("requires-explicit-update", out var requiresUpdate) &&
               requiresUpdate;
    }


    private List<PrimaryKeyFilterMapping> GetAggregatePKFindCriteria(IElementToElementMapping? queryMapping, IElement requestElement, ClassModel aggregateEntity, ClassModel compositeEntity)
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

    private bool TryGetFindAggregateStatements(IElementToElementMapping queryMapping, ClassModel foundEntity, out List<CSharpStatement> statements)
    {
        return TryGetFindAggregateStatements(queryMapping, (IElement)queryMapping.SourceElement, foundEntity, out statements);
    }

    private bool TryGetFindAggregateStatements(IElement requestElement, ClassModel foundEntity, out List<CSharpStatement> statements)
    {
        return TryGetFindAggregateStatements(null, requestElement, foundEntity, out statements);
    }

    private bool TryGetFindAggregateStatements(IElementToElementMapping queryMapping, IElement requestElement, ClassModel foundEntity, out List<CSharpStatement> statements)
    {
        statements = new List<CSharpStatement>();

        var aggregateAssociations = foundEntity.AssociatedClasses
            .Where(p => p.TypeReference?.Element?.AsClassModel()?.IsAggregateRoot() == true &&
                        p.IsSourceEnd() && !p.IsCollection && !p.IsNullable)
            .Distinct()
            .ToList();

        if (aggregateAssociations.Count != 1)
        {
            Logging.Log.Warning($"{foundEntity.Name} has multiple owning relationships.");
            return false;
        }
        var aggregateAssociation = aggregateAssociations.Single();
        var aggregateEntity = aggregateAssociation.Class;
        var aggregateVariableName = aggregateEntity.Name.ToLocalVariableName();
        var aggregateDataAccess = InjectDataAccessProvider(aggregateEntity);

        var idFields = GetAggregatePKFindCriteria(queryMapping, requestElement, aggregateEntity, foundEntity);
        if (!idFields.Any())
        {
            Logging.Log.Warning($"Unable to determine how to load Aggregate : {aggregateEntity.Name} for {requestElement.Name}.");
        }

        statements.Add(new CSharpAssignmentStatement($"var {aggregateVariableName}", aggregateDataAccess.FindByIdAsync(idFields)));

        statements.Add(CreateIfNullThrowNotFoundStatement(
            template: _template,
            variable: aggregateVariableName,
            message: $"Could not find {foundEntity.Name.ToPascalCase()} '{{{idFields.Select(x => x.Value).AsSingleOrTuple()}}}'"));
        return true;
    }

    private bool TryGetPaginationValues(IAssociationEnd associationEnd, CSharpClassMappingManager mappingManager, out string pageNo, out string pageSize)
    {
        var handler = (IElement)associationEnd.OtherEnd().TypeReference.Element;
        var returnsPagedResult = IsResultPaginated(handler.TypeReference);

        var pageIndexVar = handler.ChildElements.SingleOrDefault(IsPageIndexParam)?.Name;
        var accessVariable = mappingManager.GetFromReplacement(handler);
        pageNo = $"{(accessVariable != null ? $"{accessVariable}." : "")}{handler.ChildElements.SingleOrDefault(IsPageNumberParam)?.Name ?? $"{pageIndexVar} + 1"}";
        pageSize = $"{(accessVariable != null ? $"{accessVariable}." : "")}{handler.ChildElements.SingleOrDefault(IsPageSizeParam)?.Name}";

        return returnsPagedResult;
    }

    private bool IsResultPaginated(ITypeReference returnType)
    {
        return returnType.Element?.Name == "PagedResult";
    }

    private bool IsPageNumberParam(IElement param)
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

    private bool IsPageIndexParam(IElement param)
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

    private bool IsPageSizeParam(IElement param)
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
}



public record EntityDetails(ClassModel Model, string VariableName, IDataAccessProvider DataAccessProvider, bool IsNew, bool IsCollection = false);

internal static class AttributeModelExtensions
{
    public static bool IsPrimaryKey(this AttributeModel attribute)
    {
        return attribute.HasStereotype("Primary Key");
    }

    public static bool IsForeignKey(this AttributeModel attribute)
    {
        return attribute.HasStereotype("Foreign Key");
    }

    public static AssociationTargetEndModel GetForeignKeyAssociation(this AttributeModel attribute)
    {
        return attribute.GetStereotype("Foreign Key")?.GetProperty<IElement>("Association")?.AsAssociationTargetEndModel();
    }

    public static string AsSingleOrTuple(this IEnumerable<CSharpStatement> idFields)
    {
        if (idFields.Count() <= 1)
            return $"{idFields.Single()}";
        return $"({string.Join(", ", idFields.Select(idField => $"{idField}"))})";
    }
}

internal record AggregateKeyMapping(AttributeModel Key, string Match);