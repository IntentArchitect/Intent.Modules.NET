using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static ApplicationFluentValidation GetApplicationFluentValidation(this IApplicationSettingsProvider settings)
        {
            return new ApplicationFluentValidation(settings.GetGroup("459a4008-350c-42ec-b43d-9c85000babc0"));
        }
    }

    public class ApplicationFluentValidation : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public ApplicationFluentValidation(IGroupSettings groupSettings)
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

        public bool ValidateUniqueConstraintsByDefault() => bool.TryParse(_groupSettings.GetSetting("45349755-ce1c-437f-87b9-686a5d6ee8f7")?.Value.ToPascalCase(), out var result) && result;
    }
}