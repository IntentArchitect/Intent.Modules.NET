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
        public static HttpClientSettings GetHttpClientSettings(this IApplicationSettingsProvider settings)
        {
            return new HttpClientSettings(settings.GetGroup("f3da943d-d685-47cf-9982-59f1ac7127fd"));
        }
    }

    public class HttpClientSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public HttpClientSettings(IGroupSettings groupSettings)
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
        public AuthorizationTypeOptions AuthorizationType() => new AuthorizationTypeOptions(_groupSettings.GetSetting("72a03013-072b-4bb7-a67c-dad65bd14b02")?.Value);

        public class AuthorizationTypeOptions
        {
            public readonly string Value;

            public AuthorizationTypeOptions(string value)
            {
                Value = value;
            }

            public AuthorizationTypeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "passthrough-auth-header" => AuthorizationTypeOptionsEnum.PassthroughAuthHeader,
                    "token-management-server" => AuthorizationTypeOptionsEnum.TokenManagementServer,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsPassthroughAuthHeader()
            {
                return Value == "passthrough-auth-header";
            }

            public bool IsTokenManagementServer()
            {
                return Value == "token-management-server";
            }
        }

        public enum AuthorizationTypeOptionsEnum
        {
            PassthroughAuthHeader,
            TokenManagementServer,
        }
    }
}