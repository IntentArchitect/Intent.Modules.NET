using Intent.Configuration;
using Intent.Engine;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static DynamoDBSettings GetDynamoDBSettings(this IApplicationSettingsProvider settings)
        {
            return new DynamoDBSettings(settings.GetGroup("bdef1cd0-dfc7-4a5c-9eb1-0496f0ceac38"));
        }
    }

    public class DynamoDBSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public DynamoDBSettings(IGroupSettings groupSettings)
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

        public bool UseOptimisticConcurrency() => bool.TryParse(_groupSettings.GetSetting("3f304c01-ceda-4823-b17d-6a0c3662143f")?.Value.ToPascalCase(), out var result) && result;

        public bool StoreEnumsAsStrings() => bool.TryParse(_groupSettings.GetSetting("e2fd9bd6-4015-4ce0-adce-6ca60b6af951")?.Value.ToPascalCase(), out var result) && result;
    }
}