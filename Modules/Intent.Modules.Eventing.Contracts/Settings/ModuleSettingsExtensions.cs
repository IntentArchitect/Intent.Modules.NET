using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static EventingSettings GetEventingSettings(this IApplicationSettingsProvider settings)
        {
            return new EventingSettings(settings.GetGroup("38416eff-a5f1-4842-b77c-ab301d7d1d67"));
        }
    }

    public class EventingSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public EventingSettings(IGroupSettings groupSettings)
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

        public bool UseLegacyInterfaceName() => bool.TryParse(_groupSettings.GetSetting("0420c177-417d-4073-bf26-250f1907a565")?.Value.ToPascalCase(), out var result) && result;
    }
}