using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static VisualStudioDesigner GetVisualStudioDesigner(this IApplicationSettingsProvider settings)
        {
            return new VisualStudioDesigner(settings.GetGroup("ee3f6418-ee19-4bb7-9b8e-07a4f1d94499"));
        }
    }

    public class VisualStudioDesigner : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public VisualStudioDesigner(IGroupSettings groupSettings)
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

        public bool EnableNETFrameworkProjectCreation() => bool.TryParse(_groupSettings.GetSetting("2dee07a2-4956-474b-b58e-ae04404ba2a3")?.Value.ToPascalCase(), out var result) && result;
    }
}