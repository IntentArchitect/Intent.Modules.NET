using System;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureServiceBus.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static AzureServiceBusSettings GetAzureServiceBusSettings(this IApplicationSettingsProvider settings)
        {
            return new AzureServiceBusSettings(settings.GetGroup("b2b605a0-2b52-4852-a18f-ebbf760c4830"));
        }
    }

    public class AzureServiceBusSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public AzureServiceBusSettings(IGroupSettings groupSettings)
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
        public AuthenticationMethodsOptions[] AuthenticationMethods() => JsonSerializer.Deserialize<string[]>(_groupSettings.GetSetting("c2ebc5e5-ab29-4834-a008-2b433ae87eae")?.Value ?? "[]")?.Select(x => new AuthenticationMethodsOptions(x)).ToArray();

        public class AuthenticationMethodsOptions
        {
            public readonly string Value;

            public AuthenticationMethodsOptions(string value)
            {
                Value = value;
            }

            public AuthenticationMethodsOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "key-based" => AuthenticationMethodsOptionsEnum.KeyBased,
                    "managed-identity" => AuthenticationMethodsOptionsEnum.ManagedIdentity,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsKeyBased()
            {
                return Value == "key-based";
            }

            public bool IsManagedIdentity()
            {
                return Value == "managed-identity";
            }
        }

        public enum AuthenticationMethodsOptionsEnum
        {
            KeyBased,
            ManagedIdentity,
        }
    }
}