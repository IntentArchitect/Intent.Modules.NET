using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Entities.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static EntitySettings GetEntitySettings(this IApplicationSettingsProvider settings)
        {
            return new EntitySettings(settings.GetGroup("5272be11-67d1-4730-993e-e3582f96e2ee"));
        }
    }

    public class EntitySettings
    {
        private readonly IGroupSettings _groupSettings;

        public EntitySettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
        }

        public bool SeparateBusinessLogicWithPartials() => bool.TryParse(_groupSettings.GetSetting("cbae7b62-5a99-41c5-8ddf-cf21fb797a10")?.Value.ToPascalCase(), out var result) && result;
    }
}