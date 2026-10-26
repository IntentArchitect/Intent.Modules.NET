using System;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Services.Guardrails.Contracts.Api;

namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// The collection type a "Collection Type" stereotype selects. Member names mirror the
    /// stereotype's Type options, which is how the generated per-model option enums are mapped.
    /// </summary>
    public enum ContractCollectionKind
    {
        IEnumerable,
        ICollection,
        IList,
        IReadOnlyCollection,
        IReadOnlyList,
        List,
        HashSet,
        ISet,
        IQueryable,
        Array,
        Custom
    }

    /// <summary>
    /// Type names for DTO fields, operation parameters and operation return types, honouring the
    /// "Collection Type" stereotype. With no stereotype (or an empty Type), or when the type
    /// reference is not a collection, the template's default collection format applies unchanged.
    /// </summary>
    public static class ContractCollectionTypes
    {
        private const string ElementPlaceholder = "{0}";

        public static string GetContractTypeName(this ICSharpTemplate template, DTOFieldModel field)
        {
            var collectionType = field.GetCollectionType();
            var type = collectionType?.Type();
            var kind = type == null || string.IsNullOrWhiteSpace(type.Value) ? (ContractCollectionKind?)null : ToKind(type.AsEnum());
            return template.GetContractTypeName(field.TypeReference, field.InternalElement, kind, collectionType?.CustomFormat());
        }

        public static string GetContractTypeName(this ICSharpTemplate template, ParameterModel parameter)
        {
            var collectionType = parameter.GetCollectionType();
            var type = collectionType?.Type();
            var kind = type == null || string.IsNullOrWhiteSpace(type.Value) ? (ContractCollectionKind?)null : ToKind(type.AsEnum());
            return template.GetContractTypeName(parameter.TypeReference, parameter.InternalElement, kind, collectionType?.CustomFormat());
        }

        /// <summary>
        /// The operation's return type, before any async Task wrapping.
        /// </summary>
        public static string GetContractTypeName(this ICSharpTemplate template, OperationModel operation)
        {
            var collectionType = operation.GetCollectionType();
            var type = collectionType?.Type();
            var kind = type == null || string.IsNullOrWhiteSpace(type.Value) ? (ContractCollectionKind?)null : ToKind(type.AsEnum());
            return template.GetContractTypeName(operation.TypeReference, operation.InternalElement, kind, collectionType?.CustomFormat());
        }

        /// <summary>
        /// The single resolution point. The format is passed to <c>GetTypeName(typeReference, format)</c>
        /// fully qualified: Intent.Modules.Common.CSharp's <c>CSharpTypeResolver</c> parses it with
        /// <see cref="CSharpCollectionFormatter.Create(string)"/> into a namespace and a type name, and
        /// <c>UseType</c> then adds the <c>using</c> and emits the short name (or qualifies it on a clash).
        /// </summary>
        public static string GetContractTypeName(this ICSharpTemplate template, ITypeReference typeReference, IElement element, ContractCollectionKind? kind, string customFormat)
        {
            if (kind == null || typeReference?.IsCollection != true)
            {
                return template.GetTypeName(typeReference);
            }

            return template.GetTypeName(typeReference, GetCollectionFormat(kind.Value, customFormat, element));
        }

        /// <summary>
        /// The collection format for a kind. <c>{0}</c> stands for the element type.
        /// </summary>
        public static string GetCollectionFormat(ContractCollectionKind kind, string customFormat, IElement element)
        {
            return kind switch
            {
                ContractCollectionKind.IEnumerable => "System.Collections.Generic.IEnumerable<{0}>",
                ContractCollectionKind.ICollection => "System.Collections.Generic.ICollection<{0}>",
                ContractCollectionKind.IList => "System.Collections.Generic.IList<{0}>",
                ContractCollectionKind.IReadOnlyCollection => "System.Collections.Generic.IReadOnlyCollection<{0}>",
                ContractCollectionKind.IReadOnlyList => "System.Collections.Generic.IReadOnlyList<{0}>",
                ContractCollectionKind.List => "System.Collections.Generic.List<{0}>",
                ContractCollectionKind.HashSet => "System.Collections.Generic.HashSet<{0}>",
                ContractCollectionKind.ISet => "System.Collections.Generic.ISet<{0}>",
                ContractCollectionKind.IQueryable => "System.Linq.IQueryable<{0}>",
                ContractCollectionKind.Array => "{0}[]",
                ContractCollectionKind.Custom => ValidateCustomFormat(customFormat, element),
                _ => throw new InvalidOperationException($"Unsupported collection kind: {kind}")
            };
        }

        private static string ValidateCustomFormat(string customFormat, IElement element)
        {
            if (string.IsNullOrWhiteSpace(customFormat))
            {
                throw new ElementException(element,
                    $"{element.SpecializationType} '{element.Name}' has its Collection Type set to Custom but no Custom Format. " +
                    "Set Custom Format to the collection type with {0} in place of the element type, for example " +
                    "System.Collections.ObjectModel.ObservableCollection<{0}>, or choose one of the other collection types.");
            }

            var format = customFormat.Trim();
            if (!RendersElementType(format))
            {
                throw new ElementException(element,
                    $"{element.SpecializationType} '{element.Name}' has a Collection Type Custom Format of '{customFormat}', which is not a valid collection type. " +
                    "The format must be a C# type that uses {0} in place of the element type, with matching angle and square brackets, for example " +
                    "System.Collections.ObjectModel.ObservableCollection<{0}> or {0}[].");
            }

            return format;
        }

        /// <summary>
        /// The format must contain {0}, parse as a C# collection type, and actually render the element
        /// type when applied (so "Foo{0}" or unbalanced brackets are rejected).
        /// </summary>
        private static bool RendersElementType(string format)
        {
            if (!format.Contains(ElementPlaceholder, StringComparison.Ordinal))
            {
                return false;
            }

            const string probeName = "GuardrailsElementTypeProbe";
            try
            {
                var rendered = CSharpCollectionFormatter.Create(format)
                    .ApplyTo(CSharpResolvedTypeInfo.Create(probeName, string.Empty))
                    .GetFullyQualifiedTypeName();
                return rendered.Contains(probeName, StringComparison.Ordinal) &&
                       !rendered.Contains(ElementPlaceholder, StringComparison.Ordinal);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static ContractCollectionKind ToKind<TEnum>(TEnum option) where TEnum : struct, Enum
        {
            if (!Enum.TryParse(option.ToString(), ignoreCase: false, out ContractCollectionKind kind))
            {
                throw new InvalidOperationException(
                    $"Collection Type option '{option}' has no mapping in {nameof(ContractCollectionKind)}. Add it there and to {nameof(GetCollectionFormat)}.");
            }

            return kind;
        }
    }
}
