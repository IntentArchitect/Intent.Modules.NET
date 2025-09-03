using System;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static CosmosDBSettings GetCosmosDBSettings(this IApplicationSettingsProvider settings)
        {
            return new CosmosDBSettings(settings.GetGroup("dcfa949a-c512-47c4-a644-0f2b88e44794"));
        }
    }

    public class CosmosDBSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public CosmosDBSettings(IGroupSettings groupSettings)
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

        public bool UseOptimisticConcurrency() => bool.TryParse(_groupSettings.GetSetting("6f75f245-8194-4606-b6b7-2bb226394b5f")?.Value.ToPascalCase(), out var result) && result;

        public bool StoreEnumsAsStrings() => bool.TryParse(_groupSettings.GetSetting("863af816-91fb-4f11-9e9d-e62f559ab0ac")?.Value.ToPascalCase(), out var result) && result;
        public AuthenticationMethodsOptions[] AuthenticationMethods() => JsonSerializer.Deserialize<string[]>(_groupSettings.GetSetting("e19121ec-5c34-4f43-8c9f-62e0b84b01de")?.Value ?? "[]")?.Select(x => new AuthenticationMethodsOptions(x)).ToArray();

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