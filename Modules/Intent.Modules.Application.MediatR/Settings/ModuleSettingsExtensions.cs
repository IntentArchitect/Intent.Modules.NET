using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static CQRSSettings GetCQRSSettings(this IApplicationSettingsProvider settings)
        {
            return new CQRSSettings(settings.GetGroup("2392c046-0aa7-4ee5-a77a-e954c4a7aab5"));
        }
    }

    public class CQRSSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public CQRSSettings(IGroupSettings groupSettings)
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

        public bool GroupCommandsQueriesHandlersAndValidatorsIntoSingleFile() => bool.TryParse(_groupSettings.GetSetting("02413035-08e9-47b2-8b89-798f38388243")?.Value.ToPascalCase(), out var result) && result;
    }
}