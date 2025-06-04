using System;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
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
using AttributeModel = Intent.Modelers.Domain.Api.AttributeModel;

namespace Intent.Modules.Application.DomainInteractions;
// Disambiguation
using Intent.Modelers.Domain.Api;
using System.ComponentModel.Design;
using System.Drawing;

public static class DomainInteractionExtensions
{

    public static bool HasDomainInteractions(this IProcessingHandlerModel model)
    {
        return model.CreateEntityActions().Any()
               || model.QueryEntityActions().Any()
               || model.UpdateEntityActions().Any()
               || model.DeleteEntityActions().Any()
               || model.ServiceInvocationActions().Any()
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

    //public Dictionary<string, EntityDetails> TrackedEntities { get; set; } = new();

    private const string DomainServiceSpecializationId = "07f936ea-3756-48c8-babd-24ac7271daac";
    private const string ApplicationServiceSpecializationId = "b16578a5-27b1-4047-a8df-f0b783d706bd";
    private const string EntitySpecializationId = "04e12b51-ed12-42a3-9667-a6aa81bb6d10";
    private const string RepositorySpecializationId = "96ffceb2-a70a-4b69-869b-0df436c470c3";
    private const string ServiceProxySpecializationId = "07d8d1a9-6b9f-4676-b7d3-8db06299e35c";

    public IEnumerable<CSharpStatement> CreateInteractionStatements(CSharpClassMethod method, IProcessingHandlerModel model)
    {
        var handlerClass = method.Class;
        var domainInteractionManager = this;
        var statements = new List<CSharpStatement>();
        try
        {
            //foreach (var queryAction in model.QueryEntityActions())
            //{
            //    if (queryAction.Mappings.GetQueryEntityMapping() == null)
            //    {
            //        continue;
            //    }

            //    if (!queryAction.Element.IsClassModel())
            //    {
            //        continue;
            //    }

            //    var foundEntity = queryAction.Element.AsClassModel();
            //    statements.AddRange(domainInteractionManager.QueryEntity(new QueryActionContext(method, ActionType.Query, queryAction.InternalAssociationEnd)));
            //}

            foreach (var createAction in model.CreateEntityActions())
            {
                statements.AddRange(domainInteractionManager.CreateEntity(method, createAction));
            }

            foreach (var updateAction in model.UpdateEntityActions())
            {
                statements.AddRange(domainInteractionManager.QueryEntity(new QueryActionContext(method, ActionType.Update, updateAction.InternalAssociationEnd)));

                statements.Add(string.Empty);
                statements.AddRange(domainInteractionManager.UpdateEntity(method, updateAction));
            }

            //foreach (var callAction in model.ServiceInvocationActions())
            //{
            //    statements.AddRange(domainInteractionManager.InvokeService(handlerClass, callAction.InternalAssociationEnd));
            //}


            //foreach (var callAction in model.CallServiceOperationActions())
            //{
            //    statements.AddRange(domainInteractionManager.CallServiceOperation(handlerClass, callAction.InternalAssociationEnd));
            //}

            foreach (var deleteAction in model.DeleteEntityActions())
            {
                statements.AddRange(domainInteractionManager.QueryEntity(new QueryActionContext(method, ActionType.Delete, deleteAction.InternalAssociationEnd)));
                statements.AddRange(domainInteractionManager.DeleteEntity(method, deleteAction));
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
                    WireupDomainServicesForProcessingAction(handlerClass, actions.InternalElement.Mappings.Single(), processingStatements);
                    processingStatements.FirstOrDefault()?.SeparatedFromPrevious();
                    statements.AddRange(processingStatements);

                }
                catch (Exception ex)
                {
                    throw new ElementException(actions.InternalElement, "An error occurred while generating processing action logic", ex);
                }
            }

            foreach (var entity in method.TrackedEntities().Values.Where(x => x.IsNew))
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

    public List<CSharpStatement> QueryEntity(QueryActionContext queryContext)
    {
        try
        {
            var associationEnd = queryContext.AssociationEnd;
            var foundEntity = queryContext.FoundEntity;
            var queryMapping = queryContext.AssociationEnd.Mappings.GetQueryEntityMapping();
            if (queryMapping == null)
            {
                throw new ElementException(queryContext.AssociationEnd, "Query Entity Mapping has not been specified.");
            }

            var entityVariableName = associationEnd.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);

            _csharpMapping.SetFromReplacement(foundEntity, entityVariableName);
            _csharpMapping.SetFromReplacement(associationEnd, entityVariableName);
            _csharpMapping.SetToReplacement(foundEntity, entityVariableName);
            _csharpMapping.SetToReplacement(associationEnd, entityVariableName);

            var dataAccess = InjectDataAccessProvider(queryContext.Method, foundEntity, queryContext);
            CSharpStatement queryInvocation = null;
            var prerequisiteStatement = new List<CSharpStatement>();

            if (MustAccessEntityThroughAggregate(dataAccess))
            {
                if (!TryGetFindAggregateStatements(queryContext.Method, queryMapping, foundEntity, out var findAggStatements))
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
                if (queryMapping.MappedEnds.Any() && queryMapping.MappedEnds.All(x => Intent.Modelers.Domain.Api.AttributeModelExtensions.AsAttributeModel(x.TargetElement)?.IsPrimaryKey() == true)
                                                  && foundEntity.GetTypesInHierarchy().SelectMany(c => c.Attributes).Count(x => x.IsPrimaryKey()) == queryMapping.MappedEnds.Count)
                {
                    var idFields = queryMapping.MappedEnds
                        .OrderBy(x => ((IElement)x.TargetElement).Order)
                        .Select(x => new PrimaryKeyFilterMapping(
                            _csharpMapping.GenerateSourceStatementForMapping(queryMapping, x),
                            Intent.Modelers.Domain.Api.AttributeModelExtensions.AsAttributeModel(x.TargetElement).Name.ToPropertyName(),
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
                    message: $"Could not find {foundEntity.Name} '{queryFields.AsSingleOrTuple()}'"));

            }

            queryContext.Method.TrackedEntities().Add(associationEnd.Id, new EntityDetails(foundEntity.InternalElement, entityVariableName, dataAccess, false, queryContext.ImplementWithProjections() && dataAccess.IsUsingProjections ? queryContext.GetDtoProjectionReturnType() : null, associationEnd.TypeReference.IsCollection));

            return statements;
        }
        catch (ElementException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ElementException(queryContext.AssociationEnd, "An error occurred while generating the domain interactions logic", ex);
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

    public bool TryInjectRepositoryForEntity(CSharpClass handlerClass, ClassModel foundEntity, QueryActionContext context, out IDataAccessProvider dataAccessProvider)
    {

        if (!_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, foundEntity, out var repositoryInterface))
        {
            dataAccessProvider = null;
            return false;
        }

        //This is being done for Dapper
        bool hasUnitOfWork = _template.TryGetTemplate<ITemplate>(TemplateRoles.Domain.UnitOfWork, out _);


        dataAccessProvider = new RepositoryDataAccessProvider(handlerClass.InjectService(repositoryInterface), _template, _csharpMapping, hasUnitOfWork, context, foundEntity);
        return true;
    }

    // This is likely to cause bugs since it doesn't align exactly with the logic that "enabled/disables" the IApplicationDbContext template
    public bool SettingGenerateDbContextInterface()
    {
        return true;
        //GetDatabaseSettings().GenerateDbContextInterface()
        return bool.TryParse(_template.ExecutionContext.Settings.GetGroup("ac0a788e-d8b3-4eea-b56d-538608f1ded9").GetSetting("85dea0e8-8981-4c7b-908e-d99294fc37f1")?.Value.ToPascalCase(), out var result) && result;
    }

    public IEnumerable<CSharpStatement> GetReturnStatements(CSharpClassMethod method, ITypeReference returnType)
    {
        if (returnType.Element == null)
        {
            throw new Exception("No return type specified");
        }
        var statements = new List<CSharpStatement>();

        var entitiesReturningPk = GetEntitiesReturningPK(method, returnType);
        var nonUserSuppliedEntitiesReturningPks = GetEntitiesReturningPK(method, returnType, isUserSupplied: false);

        foreach (var entity in nonUserSuppliedEntitiesReturningPks.Where(x => x.IsNew).GroupBy(x => x.ElementModel.Id).Select(x => x.First()))
        {
            if (entity.ElementModel.IsClassModel())
            {
                var primaryKeys = entity.ElementModel.AsClassModel().GetTypesInHierarchy().SelectMany(c => c.Attributes.Where(a => a.IsPrimaryKey()));
                if (primaryKeys.Any(p => HasDBGeneratedPk(p)))
                {
                    statements.Add($"{entity.DataAccessProvider.SaveChangesAsync()}");
                }
            }
            else
            {
                statements.Add($"{entity.DataAccessProvider.SaveChangesAsync()}");
            }
        }

        if (returnType.Element.AsDTOModel()?.IsMapped == true &&
            method.TrackedEntities().Values.Any(x => x.ElementModel?.Id == returnType.Element.AsDTOModel().Mapping.ElementId) &&
            _template.TryGetTypeName("Application.Contract.Dto", returnType.Element, out var returnDto))
        {
            var entityDetails = method.TrackedEntities().Values.First(x => x.ElementModel?.Id == returnType.Element.AsDTOModel().Mapping.ElementId);
            if (entityDetails.ProjectedType == returnDto)
            {
                statements.Add($"return {entityDetails.VariableName};");
            }
            else
            {
                //Adding Using Clause for Extension Methods
                _template.TryGetTypeName("Intent.Application.Dtos.EntityDtoMappingExtensions", returnType.Element, out var _);
                var autoMapperFieldName = method.Class.InjectService(_template.UseType("AutoMapper.IMapper"));
                string nullable = returnType.IsNullable ? "?" : "";
                statements.Add($"return {entityDetails.VariableName}{nullable}.MapTo{returnDto}{(returnType.IsCollection ? "List" : "")}({autoMapperFieldName});");
            }
        }
        else if (IsResultPaginated(returnType) &&
                 returnType.GenericTypeParameters.FirstOrDefault()?.Element.AsDTOModel()?.IsMapped == true &&
                 method.TrackedEntities().Values.Any(x => x.ElementModel?.Id == returnType.GenericTypeParameters.First().Element.AsDTOModel().Mapping.ElementId) &&
                 _template.TryGetTypeName("Application.Contract.Dto", returnType.GenericTypeParameters.First().Element, out returnDto))
        {
            var entityDetails = method.TrackedEntities().Values.First(x => x.ElementModel.Id == returnType.GenericTypeParameters.First().Element.AsDTOModel().Mapping.ElementId);
            if (entityDetails.ProjectedType == returnDto)
            {
                statements.Add($"return {entityDetails.VariableName}.MapToPagedResult();");
            }
            else
            {
                var autoMapperFieldName = method.Class.InjectService(_template.UseType("AutoMapper.IMapper"));
                statements.Add($"return {entityDetails.VariableName}.MapToPagedResult(x => x.MapTo{returnDto}({autoMapperFieldName}));");
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
            statements.Add(new CSharpStatement($"// TODO: Implement return type mapping...").SeparatedFromPrevious());
            statements.Add("throw new NotImplementedException(\"Implement return type mapping...\");");
        }

        return statements;
    }

    private List<EntityDetails> GetEntitiesReturningPK(CSharpClassMethod method, ITypeReference returnType, bool? isUserSupplied = null)
    {
        if (returnType.Element.IsDTOModel())
        {
            var dto = returnType.Element.AsDTOModel();

            var mappedPks = dto.Fields
                .Where(x => x.Mapping != null && Intent.Modelers.Domain.Api.AttributeModelExtensions.IsAttributeModel(x.Mapping.Element) && Intent.Modelers.Domain.Api.AttributeModelExtensions.AsAttributeModel(x.Mapping.Element).IsPrimaryKey(isUserSupplied))
                .Select(x => Intent.Modelers.Domain.Api.AttributeModelExtensions.AsAttributeModel(x.Mapping.Element).InternalElement.ParentElement.Id)
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

    private bool HasDBGeneratedPk(AttributeModel attribute)
    {
        return attribute.IsPrimaryKey() && attribute.GetStereotypeProperty("Primary Key", "Data source", "Default") == "Default";
    }

    public IEnumerable<CSharpStatement> CreateEntity(CSharpClassMethod method, CreateEntityActionTargetEndModel createAction)
    {
        try
        {
            var handlerClass = method.Class;
            var entity = createAction.Element.AsClassModel() ?? createAction.Element.AsClassConstructorModel().ParentClass;

            var entityVariableName = createAction.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);
            var dataAccess = InjectDataAccessProvider(method, entity);

            method.TrackedEntities().Add(createAction.Id, new EntityDetails(entity.InternalElement, entityVariableName, dataAccess, true));

            var mapping = createAction.Mappings.SingleOrDefault();
            var statements = new List<CSharpStatement>();

            if (MustAccessEntityThroughAggregate(dataAccess))
            {
                if (!TryGetFindAggregateStatements(method, mapping.SourceElement as IElement, entity, out statements))
                {
                    Logging.Log.Warning($"Unable to implement creation logic for handler '{handlerClass.Name}'. See earlier warnings for more information.");
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

    public IEnumerable<CSharpStatement> UpdateEntity(CSharpClassMethod method, UpdateEntityActionTargetEndModel updateAction)
    {
        try
        {
            var entityDetails = method.TrackedEntities()[updateAction.Id];
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

                if (RepositoryRequiresExplicitUpdate(_template, entity))
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
                    WireupDomainServicesForOperations(method.Class, updateAction, updateStatements);
                    AdjustOperationInvocationForAsyncAndReturn(method, updateMapping, updateStatements);

                    statements.AddRange(updateStatements);
                }

                if (RepositoryRequiresExplicitUpdate(_template, entity))
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

    private void AdjustOperationInvocationForAsyncAndReturn(CSharpClassMethod method, IElementToElementMapping updateMapping, IList<CSharpStatement> updateStatements)
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

                method.TrackedEntities().Add(invocation.TargetElement.Id, new EntityDetails((IElement)invocation.TargetElement.TypeReference.Element, variableName, null, false, null, invocation.TargetElement.TypeReference.IsCollection));
            }
        }
    }

    public IEnumerable<CSharpStatement> DeleteEntity(CSharpClassMethod method, DeleteEntityActionTargetEndModel deleteAction)
    {
        try
        {
            var entityDetails = method.TrackedEntities()[deleteAction.Id];
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

    //public IEnumerable<CSharpStatement> InvokeService(CSharpClass handlerClass, IAssociationEnd invocation)
    //{
    //    switch (invocation.TypeReference.Element.SpecializationType)
    //    {
    //        case Intent.Modelers.Services.Api.OperationModel.SpecializationType:
    //            return CallServiceOperation(handlerClass, invocation);
    //        case CommandModel.SpecializationType:
    //        case QueryModel.SpecializationType:
    //            return MediatorSend(handlerClass, invocation);
    //    }

    //    return [];
    //}

    //public IEnumerable<CSharpStatement> MediatorSend(CSharpClass handlerClass, IAssociationEnd callServiceOperation)
    //{
    //    var @class = handlerClass;
    //    var ctor = @class.Constructors.First();
    //    var template = handlerClass.File.Template;
    //    if (ctor.Parameters.All(x => x.Type != template.UseType("MediatR.ISender")))
    //    {
    //        ctor.AddParameter(template.UseType("MediatR.ISender"), "mediator", param =>
    //        {
    //            param.IntroduceReadonlyField((_, s) => s.ThrowArgumentNullException());
    //        });
    //    }

    //    var requestName = callServiceOperation.TypeReference.Element.Name.ToLocalVariableName();
    //    var statements = new List<CSharpStatement>();
    //    statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(requestName), _csharpMapping.GenerateCreationStatement(callServiceOperation.Mappings.Single())).WithSemicolon().SeparatedFromPrevious());
    //    var response = callServiceOperation.TypeReference.Element?.TypeReference?.Element;
    //    if (response != null)
    //    {
    //        statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(response.Name.ToLocalVariableName()), new CSharpInvocationStatement("await _mediator.Send").AddArgument(requestName).AddArgument("cancellationToken")));
    //    }
    //    else
    //    {
    //        statements.Add(new CSharpInvocationStatement("await _mediator.Send").AddArgument(requestName).AddArgument("cancellationToken"));
    //    }
    //    return statements;
    //}

    //public IEnumerable<CSharpStatement> CallServiceOperation(CSharpClass handlerClass, IAssociationEnd callServiceOperation)
    //{
    //    try
    //    {
    //        var statements = new List<CSharpStatement>();
    //        if (!HasServiceDependency(callServiceOperation, out var dependencyInfo) || callServiceOperation.Mappings.Any() is false)
    //        {
    //            return Array.Empty<CSharpStatement>();
    //        }

    //        // So that the mapping system can resolve the name of the operation from the interface itself:
    //        _template.AddTypeSource(dependencyInfo.ServiceInterfaceTemplate.Id);

    //        string? serviceField;
    //        if (dependencyInfo.Injectable)
    //        {
    //            serviceField = InjectService(_template.GetTypeName(dependencyInfo.ServiceInterfaceTemplate), handlerClass);
    //        }
    //        else
    //        {
    //            serviceField = TrackedEntities.LastOrDefault().Value?.VariableName;
    //            if (serviceField is null)
    //            {
    //                throw new ElementException(callServiceOperation, @"Call Service Operation performed without a prior call to ""Create"" or ""Query"" an Entity.");
    //            }
    //        }

    //        var methodInvocation = _csharpMapping.GenerateCreationStatement(callServiceOperation.Mappings.First());
    //        CSharpStatement invoke = new CSharpAccessMemberStatement(serviceField, methodInvocation);

    //        var invStatement = methodInvocation as CSharpInvocationStatement;
    //        if (invStatement?.IsAsyncInvocation() == true)
    //        {
    //            invStatement.AddArgument("cancellationToken");
    //            invoke = new CSharpAwaitExpression(invoke);
    //        }

    //        var operationModel = (IElement)callServiceOperation.TypeReference.Element;
    //        if (operationModel.TypeReference.Element != null)
    //        {
    //            var variableName = callServiceOperation.Name.ToLocalVariableName();
    //            _csharpMapping.SetFromReplacement(callServiceOperation, variableName);
    //            _csharpMapping.SetToReplacement(callServiceOperation, variableName);

    //            statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(variableName), invoke));


    //            TrackedEntities.Add(callServiceOperation.Id, new EntityDetails((IElement)operationModel.TypeReference.Element, variableName, null, false, null, operationModel.TypeReference.IsCollection));
    //        }
    //        else if (invStatement?.Expression.Reference is ICSharpMethodDeclaration methodDeclaration &&
    //                 (methodDeclaration.ReturnTypeInfo.GetTaskGenericType() is CSharpTypeTuple || methodDeclaration.ReturnTypeInfo is CSharpTypeTuple))
    //        {
    //            var tuple = (CSharpTypeTuple)methodDeclaration.ReturnTypeInfo.GetTaskGenericType() ?? (CSharpTypeTuple)methodDeclaration.ReturnTypeInfo;
    //            var declaration = new CSharpDeclarationExpression(tuple.Elements.Select(s => s.Name.ToCamelCase()).ToList());
    //            statements.Add(new CSharpAssignmentStatement(declaration, invoke));
    //        }
    //        else
    //        {
    //            statements.Add(invoke);
    //        }

    //        WireupDomainServicesForOperations(handlerClass, callServiceOperation, statements);

    //        return statements;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new ElementException(callServiceOperation, "An error occurred while generating the domain interactions logic", ex);
    //    }

    //    bool HasServiceDependency(IAssociationEnd callServiceOperation, out (ICSharpFileBuilderTemplate ServiceInterfaceTemplate, bool Injectable) dependencyInfo)
    //    {
    //        var serviceModel = ((IElement)callServiceOperation.TypeReference.Element).ParentElement;
    //        switch (serviceModel.SpecializationTypeId)
    //        {
    //            case DomainServiceSpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.DomainServices.Interface, serviceModel, out var domainServiceTemplate):
    //                dependencyInfo = (domainServiceTemplate, true);
    //                return true;
    //            case ApplicationServiceSpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Services.Interface, serviceModel, out var applicationServiceTemplate):
    //                dependencyInfo = (applicationServiceTemplate, true);
    //                return true;
    //            case RepositorySpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Repository.Interface.Entity, serviceModel, out var repositoryTemplate):
    //                dependencyInfo = (repositoryTemplate, true);
    //                return true;
    //            case EntitySpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, serviceModel, out var entityTemplate):
    //                dependencyInfo = (entityTemplate, false);
    //                return true;
    //            case ServiceProxySpecializationId when _template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Services.ClientInterface, serviceModel, out var clientInterfaceTemplate):
    //                dependencyInfo = (clientInterfaceTemplate, true);
    //                return true;
    //            default:
    //                dependencyInfo = default;
    //                return false;
    //        }
    //    }
    //}

    private IDataAccessProvider InjectDataAccessProvider(CSharpClassMethod method, ClassModel foundEntity, QueryActionContext queryContext = null)
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

    public bool TryInjectRepositoryForEntity(CSharpClassMethod method, ClassModel foundEntity, QueryActionContext context, out IDataAccessProvider dataAccessProvider)
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

    private bool TryInjectDbContext(CSharpClassMethod method, ClassModel entity, QueryActionContext queryContext, out IDataAccessProvider dataAccessProvider)
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

    public bool TryInjectDataAccessForComposite(CSharpClassMethod method, ClassModel foundEntity, out IDataAccessProvider dataAccessProvider)
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

    private void WireupDomainServicesForConstructors(CSharpClass handlerClass, CreateEntityActionTargetEndModel createAction, CSharpStatement constructionStatement)
    {
        var constructor = createAction.Element.AsClassConstructorModel();
        if (constructor != null)
        {
            WireupDomainService(constructionStatement as CSharpInvocationStatement, constructor.Parameters, handlerClass);
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
            getOperation = (x) => operation;
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
            WireupDomainService(invocation, operation.Parameters, handlerClass);
        }
    }

    private void WireupDomainServicesForProcessingAction(CSharpClass handlerClass, IElementToElementMapping mapping, IList<CSharpStatement> processingActions)
    {
        var mappedOperations = mapping.MappedEnds.Where(me => OperationModelExtensions.IsOperationModel(me.TargetElement)).Select(me => OperationModelExtensions.AsOperationModel(me.TargetElement)).ToList();

        if (!mappedOperations.Any())
        {
            return;
        }
        if (!mappedOperations.Any(o => o.Parameters.Any(p => p.TypeReference.Element.SpecializationTypeId == DomainServiceSpecializationId)))
        {
            return;
        }


        foreach (var updateStatement in processingActions)
        {
            if (updateStatement is not CSharpInvocationStatement invocation)
            {
                continue;
            }
            string operationName = invocation.Expression.Reference is ICSharpMethodDeclaration iCSharpMethodDeclaration ? iCSharpMethodDeclaration.Name.ToCSharpIdentifier() : null;
            var operation = mappedOperations.FirstOrDefault(operation => operation.Name == operationName);

            if (operation == null)
            {
                continue;
            }
            WireupDomainService(invocation, operation.Parameters, handlerClass);
        }
    }

    private void WireupDomainServicesForOperations(CSharpClass handlerClass, IAssociationEnd callServiceOperation, List<CSharpStatement> statements)
    {
        var operation = OperationModelExtensions.AsOperationModel(callServiceOperation.TypeReference.Element);
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
                default:
                    break;
            }

            var invocation = statement as CSharpInvocationStatement;
            if (invocation is null)
            {
                return;
            }

            WireupDomainService(invocation, operation.Parameters, handlerClass);
        }
    }

    private void WireupDomainService(CSharpInvocationStatement invocation, IList<Intent.Modelers.Domain.Api.ParameterModel> parameters, CSharpClass handlerClass)
    {
        if (invocation is null)
        {
            return;
        }

        for (var i = 0; i < parameters.Count; i++)
        {
            var arg = parameters[i];
            if (arg.TypeReference.Element.SpecializationTypeId != DomainServiceSpecializationId)
            {
                continue;
            }

            if (!_template.TryGetTypeName(TemplateRoles.Domain.DomainServices.Interface, arg.TypeReference.Element.Id, out var domainServiceInterface))
            {
                continue;
            }
            var fieldName = handlerClass.InjectService(domainServiceInterface, domainServiceInterface.Substring(1).ToParameterName());
            //Change `default` or `parameterName: default` into `_domainService` (fieldName)
            invocation.Statements[i].Replace(invocation.Statements[i].GetText("").Replace("default", fieldName));
        }
    }

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

    private bool RepositoryRequiresExplicitUpdate(ICSharpTemplate _template, IMetadataModel forEntity)
    {
        return _template.TryGetTemplate<ICSharpFileBuilderTemplate>(
                   TemplateRoles.Repository.Interface.Entity,
                   forEntity,
                   out var repositoryInterfaceTemplate) &&
               repositoryInterfaceTemplate.CSharpFile.Interfaces[0].TryGetMetadata<bool>("requires-explicit-update", out var requiresUpdate) &&
               requiresUpdate;
    }


    private List<PrimaryKeyFilterMapping> GetAggregatePKFindCriteria(IElement requestElement, ClassModel aggregateEntity, ClassModel compositeEntity)
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


    private bool TryGetFindAggregateStatements(CSharpClassMethod method, IElementToElementMapping queryMapping, ClassModel foundEntity, out List<CSharpStatement> statements)
    {
        return TryGetFindAggregateStatements(method, queryMapping, (IElement)queryMapping.SourceElement, foundEntity, out statements);
    }

    private bool TryGetFindAggregateStatements(CSharpClassMethod method, IElement requestElement, ClassModel foundEntity, out List<CSharpStatement> statements)
    {
        return TryGetFindAggregateStatements(method, null, requestElement, foundEntity, out statements);
    }

    private bool TryGetFindAggregateStatements(CSharpClassMethod method, IElementToElementMapping queryMapping, IElement requestElement, ClassModel foundEntity, out List<CSharpStatement> statements)
    {
        statements = new List<CSharpStatement>();
        var aggregateEntity = GetAssociationsToAggregateRoot(foundEntity).First().Class;
        var aggregateVariableName = aggregateEntity.Name.ToLocalVariableName();
        var aggregateDataAccess = InjectDataAccessProvider(method, aggregateEntity);

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
        foreach (var associationEndModel in GetAssociationsToAggregateRoot(foundEntity).SkipLast(1))
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

    /// <summary>
    /// Returns the list of association ends that traverse from the provided <paramref name="entity"/> to its aggregate root.
    /// The association ends are the "Source" ends (i.e. the ends connected to the owning entity)
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    private static List<AssociationEndModel> GetAssociationsToAggregateRoot(ClassModel entity)
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

    private bool TryGetPaginationValues(IAssociationEnd associationEnd, CSharpClassMappingManager mappingManager, out string pageNo, out string pageSize, out string? orderBy, out bool orderByIsNullable)
    {
        orderByIsNullable = false;
        var handler = (IElement)associationEnd.OtherEnd().TypeReference.Element;
        var returnsPagedResult = IsResultPaginated(handler.TypeReference);

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

public static class CSharpClassMethodExtensions
{
    public static Dictionary<string, EntityDetails> TrackedEntities(this CSharpClassMethod method)
    {
        if (!method.TryGetMetadata<Dictionary<string, EntityDetails>>("tracked-entities", out var trackedEntities))
        {
            trackedEntities = new Dictionary<string, EntityDetails>();
            if (method.HasMetadata("tracked-entities"))
            {
                method.Metadata.Remove("tracked-entities");
            }
            method.AddMetadata("tracked-entities", trackedEntities);
        }

        return trackedEntities;
    }
}


internal static class AttributeModelExtensions
{
    public static bool IsPrimaryKey(this AttributeModel attribute, bool? isUserSupplied = null)
    {
        if (!attribute.HasStereotype("Primary Key"))
        {
            return false;
        }

        if (!isUserSupplied.HasValue)
        {
            return true;
        }

        if (!attribute.GetStereotype("Primary Key").TryGetProperty("Data source", out var property))
        {
            return isUserSupplied == false;
        }

        return property.Value == "User supplied" == isUserSupplied.Value;
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