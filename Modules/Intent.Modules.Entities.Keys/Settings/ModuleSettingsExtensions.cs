using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Entities.Keys.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static EntityKeySettings GetEntityKeySettings(this IApplicationSettingsProvider settings)
        {
            return new EntityKeySettings(settings.GetGroup("54be52b5-ac59-42d6-b0af-e33722f082e7"));
        }
    }

    public class EntityKeySettings
    {
        private readonly IGroupSettings _groupSettings;

        public EntityKeySettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
        }

        public KeyTypeOptions KeyType() => new KeyTypeOptions(_groupSettings.GetSetting("ef83f85d-bb8d-4b10-8842-9f35f9f54165")?.Value);

        public class KeyTypeOptions
        {
            public readonly string Value;

            public KeyTypeOptions(string value)
            {
                Value = value;
            }

            public KeyTypeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "System.Guid" => KeyTypeOptionsEnum.Guid,
                    "long" => KeyTypeOptionsEnum.Long,
                    "int" => KeyTypeOptionsEnum.Int,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsGuid()
            {
                return Value == "System.Guid";
            }

            public bool IsLong()
            {
                return Value == "long";
            }

            public bool IsInt()
            {
                return Value == "int";
            }
        }

        public enum KeyTypeOptionsEnum
        {
            Guid,
            Long,
            Int,
        }

        public bool CreateKeysExplicitly() => bool.TryParse(_groupSettings.GetSetting("5aca6e0c-1b64-425b-9046-f0bc81c44311")?.Value.ToPascalCase(), out var result) && result;
    }
}