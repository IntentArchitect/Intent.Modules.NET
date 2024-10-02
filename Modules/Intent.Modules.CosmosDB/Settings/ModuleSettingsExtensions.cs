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
            return new CosmosDb(settings.GetGroup("dcfa949a-c512-47c4-a644-0f2b88e44794"));
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

        public bool UseOptimisticConcurrency() => bool.TryParse(_groupSettings.GetSetting("6f75f245-8194-4606-b6b7-2bb226394b5f")?.Value.ToPascalCase(), out var result) && result;

        public bool StoreEnumsAsStrings() => bool.TryParse(_groupSettings.GetSetting("863af816-91fb-4f11-9e9d-e62f559ab0ac")?.Value.ToPascalCase(), out var result) && result;
    }
}