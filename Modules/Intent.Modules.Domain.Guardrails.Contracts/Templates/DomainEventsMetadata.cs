using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    /// <summary>
    /// Raw metadata access for Domain Events (Intent.Modelers.Domain.Events).
    /// <para>
    /// User decision: this module does not depend on Intent.Modelers.Domain.Events, and references neither its NuGet
    /// package nor its assembly. A consumer installs that module only when its code has domain events; without it
    /// there are no Domain Event elements and nothing is generated. So there are no typed models (no DomainEventModel)
    /// and, because the four Domain Event stereotypes target elements by specialization id, no generated typed
    /// accessors either. Reading by element type id and stereotype definition id is the fallback the typed-accessor
    /// rule permits when no typed accessor exists. Every raw id and property name is kept here, in one place.
    /// </para>
    /// <para>Type ids are from Intent.Modelers.Domain.Events 4.4.7.</para>
    /// </summary>
    internal static class DomainEventsMetadata
    {
        public const string DomainEventTypeId = "0814e459-fb9b-47db-b7eb-32ce30397e8a";
        public const string PropertyTypeId = "b4d69073-5abb-4968-b41b-545b2f7408ed";
        public const string GeneralizationTypeId = "fa57ec52-536d-46a8-8aa0-4589812665c1";

        // This module's stereotypes for Domain Events and their properties (the "Domain Events" folder of its package).
        public const string FileConfigDefinitionId = "42c50d5f-97a1-47b2-b76a-f7bb5936650f";
        public const string TypeConfigurationDefinitionId = "41304368-35ef-4063-ae8a-017587f7c8ac";
        public const string CollectionTypeDefinitionId = "3eb6bbc8-7734-4523-849d-595c1bbcc810";
        public const string PropertyConfigurationDefinitionId = "68051676-fae8-4ae9-8b46-d19fe8ae776d";

        private const string FilenameProperty = "Filename";
        private const string NamespaceProperty = "Namespace";
        private const string NoNamespaceProperty = "No Namespace";
        private const string TypeProperty = "Type";
        private const string ModifiersProperty = "Modifiers";
        private const string CustomFormatProperty = "Custom Format";
        private const string RequiredProperty = "Required";
        private const string AccessModifierProperty = "Access Modifier";

        public static bool IsDomainEvent(this IElement element)
        {
            return element?.SpecializationTypeId == DomainEventTypeId;
        }

        /// <summary>
        /// The Domain designer's Domain Event elements. Empty when Intent.Modelers.Domain.Events isn't installed:
        /// the engine's GetElementsOfType returns an empty sequence for a type id no element has.
        /// </summary>
        public static IEnumerable<IElement> GetDomainEvents(this IDesigner designer)
        {
            return designer.GetElementsOfType(DomainEventTypeId);
        }

        public static IEnumerable<IElement> GetDomainEventProperties(this IElement domainEvent)
        {
            return domainEvent.ChildElements.Where(x => x.SpecializationTypeId == PropertyTypeId);
        }

        /// <summary>
        /// The Domain Event this one inherits from, through a Domain Event Generalization, or null. Mirrors the
        /// generated Generalizations() navigation of the Domain modelers: on the derived element's associated ends,
        /// the generalization's target end points at the base element.
        /// </summary>
        public static IElement GetBaseDomainEvent(this IElement domainEvent)
        {
            var bases = domainEvent.AssociatedElements
                .Where(x => x.Association?.SpecializationTypeId == GeneralizationTypeId && x.IsTargetEnd())
                .Select(x => x.TypeReference?.Element as IElement)
                .Where(x => x != null)
                .ToList();

            if (bases.Count > 1)
            {
                throw new ElementException(domainEvent,
                    $"Domain Event '{domainEvent.Name}' inherits from more than one Domain Event ({string.Join(", ", bases.Select(x => x.Name))}). " +
                    "A C# class or record can have only one base type. Remove all but one of its Domain Event Generalizations.");
            }

            return bases.SingleOrDefault();
        }

        public static string GetFileConfigFilename(this IElement domainEvent)
        {
            return domainEvent.GetStereotype(FileConfigDefinitionId)?.GetProperty<string>(FilenameProperty);
        }

        public static string GetFileConfigNamespace(this IElement domainEvent)
        {
            return domainEvent.GetStereotype(FileConfigDefinitionId)?.GetProperty<string>(NamespaceProperty);
        }

        public static bool GetFileConfigNoNamespace(this IElement domainEvent)
        {
            return domainEvent.GetStereotype(FileConfigDefinitionId)?.GetProperty<bool>(NoNamespaceProperty) == true;
        }

        /// <summary>
        /// Type Configuration: "record" means a record and "positional record" a positional record; "class", an empty Type or
        /// no stereotype mean a class, as with the typed accessors' IsRecord() and IsPositionalRecord().
        /// </summary>
        public static ContractTypeKind GetDomainEventContractTypeKind(this IElement domainEvent)
        {
            var type = domainEvent.GetStereotype(TypeConfigurationDefinitionId)?.GetProperty<string>(TypeProperty);
            return ContractTypeKindExtensions.ToKind(type == "record", type == "positional record");
        }

        /// <summary>Type Configuration Modifiers, or null when not set (see <see cref="ContractTypeModifiers"/>).</summary>
        public static string GetDomainEventModifiers(this IElement domainEvent)
        {
            return domainEvent.GetStereotype(TypeConfigurationDefinitionId)?.GetProperty<string>(ModifiersProperty);
        }

        /// <summary>True when a Domain Event Property's Property Configuration has Required ticked.</summary>
        public static bool IsRequiredDomainEventProperty(this IElement property)
        {
            return property.GetStereotype(PropertyConfigurationDefinitionId)?.GetProperty<bool>(RequiredProperty) == true;
        }

        /// <summary>A Domain Event Property's Property Configuration Access Modifier, or null when not set.</summary>
        public static string GetDomainEventPropertyAccessModifier(this IElement property)
        {
            return property.GetStereotype(PropertyConfigurationDefinitionId)?.GetProperty<string>(AccessModifierProperty);
        }

        /// <summary>
        /// The Collection Type stereotype on a Domain Event Property: (Type option or null when empty, Custom Format),
        /// or null when the stereotype is absent. The option strings are the stereotype's select values.
        /// </summary>
        public static (CollectionTypeOption? Option, string CustomFormat)? GetCollectionTypeSetting(this IElement property)
        {
            var stereotype = property.GetStereotype(CollectionTypeDefinitionId);
            if (stereotype == null)
            {
                return null;
            }

            var type = stereotype.GetProperty<string>(TypeProperty);
            CollectionTypeOption? option = type switch
            {
                null or "" => null,
                "IEnumerable" => CollectionTypeOption.IEnumerable,
                "ICollection" => CollectionTypeOption.ICollection,
                "IList" => CollectionTypeOption.IList,
                "IReadOnlyCollection" => CollectionTypeOption.IReadOnlyCollection,
                "IReadOnlyList" => CollectionTypeOption.IReadOnlyList,
                "List" => CollectionTypeOption.List,
                "HashSet" => CollectionTypeOption.HashSet,
                "ISet" => CollectionTypeOption.ISet,
                "IQueryable" => CollectionTypeOption.IQueryable,
                "Array" => CollectionTypeOption.Array,
                "Custom" => CollectionTypeOption.Custom,
                _ => throw new InvalidOperationException(
                    $"Unsupported Collection Type option '{type}' on {property.SpecializationType} '{property.Name}' ({property.Id}).")
            };

            return (option, stereotype.GetProperty<string>(CustomFormatProperty));
        }

        /// <summary>
        /// The element's designer folder path. GetFolderPath() only follows folders for IHasFolder models, so for this
        /// raw IElement model it would ignore them; this applies the same rule through the same FolderModel chain.
        /// </summary>
        public static string GetElementFolderPath(this IElement element)
        {
            return string.Join("/", new ElementFolder(element).GetParentFolderNames());
        }

        /// <summary>
        /// The namespace derived from the output target and the element's designer folders, with the same Folder
        /// Options (Namespace Provider) rule as GetNamespace(), which also ignores folders for a raw IElement model.
        /// </summary>
        public static string GetElementNamespace(this IIntentTemplate template, IElement element)
        {
            var folders = new ElementFolder(element).GetParentFolders()
                .Where(x => !string.IsNullOrWhiteSpace(x.Name) && (!x.HasFolderOptions() || x.GetFolderOptions().NamespaceProvider()))
                .Select(x => x.Name);
            return string.Join(".", new[] { template.OutputTarget.GetNamespace() }.Concat(folders));
        }

        /// <summary>The same parent-folder rule the typed Domain models use (e.g. ClassModel.Folder).</summary>
        private sealed class ElementFolder : IHasFolder
        {
            public ElementFolder(IElement element)
            {
                Folder = element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId
                    ? new FolderModel(element.ParentElement)
                    : null;
            }

            public FolderModel Folder { get; }
        }
    }
}
