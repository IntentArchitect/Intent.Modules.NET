using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static Blazor GetBlazor(this IApplicationSettingsProvider settings)
        {
            return new Blazor(settings.GetGroup("489a67db-31b2-4d51-96d7-52637c3795be"));
        }
    }

    public class Blazor : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public Blazor(IGroupSettings groupSettings)
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

        public bool UseCodeBehindFilesForComponents() => bool.TryParse(_groupSettings.GetSetting("4c4c903b-6fbb-4a6b-b1f7-98a180f010f2")?.Value.ToPascalCase(), out var result) && result;
    }
}