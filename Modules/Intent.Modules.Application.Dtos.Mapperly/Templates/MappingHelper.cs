using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Application.Dtos.Mapperly.Templates.DtoMappingProfile;
using Intent.Utils;
using DataContractGeneralizationModel = Intent.Modelers.Domain.Api.DataContractGeneralizationModel;
using GeneralizationModel = Intent.Modelers.Domain.Api.GeneralizationModel;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;

namespace Intent.Modules.Application.Dtos.Mapperly.Templates;

internal static class MappingHelper
{

    internal static ICSharpFileBuilderTemplate GetEntityTemplate(IntentTemplateBase template, DTOModel templateModel)
    {
        if (template.TryGetTemplate(TemplateRoles.Domain.Entity.Primary, templateModel.Mapping.ElementId, out ICSharpFileBuilderTemplate entityTemplate) ||
            template.TryGetTemplate(TemplateRoles.Domain.ValueObject, templateModel.Mapping.ElementId, out entityTemplate) ||
            template.TryGetTemplate(TemplateRoles.Domain.DataContract, templateModel.Mapping.ElementId, out entityTemplate))
        {
            return entityTemplate;
        }

        throw new InvalidOperationException($"Could not resolve mapped type '{templateModel.Mapping.Element.Name}' for DTO '{templateModel.Name}'.");
    }

    // //If your persistence layer has different persistence models from the domain models
    // //you need them registered with AutoMapper for OData to work.
    // internal static bool RequiresPersistenceMappings(ISoftwareFactoryExecutionContext application, IntentTemplateBase template, IElement? mappedElement, out string persistenceContractName)
    // {
    //     persistenceContractName = null;
    //
    //     if (mappedElement == null) { return false; }
    //
    //     if (!(application.InstalledModules.Any(m => m.ModuleId == "Intent.CosmosDB") && application.InstalledModules.Any(m => m.ModuleId == "Intent.AspNetCore.ODataQuery")))
    //     {
    //         return false;
    //     }
    //
    //     if (!IsCosmosModel(mappedElement))
    //     {
    //         return false;
    //     }
    //
    //     if (!application.TemplateExists("Intent.CosmosDB.CosmosDBDocumentInterface", mappedElement.Id))
    //     {
    //         return false;
    //     }
    //
    //     var docInterfaceTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.CosmosDB.CosmosDBDocumentInterface", mappedElement.Id);
    //     persistenceContractName = template.GetTypeName(docInterfaceTemplate);
    //     return true;
    // }

    internal static (string Expression, string AttributePath) GetMappingExpression(ICSharpFileBuilderTemplate template, DTOFieldModel field)
    {
        var pathTargets = field.Mapping.Path;

        if (pathTargets.Count != 1
            || !pathTargets.First().Element.IsAssociationEndModel()
            || !template.GetTypeInfo(field.TypeReference).IsPrimitive)
        {
            var expressionPath = GetPathForExpression(pathTargets, field.TypeReference.IsNullable);
            var attributePath = GetPathForAttribute(pathTargets);

            return ($"src.{expressionPath}", attributePath);
        }

        var association = pathTargets.First().Element.AsAssociationEndModel().Association;
        return (association.SourceEnd.Multiplicity, association.TargetEnd.Multiplicity) switch
        {
            (Multiplicity.ZeroToOne, Multiplicity.ZeroToOne) => GetPk(pathTargets),
            (Multiplicity.ZeroToOne, Multiplicity.One) => GetPk(pathTargets),
            (Multiplicity.ZeroToOne, Multiplicity.Many) => GetMultiplePk(template, pathTargets),
            (Multiplicity.One, Multiplicity.ZeroToOne) => GetPk(pathTargets),
            (Multiplicity.One, Multiplicity.One) => GetPk(pathTargets),
            (Multiplicity.One, Multiplicity.Many) => GetMultiplePk(template, pathTargets),
            (Multiplicity.Many, Multiplicity.ZeroToOne) => GetLocalFk(pathTargets),
            (Multiplicity.Many, Multiplicity.One) => GetLocalFk(pathTargets),
            (Multiplicity.Many, Multiplicity.Many) => GetMultiplePk(template, pathTargets),
            _ => throw new InvalidOperationException($"Problem resolving association {association.SourceEnd.Multiplicity} -> {association.TargetEnd.Multiplicity}")
        };
    }

