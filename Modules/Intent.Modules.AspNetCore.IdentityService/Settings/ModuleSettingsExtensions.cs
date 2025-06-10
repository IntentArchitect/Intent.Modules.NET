using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IdentityService.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static IdentityServiceSettings GetIdentityServiceSettings(this IApplicationSettingsProvider settings)
        {
            return new IdentityServiceSettings(settings.GetGroup("568e879f-e751-4831-9b0f-9a5e52fba9db"));
        }
    }

    public class IdentityServiceSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public IdentityServiceSettings(IGroupSettings groupSettings)
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

        public bool RequiresConfirmedAccount() => bool.TryParse(_groupSettings.GetSetting("64090931-0530-47d5-9fa5-0dae3f809187")?.Value.ToPascalCase(), out var result) && result;
    }
}