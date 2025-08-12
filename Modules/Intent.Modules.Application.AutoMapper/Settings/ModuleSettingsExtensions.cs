using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Application.AutoMapper.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static AutoMapperSettings GetAutoMapperSettings(this IApplicationSettingsProvider settings)
        {
            return new AutoMapperSettings(settings.GetGroup("137e5f2f-8432-4ade-b797-9a8e5e703e6d"));
        }
    }

    public class AutoMapperSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public AutoMapperSettings(IGroupSettings groupSettings)
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
        public ProfileLocationOptions ProfileLocation() => new ProfileLocationOptions(_groupSettings.GetSetting("e2a4568b-8eac-49a4-b7df-9dbe9dc04d20")?.Value);

        public class ProfileLocationOptions
        {
            public readonly string Value;

            public ProfileLocationOptions(string value)
            {
                Value = value;
            }

            public ProfileLocationOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "profile-in-dto" => ProfileLocationOptionsEnum.ProfileInDto,
                    "profile-separate" => ProfileLocationOptionsEnum.ProfileSeparate,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsProfileInDto()
            {
                return Value == "profile-in-dto";
            }

            public bool IsProfileSeparate()
            {
                return Value == "profile-separate";
            }
        }

        public enum ProfileLocationOptionsEnum
        {
            ProfileInDto,
            ProfileSeparate,
        }

        public bool UsePreCommercialVersion() => bool.TryParse(_groupSettings.GetSetting("e6c94301-0e22-4668-8909-e2e4d43b1416")?.Value.ToPascalCase(), out var result) && result;
    }
}