    // private static bool IsCosmosModel(IElement? mappedElement)
    // {
    //     if (!mappedElement.Package.HasStereotype("Document Database"))
    //         return false;
    //     var setting = mappedElement.Package.GetStereotypeProperty<IElement>("Document Database", "Provider");
    //     if (setting != null && setting.Id != "3e1a00f7-c6f1-4785-a544-bbcb17602b31")//CosmosDB Provider)
    //     {
    //         return false;
    //     }
    //     return true;
    // }

    private static (string Expression, string AttributePath) GetPk(IList<IElementMappingPathTarget> pathTargets)
    {
        var expressionPath = GetPathForExpression(pathTargets, isTargetNullable: false);
        var attributePath = GetPathForAttribute(pathTargets);

        return ($"src.{expressionPath}.Id", $"{attributePath}.Id");
    }

    private static (string Expression, string AttributePath) GetMultiplePk(ICSharpFileBuilderTemplate template, IList<IElementMappingPathTarget> pathTargets)
    {
        template.AddUsing("System.Linq");

        var expressionPath = GetPathForExpression(pathTargets, isTargetNullable: false);
        var attributePath = GetPathForAttribute(pathTargets);

        return ($"src.{expressionPath}.Select(x => x.Id).ToArray()", attributePath);
    }

    private static (string Expression, string AttributePath) GetLocalFk(IList<IElementMappingPathTarget> pathTargets)
    {
        var association = pathTargets.First().Element.AsAssociationEndModel();
        var fkName = $"{association.Name.ToPascalCase()}Id";

        return ($"src.{fkName}", fkName);
    }

    /// <summary>
    /// Returns a clean path without any nullability operators, suitable for use in nameof() expressions
    /// or MapProperty attribute parameters. Example: "Preferences.Newsletter"
    /// </summary>
    private static string GetPathForAttribute(IEnumerable<IElementMappingPathTarget> pathTargets)
    {
        var path = string.Join(".", pathTargets
            .Where(pathTarget => pathTarget.Specialization != "Generalization Target End" &&
                                 pathTarget.Specialization != GeneralizationModel.SpecializationType &&
                                 pathTarget.Specialization != "Data Contract Generalization Target End" &&
                                 pathTarget.Specialization != DataContractGeneralizationModel.SpecializationType)
            .Select(pathTarget =>
            {
                var operationCall = pathTarget.Specialization == OperationModel.SpecializationType ? "()" : string.Empty;
                return $"{pathTarget.Name}{operationCall}";
            }));

        return path;
    }

    /// <summary>
    /// Returns a path with proper nullability operators for runtime expressions.
    /// Uses '?' for nullable navigation when target is nullable (null-conditional operator).
    /// Uses '!' for nullable navigation when target is non-nullable (null-forgiving operator).
    /// Example: "Preferences?.Newsletter" or "Preferences!.Newsletter"
    /// </summary>
    private static string GetPathForExpression(IEnumerable<IElementMappingPathTarget> pathTargets, bool isTargetNullable)
    {
        var targets = pathTargets
            .Where(pathTarget => pathTarget.Specialization != "Generalization Target End" &&
                                 pathTarget.Specialization != GeneralizationModel.SpecializationType &&
                                 pathTarget.Specialization != "Data Contract Generalization Target End" &&
                                 pathTarget.Specialization != DataContractGeneralizationModel.SpecializationType)
            .ToList();

        var parts = new List<string>();
        
        for (int i = 0; i < targets.Count; i++)
        {
            var pathTarget = targets[i];
            var operationCall = pathTarget.Specialization == OperationModel.SpecializationType ? "()" : string.Empty;
            var isNullable = pathTarget.Element?.TypeReference.IsNullable == true;
            var isLastInChain = i == targets.Count - 1;

            string nullabilityOperator;
            if (isNullable && !isLastInChain)
            {
                // For intermediate nullable navigation properties:
                // Use '?' if target is nullable (allows null propagation)
                // Use '!' if target is non-nullable (asserts non-null)
                nullabilityOperator = isTargetNullable ? "?" : "!";
            }
            else
            {
                nullabilityOperator = string.Empty;
            }

            parts.Add($"{pathTarget.Name}{operationCall}{nullabilityOperator}");
        }

        return string.Join(".", parts);
    }

