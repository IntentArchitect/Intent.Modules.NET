using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static FluentValidationApplicationLayer GetFluentValidationApplicationLayer(this IApplicationSettingsProvider settings)
        {
            return new FluentValidationApplicationLayer(settings.GetGroup("459a4008-350c-42ec-b43d-9c85000babc0"));
        }
    }

    public class FluentValidationApplicationLayer : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public FluentValidationApplicationLayer(IGroupSettings groupSettings)
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
        public UniqueConstraintValidationOptions UniqueConstraintValidation() => new UniqueConstraintValidationOptions(_groupSettings.GetSetting("da76a14e-3ea4-4ebb-a262-69cbb0f9889b")?.Value);

        public class UniqueConstraintValidationOptions
        {
            public readonly string Value;

            public UniqueConstraintValidationOptions(string value)
            {
                Value = value;
            }

            public UniqueConstraintValidationOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "default-enabled" => UniqueConstraintValidationOptionsEnum.DefaultEnabled,
                    "default-disabled" => UniqueConstraintValidationOptionsEnum.DefaultDisabled,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsDefaultEnabled()
            {
                return Value == "default-enabled";
            }

            public bool IsDefaultDisabled()
            {
                return Value == "default-disabled";
            }
        }

        public enum UniqueConstraintValidationOptionsEnum
        {
            DefaultEnabled,
            DefaultDisabled,
        }
    }
}