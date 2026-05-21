using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.Eventing.NServiceBus.Api
{
    /// <summary>
    /// Stereotype extension methods for NServiceBus-specific settings on Eventing designer elements.
    /// The NServiceBus stereotype serves as a transport discriminator in multi-broker setups.
    /// The NServiceBus Message Settings stereotype carries per-message routing configuration.
    /// These DefinitionIds must match stereotype definitions registered in the Intent Architect Eventing designer.
    /// </summary>
    public static class NServiceBusStereotypeExtensions
    {
        // ── NServiceBus (transport discriminator) ──────────────────────────

        public static NServiceBusMessageStereotype GetNServiceBusMessage(this MessageModel model)
        {
            var s = model.GetStereotype(NServiceBusMessageStereotype.DefinitionId);
            return s != null ? new NServiceBusMessageStereotype(s) : null;
        }

        public static bool HasNServiceBusMessage(this MessageModel model) =>
            model.HasStereotype(NServiceBusMessageStereotype.DefinitionId);

        public static NServiceBusMessageStereotype GetNServiceBusMessage(this IntegrationCommandModel model)
        {
            var s = model.GetStereotype(NServiceBusMessageStereotype.DefinitionId);
            return s != null ? new NServiceBusMessageStereotype(s) : null;
        }

        public static bool HasNServiceBusMessage(this IntegrationCommandModel model) =>
            model.HasStereotype(NServiceBusMessageStereotype.DefinitionId);

        // ── NServiceBus Message Settings (per-command routing override) ────

        public static NServiceBusMessageSettingsStereotype GetNServiceBusMessageSettings(this IntegrationCommandModel model)
        {
            var s = model.GetStereotype(NServiceBusMessageSettingsStereotype.DefinitionId);
            return s != null ? new NServiceBusMessageSettingsStereotype(s) : null;
        }

        public static bool HasNServiceBusMessageSettings(this IntegrationCommandModel model) =>
            model.HasStereotype(NServiceBusMessageSettingsStereotype.DefinitionId);

        // ── Stereotype wrapper classes ──────────────────────────────────────

        public class NServiceBusMessageStereotype
        {
            private readonly IStereotype _stereotype;

            // Presence of this stereotype identifies NServiceBus as the transport for this message.
            // Register a matching stereotype definition in the Intent Architect Eventing designer extension to enable this.
            public const string DefinitionId = "5f2a9b3c-1d4e-4f60-8a7b-c9d0e1f2a3b4";

            public NServiceBusMessageStereotype(IStereotype stereotype) => _stereotype = stereotype;
            public string Name => _stereotype.Name;
        }

        public class NServiceBusMessageSettingsStereotype
        {
            private readonly IStereotype _stereotype;

            public const string DefinitionId = "7e8f9a0b-1c2d-3e4f-5061-728394a5b6c7";

            public NServiceBusMessageSettingsStereotype(IStereotype stereotype) => _stereotype = stereotype;
            public string Name => _stereotype.Name;

            /// <summary>Destination endpoint name for this command. Overrides the convention-based default.</summary>
            public string EndpointName() => _stereotype.GetProperty("Endpoint Name")?.Value;
        }
    }
}
