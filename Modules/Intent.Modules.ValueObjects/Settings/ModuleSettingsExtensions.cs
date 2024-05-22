using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.ValueObjects.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static ValueObjectSettings GetValueObjectSettings(this IApplicationSettingsProvider settings)
        {
            return new ValueObjectSettings(settings.GetGroup("82b8662d-4368-4dc6-9516-5acb1fa85464"));
        }
    }

    public class ValueObjectSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public ValueObjectSettings(IGroupSettings groupSettings)
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
        public ValueObjectTypeOptions ValueObjectType() => new ValueObjectTypeOptions(_groupSettings.GetSetting("704c2e3d-1a78-4af2-b363-533f2da2089b")?.Value);

        public class ValueObjectTypeOptions
        {
            public readonly string Value;

            public ValueObjectTypeOptions(string value)
            {
                Value = value;
            }

            public ValueObjectTypeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "class" => ValueObjectTypeOptionsEnum.Class,
                    "record" => ValueObjectTypeOptionsEnum.Record,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsClass()
            {
                return Value == "class";
            }

            public bool IsRecord()
            {
                return Value == "record";
            }
        }

        public enum ValueObjectTypeOptionsEnum
        {
            Class,
            Record,
        }
    }
}