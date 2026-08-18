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
using Intent.Modules.Common.Types.Api;
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

        /// <summary>
        /// The source parameter name every generated mapping method uses.
        /// </summary>
        private const string ProjectFromPrefix = "projectFrom.";

        /// <summary>
        /// The AutoMapper source parameter name, recognised as a second authored Prefix Form and
        /// rewritten to <see cref="ProjectFromPrefix"/> so both forms converge on identical text.
        /// </summary>
        private const string SrcPrefix = "src.";

        internal static void AddInitializerEntries(
            CSharpObjectInitializerBlock initBlock,
            CSharpTemplateBase<DTOModel> template,
            DTOModel model)
        {
            var isLenient = IsLenientNullPathHandling(template);

            foreach (var field in model.Fields.Where(f => f.Mapping != null))
            {
                var expression = BuildEntryExpression(template, field, isLenient);
                initBlock.AddInitStatement(field.Name.ToPascalCase(), expression);
            }
        }

        /// <summary>
        /// Reads the module's "Null Path Handling" setting. IsLenient() is deliberately used as the
        /// positive test: it returns false for an absent or unrecognised value, which is exactly the
        /// required "absent means Strict" behaviour. AsEnum() and IsStrict() both throw
        /// ArgumentOutOfRangeException on an unset value, which would fail the Software Factory for
        /// any application that has not pinned the setting.
        /// </summary>
        private static bool IsLenientNullPathHandling(CSharpTemplateBase<DTOModel> template)
        {
            return Settings.ModuleSettingsExtensions
                .GetObjectMapping(template.ExecutionContext.Settings)
                .NullPathHandling()
                .IsLenient();
        }

        /// <summary>
        /// The inputs the per-hop null-handling matrix needs: the module setting, and whether the
        /// DTO field this expression is being built for is itself nullable.
        /// </summary>
        private readonly struct NullPathContext
        {
            public NullPathContext(bool isLenient, bool targetIsNullable)
            {
                IsLenient = isLenient;
                TargetIsNullable = targetIsNullable;
            }

            public bool IsLenient { get; }
            public bool TargetIsNullable { get; }
        }

        private static string BuildEntryExpression(CSharpTemplateBase<DTOModel> template, DTOFieldModel field, bool isLenient)
        {
            var pathTargets = field.Mapping.Path;
            var nullPath = new NullPathContext(isLenient, field.TypeReference.IsNullable);

            // Expression-type mapping (contains special chars like ?, (, !, etc.) — authored freehand
            // in the designer, in one of two recognised Prefix Forms.
            if (IsExpression(pathTargets))
            {
                return NormalizeExpression(string.Join(".", pathTargets.Select(p => p.Name)));
            }

            // FK extraction (single association end + primitive field type)
            if (pathTargets.Count == 1
                && pathTargets[0].Element.IsAssociationEndModel()
                && TargetsPrimitive(template, field.TypeReference))
            {
                var association = pathTargets[0].Element.AsAssociationEndModel().Association;
                return (association.SourceEnd.Multiplicity, association.TargetEnd.Multiplicity) switch
                {
                    (Multiplicity.ZeroToOne, Multiplicity.ZeroToOne) or
                    (Multiplicity.ZeroToOne, Multiplicity.One) or
                    (Multiplicity.One, Multiplicity.ZeroToOne) or
                    (Multiplicity.One, Multiplicity.One) =>
                    BuildPkExpression(pathTargets, nullPath),
                    (Multiplicity.ZeroToOne, Multiplicity.Many) or
                    (Multiplicity.One, Multiplicity.Many) =>
                    BuildMultiplePkExpression(pathTargets, field, nullPath),
                    (Multiplicity.Many, Multiplicity.ZeroToOne) or
                    (Multiplicity.Many, Multiplicity.One) =>
                    BuildLocalFkExpression(pathTargets),
                    (Multiplicity.Many, Multiplicity.Many) =>
                    BuildMultiplePkExpression(pathTargets, field, nullPath),
                    _ => throw new InvalidOperationException(
                        $"Unsupported association multiplicity: {association.SourceEnd.Multiplicity} -> {association.TargetEnd.Multiplicity}")
                };
            }

            // General path
            var (path, isLastNullable, needsFallback) = BuildPath(pathTargets, nullPath);

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
                    return $"projectFrom.{path}?.Select(x => x.{mapMethod}()).ToList(){CollectionFallback(field)}";
                }

                var (nestedSeparator, nestedFallback) = ResolveSeparator(isLastNullable, nullPath);
                return WithFallback($"projectFrom.{path}{nestedSeparator}{mapMethod}()", needsFallback || nestedFallback);
            }

            // Collection field with multi-hop path — navigation is a collection, trailing segment is a property
            // e.g. Tags → Name  →  projectFrom.Tags?.Select(x => x.Name).ToList()
            if (field.TypeReference.IsCollection && pathTargets.Count >= 2)
            {
                var collectionName = pathTargets[0].Name.ToPascalCase();

                // Hops inside the projection are relative to a single collection element, so the
                // element type — not the collection field — is what they map into. The field's own
                // nullability governs only the trailing `?? []`.
                var (innerPath, _, innerFallback) = BuildPath(
                    pathTargets.Skip(1).ToList(),
                    new NullPathContext(isLenient, targetIsNullable: false));

                var projection = WithFallback(innerPath, innerFallback);
                return $"projectFrom.{collectionName}?.Select(x => x.{projection}).ToList(){CollectionFallback(field)}";
            }

            // Enum cast (domain enum → DTO enum with a different type name)
            if (ShouldCast(template, field))
            {
                var castTo = template.GetTypeName(field.TypeReference);
                return needsFallback
                    ? $"({castTo})(projectFrom.{path} ?? default!)"
                    : $"({castTo})projectFrom.{path}";
            }

            return WithFallback($"projectFrom.{path}", needsFallback);
        }

        /// <summary>
        /// Normalises a freehand expression onto this module's "projectFrom." source parameter.
        /// Mirrors the AutoMapper module's algorithm, with "projectFrom." recognised as a second
        /// Prefix Form and with every occurrence of the recognised token rewritten rather than only
        /// the leading one — otherwise a multi-reference expression such as
        /// "src.Amount &gt; 0 ? src.Amount : 0" would not converge on the same text as its
        /// "projectFrom." equivalent.
        /// </summary>
        private static string NormalizeExpression(string expression)
        {
            var trimmed = expression.TrimStart();

            if (trimmed.StartsWith(SrcPrefix, StringComparison.Ordinal))
            {
                // Rewrite, do not prepend, so "src.X" and "projectFrom.X" produce identical text.
                return ReplacePrefixToken(PascalCasePropertyAccesses(trimmed), "src", "projectFrom");
            }

            if (trimmed.StartsWith(ProjectFromPrefix, StringComparison.Ordinal))
            {
                // Already authored in this module's Prefix Form — leave it alone.
                return PascalCasePropertyAccesses(trimmed);
            }

            // Anything else gets the source parameter prepended to the expression AS A WHOLE,
            // exactly as AutoMapper does — including the cases AutoMapper gets wrong. No design-time
            // diagnostic is raised; an unresolvable member access simply passes through unchanged.
            return ProjectFromPrefix + PascalCasePropertyAccesses(expression);
        }

        private static string ReplacePrefixToken(string expression, string from, string to)
        {
            return Regex.Replace(expression, $@"\b{Regex.Escape(from)}\.", $"{to}.");
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

        /// <summary>
        /// The per-hop null-handling matrix. A hop is the element being dereferenced; its own
        /// nullability, the target DTO field's nullability and the module setting decide the
        /// separator, and whether the whole initializer entry needs a trailing "?? default!".
        /// </summary>
        private static (string Separator, bool NeedsFallback) ResolveSeparator(bool hopIsNullable, NullPathContext nullPath)
        {
            // A non-nullable hop is never guarded, under either setting.
            if (!hopIsNullable) return (".", false);

            // A nullable target absorbs the null, so the setting has no effect.
            if (nullPath.TargetIsNullable) return ("?.", false);

            // Nullable hop into a non-nullable target — this is the only case the setting governs.
            // Neither branch fails the Software Factory; the designer's own validation is the
            // warning mechanism.
            return nullPath.IsLenient
                ? ("?.", true)
                : ("!.", false);
        }

        private static string WithFallback(string expression, bool needsFallback)
        {
            // "default!" is emitted uniformly, including on value-type and enum targets where the
            // null-forgiving operator is a legal no-op, so there is one fallback idiom rather than two.
            return needsFallback ? $"{expression} ?? default!" : expression;
        }

        /// <summary>
        /// A non-nullable collection field materializes to an empty list rather than null; a nullable
        /// one is left to yield null.
        /// </summary>
        private static string CollectionFallback(DTOFieldModel field)
        {
            return field.TypeReference.IsNullable ? string.Empty : " ?? []";
        }

        private static (string Path, bool IsLastNullable, bool NeedsFallback) BuildPath(
            IEnumerable<IElementMappingPathTarget> pathTargets,
            NullPathContext nullPath)
        {
            var filtered = pathTargets
                .Where(p => p.Specialization != "Generalization Target End"
                    && p.Specialization != GeneralizationModel.SpecializationType
                    && p.Specialization != "Data Contract Generalization Target End"
                    && p.Specialization != DataContractGeneralizationModel.SpecializationType)
                .ToList();

            if (!filtered.Any()) return ("", false, false);

            var sb = new StringBuilder();
            var needsFallback = false;
            for (int i = 0; i < filtered.Count; i++)
            {
                var target = filtered[i];
                var name = target.Name.ToPascalCase();
                var isOperation = target.Specialization == OperationModel.SpecializationType;

                if (i > 0)
                {
                    // The hop being crossed is the PREVIOUS element.
                    var hopIsNullable = filtered[i - 1].Element?.TypeReference?.IsNullable == true;
                    var (separator, fallback) = ResolveSeparator(hopIsNullable, nullPath);
                    sb.Append(separator);
                    needsFallback |= fallback;
                }

                sb.Append(name);
                if (isOperation) sb.Append("()");
            }

            var isLastNullable = filtered.Last().Element?.TypeReference?.IsNullable == true;
            return (sb.ToString(), isLastNullable, needsFallback);
        }

        private static string BuildPkExpression(IList<IElementMappingPathTarget> pathTargets, NullPathContext nullPath)
        {
            var (path, isLastNullable, needsFallback) = BuildPath(pathTargets, nullPath);
            var (separator, trailingFallback) = ResolveSeparator(isLastNullable, nullPath);
            return WithFallback($"projectFrom.{path}{separator}Id", needsFallback || trailingFallback);
        }

        /// <summary>
        /// True when the field's target type is a primitive. <c>GetTypeInfo</c> resolves a collection
        /// to the collection type itself (e.g. <c>List&lt;Guid&gt;</c>), which reports as non-primitive,
        /// so a multi-PK field such as <c>LineIds: List&lt;Guid&gt;</c> is recognised via its element type.
        /// Without this, such a field falls through to the general path and emits the raw navigation
        /// collection instead of a projection of its keys.
        /// </summary>
        private static bool TargetsPrimitive(CSharpTemplateBase<DTOModel> template, ITypeReference typeReference)
        {
            if (template.GetTypeInfo(typeReference).IsPrimitive)
            {
                return true;
            }

            return typeReference.IsCollection
                && typeReference.Element?.SpecializationTypeId == TypeDefinitionModel.SpecializationTypeId;
        }

        private static string BuildMultiplePkExpression(IList<IElementMappingPathTarget> pathTargets, DTOFieldModel field, NullPathContext nullPath)
        {
            var (path, _, _) = BuildPath(pathTargets, nullPath);
            return $"projectFrom.{path}?.Select(x => x.Id).ToList(){CollectionFallback(field)}";
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

        // Capitalises property-access identifiers inside free-form expressions, e.g. "a.id" → "a.Id".
        // Only touches identifiers that follow a '.' and start with a lowercase letter, leaving
        // lambda parameters (which appear before '=>') and already-cased names untouched.
        private static string PascalCasePropertyAccesses(string expression)
        {
            return Regex.Replace(expression, @"\.([a-z][a-zA-Z0-9_]*)", m =>
                "." + char.ToUpperInvariant(m.Groups[1].Value[0]) + m.Groups[1].Value.Substring(1));
        }
    }
}
