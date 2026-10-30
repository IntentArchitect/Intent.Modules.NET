using System;
using System.Collections.Generic;
using Intent.Domain.Guardrails.Contracts.Api;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.TypeResolvers;
using CommonTypesOperationModel = Intent.Modules.Common.Types.Api.OperationModel;
using CommonTypesParameterModel = Intent.Modules.Common.Types.Api.ParameterModel;

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    /// <summary>The options of the "Collection Type" stereotype.</summary>
    internal enum CollectionTypeOption
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
    /// Reads the "Collection Type" stereotype, which records the collection type the code really uses where it
    /// differs from the template default. Each method returns a collection format for
    /// <c>GetTypeName(typeReference, format)</c>, or null to keep the default: no stereotype, an empty Type, or a
    /// type reference that isn't a collection (the stereotype is then ignored).
    /// </summary>
    internal static class ContractCollectionTypes
    {
        public static string GetCollectionFormat(this AttributeModel model)
        {
            if (model.TypeReference?.IsCollection != true || !model.TryGetCollectionType(out var collectionType))
            {
                return null;
            }

            var type = collectionType.Type();
            return FormatFor(model.InternalElement, collectionType.CustomFormat(), Select(
                isIEnumerable: type.IsIEnumerable(), isICollection: type.IsICollection(), isIList: type.IsIList(),
                isIReadOnlyCollection: type.IsIReadOnlyCollection(), isIReadOnlyList: type.IsIReadOnlyList(), isList: type.IsList(),
                isHashSet: type.IsHashSet(), isISet: type.IsISet(), isIQueryable: type.IsIQueryable(), isArray: type.IsArray(), isCustom: type.IsCustom()));
        }

        /// <summary>
        /// Navigation properties: <see cref="ClassModel.AssociatedClasses"/> holds the concrete target and source end models.
        /// </summary>
        public static string GetCollectionFormat(this AssociationEndModel associationEnd)
        {
            if (!associationEnd.IsCollection)
            {
                return null;
            }

            switch (associationEnd)
            {
                case AssociationTargetEndModel targetEnd when targetEnd.TryGetCollectionType(out var collectionType):
                {
                    var type = collectionType.Type();
                    return FormatFor(targetEnd.InternalElement, collectionType.CustomFormat(), Select(
                        isIEnumerable: type.IsIEnumerable(), isICollection: type.IsICollection(), isIList: type.IsIList(),
                        isIReadOnlyCollection: type.IsIReadOnlyCollection(), isIReadOnlyList: type.IsIReadOnlyList(), isList: type.IsList(),
                        isHashSet: type.IsHashSet(), isISet: type.IsISet(), isIQueryable: type.IsIQueryable(), isArray: type.IsArray(), isCustom: type.IsCustom()));
                }
                case AssociationSourceEndModel sourceEnd when sourceEnd.TryGetCollectionType(out var collectionType):
                {
                    var type = collectionType.Type();
                    return FormatFor(sourceEnd.InternalElement, collectionType.CustomFormat(), Select(
                        isIEnumerable: type.IsIEnumerable(), isICollection: type.IsICollection(), isIList: type.IsIList(),
                        isIReadOnlyCollection: type.IsIReadOnlyCollection(), isIReadOnlyList: type.IsIReadOnlyList(), isList: type.IsList(),
                        isHashSet: type.IsHashSet(), isISet: type.IsISet(), isIQueryable: type.IsIQueryable(), isArray: type.IsArray(), isCustom: type.IsCustom()));
                }
                default:
                    return null;
            }
        }

        /// <summary>
        /// Constructor and operation parameters. The accessor is generated against the Common.Types parameter model;
        /// both wrap the same designer element.
        /// </summary>
        public static string GetCollectionFormat(this ParameterModel parameter)
        {
            if (parameter.TypeReference?.IsCollection != true ||
                !new CommonTypesParameterModel(parameter.InternalElement).TryGetCollectionType(out var collectionType))
            {
                return null;
            }

            var type = collectionType.Type();
            return FormatFor(parameter.InternalElement, collectionType.CustomFormat(), Select(
                isIEnumerable: type.IsIEnumerable(), isICollection: type.IsICollection(), isIList: type.IsIList(),
                isIReadOnlyCollection: type.IsIReadOnlyCollection(), isIReadOnlyList: type.IsIReadOnlyList(), isList: type.IsList(),
                isHashSet: type.IsHashSet(), isISet: type.IsISet(), isIQueryable: type.IsIQueryable(), isArray: type.IsArray(), isCustom: type.IsCustom()));
        }

        /// <summary>
        /// An operation's return type. The accessor is generated against the Common.Types operation model; both wrap
        /// the same designer element.
        /// </summary>
        public static string GetCollectionFormat(this OperationModel operation)
        {
            if (operation.TypeReference?.IsCollection != true ||
                !new CommonTypesOperationModel(operation.InternalElement).TryGetCollectionType(out var collectionType))
            {
                return null;
            }

            var type = collectionType.Type();
            return FormatFor(operation.InternalElement, collectionType.CustomFormat(), Select(
                isIEnumerable: type.IsIEnumerable(), isICollection: type.IsICollection(), isIList: type.IsIList(),
                isIReadOnlyCollection: type.IsIReadOnlyCollection(), isIReadOnlyList: type.IsIReadOnlyList(), isList: type.IsList(),
                isHashSet: type.IsHashSet(), isISet: type.IsISet(), isIQueryable: type.IsIQueryable(), isArray: type.IsArray(), isCustom: type.IsCustom()));
        }

        /// <summary>
        /// A Domain Event's Property. There is no typed accessor (see <see cref="DomainEventsMetadata"/>), so the
        /// stereotype is read by definition id, then mapped through the same <see cref="FormatFor"/>.
        /// </summary>
        public static string GetDomainEventPropertyCollectionFormat(this IElement property)
        {
            if (property.TypeReference?.IsCollection != true)
            {
                return null;
            }

            var setting = property.GetCollectionTypeSetting();
            return setting == null ? null : FormatFor(property, setting.Value.CustomFormat, setting.Value.Option);
        }

        /// <summary>
        /// The single mapping from a Collection Type option to a collection format; <c>{0}</c> is the element type.
        /// Returns null for no option (an empty Type), which keeps the template default.
        /// </summary>
        internal static string FormatFor(IElement element, string customFormat, CollectionTypeOption? option)
        {
            return option switch
            {
                null => null,
                CollectionTypeOption.IEnumerable => "System.Collections.Generic.IEnumerable<{0}>",
                CollectionTypeOption.ICollection => "System.Collections.Generic.ICollection<{0}>",
                CollectionTypeOption.IList => "System.Collections.Generic.IList<{0}>",
                CollectionTypeOption.IReadOnlyCollection => "System.Collections.Generic.IReadOnlyCollection<{0}>",
                CollectionTypeOption.IReadOnlyList => "System.Collections.Generic.IReadOnlyList<{0}>",
                CollectionTypeOption.List => "System.Collections.Generic.List<{0}>",
                CollectionTypeOption.HashSet => "System.Collections.Generic.HashSet<{0}>",
                CollectionTypeOption.ISet => "System.Collections.Generic.ISet<{0}>",
                CollectionTypeOption.IQueryable => "System.Linq.IQueryable<{0}>",
                CollectionTypeOption.Array => "{0}[]",
                CollectionTypeOption.Custom => ValidateCustomFormat(element, customFormat),
                _ => throw new InvalidOperationException($"Unsupported collection type option: {option}")
            };
        }

        private static string ValidateCustomFormat(IElement element, string customFormat)
        {
            var format = customFormat?.Trim();
            var isValid = !string.IsNullOrEmpty(format) && format.Contains("{0}", StringComparison.Ordinal) && HasBalancedBrackets(format);
            if (isValid)
            {
                try
                {
                    // The same parser GetTypeName applies.
                    CSharpCollectionFormatter.Create(format);
                }
                catch (Exception)
                {
                    isValid = false;
                }
            }

            if (!isValid)
            {
                throw new ElementException(element,
                    $"{element.SpecializationType} '{element.Name}' has a Collection Type of Custom with a Custom Format of '{customFormat}', which is not a valid collection format. " +
                    "Set Custom Format to the collection type with {0} where the element type goes, for example MyCompany.Collections.PagedList<{0}>, " +
                    "or choose one of the listed Type options.");
            }

            return format;
        }

        // CSharpCollectionFormatter only checks brackets that close the format, so e.g. "MyList<{0}" would pass it
        // and be emitted literally.
        private static bool HasBalancedBrackets(string format)
        {
            const string openers = "<[(";
            const string closers = ">])";
            var open = new Stack<char>();
            foreach (var c in format)
            {
                if (openers.IndexOf(c) >= 0)
                {
                    open.Push(c);
                    continue;
                }

                var closer = closers.IndexOf(c);
                if (closer < 0)
                {
                    continue;
                }

                if (open.Count == 0 || open.Pop() != openers[closer])
                {
                    return false;
                }
            }

            return open.Count == 0;
        }

        private static CollectionTypeOption? Select(
            bool isIEnumerable, bool isICollection, bool isIList, bool isIReadOnlyCollection, bool isIReadOnlyList, bool isList,
            bool isHashSet, bool isISet, bool isIQueryable, bool isArray, bool isCustom)
        {
            if (isIEnumerable) return CollectionTypeOption.IEnumerable;
            if (isICollection) return CollectionTypeOption.ICollection;
            if (isIList) return CollectionTypeOption.IList;
            if (isIReadOnlyCollection) return CollectionTypeOption.IReadOnlyCollection;
            if (isIReadOnlyList) return CollectionTypeOption.IReadOnlyList;
            if (isList) return CollectionTypeOption.List;
            if (isHashSet) return CollectionTypeOption.HashSet;
            if (isISet) return CollectionTypeOption.ISet;
            if (isIQueryable) return CollectionTypeOption.IQueryable;
            if (isArray) return CollectionTypeOption.Array;
            if (isCustom) return CollectionTypeOption.Custom;
            return null;
        }
    }
}
