using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.FluentValidation.Shared;

internal record ConstraintField(string FieldName, string CompositeGroupName)
{
    public int GroupCount { get; set; }
}

/// <summary>
/// Encapsulates all logic related to unique-constraint validation rules, including
/// field collection, rule-chain population, and attribute/class mapping resolution.
/// </summary>
internal static class UniqueConstraintRules
{
    // ─── Public API ──────────────────────────────────────────────────────────────

    public static IReadOnlyCollection<ConstraintField> GetConstraintFields(
        DTOModel dtoModel,
        IEnumerable<IAssociationEnd>? sourceElementAdvancedMappings,
        bool enabled)
    {
        if (!enabled || (!IsCreateDto(dtoModel) && !IsUpdateDto(dtoModel)))
        {
            return ArraySegment<ConstraintField>.Empty;
        }

        var hasMappedClass = TryGetMappedClass(dtoModel, out var mappedClass) || TryGetAdvancedMappedClass(dtoModel, sourceElementAdvancedMappings, out mappedClass);
        if (!hasMappedClass)
        {
            return ArraySegment<ConstraintField>.Empty;
        }

        var indexStereotypeAttributes = mappedClass!.Attributes
            .Where(p => p.HasStereotype("Index") &&
                        p.GetStereotypeProperty("Index", "IsUnique", false))
            .Select(s => new ConstraintField(s.Name, s.GetStereotypeProperty("Index", "UniqueKey", string.Empty)))
            .ToArray();

        const string indexElementTypeId = "436e3afe-b4ef-481c-b803-0d1e7d263561";
        const string indexColumnTypeId = "c5ba925d-5c08-4809-a848-585a0cd4ddd3";

        var indexElements = mappedClass.InternalElement.ChildElements
            .Where(p => p.SpecializationTypeId == indexElementTypeId && p.GetStereotypeProperty("Settings", "Unique", false))
            .ToArray();

        var indexElementAttributes = new List<ConstraintField>();
        foreach (var indexElement in indexElements)
        {
            foreach (var indexColumnIndex in indexElement.ChildElements)
            {
                if (indexColumnIndex.SpecializationTypeId != indexColumnTypeId)
                {
                    continue;
                }
                var mappedAttribute = indexColumnIndex.MappedElement?.Element?.AsAttributeModel();
                if (mappedAttribute is null)
                {
                    continue;
                }
                if (indexColumnIndex.GetStereotypeProperty<string?>("Settings", "Type")?.ToLower() == "included")
                {
                    continue;
                }

                indexElementAttributes.Add(new ConstraintField(mappedAttribute.Name, indexElement.Name));
            }
        }

        var indexes = indexStereotypeAttributes.Concat(indexElementAttributes).ToArray();

        foreach (var group in indexes.GroupBy(g => g.CompositeGroupName))
        {
            var count = group.Count();
            foreach (var item in indexes)
            {
                if (item.CompositeGroupName == group.Key)
                {
                    item.GroupCount = count;
                }
            }
        }

        return indexes;
    }

    /// <summary>
    /// Appends the <c>MustAsync</c> unique-constraint chain statement for a single-field index.
    /// No-op if <paramref name="field"/> is not part of a single-field unique constraint.
    /// </summary>
    public static void ApplyFieldRules(
        DTOFieldModel field,
        CSharpMethodChainStatement validationRuleChain,
        IReadOnlyCollection<ConstraintField> indexFields)
    {
        if (!indexFields.Any(p => p.FieldName == field.Name && p.GroupCount == 1))
        {
            return;
        }

        validationRuleChain.AddChainStatement($"MustAsync(CheckUniqueConstraint_{field.Name.ToPascalCase()})", x => x.AddMetadata("requires-repository", true));
        validationRuleChain.AddChainStatement($@"WithMessage(""{field.Name.ToPascalCase()} already exists."")");
    }

    /// <summary>
    /// Returns DTO-level <c>RuleFor(v => v)</c> chains for composite (multi-field) unique constraints.
    /// </summary>
    public static IEnumerable<CSharpMethodChainStatement> GetDtoLevelValidators(IReadOnlyCollection<ConstraintField> indexFields)
    {
        if (!indexFields.Any(p => p.GroupCount > 1))
        {
            yield break;
        }

        var validationRuleChain = new CSharpMethodChainStatement("RuleFor(v => v)");
        var indexGroups = indexFields.Where(p => p.GroupCount > 1).GroupBy(g => g.CompositeGroupName).ToArray();
        foreach (var indexGroup in indexGroups)
        {
            validationRuleChain.AddChainStatement($"MustAsync(CheckUniqueConstraint_{string.Join("_", indexGroup.Select(s => s.FieldName.ToPascalCase()))})",
                x => x.AddMetadata("requires-repository", true));
            validationRuleChain.AddChainStatement($@"WithMessage(""The combination of {string.Join(" and ", indexGroup.Select(s => s.FieldName))} already exists."")");
        }

        yield return validationRuleChain;
    }

    public static CSharpStatement GetDtoAndDomainAttributeComparisonExpression(
        string domainEntityVarName,
        string dtoModelVarName,
        DTOModel dtoModel,
        IReadOnlyCollection<ConstraintField> constraintFields)
    {
        var sb = new StringBuilder();

        foreach (var field in dtoModel.Fields)
        {
            if ((!TryGetMappedAttribute(field, out var mappedAttribute) && !TryGetAdvancedMappedAttribute(field, out mappedAttribute)) ||
                constraintFields.All(p => p.FieldName != mappedAttribute!.Name))
            {
                continue;
            }

            if (sb.Length > 0)
            {
                sb.Append(" && ");
            }

            sb.Append($"{domainEntityVarName}.{mappedAttribute!.Name} == {dtoModelVarName}.{field.Name}");
        }

        return sb.ToString();
    }

