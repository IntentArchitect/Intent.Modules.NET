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
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Application.DomainInteractions.Extensions;

// Disambiguation
using Intent.Modelers.Domain.Api;

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


    public static IEnumerable<CSharpStatement> GetReturnStatements(this CSharpClassMethod method, ITypeReference returnType)
    {
        if (returnType.Element == null)
        {
            throw new Exception("No return type specified");
        }
        var statements = new List<CSharpStatement>();

        var _template = method.File.Template;
        var entitiesReturningPk = GetEntitiesReturningPK(method, returnType);
        var nonUserSuppliedEntitiesReturningPks = GetEntitiesReturningPK(method, returnType, isUserSupplied: false);

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
        else if (returnType.IsResultPaginated() &&
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

    private static List<EntityDetails> GetEntitiesReturningPK(CSharpClassMethod method, ITypeReference returnType, bool? isUserSupplied = null)
    {
        if (returnType.Element.IsDTOModel())
        {
            var dto = returnType.Element.AsDTOModel();

            var mappedPks = dto.Fields
                .Where(x => x.Mapping != null && x.Mapping.Element.IsAttributeModel() && x.Mapping.Element.AsAttributeModel().IsPrimaryKey(isUserSupplied))
                .Select(x => x.Mapping.Element.AsAttributeModel().InternalElement.ParentElement.Id)
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