using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.ObjectMapping.Templates.MappingExtensions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modelers.Domain.Api;
using DataContractGeneralizationModel = Intent.Modelers.Domain.Api.DataContractGeneralizationModel;
using GeneralizationModel = Intent.Modelers.Domain.Api.GeneralizationModel;
using Multiplicity = Intent.Modelers.Domain.Api.Multiplicity;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;

namespace Intent.Modules.Application.Dtos.ObjectMapping.Templates
{
    internal static class MappingHelper
    {
        private const string EnumSpecializationId = "85fba0e9-9161-4c85-a603-a229ef312beb";

        internal static void AddInitializerEntries(
            CSharpObjectInitializerBlock initBlock,
            CSharpTemplateBase<DTOModel> template,
            DTOModel model)
        {
            foreach (var field in model.Fields.Where(f => f.Mapping != null))
            {
                var expression = BuildEntryExpression(template, field);
                initBlock.AddInitStatement(field.Name.ToPascalCase(), expression);
            }
        }

        private static string BuildEntryExpression(CSharpTemplateBase<DTOModel> template, DTOFieldModel field)
        {
            var pathTargets = field.Mapping.Path;

            // Expression-type mapping (contains special chars like ?, (, !, etc.) — authored directly
            // against this module's own "projectFrom" parameter name (e.g. "projectFrom.X ? ... : ...").
            // There is no other source-variable convention to translate from; emit as typed, PascalCased.
            if (IsExpression(pathTargets))
            {
                return PascalCasePropertyAccesses(string.Join(".", pathTargets.Select(p => p.Name)));
            }

            // FK extraction (single association end + primitive field type)
            if (pathTargets.Count == 1
                && pathTargets[0].Element.IsAssociationEndModel()
                && template.GetTypeInfo(field.TypeReference).IsPrimitive)
            {
                var association = pathTargets[0].Element.AsAssociationEndModel().Association;
                return (association.SourceEnd.Multiplicity, association.TargetEnd.Multiplicity) switch
                {
                    (Multiplicity.ZeroToOne, Multiplicity.ZeroToOne) or
                    (Multiplicity.ZeroToOne, Multiplicity.One) or
                    (Multiplicity.One, Multiplicity.ZeroToOne) or
                    (Multiplicity.One, Multiplicity.One) =>
                        BuildPkExpression(pathTargets),
                    (Multiplicity.ZeroToOne, Multiplicity.Many) or
                    (Multiplicity.One, Multiplicity.Many) =>
                        BuildMultiplePkExpression(pathTargets),
                    (Multiplicity.Many, Multiplicity.ZeroToOne) or
                    (Multiplicity.Many, Multiplicity.One) =>
                        BuildLocalFkExpression(pathTargets),
                    (Multiplicity.Many, Multiplicity.Many) =>
                        BuildMultiplePkExpression(pathTargets),
                    _ => throw new InvalidOperationException(
                        $"Unsupported association multiplicity: {association.SourceEnd.Multiplicity} -> {association.TargetEnd.Multiplicity}")
                };
            }

            // General path
            var (path, isLastNullable) = BuildPath(pathTargets);

            // Nested DTO — field type has its own MappingExtensions template
            var nestedElementId = field.TypeReference.Element?.Id;
            if (nestedElementId != null
                && field.TypeReference.Element?.SpecializationTypeId != EnumSpecializationId
                && template.TryGetTemplate<ICSharpFileBuilderTemplate>(MappingExtensionsTemplate.TemplateId, nestedElementId, out _))
            {
                // Trigger using registration for the extension class namespace
                template.GetTypeName(MappingExtensionsTemplate.TemplateId, nestedElementId);
                var mapMethod = $"MapTo{field.TypeReference.Element!.Name}";

                if (field.TypeReference.IsCollection)
                {
                    return $"projectFrom.{path}?.Select(x => x.{mapMethod}()).ToList() ?? []";
                }

                var nullConditional = isLastNullable ? "?" : "";
                return $"projectFrom.{path}{nullConditional}.{mapMethod}()";
            }

            // Collection field with multi-hop path — navigation is a collection, trailing segment is a property
            // e.g. Tags → Id  →  projectFrom.Tags?.Select(x => x.Id).ToList() ?? []
            if (field.TypeReference.IsCollection && pathTargets.Count >= 2)
            {
                var collectionName = pathTargets[0].Name.ToPascalCase();
                var (innerPath, _) = BuildPath(pathTargets.Skip(1).ToList());
                return $"projectFrom.{collectionName}?.Select(x => x.{innerPath}).ToList() ?? []";
            }

            // Enum cast (domain enum → DTO enum with a different type name)
            if (ShouldCast(template, field))
            {
                return $"({template.GetTypeName(field.TypeReference)})projectFrom.{path}";
            }

            return $"projectFrom.{path}";
        }

