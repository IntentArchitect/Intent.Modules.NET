using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Identity.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static IdentitySettings GetIdentitySettings(this IApplicationSettingsProvider settings)
        {
            return new IdentitySettings(settings.GetGroup("1045dea6-d28f-4ab8-9b5e-6f360035fdb6"));
        }
    }

    public class IdentitySettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public IdentitySettings(IGroupSettings groupSettings)
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
        public UserIdTypeOptions UserIdType() => new UserIdTypeOptions(_groupSettings.GetSetting("0ac959d1-dc9e-4403-ae44-2a75c53a1331")?.Value);

        public class UserIdTypeOptions
        {
            public readonly string Value;

            public UserIdTypeOptions(string value)
            {
                Value = value;
            }

            public UserIdTypeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "guid" => UserIdTypeOptionsEnum.Guid,
                    "int" => UserIdTypeOptionsEnum.Int,
                    "long" => UserIdTypeOptionsEnum.Long,
                    "string" => UserIdTypeOptionsEnum.String,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsString()
            {
                return Value == "string";
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

        public enum UserIdTypeOptionsEnum
        {
            String,
            Guid,
            Long,
            Int,
        }
    }
}