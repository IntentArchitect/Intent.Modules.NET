using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static AzureFunctionsSettings GetAzureFunctionsSettings(this IApplicationSettingsProvider settings)
        {
            return new AzureFunctionsSettings(settings.GetGroup("90437e3f-cb10-4e44-b229-cc30c4807bea"));
        }
    }

    public class AzureFunctionsSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public AzureFunctionsSettings(IGroupSettings groupSettings)
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

        public bool SimpleFunctionNames() => bool.TryParse(_groupSettings.GetSetting("ff298d6c-705b-41d9-9286-be85480a0abd")?.Value.ToPascalCase(), out var result) && result;
    }
}