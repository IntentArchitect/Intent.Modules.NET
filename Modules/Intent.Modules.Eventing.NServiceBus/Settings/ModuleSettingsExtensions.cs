using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static NServiceBusSettings GetNServiceBusSettings(this IApplicationSettingsProvider settings)
        {
            return new NServiceBusSettings(settings.GetGroup("76e5fbf4-95c3-4d3e-8343-0969c4a67df6"));
        }
    }

    public class NServiceBusSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public NServiceBusSettings(IGroupSettings groupSettings)
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

        public string Transport() => _groupSettings.GetSetting("537d4def-c538-417c-bada-a785c14195b3")?.Value;

        public string EndpointName() => _groupSettings.GetSetting("755f6223-496a-4b34-83a7-04902f75a440")?.Value;

        public string ConnectionString() => _groupSettings.GetSetting("e7eb3d7d-0753-4bac-8d1a-1a56dfdfa763")?.Value;
    }
}