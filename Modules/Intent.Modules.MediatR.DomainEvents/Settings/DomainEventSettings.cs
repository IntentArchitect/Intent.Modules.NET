using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Modelers.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.MediatR.DomainEvents.Settings
{
    //Manually added this as we dont want a direct dependency here, as this module can work with Event Sourcing Modules too.
    public static class ModuleSettingsExtensions
    {
        public static DomainEventSettings GetDomainEventSettings(this IApplicationSettingsProvider settings)
        {
            return new DomainEventSettings(settings.GetGroup("88f8701c-1fee-49f9-a289-6791535158cf"));
        }
    }

    public class DomainEventSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public DomainEventSettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
        }

        public string Id => _groupSettings.Id;

        public string Title
        {
            get => _groupSettings.Title;
            set => _groupSettings.Title = value;
        }

        public ISetting GetSetting(string settingId)
        {
            return _groupSettings.GetSetting(settingId);
        }
    }

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