        private static bool ShouldCast(CSharpTemplateBase<DTOModel> template, DTOFieldModel field)
        {
            // Only enum→enum mismatches require an explicit cast; built-in primitives and complex types do not.
            var dtoTypeElement = field.TypeReference.Element;
            if (dtoTypeElement?.SpecializationTypeId != EnumSpecializationId) return false;

            var lastTarget = field.Mapping?.Path?.LastOrDefault();
            if (lastTarget == null) return false;
            var domainEnumId = lastTarget.Element?.TypeReference?.Element?.Id;
            return domainEnumId != null && dtoTypeElement.Id != domainEnumId;
        }

        private static (string Path, bool IsLastNullable) BuildPath(IEnumerable<IElementMappingPathTarget> pathTargets)
        {
            var filtered = pathTargets
                .Where(p => p.Specialization != "Generalization Target End"
                         && p.Specialization != GeneralizationModel.SpecializationType
                         && p.Specialization != "Data Contract Generalization Target End"
                         && p.Specialization != DataContractGeneralizationModel.SpecializationType)
                .ToList();

            if (!filtered.Any()) return ("", false);

            var sb = new StringBuilder();
            for (int i = 0; i < filtered.Count; i++)
            {
                var target = filtered[i];
                var name = target.Name.ToPascalCase();
                var isOperation = target.Specialization == OperationModel.SpecializationType;

                if (i > 0)
                {
                    // null-conditional is based on the PREVIOUS element's nullability
                    var prevIsNullable = filtered[i - 1].Element?.TypeReference?.IsNullable == true;
                    sb.Append(prevIsNullable ? "?." : ".");
                }

                sb.Append(name);
                if (isOperation) sb.Append("()");
            }

            var isLastNullable = filtered.Last().Element?.TypeReference?.IsNullable == true;
            return (sb.ToString(), isLastNullable);
        }

        private static string BuildPkExpression(IList<IElementMappingPathTarget> pathTargets)
        {
            var (path, isLastNullable) = BuildPath(pathTargets);
            var nullConditional = isLastNullable ? "?" : "";
            return $"projectFrom.{path}{nullConditional}.Id";
        }

        private static string BuildMultiplePkExpression(IList<IElementMappingPathTarget> pathTargets)
        {
            var (path, _) = BuildPath(pathTargets);
            return $"projectFrom.{path}?.Select(x => x.Id).ToList() ?? []";
        }

        private static string BuildLocalFkExpression(IList<IElementMappingPathTarget> pathTargets)
        {
            return $"projectFrom.{pathTargets[0].Name.ToPascalCase()}Id";
        }

        private static bool IsExpression(IList<IElementMappingPathTarget> pathTargets)
        {
            char[] specialChars = { '?', '(', '+', '{', '@', '!', '-', '=' };
            var fullPath = string.Join(".", pathTargets.Select(p => p.Name));
            return fullPath.Any(c => specialChars.Contains(c));
        }

        private static string PascalCasePropertyAccesses(string expression)
        {
            return Regex.Replace(expression, @"\.([a-z][a-zA-Z0-9_]*)", m =>
                "." + char.ToUpperInvariant(m.Groups[1].Value[0]) + m.Groups[1].Value.Substring(1));
        }
    }
}
