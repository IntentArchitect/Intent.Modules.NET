using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.SocialLogin.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static SocialLoginProviders GetSocialLoginProviders(this IApplicationSettingsProvider settings)
        {
            return new SocialLoginProviders(settings.GetGroup("f909d630-616e-4153-bd2b-7c4a1dde4014"));
        }
    }

    public class SocialLoginProviders : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public SocialLoginProviders(IGroupSettings groupSettings)
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

        public bool Google() => bool.TryParse(_groupSettings.GetSetting("bed9eb1e-5c9a-4455-b5b7-b2083de0dc90")?.Value.ToPascalCase(), out var result) && result;

        public bool Microsoft() => bool.TryParse(_groupSettings.GetSetting("95cf9668-5248-42b7-918f-b62803036dc1")?.Value.ToPascalCase(), out var result) && result;

        public bool Facebook() => bool.TryParse(_groupSettings.GetSetting("7e3db6ef-0b5c-4476-91da-5da649666f4a")?.Value.ToPascalCase(), out var result) && result;

        public bool Twitter() => bool.TryParse(_groupSettings.GetSetting("acadc640-db39-4997-9b8a-38fe94b87ad7")?.Value.ToPascalCase(), out var result) && result;
    }
}