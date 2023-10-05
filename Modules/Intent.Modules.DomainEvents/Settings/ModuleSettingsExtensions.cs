using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.DomainEvents.Settings
{

    public static class DomainSettingsExtensions
    {
        public static ImplementDomainEventingOnOptions ImplementDomainEventingOn(this DomainSettings groupSettings) => new ImplementDomainEventingOnOptions(groupSettings.GetSetting("afc698a3-1188-43e9-9ba4-df44a5f343d3")?.Value);

        public class ImplementDomainEventingOnOptions
        {
            public readonly string Value;

            public ImplementDomainEventingOnOptions(string value)
            {
                Value = value;
            }

            public ImplementDomainEventingOnOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "all" => ImplementDomainEventingOnOptionsEnum.All,
                    "modelled-events" => ImplementDomainEventingOnOptionsEnum.ModelledEvents,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsAll()
            {
                return Value == "all";
            }

            public bool IsModelledEvents()
            {
                return Value == "modelled-events";
            }
        }

        public enum ImplementDomainEventingOnOptionsEnum
        {
            All,
            ModelledEvents,
        }
    }
}