    /// <summary>
    /// Discovers all mapper dependencies for a given DTO by inspecting its fields.
    /// Returns a list of mapper templates that this DTO's mapper needs to depend on.
    /// </summary>
    internal static List<ICSharpFileBuilderTemplate> DiscoverMapperDependencies(
        IntentTemplateBase template,
        DTOModel dtoModel)
    {
        var dependencies = new List<ICSharpFileBuilderTemplate>();
        var visitedDtoIds = new HashSet<string>();

        DiscoverRecursive(template, dtoModel, dependencies, visitedDtoIds);

        return dependencies;
    }

    private static void DiscoverRecursive(
        IntentTemplateBase template,
        DTOModel dtoModel,
        List<ICSharpFileBuilderTemplate> dependencies,
        HashSet<string> visitedDtoIds)
    {
        // Prevent circular references
        if (!visitedDtoIds.Add(dtoModel.Id))
        {
            return;
        }

        // Iterate through all fields that have mappings
        foreach (var field in dtoModel.Fields.Where(f => f.Mapping != null))
        {
            // Get the target element ID (handles both direct types and collection element types)
            var targetElementId = field.TypeReference.Element?.Id;
            if (string.IsNullOrEmpty(targetElementId))
            {
                continue;
            }

            // Check if this field's type is a DTO
            if (!template.TryGetTemplate(
                    TemplateRoles.Application.Contracts.Dto,
                    targetElementId,
                    out ICSharpFileBuilderTemplate nestedDtoTemplate))
            {
                continue;
            }
            
            // Get the DTOModel from the DTO template - this is the key!
            var nestedDtoAsTemplateWithModel = nestedDtoTemplate as ITemplateWithModel;

            if (nestedDtoAsTemplateWithModel?.Model is not DTOModel nestedDtoModel)
            {
                continue;
            }

            // Found a nested DTO - now find its mapper template using the DTOModel ID
            var mapperLookupResult = template.TryGetTemplate(
                DtoMappingProfileTemplate.TemplateId,
                nestedDtoModel.Id,
                out ICSharpFileBuilderTemplate mapperTemplate);

            if (!mapperLookupResult)
            {
                continue;
            }
            
            // Add to dependencies if not already present
            if (dependencies.All(d => d.ClassName != mapperTemplate.ClassName))
            {
                dependencies.Add(mapperTemplate);
            }
        }
    }

    /// <summary>
    /// Identifies source entity properties that are not mapped to any DTO property.
    /// These unmapped properties can cause Mapperly RMG020 warnings and should be suppressed
    /// using [MapperIgnoreSource] if they are intentionally excluded from the DTO.
    /// </summary>
    internal static List<string> GetUnmappedSourceProperties(
        ICSharpFileBuilderTemplate template,
        DTOModel dtoModel)
    {
        var unmapped = new List<string>();

        // Get the entity element
        var entityElement = dtoModel.Mapping?.Element;
        if (entityElement == null)
        {
            return unmapped;
        }

        // Convert to ClassModel to access all attributes
        var entityClass = entityElement.AsClassModel();
        if (entityClass == null)
        {
            return unmapped;
        }

        // Collect all DTO field's mapped source property paths
        var mappedSourcePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var field in dtoModel.Fields.Where(f => f.Mapping != null))
        {
            // Get the mapping path (e.g., "Product.Name" or "OrderId")
            var mappingPath = string.Join(".", field.Mapping.Path
                .Select(p => p.Name.ToPascalCase()));

            mappedSourcePaths.Add(mappingPath);

            // Also add variations for nested paths (e.g., "Product" from "Product.Name")
            if (!mappingPath.Contains("."))
            {
                continue;
            }
            
            var parts = mappingPath.Split('.');
            for (var i = 0; i < parts.Length - 1; i++)
            {
                mappedSourcePaths.Add(string.Join(".", parts.Take(i + 1)));
            }
        }

        // Check each entity attribute to see if it's mapped
        foreach (var entityAttr in entityClass.Attributes)
        {
            var attrName = entityAttr.Name.ToPascalCase();

            // Check if this attribute is explicitly mapped
            if (mappedSourcePaths.Contains(attrName))
            {
                continue;
            }

            // This property is not mapped - add it to unmapped list
            unmapped.Add(attrName);
        }

        return unmapped;
    }
}