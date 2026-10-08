using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// Raw metadata ids for Domain Event Handlers, which the Intent.Modelers.Domain.Events module adds to
    /// the Services designer.
    /// <para>
    /// Deliberate, documented exception to "typed accessors only" (user decision): this module takes no
    /// dependency on Intent.Modelers.Domain.Events, so its typed <c>DomainEventHandlerModel</c> is not
    /// available. The reverse-engineering skill installs that modeler in a consumer only when the code has
    /// domain events; without it there are no handler elements and nothing is generated. Handlers are
    /// therefore read as raw <see cref="IElement"/>s by specialization type id, and their File Config and
    /// Type Configuration stereotypes (targeted by filter function, so the Module Builder generates no typed
    /// accessors for them) by definition id, which is the permitted fallback when no typed accessor exists.
    /// </para>
    /// </summary>
    public static class DomainEventsMetadata
    {
        /// <summary>"Domain Event Handler" element type (Intent.Modelers.Domain.Events).</summary>
        public const string DomainEventHandlerTypeId = "d80e61c5-7e4c-4175-9df1-0413f664824c";

        /// <summary>This module's "File Config" stereotype for Domain Event Handlers ("Domain Events" folder).</summary>
        public const string FileConfigStereotypeId = "a8268032-90eb-4ec9-9bf7-5ab5aa60886f";

        /// <summary>This module's "Type Configuration" stereotype for Domain Event Handlers (Modifiers only; handlers are always classes).</summary>
        public const string TypeConfigurationStereotypeId = "5e0a9b42-c8e1-41b8-9bd4-071267e729f1";

        private const string FilenameProperty = "Filename";
        private const string NamespaceProperty = "Namespace";
        private const string NoNamespaceProperty = "No Namespace";
        private const string ModifiersProperty = "Modifiers";

        public static bool IsDomainEventHandler(this ICanBeReferencedType element)
        {
            return string.Equals(element?.SpecializationTypeId, DomainEventHandlerTypeId, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// The handler's File Config, or null when the stereotype is not applied. Same shape as the
        /// generated typed accessors for the other contract elements.
        /// </summary>
        public static DomainEventHandlerFileConfig GetDomainEventHandlerFileConfig(this IElement element)
        {
            var stereotype = element.GetStereotype(FileConfigStereotypeId);
            return stereotype != null ? new DomainEventHandlerFileConfig(stereotype) : null;
        }

        /// <summary>Type Configuration Modifiers, or null when not set (see <see cref="ContractTypeModifiers"/>).</summary>
        public static string GetDomainEventHandlerModifiers(this IElement element)
        {
            return element.GetStereotype(TypeConfigurationStereotypeId)?.GetProperty<string>(ModifiersProperty);
        }

        public sealed class DomainEventHandlerFileConfig
        {
            private readonly IStereotype _stereotype;

            public DomainEventHandlerFileConfig(IStereotype stereotype)
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
    }
}
