using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
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
using JetBrains.Annotations;
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
    
    private const string DomainServiceSpecializationId = "07f936ea-3756-48c8-babd-24ac7271daac";
    private const string ApplicationServiceSpecializationId = "b16578a5-27b1-4047-a8df-f0b783d706bd";
    private const string EntitySpecializationId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10";
    private const string RepositorySpecializationId = "96ffceb2-a70a-4b69-869b-0df436c470c3";
    private const string ServiceProxySpecializationId = "07d8d1a9-6b9f-4676-b7d3-8db06299e35c";

    public IEnumerable<CSharpStatement> CreateInteractionStatements(CSharpClass handlerClass, IProcessingHandlerModel model)
    {
        var domainInteractionManager = this;
        var statements = new List<CSharpStatement>();
        try
        {
            foreach (var queryAction in model.QueryEntityActions())
            {
                if (queryAction.Mappings.GetQueryEntityMapping() == null)
                {
                    continue;
                }

                if (!queryAction.Element.IsClassModel())
                {
                    continue;
                }
                
                var foundEntity = queryAction.Element.AsClassModel();
                statements.AddRange(domainInteractionManager.QueryEntity(handlerClass, foundEntity, queryAction.InternalAssociationEnd));
            }

            foreach (var createAction in model.CreateEntityActions())
            {
                statements.AddRange(domainInteractionManager.CreateEntity(handlerClass, createAction));
            }

            foreach (var updateAction in model.UpdateEntityActions())
            {
                var entity = updateAction.Element.AsClassModel() ?? OperationModelExtensions.AsOperationModel(updateAction.Element).ParentClass;

                statements.AddRange(domainInteractionManager.QueryEntity(handlerClass, entity, updateAction.InternalAssociationEnd));

                statements.Add(string.Empty);
                statements.AddRange(domainInteractionManager.UpdateEntity(handlerClass, updateAction));
            }

            foreach (var callAction in model.CallServiceOperationActions())
            {
                statements.AddRange(domainInteractionManager.CallServiceOperation(handlerClass, callAction));
            }

            foreach (var deleteAction in model.DeleteEntityActions())
            {
                var foundEntity = deleteAction.Element.AsClassModel();
                statements.AddRange(domainInteractionManager.QueryEntity(handlerClass, foundEntity, deleteAction.InternalAssociationEnd));
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
    
    public List<CSharpStatement> QueryEntity(CSharpClass handlerClass, ClassModel foundEntity, IAssociationEnd associationEnd)
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

            var dataAccess = InjectDataAccessProvider(handlerClass, foundEntity);
            CSharpStatement queryInvocation = null;
            var prerequisiteStatement = new List<CSharpStatement>();

            if (MustAccessEntityThroughAggregate(dataAccess))
            {
                if (!TryGetFindAggregateStatements(handlerClass, queryMapping, foundEntity, out var findAggStatements))
                {
                    return new List<CSharpStatement>();
                }

                prerequisiteStatement.AddRange(findAggStatements);

                if (associationEnd.TypeReference.IsCollection)
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
                            _csharpMapping.GenerateSourceStatementForMapping(queryMapping, x),
                            x.TargetElement.AsAttributeModel().Name.ToPropertyName(),
                            x))
                        .ToList();

                    if (associationEnd.TypeReference.IsCollection && idFields.All(x => x.Mapping.SourceElement.TypeReference.IsCollection))
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

                    if (TryGetPaginationValues(associationEnd, _csharpMapping, out var pageNo, out var pageSize, out var orderBy, out var orderByIsNUllable))
                    {
                        queryInvocation = dataAccess.FindAllAsync(queryMapping, pageNo, pageSize, orderBy, orderByIsNUllable, out var requiredStatements);
                        prerequisiteStatement.AddRange(requiredStatements);
                    }
                    else if (associationEnd.TypeReference.IsCollection)
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

            if (!associationEnd.TypeReference.IsNullable && !associationEnd.TypeReference.IsCollection && !IsResultPaginated(associationEnd.OtherEnd().TypeReference.Element.TypeReference))
            {
                var queryFields = queryMapping.MappedEnds
                    .Select(x => new CSharpStatement($"{{{_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}}}"))
                    .ToList();
                if (queryFields.Count == 0)
                {
                    throw new ElementException(associationEnd, "Query for single entity has no mappings specified. Either indicate mappings or set Is Collection to true if trying to fetch all entities as a collection.");
                }

                statements.Add(CreateIfNullThrowNotFoundStatement(
                    template: _template,
                    variable: entityVariableName,
                    message: $"Could not find {foundEntity.Name.ToPascalCase()} '{queryFields.AsSingleOrTuple()}'"));

            }

            TrackedEntities.Add(associationEnd.Id, new EntityDetails(foundEntity.InternalElement, entityVariableName, dataAccess, false, associationEnd.TypeReference.IsCollection));

            return statements;
        }
        catch (ElementException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ElementException(associationEnd, "An error occurred while generating the domain interactions logic", ex);
        }
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

    public bool TryInjectDataAccessForComposite(CSharpClass handlerClass, ClassModel foundEntity, out IDataAccessProvider dataAccessProvider)
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
                    bool requiresExplicitUpdate = RepositoryRequiresExplicitUpdate(aggregateEntity);
                    var repositoryName = InjectService(repositoryInterface, handlerClass);
                    dataAccessProvider = new CompositeDataAccessProvider(
                        saveChangesAccessor: $"{repositoryName}.UnitOfWork",
                        accessor: $"{aggregateEntity.Name.ToLocalVariableName()}.{aggregateAssociations.Single().OtherEnd().Name}",
                        explicitUpdateStatement: requiresExplicitUpdate ? $"{repositoryName}.Update({aggregateEntity.Name.ToLocalVariableName()});" : null,
                        template: _template,
                        mappingManager: _csharpMapping
                        );

                    return true;
                }
                else if (_template.TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out var dbContextInterface) &&
                    SettingGenerateDbContextInterface())
                {
                    var dbContextField = InjectService(dbContextInterface, handlerClass, "dbContext");
                    dataAccessProvider = new CompositeDataAccessProvider(
                        saveChangesAccessor: dbContextField,
                        accessor: $"{aggregateEntity.Name.ToLocalVariableName()}.{aggregateAssociations.Single().OtherEnd().Name}",
                        explicitUpdateStatement: null, 
                        template: _template,
                        mappingManager: _csharpMapping);
                    return true;
                }
            }
        }
        dataAccessProvider = null;
        return false;
    }
    
    public bool TryInjectRepositoryForEntity(CSharpClass handlerClass, ClassModel foundEntity, out IDataAccessProvider dataAccessProvider)
    {

        if (!_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, foundEntity, out var repositoryInterface))
        {
            dataAccessProvider = null;
            return false;
        }

        //This is being done for Dapper
        bool hasUnitOfWork = _template.TryGetTemplate<ITemplate>(TemplateRoles.Domain.UnitOfWork, out _);

		dataAccessProvider = new RepositoryDataAccessProvider(InjectService(repositoryInterface, handlerClass), _template, _csharpMapping, hasUnitOfWork, foundEntity);
        return true;
    }

    // This is likely to cause bugs since it doesn't align exactly with the logic that "enabled/disables" the IApplicationDbContext template
    public bool SettingGenerateDbContextInterface()
    {
        return true;
        //GetDatabaseSettings().GenerateDbContextInterface()
        return bool.TryParse(_template.ExecutionContext.Settings.GetGroup("ac0a788e-d8b3-4eea-b56d-538608f1ded9").GetSetting("85dea0e8-8981-4c7b-908e-d99294fc37f1")?.Value.ToPascalCase(), out var result) && result;
    }

    public IEnumerable<CSharpStatement> GetReturnStatements(CSharpClass handlerClass, ITypeReference returnType)
	{
		if (returnType.Element == null)
		{
			throw new Exception("No return type specified");
		}
		var statements = new List<CSharpStatement>();
		
        var entitiesReturningPk = GetEntitiesReturningPK(returnType);

		foreach (var entity in entitiesReturningPk.Where(x => x.IsNew).GroupBy(x => x.ElementModel.Id).Select(x => x.First()))
		{
			statements.Add($"{entity.DataAccessProvider.SaveChangesAsync()}");
		}

		if (TrackedEntities.Any() && returnType.Element.AsDTOModel()?.IsMapped == true && _template.TryGetTypeName("Application.Contract.Dto", returnType.Element, out var returnDto))
		{
			var mappedElementId = returnType.Element.AsDTOModel().Mapping.ElementId;
			var entityDetails = TrackedEntities.Values.First(x => x.ElementModel?.Id == mappedElementId);
			var autoMapperFieldName = InjectService(_template.UseType("AutoMapper.IMapper"), handlerClass);
			string nullable = returnType.IsNullable ? "?" : "";
			statements.Add($"return {entityDetails.VariableName}{nullable}.MapTo{returnDto}{(returnType.IsCollection ? "List" : "")}({autoMapperFieldName});");
		}
		else if (TrackedEntities.Any() && IsResultPaginated(returnType) && returnType.GenericTypeParameters.FirstOrDefault()?.Element.AsDTOModel()?.IsMapped == true && _template.TryGetTypeName("Application.Contract.Dto", returnType.GenericTypeParameters.First().Element, out returnDto))
		{
			var entityDetails = TrackedEntities.Values.First(x => x.ElementModel.Id == returnType.GenericTypeParameters.First().Element.AsDTOModel().Mapping.ElementId);
			var autoMapperFieldName = InjectService(_template.UseType("AutoMapper.IMapper"), handlerClass);
			statements.Add($"return {entityDetails.VariableName}.MapToPagedResult(x => x.MapTo{returnDto}({autoMapperFieldName}));");
		}
		else if (returnType.Element.IsTypeDefinitionModel() && entitiesReturningPk.Count == 1) // No need for TrackedEntities thus no check for it
		{
			var entityDetails = entitiesReturningPk.Single();
			var entity = entityDetails.ElementModel.AsClassModel();
			statements.Add($"return {entityDetails.VariableName}.{entity.GetTypesInHierarchy().SelectMany(x => x.Attributes).FirstOrDefault(x => x.IsPrimaryKey())?.Name.ToPascalCase() ?? "Id"};");
		}
		else if (TrackedEntities.Any() && TrackedEntities.Values.Any(x => returnType.Element.Id == x.ElementModel.Id))
		{
			var entityDetails = TrackedEntities.Values.First(x => returnType.Element.Id == x.ElementModel.Id);
			statements.Add($"return {entityDetails.VariableName};");
		}
		else
		{
            statements.Add(new CSharpStatement($"// TODO: Implement return type mapping...").SeparatedFromPrevious());
            statements.Add("throw new NotImplementedException(\"Implement return type mapping...\");");
		}

		return statements;
	}

	private List<EntityDetails> GetEntitiesReturningPK(ITypeReference returnType)
	{
        if (returnType.Element.IsDTOModel())
        {
            var dto = returnType.Element.AsDTOModel();

            var mappedPks = dto.Fields
                .Where(x => x.Mapping != null && x.Mapping.Element.IsAttributeModel() && x.Mapping.Element.AsAttributeModel().IsPrimaryKey())
                .Select(x => x.Mapping.Element.AsAttributeModel().InternalElement.ParentElement.Id)
                .Distinct() 
                .ToList();
            if (mappedPks.Any())
            {
                return TrackedEntities.Values
                .Where(x => x.ElementModel.IsClassModel() && mappedPks.Contains(x.ElementModel.Id))
                .ToList();
            }
            return new List<EntityDetails>();
		}
		return TrackedEntities.Values
			.Where(x => x.ElementModel.AsClassModel()?.GetTypesInHierarchy()
				.SelectMany(c => c.Attributes)
				.Count(a => a.IsPrimaryKey() && a.TypeReference.Element.Id == returnType.Element.Id) == 1)
			.ToList();
	}

	public IEnumerable<CSharpStatement> CreateEntity(CSharpClass handlerClass, CreateEntityActionTargetEndModel createAction)
    {
        try
        {
			var entity = createAction.Element.AsClassModel() ?? createAction.Element.AsClassConstructorModel().ParentClass;

            var entityVariableName = createAction.Name;
            var dataAccess = InjectDataAccessProvider(handlerClass, entity);

            TrackedEntities.Add(createAction.Id, new EntityDetails(entity.InternalElement, createAction.Name, dataAccess, true));

            var mapping = createAction.Mappings.SingleOrDefault();
            var statements = new List<CSharpStatement>();

            if (MustAccessEntityThroughAggregate(dataAccess))
            {
                if (!TryGetFindAggregateStatements(handlerClass, mapping.SourceElement as IElement, entity, out statements))
                {
                    return new List<CSharpStatement>();
                }
            }

            if (mapping != null)
			{
				var constructionStatement = _csharpMapping.GenerateCreationStatement(mapping);

				WireupDomainServicesForConstructors(handlerClass, createAction, constructionStatement);

				statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(entityVariableName), constructionStatement).WithSemicolon());
			}
			else
            {
                statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(entityVariableName), $"new {entity.Name}();"));
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

	public IEnumerable<CSharpStatement> UpdateEntity(CSharpClass handlerClass, UpdateEntityActionTargetEndModel updateAction)
    {
        try
        {
            var entityDetails = TrackedEntities[updateAction.Id];
            var entity = entityDetails.ElementModel.AsClassModel();
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
					var updateStatements = _csharpMapping.GenerateUpdateStatements(updateMapping);
					WireupDomainServicesForOperations(handlerClass, updateAction, updateStatements);
					AdjustOperationInvocationForAsyncAndReturn(updateMapping, updateStatements);

					statements.AddRange(updateStatements);
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

	private void AdjustOperationInvocationForAsyncAndReturn(IElementToElementMapping updateMapping, IList<CSharpStatement> updateStatements)
	{
        
		if (updateMapping.MappedEnds.Any(me => OperationModelExtensions.IsOperationModel(me.TargetElement)))             
		{
            foreach (var invocation in updateMapping.MappedEnds.Where(me => OperationModelExtensions.IsOperationModel(me.TargetElement)))
            {

				var operationName = ((IElement)invocation.TargetElement).Name;
				var variableName = $"{operationName.ToCamelCase()}Result";
                bool hasReturn = invocation.TargetElement.TypeReference?.Element != null;

                for (int i = 0; i < updateStatements.Count; i++)
                {
					if (updateStatements[i] is CSharpInvocationStatement s && s.Expression.Reference is ICSharpMethodDeclaration md && md.Name == operationName)
					{
						if (s.IsAsyncInvocation())
						{
							s.AddArgument("cancellationToken");
							updateStatements[i] = new CSharpAwaitExpression(updateStatements[i]);
						}
                        if (hasReturn)
                        {
                            updateStatements[i] = new CSharpAssignmentStatement(new CSharpVariableDeclaration(variableName), updateStatements[i]);
                        }
					}
				}

				TrackedEntities.Add(invocation.TargetElement.Id, new EntityDetails((IElement)invocation.TargetElement.TypeReference.Element, variableName, null, false, invocation.TargetElement.TypeReference.IsCollection));
            }
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

    public IEnumerable<CSharpStatement> CallServiceOperation(CSharpClass handlerClass, CallServiceOperationTargetEndModel callServiceOperation)
    {
        try
        {
            var statements = new List<CSharpStatement>();
            if (!HasServiceDependency(callServiceOperation, out var dependencyInfo) || callServiceOperation.Mappings.Any() is false)
            {
                return Array.Empty<CSharpStatement>();
            }

            // So that the mapping system can resolve the name of the operation from the interface itself:
            _template.AddTypeSource(dependencyInfo.ServiceInterfaceTemplate.Id);

            string serviceField;
            if (dependencyInfo.Injectable)
            {
                serviceField = InjectService(_template.GetTypeName(dependencyInfo.ServiceInterfaceTemplate), handlerClass);
            }
            else
            {
                serviceField = this.TrackedEntities.Last().Value.VariableName;
            }

            var methodInvocation = _csharpMapping.GenerateCreationStatement(callServiceOperation.Mappings.First());
            CSharpStatement invoke = new CSharpAccessMemberStatement(serviceField, methodInvocation);

            var invStatement = methodInvocation as CSharpInvocationStatement;
			if (invStatement?.IsAsyncInvocation() == true)
			{
                invStatement.AddArgument("cancellationToken");
                invoke = new CSharpAwaitExpression(invoke);
			}
            
			var operationModel = (IElement)callServiceOperation.Element;
			if (operationModel.TypeReference.Element != null)
            {
                var variableName = callServiceOperation.Name.ToLocalVariableName();
                _csharpMapping.SetFromReplacement(callServiceOperation, variableName);
                _csharpMapping.SetToReplacement(callServiceOperation, variableName);
                
                statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(variableName), invoke));


				TrackedEntities.Add(callServiceOperation.Id, new EntityDetails((IElement)operationModel.TypeReference.Element, variableName, null, false, operationModel.TypeReference.IsCollection));
            }
            else if (invStatement?.Expression.Reference is ICSharpMethodDeclaration methodDeclaration &&
                     (methodDeclaration.ReturnTypeInfo.GetTaskGenericType() is CSharpTypeTuple || methodDeclaration.ReturnTypeInfo is CSharpTypeTuple))
            {
                var tuple = (CSharpTypeTuple)methodDeclaration.ReturnTypeInfo.GetTaskGenericType() ?? (CSharpTypeTuple)methodDeclaration.ReturnTypeInfo;
                var declaration = new CSharpDeclarationExpression(tuple.Elements.Select(s => s.Name.ToCamelCase()).ToList());
                statements.Add(new CSharpAssignmentStatement(declaration, invoke));
            }
            else
            {
                statements.Add(invoke);
            }
            
            WireupDomainServicesForOperations(handlerClass, callServiceOperation, statements);

            return statements;
        }
        catch (Exception ex)
        {
            throw new ElementException(callServiceOperation.InternalAssociationEnd, "An error occurred while generating the domain interactions logic", ex);
        }

        bool HasServiceDependency(CallServiceOperationTargetEndModel callServiceOperation, out (ICSharpFileBuilderTemplate ServiceInterfaceTemplate, bool Injectable) dependencyInfo)
        {
            var serviceModel = ((IElement)callServiceOperation.Element).ParentElement;
            switch (serviceModel.SpecializationTypeId)
            {
                case DomainServiceSpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.DomainServices.Interface, serviceModel, out var domainServiceTemplate):
                    dependencyInfo = (domainServiceTemplate, true);
                    return true;
                case ApplicationServiceSpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Services.Interface, serviceModel, out var applicationServiceTemplate):
                    dependencyInfo = (applicationServiceTemplate, true);
                    return true;
                case RepositorySpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Repository.Interface.Entity, serviceModel, out var repositoryTemplate):
                    dependencyInfo = (repositoryTemplate, true);
                    return true;
                case EntitySpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, serviceModel, out var entityTemplate):
                    dependencyInfo = (entityTemplate, false);
                    return true;
                case ServiceProxySpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Services.ClientInterface, serviceModel, out var clientInterfaceTemplate):
                    dependencyInfo = (clientInterfaceTemplate, true);
                    return true;
                default:
                    dependencyInfo = default;
                    return false;
            }
        }
    }

    private IDataAccessProvider InjectDataAccessProvider(CSharpClass handlerClass, ClassModel foundEntity)
    {
        if (TryInjectRepositoryForEntity(handlerClass, foundEntity, out var dataAccess))
        {
            return dataAccess;
        }
        if (TryInjectDataAccessForComposite(handlerClass, foundEntity, out dataAccess))
        {
            return dataAccess;
        }
        if (TryInjectDbContext(handlerClass, foundEntity, out dataAccess))
        {
            return dataAccess;
        }
        throw new Exception("No CRUD Data Access Provider found. Please install a Module with a Repository Pattern or EF Core Module.");
    }

    private bool TryInjectDbContext(CSharpClass handlerClass, ClassModel entity, out IDataAccessProvider dataAccessProvider)
    {
        if (!_template.TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out var dbContextInterface) ||
            !SettingGenerateDbContextInterface())
        {
            dataAccessProvider = null;
            return false;
        }
        var dbContextField = InjectService(dbContextInterface, handlerClass, "dbContext");
        dataAccessProvider = new DbContextDataAccessProvider(dbContextField, entity, _template, _csharpMapping);
        return true;
    }


    private string InjectService(string interfaceName, CSharpClass handlerClass, string parameterName = null)
    {
        var fieldName = default(string);

        var ctor = handlerClass.Constructors.First();
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
    
	private void WireupDomainServicesForConstructors(CSharpClass handlerClass, CreateEntityActionTargetEndModel createAction, CSharpStatement constructionStatement)
	{
		var constructor = createAction.Element.AsClassConstructorModel();
		if (constructor != null)
		{
			if (constructor.Parameters.Any(p => p.TypeReference.Element.SpecializationTypeId == DomainServiceSpecializationId)) // Domain Service
			{
				var invocation = constructionStatement as CSharpInvocationStatement;
				for (var i = 0; i < constructor.Parameters.Count; i++)
				{
					var arg = constructor.Parameters[i];
					if (arg.TypeReference.Element.SpecializationTypeId != DomainServiceSpecializationId)
					{
						continue;
					}
					if (_template.TryGetTypeName(TemplateRoles.Domain.DomainServices.Interface, arg.TypeReference.Element.Id, out var domainServiceInterface))
					{
						var fieldname = InjectService(domainServiceInterface, handlerClass, domainServiceInterface.Substring(1).ToParameterName());
						//Change `default` or `parameterName: default` into `_domainService` (fieldName)
						invocation.Statements[i].Replace(invocation.Statements[i].GetText("").Replace("default", fieldname));
					}
				}
			}
		}
	}

	private void WireupDomainServicesForOperations(CSharpClass handlerClass, UpdateEntityActionTargetEndModel updateAction, IList<CSharpStatement> updateStatements)
    {
        Func<CSharpInvocationStatement, Intent.Modelers.Domain.Api.OperationModel> getOperation;
        if (OperationModelExtensions.IsOperationModel(updateAction.Element))
        {
            var operation = OperationModelExtensions.AsOperationModel(updateAction.Element);
            if (operation == null)
            {
                return;
            }
            if (operation.Parameters.All(p => p.TypeReference.Element.SpecializationTypeId != DomainServiceSpecializationId))
            {
                return;
            }
            getOperation = (x) =>  operation;
        }
        else
        {
            var updateMappings = updateAction.Mappings.GetUpdateEntityMapping();
            var mappedOperations = updateMappings.MappedEnds.Where(me => OperationModelExtensions.IsOperationModel(me.TargetElement)).Select(me => OperationModelExtensions.AsOperationModel(me.TargetElement)).ToList();

            if (!mappedOperations.Any())
            {
                return;
            }
            if (!mappedOperations.Any(o => o.Parameters.Any(p => p.TypeReference.Element.SpecializationTypeId == DomainServiceSpecializationId)))
            {
                return;
            }

            getOperation = (invocation) => 
            {
                string operationName = invocation.Expression.Reference is ICSharpMethodDeclaration iCSharpMethodDeclaration ? iCSharpMethodDeclaration.Name.ToCSharpIdentifier() : null;
                return mappedOperations.FirstOrDefault(operation => operation.Name == operationName);
            };
        }

        foreach (var updateStatement in updateStatements)
        {
            if (updateStatement is not CSharpInvocationStatement invocation)
            {
                continue;
            }
            var operation = getOperation(invocation);

            if (operation == null)
            {
                continue;
            }    
            for (var i = 0; i < operation.Parameters.Count; i++)
            {
                var arg = operation.Parameters[i];
                if (arg.TypeReference.Element.SpecializationTypeId != DomainServiceSpecializationId)
                {
                    continue;
                }

                if (!_template.TryGetTypeName(TemplateRoles.Domain.DomainServices.Interface, arg.TypeReference.Element.Id, out var domainServiceInterface))
                {
                    continue;
                }
                var fieldName = InjectService(domainServiceInterface, handlerClass, domainServiceInterface.Substring(1).ToParameterName());
                //Change `default` or `parameterName: default` into `_domainService` (fieldName)
                invocation.Statements[i].Replace(invocation.Statements[i].GetText("").Replace("default", fieldName));
            }
        }
    }
    
    private void WireupDomainServicesForOperations(CSharpClass handlerClass, CallServiceOperationTargetEndModel callServiceOperation, List<CSharpStatement> statements)
    {
        var operation = OperationModelExtensions.AsOperationModel(callServiceOperation.Element);
        if (operation == null)
        {
            return;
        }
        if (operation.Parameters.All(p => p.TypeReference.Element.SpecializationTypeId != DomainServiceSpecializationId))
        {
            return; 
        }
        
        foreach (var statement in statements)
        {
            SubstituteServiceParameters(statement);
        }

        return;
        
        void SubstituteServiceParameters(CSharpStatement statement)
        {
            switch (statement)
            {
                case CSharpAssignmentStatement assign:
                    SubstituteServiceParameters(assign.Rhs);
                    return;
                case CSharpAccessMemberStatement access:
                {
                    SubstituteServiceParameters(access.Member);
                    return;
                }
            }

            var invocation = statement as CSharpInvocationStatement;
            if (invocation is null)
            {
                return;
            }
            
            for (var i = 0; i < operation.Parameters.Count; i++)
            {
                var arg = operation.Parameters[i];
                if (arg.TypeReference.Element.SpecializationTypeId != DomainServiceSpecializationId)
                {
                    continue;
                }

                if (!_template.TryGetTypeName(TemplateRoles.Domain.DomainServices.Interface, arg.TypeReference.Element.Id, out var domainServiceInterface))
                {
                    continue;
                }
                var fieldName = InjectService(domainServiceInterface, handlerClass, domainServiceInterface.Substring(1).ToParameterName());
                //Change `default` or `parameterName: default` into `_domainService` (fieldName)
                invocation.Statements[i].Replace(invocation.Statements[i].GetText("").Replace("default", fieldName));
            }
        }
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

    private bool TryGetFindAggregateStatements(CSharpClass handlerClass, IElementToElementMapping queryMapping, ClassModel foundEntity, out List<CSharpStatement> statements)
    {
        return TryGetFindAggregateStatements(handlerClass, queryMapping, (IElement)queryMapping.SourceElement, foundEntity, out statements);
    }

    private bool TryGetFindAggregateStatements(CSharpClass handlerClass, IElement requestElement, ClassModel foundEntity, out List<CSharpStatement> statements)
    {
        return TryGetFindAggregateStatements(handlerClass, null, requestElement, foundEntity, out statements);
    }

    private bool TryGetFindAggregateStatements(CSharpClass handlerClass, IElementToElementMapping queryMapping, IElement requestElement, ClassModel foundEntity, out List<CSharpStatement> statements)
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
        var aggregateDataAccess = InjectDataAccessProvider(handlerClass, aggregateEntity);

        var idFields = GetAggregatePKFindCriteria(queryMapping, requestElement, aggregateEntity, foundEntity);
        if (!idFields.Any())
        {
            Logging.Log.Warning($"Unable to determine how to load Aggregate : {aggregateEntity.Name} for {requestElement.Name}.");
        }

        statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(aggregateVariableName), aggregateDataAccess.FindByIdAsync(idFields)));

        statements.Add(CreateIfNullThrowNotFoundStatement(
            template: _template,
            variable: aggregateVariableName,
            message: $"Could not find {foundEntity.Name.ToPascalCase()} '{{{idFields.Select(x => x.ValueExpression).AsSingleOrTuple()}}}'"));
        return true;
    }

    private bool TryGetPaginationValues(IAssociationEnd associationEnd, CSharpClassMappingManager mappingManager, out string pageNo, out string pageSize, out string? orderBy, out bool orderByIsNullable)
    {
        orderByIsNullable = false;
        var handler = (IElement)associationEnd.OtherEnd().TypeReference.Element;
        var returnsPagedResult = IsResultPaginated(handler.TypeReference);

        var pageIndexVar = handler.ChildElements.SingleOrDefault(IsPageIndexParam)?.Name;
        var accessVariable = mappingManager.GetFromReplacement(handler);
        pageNo = $"{(accessVariable != null ? $"{accessVariable}." : "")}{handler.ChildElements.SingleOrDefault(IsPageNumberParam)?.Name ?? $"{pageIndexVar} + 1"}";
        pageSize = $"{(accessVariable != null ? $"{accessVariable}." : "")}{handler.ChildElements.SingleOrDefault(IsPageSizeParam)?.Name}";

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

    private bool IsOrderByParam(IElement param)
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
}

public record EntityDetails(IElement ElementModel, string VariableName, IDataAccessProvider DataAccessProvider, bool IsNew, bool IsCollection = false);

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