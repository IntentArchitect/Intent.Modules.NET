using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// Raw metadata ids for the Services designer elements that optional modelers add: Domain Event Handlers
    /// (Intent.Modelers.Domain.Events), Messages shown as "Integration Event", Integration Commands and Eventing DTOs
    /// (Intent.Modelers.Eventing), Integration Event Handlers (Intent.Modelers.Services.EventInteractions), and Service
    /// Proxies with their Operations and Parameters (Intent.Modelers.Types.ServiceProxies).
    /// <para>
    /// Deliberate, documented exception to "typed accessors only" (user decision): this module was built with no
    /// dependency on those modelers, so their typed models are not used. Intent.Modelers.Eventing has since become a
    /// dependency, for this module's subscription associations, but its elements are still read raw. The
    /// reverse-engineering skill installs the other modelers in a consumer only when the code needs them; without one
    /// there are no such elements and nothing is generated. The
    /// elements are therefore read as raw <see cref="IElement"/>s by specialization type id, and this module's own
    /// File Config, Type Configuration and Collection Type stereotypes for them ("Optional Modeler Elements" folder,
    /// targeted by filter function, so the Module Builder generates no typed accessors) by definition id and property
    /// name, which is the permitted fallback when no typed accessor exists. Renaming one of those properties in the
    /// designer would silently make it read empty.
    /// </para>
    /// </summary>
    public static class OptionalModelerMetadata
    {
        /// <summary>"Domain Event Handler" element type (Intent.Modelers.Domain.Events).</summary>
        public const string DomainEventHandlerTypeId = "d80e61c5-7e4c-4175-9df1-0413f664824c";

        /// <summary>"Message" element type, shown as "Integration Event" (Intent.Modelers.Eventing).</summary>
        public const string MessageTypeId = "cbe970af-5bad-4d92-a3ed-a24b9fdaa23e";

        /// <summary>"Integration Command" element type (Intent.Modelers.Eventing).</summary>
        public const string IntegrationCommandTypeId = "7f01ca8e-0e3c-4735-ae23-a45169f71625";

        /// <summary>"Eventing DTO" element type (Intent.Modelers.Eventing).</summary>
        public const string EventingDtoTypeId = "544f1d57-27ce-4985-a4ec-cc01568d72b0";

        /// <summary>"Property" element type, the child of a Message or Integration Command (Intent.Modelers.Eventing).</summary>
        public const string EventingPropertyTypeId = "bde29850-5fb9-4f47-9941-b9e182fd9bdc";

        /// <summary>"Eventing DTO-Field" element type, the child of an Eventing DTO (Intent.Modelers.Eventing).</summary>
        public const string EventingDtoFieldTypeId = "93eea5d7-a6a6-4fb8-9c87-d2e4c913fbe7";

        /// <summary>
        /// "Generalization" association type between Eventing DTOs (Intent.Modelers.Eventing). The Services designer's own
        /// DTO Generalization has the same name but a different id, so it is matched by id.
        /// </summary>
        public const string EventingGeneralizationTypeId = "ccf59371-009d-44dd-9417-a907b463b223";

        /// <summary>"Integration Event Handler" element type (Intent.Modelers.Services.EventInteractions).</summary>
        public const string IntegrationEventHandlerTypeId = "c0582230-22f5-4f74-8eb0-ec6fc9364900";

        /// <summary>"Service Proxy" element type (Intent.Modelers.Types.ServiceProxies).</summary>
        public const string ServiceProxyTypeId = "07d8d1a9-6b9f-4676-b7d3-8db06299e35c";

        /// <summary>
        /// "Operation" element type, the child of a Service Proxy (Intent.Modelers.Types.ServiceProxies). Not the Services
        /// designer's Operation, which has a different id.
        /// </summary>
        public const string ServiceProxyOperationTypeId = "aee6811e-b2f6-4562-a8eb-502029f63bc8";

        /// <summary>
        /// "Parameter" element type, the child of a Service Proxy Operation (Intent.Modelers.Types.ServiceProxies). Not the
        /// Services designer's Parameter, which has a different id.
        /// </summary>
        public const string ServiceProxyParameterTypeId = "f11273c1-217f-4f42-b690-3d8b488f95a8";

        // The interaction associations the drift check reads as intent facts (Drift/DriftModel.cs), drawn from a
        // Command, Query, Service Operation or event handler subscription to what it acts on.

        /// <summary>"Create Entity Action" association type (Intent.Modelers.Services.DomainInteractions).</summary>
        public const string CreateEntityActionTypeId = "7a3f0474-3cf8-4249-baac-8c07c49465e0";

        /// <summary>"Update Entity Action" association type (Intent.Modelers.Services.DomainInteractions).</summary>
        public const string UpdateEntityActionTypeId = "9ea0382a-4617-412a-a8c8-af987bbce226";

        /// <summary>"Delete Entity Action" association type (Intent.Modelers.Services.DomainInteractions).</summary>
        public const string DeleteEntityActionTypeId = "bfc823fb-60ab-451d-ba62-12671fe7e28e";

        /// <summary>"Query Entity Action" association type (Intent.Modelers.Services.DomainInteractions).</summary>
        public const string QueryEntityActionTypeId = "47ab5888-a258-4bec-a9fc-a83de69eb79d";

        /// <summary>
        /// "Perform Invocation" association type (Intent.Modelers.Services). Intent.Modelers.Services.DomainInteractions
        /// calls the same type "Call Service Operation", its earlier name.
        /// </summary>
        public const string PerformInvocationTypeId = "3e69085c-fa2f-44bd-93eb-41075fd472f8";

        /// <summary>"Publish Integration Event" association type (Intent.Modelers.Services.EventInteractions).</summary>
        public const string PublishIntegrationEventTypeId = "580b6b26-eab5-4602-a408-e76e2d292d2c";

        /// <summary>"Send Integration Command" association type (Intent.Modelers.Services.EventInteractions).</summary>
        public const string SendIntegrationCommandTypeId = "389a7478-a8f1-4acc-adff-a73ce4aa7e6d";

        // The subscriptions an event handler's interactions are drawn from: the association's target end, under the handler.

        /// <summary>"Subscribe Integration Event" association type, from an Integration Event Handler (Intent.Modelers.Services.EventInteractions).</summary>
        public const string SubscribeIntegrationEventTypeId = "80aa7f6d-64e5-4d24-a81e-6bc212925ca7";

        /// <summary>"Subscribe Integration Command" association type, from an Integration Event Handler (Intent.Modelers.Services.EventInteractions).</summary>
        public const string SubscribeIntegrationCommandTypeId = "f485e4aa-a032-4b9a-aa85-5d2c62d75799";

        /// <summary>"Domain Event Handler Association" type, from a Domain Event Handler (Intent.Modelers.Domain.Events).</summary>
        public const string DomainEventHandlerAssociationTypeId = "90831494-f069-44eb-b488-ab2dba7518ea";


        /// <summary>This module's shared "File Config" stereotype for every optional modeler element above.</summary>
        public const string FileConfigStereotypeId = "a8268032-90eb-4ec9-9bf7-5ab5aa60886f";

        /// <summary>This module's "Type Configuration" stereotype for handlers (Modifiers only; handlers are always classes).</summary>
        public const string HandlerTypeConfigurationStereotypeId = "5e0a9b42-c8e1-41b8-9bd4-071267e729f1";

        /// <summary>This module's "Type Configuration" stereotype for Messages, Integration Commands and Eventing DTOs (Type and Modifiers).</summary>
        public const string TypeConfigurationStereotypeId = "9bb8b979-bcb2-4908-91d5-b65028475bb6";

        /// <summary>This module's "Collection Type" stereotype for Message and Integration Command Properties and Eventing DTO-Fields.</summary>
        public const string CollectionTypeStereotypeId = "def585dc-4000-4215-969b-fcf9afd61c34";

        /// <summary>This module's "Property Configuration" stereotype for Message and Integration Command Properties and Eventing DTO-Fields.</summary>
        public const string PropertyConfigurationStereotypeId = "b310df29-d6be-4569-ae15-7006bb1c8286";

        /// <summary>This module's "Service Proxy Configuration" stereotype for Service Proxies (Type and Modifiers).</summary>
        public const string ServiceProxyConfigurationStereotypeId = "35cf5540-f14a-4ef3-ae22-f9d24aa9c90a";

        /// <summary>
        /// This module's "Async Configuration" stereotype for Service Proxy Operations, with the same properties and options as
        /// the typed one on Services Operations (see <see cref="ContractAsync"/>).
        /// </summary>
        public const string ServiceProxyAsyncConfigurationStereotypeId = "38da9465-4099-4b22-ac5a-a01cde22dec2";

        /// <summary>This module's "Default Value" stereotype for Service Proxy Parameters, which have no Value of their own.</summary>
        public const string DefaultValueStereotypeId = "7eb59935-6069-4d59-bbbe-15231c53aeae";

        /// <summary>
        /// This module's "Operation Configuration" stereotype for Service Proxy Operations, with the same properties and options
        /// as the typed one on Services Operations (see <see cref="ContractMemberModifiers"/>).
        /// </summary>
        public const string ServiceProxyOperationConfigurationStereotypeId = "47002d39-239c-416d-a0ab-3bac1b88d786";

        /// <summary>
        /// This module's "Parameter Modifier" stereotype for Service Proxy Parameters, with the same property and options as the
        /// typed one on Services Parameters (see <see cref="ContractParameterModifiers"/>).
        /// </summary>
        public const string ServiceProxyParameterModifierStereotypeId = "9fa9dc48-2ab8-4583-a898-48f9571dcaf4";


        private const string FilenameProperty = "Filename";
        private const string NamespaceProperty = "Namespace";
        private const string NoNamespaceProperty = "No Namespace";
        private const string TypeProperty = "Type";
        private const string ModifiersProperty = "Modifiers";
        private const string CustomFormatProperty = "Custom Format";
        private const string RequiredProperty = "Required";
        private const string AccessModifierProperty = "Access Modifier";
        private const string ValueProperty = "Value";
        private const string StaticProperty = "Static";
        private const string VirtualProperty = "Virtual";
        private const string OverrideProperty = "Override";
        private const string SealedProperty = "Sealed";
        private const string NewProperty = "New";
        private const string ModifierProperty = "Modifier";


        public static bool IsDomainEventHandler(this ICanBeReferencedType element) => IsOfType(element, DomainEventHandlerTypeId);

        public static bool IsIntegrationEvent(this ICanBeReferencedType element) => IsOfType(element, MessageTypeId);

        public static bool IsIntegrationCommand(this ICanBeReferencedType element) => IsOfType(element, IntegrationCommandTypeId);

        public static bool IsEventingDto(this ICanBeReferencedType element) => IsOfType(element, EventingDtoTypeId);

        public static bool IsIntegrationEventHandler(this ICanBeReferencedType element) => IsOfType(element, IntegrationEventHandlerTypeId);

        public static bool IsServiceProxy(this ICanBeReferencedType element) => IsOfType(element, ServiceProxyTypeId);

        /// <summary>The Operations of a Service Proxy, in model order.</summary>
        public static IList<IElement> GetServiceProxyOperations(this IElement element)
        {
            return element.ChildElements.Where(x => IsOfType(x, ServiceProxyOperationTypeId)).ToList();
        }

        /// <summary>The Parameters of a Service Proxy Operation, in model order.</summary>
        public static IList<IElement> GetServiceProxyParameters(this IElement operation)
        {
            return operation.ChildElements.Where(x => IsOfType(x, ServiceProxyParameterTypeId)).ToList();
        }

        /// <summary>A Service Proxy's Service Proxy Configuration, or null when the stereotype is not applied.</summary>
        public static ServiceProxyConfiguration GetServiceProxyConfiguration(this IElement element)
        {
            var stereotype = element.GetStereotype(ServiceProxyConfigurationStereotypeId);
            return stereotype != null ? new ServiceProxyConfiguration(element, stereotype) : null;
        }

        /// <summary>
        /// A Service Proxy Operation's Async Configuration stereotype, or null when it is not applied. It has no typed accessor,
        /// so <see cref="ContractAsync"/> reads it by property name.
        /// </summary>
        public static IStereotype GetServiceProxyAsyncConfiguration(this IElement operation)
        {
            return operation.GetStereotype(ServiceProxyAsyncConfigurationStereotypeId);
        }

        /// <summary>
        /// A Service Proxy Parameter's Default Value, or null when the stereotype is not applied. Applied with an empty Value is
        /// an error on the element: the stereotype exists only to hold the default, so leaving it empty would silently remove
        /// the code's default.
        /// </summary>
        public static string GetServiceProxyDefaultValue(this IElement parameter)
        {
            var stereotype = parameter.GetStereotype(DefaultValueStereotypeId);
            if (stereotype == null)
            {
                return null;
            }

            var value = stereotype.GetProperty<string>(ValueProperty);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ElementException(parameter,
                    $"Parameter '{parameter.Name}' has the Default Value stereotype, but its Value is empty. " +
                    "Set Value to the default the code declares, for example 1 or null, or remove the stereotype if the parameter has no default.");
            }

            return value;
        }

        /// <summary>A Service Proxy Operation's Operation Configuration, or null when the stereotype is not applied.</summary>
        public static ServiceProxyOperationConfiguration GetServiceProxyOperationConfiguration(this IElement operation)
        {
            var stereotype = operation.GetStereotype(ServiceProxyOperationConfigurationStereotypeId);
            return stereotype != null ? new ServiceProxyOperationConfiguration(stereotype) : null;
        }

        /// <summary>
        /// A Service Proxy Parameter's Parameter Modifier (ref, out, in, ref readonly or params), or null when the stereotype is
        /// not applied or its Modifier is empty. The options are the typed Parameter Modifier's.
        /// </summary>
        public static string GetServiceProxyParameterModifier(this IElement parameter)
        {
            var modifier = parameter.GetStereotype(ServiceProxyParameterModifierStereotypeId)?.GetProperty<string>(ModifierProperty);
            if (string.IsNullOrWhiteSpace(modifier))
            {
                return null;
            }

            if (modifier is "ref" or "out" or "in" or "ref readonly" or "params")
            {
                return modifier;
            }

            throw new InvalidOperationException($"Unsupported Parameter Modifier option: {modifier}");
        }


        /// <summary>
        /// True when the element lives in one of this application's own packages. Eventing packages are routinely
        /// referenced across applications (a subscriber references the publisher's package), and their elements then
        /// appear in the subscriber's Services designer too, so the eventing templates and EnumContract generate only
        /// what the application owns.
        /// </summary>
        public static bool IsOwnedBy(this IElement element, IApplication application)
        {
            return string.Equals(element.Package?.ApplicationId, application.Id, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>The Properties of a Message or Integration Command, in model order.</summary>
        public static IList<IElement> GetEventingProperties(this IElement element)
        {
            return element.ChildElements.Where(x => IsOfType(x, EventingPropertyTypeId)).ToList();
        }

        /// <summary>The Fields of an Eventing DTO, in model order.</summary>
        public static IList<IElement> GetEventingDtoFields(this IElement element)
        {
            return element.ChildElements.Where(x => IsOfType(x, EventingDtoFieldTypeId)).ToList();
        }

        /// <summary>
        /// The Eventing DTO this Eventing DTO inherits from, as the Generalization's target end (so generic arguments are
        /// kept), or null when it has none. C# allows one base class, so more than one is an error on the element.
        /// </summary>
        public static ITypeReference GetEventingParentDtoTypeReference(this IElement element)
        {
            var generalizations = element.AssociatedElements
                .Where(x => IsOfType(x.Association, EventingGeneralizationTypeId) && x.IsTargetEnd())
                .ToList();
            if (generalizations.Count > 1)
            {
                throw new ElementException(element,
                    $"Eventing DTO '{element.Name}' inherits from {generalizations.Count} Eventing DTOs ({string.Join(", ", generalizations.Select(x => x.TypeReference?.Element?.Name))}), " +
                    "but a C# class or record can only have one base type. Keep the Generalization to the base type the code declares and remove the others.");
            }

            return generalizations.SingleOrDefault()?.TypeReference;
        }

        /// <summary>True when another Eventing DTO inherits from this one through a Generalization.</summary>
        public static bool IsInheritedByAnyEventingDto(this IElement element)
        {
            return element.AssociatedElements.Any(x => IsOfType(x.Association, EventingGeneralizationTypeId) && x.IsSourceEnd());
        }

        /// <summary>
        /// The element's File Config, or null when the stereotype is not applied. Same shape as the generated typed
        /// accessors for the other contract elements.
        /// </summary>
        public static OptionalModelerFileConfig GetOptionalModelerFileConfig(this IElement element)
        {
            var stereotype = element.GetStereotype(FileConfigStereotypeId);
            return stereotype != null ? new OptionalModelerFileConfig(stereotype) : null;
        }

        /// <summary>A handler's Type Configuration Modifiers, or null when not set (see <see cref="ContractTypeModifiers"/>).</summary>
        public static string GetHandlerModifiers(this IElement element)
        {
            return element.GetStereotype(HandlerTypeConfigurationStereotypeId)?.GetProperty<string>(ModifiersProperty);
        }

        /// <summary>True when a Property's or Eventing DTO-Field's Property Configuration has Required ticked.</summary>
        public static bool IsOptionalModelerPropertyRequired(this IElement element)
        {
            return element.GetStereotype(PropertyConfigurationStereotypeId)?.GetProperty<bool>(RequiredProperty) == true;
        }

        /// <summary>A Property's or Eventing DTO-Field's Property Configuration Access Modifier, or null when not set.</summary>
        public static string GetOptionalModelerPropertyAccessModifier(this IElement element)
        {
            return element.GetStereotype(PropertyConfigurationStereotypeId)?.GetProperty<string>(AccessModifierProperty);
        }

        /// <summary>A Message's, Integration Command's or Eventing DTO's Type Configuration, or null when the stereotype is not applied.</summary>
        public static OptionalModelerTypeConfiguration GetOptionalModelerTypeConfiguration(this IElement element)
        {
            var stereotype = element.GetStereotype(TypeConfigurationStereotypeId);
            return stereotype != null ? new OptionalModelerTypeConfiguration(stereotype) : null;
        }

        /// <summary>
        /// A Property's or Eventing DTO-Field's Collection Type, or null when the stereotype is not applied or its Type is
        /// empty. The Type options are the same as the typed Collection Type's, so they map onto <see cref="ContractCollectionKind"/>.
        /// </summary>
        public static OptionalModelerCollectionType GetOptionalModelerCollectionType(this IElement element)
        {
            var stereotype = element.GetStereotype(CollectionTypeStereotypeId);
            var type = stereotype?.GetProperty<string>(TypeProperty);
            if (string.IsNullOrWhiteSpace(type))
            {
                return null;
            }

            if (!Enum.TryParse(type, ignoreCase: false, out ContractCollectionKind kind))
            {
                throw new InvalidOperationException(
                    $"Collection Type option '{type}' has no mapping in {nameof(ContractCollectionKind)}. Add it there and to {nameof(ContractCollectionTypes.GetCollectionFormat)}.");
            }

            return new OptionalModelerCollectionType(kind, stereotype.GetProperty<string>(CustomFormatProperty));
        }

        private static bool IsOfType(ICanBeReferencedType element, string specializationTypeId)
        {
            return string.Equals(element?.SpecializationTypeId, specializationTypeId, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsOfType(IAssociation association, string specializationTypeId)
        {
            return string.Equals(association?.SpecializationTypeId, specializationTypeId, StringComparison.OrdinalIgnoreCase);
        }

        public sealed class OptionalModelerFileConfig
        {
            private readonly IStereotype _stereotype;

            public OptionalModelerFileConfig(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Filename()
            {
                return _stereotype.GetProperty<string>(FilenameProperty);
            }

            public string Namespace()
            {
                return _stereotype.GetProperty<string>(NamespaceProperty);
            }

            public bool NoNamespace()
            {
                return _stereotype.GetProperty<bool>(NoNamespaceProperty);
            }
        }

        public sealed class OptionalModelerTypeConfiguration
        {
            private readonly IStereotype _stereotype;

            public OptionalModelerTypeConfiguration(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            /// <summary>Empty or absent means class, as for the typed Type Configuration.</summary>
            public bool IsRecord()
            {
                return string.Equals(_stereotype.GetProperty<string>(TypeProperty), "record", StringComparison.Ordinal);
            }

            /// <summary>A record whose properties are its positional parameters (see <see cref="ContractTypeKind.PositionalRecord"/>).</summary>
            public bool IsPositionalRecord()
            {
                return string.Equals(_stereotype.GetProperty<string>(TypeProperty), "positional record", StringComparison.Ordinal);
            }

            public string Modifiers()
            {
                return _stereotype.GetProperty<string>(ModifiersProperty);
            }
        }

        public sealed class OptionalModelerCollectionType
        {
            public OptionalModelerCollectionType(ContractCollectionKind kind, string customFormat)
            {
                Kind = kind;
                CustomFormat = customFormat;
            }

            public ContractCollectionKind Kind { get; }

            public string CustomFormat { get; }
        }

        public sealed class ServiceProxyConfiguration
        {
            private readonly IElement _element;
            private readonly IStereotype _stereotype;

            public ServiceProxyConfiguration(IElement element, IStereotype stereotype)
            {
                _element = element;
                _stereotype = stereotype;
            }

            /// <summary>
            /// True for Type interface: the proxy is the client's interface, for code whose implementation is generated at
            /// runtime (e.g. Refit). Empty or class means the concrete client class; any other value is an error on the element.
            /// </summary>
            public bool IsInterface()
            {
                var type = _stereotype.GetProperty<string>(TypeProperty);
                switch (type?.Trim())
                {
                    case null:
                    case "":
                    case "class":
                        return false;
                    case "interface":
                        return true;
                    default:
                        throw new ElementException(_element,
                            $"Service Proxy '{_element.Name}' has a Service Proxy Configuration Type of '{type}', which isn't valid. " +
                            "Choose class for the concrete client class, or interface when the code has only the client's interface.");
                }
            }

            public string Modifiers()
            {
                return _stereotype.GetProperty<string>(ModifiersProperty);
            }
        }

        public sealed class ServiceProxyOperationConfiguration
        {
            private readonly IStereotype _stereotype;

            public ServiceProxyOperationConfiguration(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            /// <summary>Empty means public, as for the typed Operation Configuration.</summary>
            public string AccessModifier()
            {
                return _stereotype.GetProperty<string>(AccessModifierProperty);
            }

            public bool Static()
            {
                return _stereotype.GetProperty<bool>(StaticProperty);
            }

            public bool Virtual()
            {
                return _stereotype.GetProperty<bool>(VirtualProperty);
            }

            public bool Override()
            {
                return _stereotype.GetProperty<bool>(OverrideProperty);
            }

            public bool Sealed()
            {
                return _stereotype.GetProperty<bool>(SealedProperty);
            }

            public bool New()
            {
                return _stereotype.GetProperty<bool>(NewProperty);
            }

            /// <summary>True when it declares anything but its defaults: public, with nothing ticked.</summary>
            public bool DeclaresModifiers()
            {
                return AccessModifier() is not (null or "" or "public") || Static() || Virtual() || Override() || Sealed() || New();
            }
        }
    }
}
