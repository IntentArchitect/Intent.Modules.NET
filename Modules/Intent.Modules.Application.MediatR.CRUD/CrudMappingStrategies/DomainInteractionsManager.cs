using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.MediatR.CRUD.CrudStrategies;
using Intent.Modules.Application.MediatR.CRUD.Mapping.Resolvers;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.CrudMappingStrategies;

public class DomainInteractionsManager
{
    private readonly ICSharpFileBuilderTemplate _template;
    private readonly CSharpClassMappingManager _csharpMapping;

    public DomainInteractionsManager(ICSharpFileBuilderTemplate template)
    {
        _template = template;
        var model = (_template as ITemplateWithModel)?.Model as IElementWrapper;
        //var queryTemplate = _template.ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Application.Query, model);
        var queryTemplate = _template.GetTypeInfo(model.InternalElement.AsTypeReference()).Template as ICSharpFileBuilderTemplate ?? throw new Exception("Model template could not be determined for " + _template.Id);
        // commandTemplate needs to be passed in so that the DefaultMapping can correctly resolve the mappings (e.g. pascal-casing for properties, even if the mapping is to a camel-case element)
        _csharpMapping = new CSharpClassMappingManager(queryTemplate); // TODO: Improve this template resolution system - it's not clear which template should be passed in initially.
        _csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(_template));
        _csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(_template));
        _csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(_template));
        _csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(_template));

        _csharpMapping.SetFromReplacement(model.InternalElement, "request");
        _template.CSharpFile.AddMetadata("mapping-manager", _csharpMapping);
    }

    public Dictionary<string, EntityDetails> TrackedEntities { get; set; } = new();

    public List<CSharpStatement> QueryEntity(ClassModel foundEntity, IAssociationEnd associationEnd)
    {
        var queryMapping = associationEnd.Mappings.GetQueryEntityMapping();

        var entityVariableName = associationEnd.Name;

        _csharpMapping.SetFromReplacement(foundEntity, entityVariableName);
        _csharpMapping.SetFromReplacement(associationEnd, entityVariableName);
        _csharpMapping.SetToReplacement(foundEntity, entityVariableName);
        _csharpMapping.SetToReplacement(associationEnd, entityVariableName);

        InjectRepositoryForEntity(foundEntity, out var repositoryFieldName);

        var statements = new List<CSharpStatement>();
        if (queryMapping.MappedEnds.Any() && queryMapping.MappedEnds.All(x => x.TargetElement.AsAttributeModel()?.IsPrimaryKey() == true))
        {
            var idFields = queryMapping.MappedEnds
                .OrderBy(x => ((IElement)x.TargetElement).Order)
                .Select(x => _csharpMapping.GenerateSourceStatementForMapping(queryMapping, x))
                .ToList();

            if (associationEnd.TypeReference.IsCollection && idFields.Count == 1 && queryMapping.MappedEnds.Single().SourceElement?.TypeReference.IsCollection == true)
            {
                statements.Add($"var {entityVariableName} = await {repositoryFieldName}.FindByIdsAsync({idFields.AsSingleOrTuple()}.ToArray(), cancellationToken);");
            }
            else
            {
                statements.Add($"var {entityVariableName} = await {repositoryFieldName}.FindByIdAsync({idFields.AsSingleOrTuple()}, cancellationToken);");
            }
        }
        else if (associationEnd.TypeReference.IsCollection)
        {
            var queryFields = queryMapping.MappedEnds
                .Select(x => $"x.{x.TargetElement.Name} == {_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}")
                .ToList();
            var expression = queryFields.Any() ? $"x => {string.Join(" && ", queryFields)}, " : "";
            statements.Add($"var {entityVariableName} = await {repositoryFieldName}.FindAllAsync({expression}cancellationToken);");
        }
        else
        {
            var queryFields = queryMapping.MappedEnds
                .Select(x => $"x.{x.TargetElement.Name} == {_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}")
                .ToList();
            var expression = queryFields.Any() ? $"x => {string.Join(" && ", queryFields)}, " : "";
            statements.Add($"var {entityVariableName} = await {repositoryFieldName}.FindAsync({expression}cancellationToken);");
        }

        if (!associationEnd.TypeReference.IsNullable && !associationEnd.TypeReference.IsCollection)
        {
            var queryFields = queryMapping.MappedEnds
                .Select(x => new CSharpStatement($"{{{_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}}}"))
                .ToList();
            statements.Add(_template.CreateThrowNotFoundIfNullStatement(
                variable: entityVariableName,
                message: $"Could not find {foundEntity.Name.ToPascalCase()} '{queryFields.AsSingleOrTuple()}'"));

        }
        TrackedEntities.Add(associationEnd.Id, new EntityDetails(foundEntity, entityVariableName, repositoryFieldName, false, associationEnd.TypeReference.IsCollection));

        return statements;
    }

    public void InjectRepositoryForEntity(ClassModel foundEntity, out string repositoryFieldName)
    {
        var repositoryInterface = (_template as IntentTemplateBase).GetEntityRepositoryInterfaceName(foundEntity);
        var repositoryName = repositoryInterface[1..].ToCamelCase();
        var temp = default(string);

        var ctor = _template.CSharpFile.Classes.First().Constructors.First();
        ctor.AddParameter(repositoryInterface, repositoryName.ToParameterName(),
            param => param.IntroduceReadonlyField(field => temp = field.Name));

        repositoryFieldName = temp;
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
            statements.Add($"await {entity.RepositoryFieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);");
        }

        if (returnType.Element.AsDTOModel()?.IsMapped == true && _template.TryGetTypeName("Application.Contract.Dto", returnType.Element, out var returnDto))
        {
            var entityDetails = TrackedEntities.Values.First(x => x.Model.Id == returnType.Element.AsDTOModel().Mapping.ElementId);
            var ctor = _template.CSharpFile.Classes.First().Constructors.First();
            ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", param => param.IntroduceReadonlyField());
            statements.Add($"return {entityDetails.VariableName}.MapTo{returnDto}{(returnType.IsCollection ? "List" : "")}(_mapper);");
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
        var entity = createAction.Element.AsClassModel() ?? createAction.Element.AsClassConstructorModel()?.ParentClass;

        InjectRepositoryForEntity(entity, out var repositoryFieldName);

        TrackedEntities.Add(createAction.Id, new EntityDetails(entity, createAction.Name, repositoryFieldName, true));

        var mapping = createAction.Mappings.SingleOrDefault();
        var entityVariableName = createAction.Name;
        var statements = new List<CSharpStatement>
        {
            new CSharpAssignmentStatement($"var {entityVariableName}", _csharpMapping.GenerateCreationStatement(mapping)).WithSemicolon()
        };

        _csharpMapping.SetFromReplacement(createAction.InternalAssociationEnd, entityVariableName);
        _csharpMapping.SetFromReplacement(entity, entityVariableName);
        _csharpMapping.SetToReplacement(createAction.InternalAssociationEnd, entityVariableName);

        foreach (var actions in createAction.ProcessingActions)
        {
            statements.Add(string.Empty);
            statements.AddRange(_csharpMapping.GenerateUpdateStatements(actions.InternalElement.Mappings.Single()));
            statements.Add(string.Empty);
        }
        return statements;
    }

    public IEnumerable<CSharpStatement> UpdateEntity(UpdateEntityActionTargetEndModel updateAction)
    {
        var entityDetails = TrackedEntities[updateAction.Id];
        var foundEntity = entityDetails.Model;
        var queryMapping = updateAction.Mappings.GetQueryEntityMapping();
        var updateMapping = updateAction.Mappings.GetUpdateEntityMapping();

        var statements = new List<CSharpStatement>();
        if (queryMapping != null)
        {
            if (entityDetails.IsCollection)
            {
                _csharpMapping.SetToReplacement(foundEntity, entityDetails.VariableName.Singularize());
                statements.Add(new CSharpForEachStatement(entityDetails.VariableName.Singularize(), entityDetails.VariableName)
                    .AddStatements(_csharpMapping.GenerateUpdateStatements(updateMapping)));
                if (RepositoryRequiresExplicitUpdate(foundEntity))
                {
                    statements.Add(new CSharpInvocationStatement(entityDetails.RepositoryFieldName, "Update").AddArgument(entityDetails.VariableName.Singularize()));
                }
            }
            else
            {
                statements.AddRange(_csharpMapping.GenerateUpdateStatements(updateMapping));
                if (RepositoryRequiresExplicitUpdate(foundEntity))
                {
                    statements.Add(new CSharpInvocationStatement(entityDetails.RepositoryFieldName, "Update").AddArgument(entityDetails.VariableName));
                }
            }

            foreach (var actions in updateAction.ProcessingActions)
            {
                statements.Add(string.Empty);
                statements.AddRange(_csharpMapping.GenerateUpdateStatements(actions.InternalElement.Mappings.Single()));
                statements.Add(string.Empty);
            }
        }

        return statements;
    }

    public IEnumerable<CSharpStatement> DeleteEntity(DeleteEntityActionTargetEndModel deleteAction)
    {
        var entityDetails = TrackedEntities[deleteAction.Id];
        var statements = new List<CSharpStatement>();
        if (entityDetails.IsCollection)
        {
            statements.Add(new CSharpForEachStatement(entityDetails.VariableName.Singularize(), entityDetails.VariableName)
                .AddStatement(new CSharpInvocationStatement(entityDetails.RepositoryFieldName, "Remove").AddArgument(entityDetails.VariableName.Singularize())));
        }
        else
        {
            statements.Add(new CSharpInvocationStatement(entityDetails.RepositoryFieldName, "Remove").AddArgument(entityDetails.VariableName));
        }
        return statements;
    }

    private bool RepositoryRequiresExplicitUpdate(IMetadataModel forEntity)
    {
        return _template.TryGetTemplate<ICSharpFileBuilderTemplate>(
                   TemplateFulfillingRoles.Repository.Interface.Entity,
                   forEntity,
                   out var repositoryInterfaceTemplate) &&
               repositoryInterfaceTemplate.CSharpFile.Interfaces[0].TryGetMetadata<bool>("requires-explicit-update", out var requiresUpdate) &&
               requiresUpdate;
    }
}