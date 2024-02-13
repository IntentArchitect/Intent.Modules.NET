using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static CosmosDb GetCosmosDb(this IApplicationSettingsProvider settings)
        {
            return new CosmosDb(settings.GetGroup("e9f6758c-f8db-4cad-af33-827b867be548"));
        }
    }

    public class CosmosDb : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public CosmosDb(IGroupSettings groupSettings)
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

        public bool UseOptimisticConcurrency() => bool.TryParse(_groupSettings.GetSetting("fcb114f7-77b4-4c6a-96c0-41bb146f4166")?.Value.ToPascalCase(), out var result) && result;
    }
}