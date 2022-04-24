using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Entities.Keys.Settings
{

    public static class DatabaseSettingsExtensions
    {

        public static KeyTypeOptions KeyType(this DatabaseSettings groupSettings) => new KeyTypeOptions(groupSettings.GetSetting("ef83f85d-bb8d-4b10-8842-9f35f9f54165")?.Value);

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
                    "guid" => KeyTypeOptionsEnum.Guid,
                    "long" => KeyTypeOptionsEnum.Long,
                    "int" => KeyTypeOptionsEnum.Int,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsGuid()
            {
                return Value == "guid";
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

        public static KeyCreationModeOptions KeyCreationMode(this DatabaseSettings groupSettings) => new KeyCreationModeOptions(groupSettings.GetSetting("5aca6e0c-1b64-425b-9046-f0bc81c44311")?.Value);

        public class KeyCreationModeOptions
        {
            public readonly string Value;

            public KeyCreationModeOptions(string value)
            {
                Value = value;
            }

            public KeyCreationModeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "manual" => KeyCreationModeOptionsEnum.Manual,
                    "implicit" => KeyCreationModeOptionsEnum.Implicit,
                    "explicit" => KeyCreationModeOptionsEnum.Explicit,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsManual()
            {
                return Value == "manual";
            }

            public bool IsImplicit()
            {
                return Value == "implicit";
            }

            public bool IsExplicit()
            {
                return Value == "explicit";
            }
        }

        public enum KeyCreationModeOptionsEnum
        {
            Manual,
            Implicit,
            Explicit,
        }
    }
}