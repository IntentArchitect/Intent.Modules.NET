using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.AutoMapper.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static DatabaseSettings GetDatabaseSettings(this IApplicationSettingsProvider settings)
        {
            return new DatabaseSettings(settings.GetGroup("10a401d1-030a-4dd3-9742-9710bd8b5a9f"));
        }
    }

    public class DatabaseSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public DatabaseSettings(IGroupSettings groupSettings)
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
        public KeyTypeOptions KeyType() => new KeyTypeOptions(_groupSettings.GetSetting("9e89d26e-ca9a-489e-8dcb-d57d78b03af9")?.Value);

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
    }
}