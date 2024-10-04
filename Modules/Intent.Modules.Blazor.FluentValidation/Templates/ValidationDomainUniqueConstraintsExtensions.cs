using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using Intent.Modules.FluentValidation.Shared;

namespace Intent.Modules.Blazor.FluentValidation.Templates;

public static class ValidationDomainUniqueConstraintsExtensions
{
    public static void AddCheckUniqueConstraintsPlaceholdersForField(
        this CSharpMethodChainStatement validationRuleChain,
        IFluentValidationTemplate template,
        IElement model,
        IElement field)
    {
        var indexFields = GetUniqueConstraintFields(model);

        if (!indexFields.Any(p => p.FieldName == field.Name && p.GroupCount == 1))
        {
            return;
        }

        validationRuleChain.AddChainStatement($"MustAsync(CheckUniqueConstraint_{field.Name.ToPascalCase()})", stmt => stmt.AddMetadata("requires-repository", true));
        validationRuleChain.AddChainStatement($@"WithMessage(""{field.Name.ToPascalCase()} already exists."")");

        var @class = template.CSharpFile.Classes.First();
        var toValidateTypeName = template.GetTypeName(template.ToValidateTemplateId, model);
        @class.AddMethod($"{template.UseType("System.Threading.Tasks.Task")}<bool>", $"CheckUniqueConstraint_{field.Name.ToPascalCase()}", method =>
        {
            method.Private().Async();
            if (IsUpdateDto(model))
            {
                method.AddParameter(toValidateTypeName, "model");
            }

            method.AddParameter(template.GetTypeName(field.TypeReference), "value");

            method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");

            method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());
            method.AddStatement("// Implement custom unique constraint validation here");
            method.AddStatement("return true;");
        });
    }

    public static void AddCheckUniqueConstraintsPlaceholders(
        this CSharpMethodChainStatement validationRuleChain,
        IFluentValidationTemplate template,
        IElement model)
    {
        var indexFields = GetUniqueConstraintFields(model);
        if (indexFields.Any(p => p.GroupCount > 1))
        {
            var indexGroups = indexFields.Where(p => p.GroupCount > 1).GroupBy(g => g.CompositeGroupName).ToArray();
            foreach (var indexGroup in indexGroups)
            {
                validationRuleChain.AddChainStatement($"MustAsync(CheckUniqueConstraint_{string.Join("_", indexGroup.Select(s => s.FieldName.ToPascalCase()))})", stmt => stmt.AddMetadata("requires-repository", true));
                validationRuleChain.AddChainStatement($@"WithMessage(""The combination of {string.Join(" and ", indexGroup.Select(s => s.FieldName))} already exists."")");
            }

        }

        var @class = template.CSharpFile.Classes.First();
        var toValidateTypeName = template.GetTypeName(template.ToValidateTemplateId, model);
        foreach (var indexGroup in indexFields.Where(p => p.GroupCount > 1).GroupBy(g => g.CompositeGroupName))
        {
            @class.AddMethod($"{template.UseType("System.Threading.Tasks.Task")}<bool>", $"CheckUniqueConstraint_{string.Join("_", indexGroup.Select(s => s.FieldName.ToPascalCase()))}", method =>
            {
                method.Private().Async();
                method.AddParameter(toValidateTypeName, "model");
                method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");

                method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());
                method.AddStatement("// Implement custom unique constraint validation here");
                method.AddStatement("return true;");
            });
        }
    }

    private static Dictionary<string, IReadOnlyCollection<ConstraintField>> _uniqueConstraintCache = new();
    private static IReadOnlyCollection<ConstraintField> GetUniqueConstraintFields(IElement dtoModel)
    {
        if ((!IsCreateDto(dtoModel) && !IsUpdateDto(dtoModel)))
        {
            return ArraySegment<ConstraintField>.Empty;
        }

        if (_uniqueConstraintCache.TryGetValue(dtoModel.Id, out var indexes))
        {
            return indexes;
        }

        var hasMappedClass = TryGetMappedClass(dtoModel, out var mappedClass) || TryGetAdvancedMappedClass(dtoModel, out mappedClass);
        if (!hasMappedClass)
        {
            return ArraySegment<ConstraintField>.Empty;
        }

        var indexStereotypeAttributes = mappedClass.ChildElements
            .Where(p => p.HasStereotype("Index") &&
                        p.GetStereotypeProperty("Index", "IsUnique", false))
            .Select(s => new ConstraintField(s.Name, s.GetStereotypeProperty("Index", "UniqueKey", string.Empty)))
            .ToArray();

        const string indexElementTypeId = "436e3afe-b4ef-481c-b803-0d1e7d263561";
        const string indexColumnTypeId = "c5ba925d-5c08-4809-a848-585a0cd4ddd3";

        var indexElements = mappedClass.ChildElements
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
                var mappedAttribute = indexColumnIndex.MappedElement?.Element;
                if (mappedAttribute?.SpecializationTypeId != DomainConstants.Attribute) // Domain Class Attribute
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

        indexes = indexStereotypeAttributes.Concat(indexElementAttributes).ToArray();

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

        _uniqueConstraintCache.Add(dtoModel.Id, indexes);

        return indexes;
    }


    private static bool TryGetMappedClass(IElement dtoModel, out IElement @class)
    {
        var mappedElement = dtoModel.MappedElement?.Element as IElement;
        // The loop is not needed on the service side where a Command/Query/DTO is mapped
        // to an Entity but it is needed when mapping from a Service Proxy to a Command/Query/Service Operation
        // and then to an Entity.
        while (mappedElement is not null)
        {
            if (mappedElement.SpecializationTypeId == DomainConstants.Class) // Domain Class
            {
                @class = mappedElement;
                return true;
            }

            if (mappedElement.MappedElement?.Element is null)
            {
                return TryGetAdvancedMappedClass(mappedElement, out @class);
            }
            mappedElement = mappedElement.MappedElement?.Element as IElement;
        }

        @class = default;
        return false;
    }

    private static bool TryGetAdvancedMappedClass(IElement dtoModel, out IElement @class)
    {
        var mappedEnd = dtoModel.ChildElements.FirstOrDefault(x => x.MappedToElements.FirstOrDefault(p => p.MappingType == "Data Mapping") != null)
            ?.MappedToElements.FirstOrDefault(p => p.MappingType == "Data Mapping");
        if (mappedEnd != null)
        {
            if (mappedEnd.TargetElement?.SpecializationTypeId == DomainConstants.Attribute) // Domain Class Attribute
            {
                var attribute = mappedEnd.TargetElement as IElement;
                @class = attribute?.ParentElement;
                return true;
            }

            if (TryGetMappedClass((IElement)mappedEnd.TargetElement, out @class))
            {
                return true;
            }
        }

        @class = null;
        return false;
    }

    private static bool IsCreateDto(IElement dtoModel)
    {
        return dtoModel.Name.StartsWith("create", StringComparison.InvariantCultureIgnoreCase) ||
               dtoModel.Name.StartsWith("add", StringComparison.InvariantCultureIgnoreCase) ||
               dtoModel.Name.StartsWith("new", StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool IsUpdateDto(IElement dtoModel)
    {
        return dtoModel.Name.StartsWith("update", StringComparison.InvariantCultureIgnoreCase) ||
               dtoModel.Name.StartsWith("edit", StringComparison.InvariantCultureIgnoreCase);
    }

    record ConstraintField(string FieldName, string CompositeGroupName)
    {
        public int GroupCount { get; set; }
    };


}