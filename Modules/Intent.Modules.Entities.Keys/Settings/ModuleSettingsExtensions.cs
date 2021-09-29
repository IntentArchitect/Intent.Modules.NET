using Intent.RoslynWeaver.Attributes;
using Intent.Configuration;
using Intent.Engine;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Entities.Keys.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static EntityKeySettings GetEntityKeySettings(this IApplicationSettingsProvider settings)
        {
            return new EntityKeySettings(settings.GetGroup("54be52b5-ac59-42d6-b0af-e33722f082e7"));
        }
    }

    public class EntityKeySettings
    {
        private readonly IGroupSettings _groupSettings;

        public EntityKeySettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
        }
        public string KeyType => _groupSettings.GetSetting("ef83f85d-bb8d-4b10-8842-9f35f9f54165")?.Value;
    }
}