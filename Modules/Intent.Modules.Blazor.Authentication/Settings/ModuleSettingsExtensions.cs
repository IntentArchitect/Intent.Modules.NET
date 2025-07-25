using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static AuthenticationType GetAuthenticationType(this IApplicationSettingsProvider settings)
        {
            return new AuthenticationType(settings.GetGroup("56d61127-105b-43db-9bcd-07e49b80c64b"));
        }
    }

    public class AuthenticationType : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public AuthenticationType(IGroupSettings groupSettings)
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

        public AuthenticationOptions Authentication() => new AuthenticationOptions(_groupSettings.GetSetting("f45912f0-4c74-47f7-8657-7329c4257c23")?.Value);

        public class AuthenticationOptions
        {
            public readonly string Value;

            public AuthenticationOptions(string value)
            {
                Value = value;
            }

            public AuthenticationOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "aspnetcore-identity" => AuthenticationOptionsEnum.AspnetcoreIdentity,
                    "jwt" => AuthenticationOptionsEnum.Jwt,
                    "oidc" => AuthenticationOptionsEnum.Oidc,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsAspnetcoreIdentity()
            {
                return Value == "aspnetcore-identity";
            }

            public bool IsJwt()
            {
                return Value == "jwt";
            }

            public bool IsOidc()
            {
                return Value == "oidc";
            }
        }

        public enum AuthenticationOptionsEnum
        {
            AspnetcoreIdentity,
            Jwt,
            Oidc,
        }
    }
}