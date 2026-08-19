using System;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Azure.BlobStorage.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static BlobStorageSettings GetBlobStorageSettings(this IApplicationSettingsProvider settings)
        {
            return new BlobStorageSettings(settings.GetGroup("0bdbfe0a-59ca-491e-ab98-2921b2ccd2c3"));
        }
    }

    public class BlobStorageSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public BlobStorageSettings(IGroupSettings groupSettings)
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
        public AuthenticationMethodsOptions[] AuthenticationMethods() => JsonSerializer.Deserialize<string[]>(_groupSettings.GetSetting("591a1370-88a8-40e3-9430-d69fca3acff5")?.Value ?? "[]")?.Select(x => new AuthenticationMethodsOptions(x)).ToArray();

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