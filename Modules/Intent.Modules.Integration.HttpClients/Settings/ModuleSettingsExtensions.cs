using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static IntegrationHttpClientSettings GetIntegrationHttpClientSettings(this IApplicationSettingsProvider settings)
        {
            return new IntegrationHttpClientSettings(settings.GetGroup("f3da943d-d685-47cf-9982-59f1ac7127fd"));
        }
    }

    public class IntegrationHttpClientSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public IntegrationHttpClientSettings(IGroupSettings groupSettings)
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
        public AuthorizationSetupOptions AuthorizationSetup() => new AuthorizationSetupOptions(_groupSettings.GetSetting("72a03013-072b-4bb7-a67c-dad65bd14b02")?.Value);

        public class AuthorizationSetupOptions
        {
            public readonly string Value;

            public AuthorizationSetupOptions(string value)
            {
                Value = value;
            }

            public AuthorizationSetupOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "none" => AuthorizationSetupOptionsEnum.None,
                    "authorization-header-provider" => AuthorizationSetupOptionsEnum.AuthorizationHeaderProvider,
                    "client-access-token-management" => AuthorizationSetupOptionsEnum.ClientAccessTokenManagement,
                    "transmittable-access-token" => AuthorizationSetupOptionsEnum.TransmittableAccessToken,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsClientAccessTokenManagement()
            {
                return Value == "client-access-token-management";
            }

            public bool IsAuthorizationHeaderProvider()
            {
                return Value == "authorization-header-provider";
            }

            public bool IsTransmittableAccessToken()
            {
                return Value == "transmittable-access-token";
            }

            public bool IsNone()
            {
                return Value == "none";
            }
        }

        public enum AuthorizationSetupOptionsEnum
        {
            ClientAccessTokenManagement,
            AuthorizationHeaderProvider,
            TransmittableAccessToken,
            None,
        }
    }
}