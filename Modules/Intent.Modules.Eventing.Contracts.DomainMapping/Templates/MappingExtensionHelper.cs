using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using GeneralizationModel = Intent.Modelers.Domain.Api.GeneralizationModel;

namespace Intent.Modules.Eventing.Contracts.DomainMapping.Templates;

internal static class MappingExtensionHelper
{
    public static IEnumerable<CSharpStatement> GetPropertyAssignments<TModel>(
        string domainEntityVar,
        IEnumerable<IElement> fromAttributes,
        IEnumerable<IElement> properties,
        CSharpTemplateBase<TModel> template)
    {
        var codeLines = new List<CSharpStatement>();
        foreach (var property in properties)
        {
            var mappedPropertyElement = property.MappedElement;

            if (mappedPropertyElement?.Element == null
                && fromAttributes.All(p => p.Name != property.Name))
            {
                codeLines.Add($"#warning No matching property found for {property.Name}");
                continue;
            }

            var sourcePath = GetPath(mappedPropertyElement?.Path ?? Enumerable.Empty<IElementMappingPathTarget>());
            if (!string.IsNullOrWhiteSpace(domainEntityVar))
            {
                sourcePath = $"{domainEntityVar}.{sourcePath}";
            }

            switch (mappedPropertyElement?.Element?.SpecializationTypeId)
            {
                case null:
                case AttributeModel.SpecializationTypeId:
                    if (property.TypeReference.Element.Name == "string" && mappedPropertyElement?.Element?.TypeReference.Element.Name != "string")
                    {
                        codeLines.Add($"{property.Name.ToPascalCase()} = {sourcePath}.ToString(),");
                    }
                    else
                    {
                        codeLines.Add($"{property.Name.ToPascalCase()} = {sourcePath},");
                    }
                    break;
                case AssociationTargetEndModel.SpecializationTypeId:
                    {
                        var association = mappedPropertyElement.Element.AsAssociationTargetEndModel();

                        if (association.Association.AssociationType == AssociationType.Aggregation)
                        {
                            codeLines.Add($@"#warning Field not a composite association: {property.Name.ToPascalCase()}");
                            break;
                        }

                        if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
                        {
                            codeLines.Add(association.IsNullable
                                ? $"{property.Name.ToPascalCase()} = {sourcePath} != null ? {GetMapToMethodName(property, template)}({sourcePath}) : null,"
                                : $"{property.Name.ToPascalCase()} = {GetMapToMethodName(property, template)}({sourcePath}),");
                        }
                        else
                        {
                            template.AddUsing("System.Linq");
                            codeLines.Add(
                                $"{property.Name.ToPascalCase()} = {sourcePath}{(association.IsNullable ? "?" : "")}.Select({GetMapToMethodName(property, template)}).ToList(),");
                        }
                    }
                    break;
                default:
                    var mappedPropertyName = mappedPropertyElement.Element?.Name ?? "<null>";
                    codeLines.Add($"#warning No matching type for Domain: {mappedPropertyName} and Type: {property.Name}");
                    break;
            }
        }

        return codeLines;
    }

    private static string GetPath(IEnumerable<IElementMappingPathTarget> path)
    {
        return string.Join(".", path
            .Where(x => x.Specialization != GeneralizationModel.SpecializationType)
            .Select(x =>
            {
                // Can't just .ToPascalCase(), since it turns string like "Count(x => x.IsAssigned())" into "Count(x => X.IsAssigned())"
                var name = !string.IsNullOrWhiteSpace(x.Name)
                    ? char.ToUpperInvariant(x.Name[0]) + x.Name[1..]
                    : x.Name;

                return x.Specialization == OperationModel.SpecializationType
                    ? $"{name}()"
                    : name;
            }));
    }

    private static string GetMapToMethodName(IElement element, IntentTemplateBase template)
    {
        var eventingDtoModel = element.TypeReference.Element.AsEventingDTOModel();

        return $"{template.GetDtoExtensionsName(eventingDtoModel)}.MapTo{template.GetIntegrationEventDtoName(eventingDtoModel)}";
    }
}