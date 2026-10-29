using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static MediatorSettings GetMediatorSettings(this IApplicationSettingsProvider settings)
        {
            return new MediatorSettings(settings.GetGroup("d87f0cfc-b32a-4297-aec4-46ec6944b77f"));
        }
    }

    public class MediatorSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public MediatorSettings(IGroupSettings groupSettings)
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

        public bool ConsolidateCommandQueryAssociatedFilesIntoSingleFile() => bool.TryParse(_groupSettings.GetSetting("f36cb70b-3d0a-4562-b734-f3efe4dc6a95")?.Value.ToPascalCase(), out var result) && result;
    }
}