    // ─── DTO classification helpers ──────────────────────────────────────────────

    public static bool IsCreateDto(DTOModel dtoModel)
    {
        return dtoModel.Name.StartsWith("create", StringComparison.InvariantCultureIgnoreCase) ||
               dtoModel.Name.StartsWith("add", StringComparison.InvariantCultureIgnoreCase) ||
               dtoModel.Name.StartsWith("new", StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool IsUpdateDto(DTOModel dtoModel)
    {
        return dtoModel.Name.StartsWith("update", StringComparison.InvariantCultureIgnoreCase) ||
               dtoModel.Name.StartsWith("edit", StringComparison.InvariantCultureIgnoreCase);
    }

    // ─── Class mapping resolution helpers ────────────────────────────────────────

    public static bool TryGetMappedClass(DTOModel dtoModel, out ClassModel? classModel)
    {
        // The loop is not needed on the service side where a Command/Query/DTO is mapped
        // to an Entity, but it is needed when mapping from a Service Proxy to a Command/Query/Service Operation
        // and then to an Entity.
        if (dtoModel.InternalElement.TryWalkMappingGraph(
                predicate: static candidate => candidate.IsClassModel(),
                out var mappedElement) &&
            mappedElement.IsClassModel())
        {
            classModel = mappedElement.AsClassModel();
            return true;
        }

        classModel = default;
        return false;
    }

    public static bool TryGetAdvancedMappedClass(DTOModel dtoModel, IEnumerable<IAssociationEnd>? associationedElements, out ClassModel? classModel)
    {
        if (associationedElements is null)
        {
            classModel = null;
            return false;
        }

        foreach (var associationEnd in associationedElements)
        {
            foreach (var mapping in associationEnd.Mappings)
            {
                var mappedEnd = mapping.MappedEnds.FirstOrDefault(p =>
                {
                    var possibleDto = (p.SourceElement as IElement)?.ParentElement;
                    return p.MappingType == "Data Mapping" && possibleDto is not null && possibleDto.Id == dtoModel.Id;
                });
                if (mappedEnd is null)
                {
                    continue;
                }

                classModel = (mappedEnd.TargetElement as IElement)?.ParentElement?.AsClassModel();
                if (classModel is not null)
                {
                    return true;
                }
            }
        }

        classModel = null;
        return false;
    }

    // ─── Attribute mapping resolution helpers ────────────────────────────────────

    public static bool TryGetMappedAttribute(DTOFieldModel field, out AttributeModel? attribute) => TryGetMappedAttribute(field.InternalElement, out attribute);
    private static bool TryGetMappedAttribute(IElement field, out AttributeModel? attribute, bool checkAdvancedMappings = true)
    {
        // The loop is not needed on the service side where a Command/Query/DTO is mapped
        // to an Attribute, but it is needed when mapping from a Service Proxy to a DTO Field and then to an Attribute.
        if (field.TryWalkMappingGraph(
                predicate: static candidate => candidate.IsAttributeModel(),
                out var mappedElement) &&
            mappedElement.IsAttributeModel())
        {
            attribute = mappedElement.AsAttributeModel();
            return true;
        }

        if (checkAdvancedMappings && field.MappedElement?.Element is IElement directMapped && directMapped.MappedElement?.Element is null)
        {
            return TryGetAdvancedMappedAttribute(directMapped, out attribute, checkBasicMappings: false);
        }

        attribute = default;
        return false;
    }

    public static bool TryGetAdvancedMappedAttribute(DTOFieldModel field, out AttributeModel? attribute) => TryGetAdvancedMappedAttribute(field.InternalElement, out attribute);
    private static bool TryGetAdvancedMappedAttribute(IElement field, out AttributeModel? attribute, bool checkBasicMappings = true)
    {
        var mappedEnd = field.MappedToElements.FirstOrDefault(p => p.MappingType == "Data Mapping");
        if (mappedEnd != null)
        {
            if (mappedEnd.TargetElement.IsAttributeModel())
            {
                attribute = mappedEnd.TargetElement.AsAttributeModel();
                return true;
            }

            if (checkBasicMappings && TryGetMappedAttribute((IElement)mappedEnd.TargetElement, out attribute, checkAdvancedMappings: false))
            {
                return true;
            }
        }

        attribute = null;
        return false;
    }

    public static bool TryGetAssociationMappedAttribute(
        DTOFieldModel field,
        List<IAssociationEnd>? associationedElements,
        out AttributeModel? attribute)
    {
        var parentAssociations = field.InternalElement.ParentElement?.AssociatedElements;
        var associations = associationedElements?.Count > 0
            ? associationedElements
            : parentAssociations;
        if (associations is null)
        {
            attribute = null;
            return false;
        }

        foreach (var associationEnd in associations)
        {
            foreach (var mapping in associationEnd.Mappings)
            {
                var mappedEnd = mapping.MappedEnds.FirstOrDefault(p =>
                    p.MappingType == "Data Mapping" &&
                    (p.SourceElement as IElement)?.Id == field.Id);

                if (mappedEnd?.TargetElement is not IElement targetElement ||
                    !targetElement.IsAttributeModel())
                {
                    continue;
                }
                attribute = targetElement.AsAttributeModel();
                return true;
            }
        }

        attribute = null;
        return false;
    }
}
