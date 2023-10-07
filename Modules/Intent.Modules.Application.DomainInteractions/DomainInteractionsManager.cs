using System;
using System.Collections.Generic;
using System.Linq;
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
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Templates;

namespace Intent.Modules.Application.DomainInteractions;

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

            if (!queryFields.Any())
            {
                throw new ElementException(associationEnd, "No query fields have been mapped for this Query Entity Action, which signifies a single return value.");
            }
            var expression = queryFields.Any() ? $"x => {string.Join(" && ", queryFields)}, " : "";
            statements.Add($"var {entityVariableName} = await {repositoryFieldName}.FindAsync({expression}cancellationToken);");
        }

        if (!associationEnd.TypeReference.IsNullable && !associationEnd.TypeReference.IsCollection)
        {
            var queryFields = queryMapping.MappedEnds
                .Select(x => new CSharpStatement($"{{{_csharpMapping.GenerateSourceStatementForMapping(queryMapping, x)}}}"))
                .ToList();
            statements.Add(CreateThrowNotFoundIfNullStatement(
                template: _template,
                variable: entityVariableName,
                message: $"Could not find {foundEntity.Name.ToPascalCase()} '{queryFields.AsSingleOrTuple()}'"));

        }
        TrackedEntities.Add(associationEnd.Id, new EntityDetails(foundEntity, entityVariableName, repositoryFieldName, false, associationEnd.TypeReference.IsCollection));

        return statements;
    }

    public CSharpStatement CreateThrowNotFoundIfNullStatement(
        ICSharpTemplate template,
        string variable,
        string message)
    {
        var ifStatement = new CSharpIfStatement($"{variable} is null");
        ifStatement.SeparatedFromPrevious(false);
        ifStatement.AddStatement($@"throw new {template.GetNotFoundExceptionName()}($""{message}"");");

        return ifStatement;
    }

    public void InjectRepositoryForEntity(ClassModel foundEntity, out string repositoryFieldName)
    {
        var repositoryInterface = (_template as IntentTemplateBase).GetEntityRepositoryInterfaceName(foundEntity);
        var repositoryName = repositoryInterface[1..].ToCamelCase();
        var temp = default(string);

        var ctor = _template.CSharpFile.Classes.First().Constructors.First();
        if (ctor.Parameters.All(x => x.Type != repositoryInterface))
        {
            ctor.AddParameter(repositoryInterface, repositoryName.ToParameterName(),
                param => param.IntroduceReadonlyField(field => temp = field.Name));
            repositoryFieldName = temp;
        }
        else
        {
            repositoryFieldName = ctor.Parameters.First(x => x.Type == repositoryInterface).Name.ToPrivateMemberName();
        }
    }

    private void InjectAutoMapper(out string fieldName)
    {
        var temp = default(string);
        var ctor = _template.CSharpFile.Classes.First().Constructors.First();
        if (ctor.Parameters.All(x => x.Type != _template.UseType("AutoMapper.IMapper")))
        {
            ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper",
                param => param.IntroduceReadonlyField(field => temp = field.Name));
            fieldName = temp;
        }
        else
        {
            fieldName = ctor.Parameters.First(x => x.Type == _template.UseType("AutoMapper.IMapper")).Name.ToPrivateMemberName();
        }
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
            InjectAutoMapper(out var autoMapperFieldName);
            statements.Add($"return {entityDetails.VariableName}.MapTo{returnDto}{(returnType.IsCollection ? "List" : "")}({autoMapperFieldName});");
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
        var entity = createAction.Element.AsClassModel() ?? createAction.Element.AsClassConstructorModel().ParentClass;

        InjectRepositoryForEntity(entity, out var repositoryFieldName);

        TrackedEntities.Add(createAction.Id, new EntityDetails(entity, createAction.Name, repositoryFieldName, true));

        var entityVariableName = createAction.Name;
        var statements = new List<CSharpStatement>();

        var mapping = createAction.Mappings.SingleOrDefault();
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
                statements.Add(new CSharpInvocationStatement(entityDetails.RepositoryFieldName, "Update").AddArgument(entityDetails.VariableName.Singularize()));
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
                statements.Add(new CSharpInvocationStatement(entityDetails.RepositoryFieldName, "Update").AddArgument(entityDetails.VariableName));
            }
        }

        foreach (var actions in updateAction.ProcessingActions)
        {
            statements.Add(string.Empty);
            statements.AddRange(_csharpMapping.GenerateUpdateStatements(actions.InternalElement.Mappings.Single()));
            statements.Add(string.Empty);
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

public record EntityDetails(ClassModel Model, string VariableName, string RepositoryFieldName, bool IsNew, bool IsCollection = false);

public static class MappingExtensions
{
    public static IElementToElementMapping GetQueryEntityMapping(this IEnumerable<IElementToElementMapping> mappings)
    {
        return mappings.SingleOrDefault(x => x.MappingType == "Query Entity Mapping");
    }

    public static IElementToElementMapping GetUpdateEntityMapping(this IEnumerable<IElementToElementMapping> mappings)
    {
        return mappings.SingleOrDefault(x => x.MappingType == "Update Entity Mapping");
    }
}

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