using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Security.JWT.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static JWTSecuritySettings GetJWTSecuritySettings(this IApplicationSettingsProvider settings)
        {
            return new JWTSecuritySettings(settings.GetGroup("2c1a1918-97eb-4b21-8a15-021dd72db73c"));
        }
    }

    public class JWTSecuritySettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public JWTSecuritySettings(IGroupSettings groupSettings)
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
        public JWTBearerAuthenticationTypeOptions JWTBearerAuthenticationType() => new JWTBearerAuthenticationTypeOptions(_groupSettings.GetSetting("c2fba79a-6285-4ec8-ad76-585f089d8aa5")?.Value);

        public class JWTBearerAuthenticationTypeOptions
        {
            public readonly string Value;

            public JWTBearerAuthenticationTypeOptions(string value)
            {
                Value = value;
            }

            public JWTBearerAuthenticationTypeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "oidc" => JWTBearerAuthenticationTypeOptionsEnum.Oidc,
                    "manual" => JWTBearerAuthenticationTypeOptionsEnum.Manual,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsOidc()
            {
                return Value == "oidc";
            }

            public bool IsManual()
            {
                return Value == "manual";
            }
        }

        public enum JWTBearerAuthenticationTypeOptionsEnum
        {
            Oidc,
            Manual,
        }
    }
}