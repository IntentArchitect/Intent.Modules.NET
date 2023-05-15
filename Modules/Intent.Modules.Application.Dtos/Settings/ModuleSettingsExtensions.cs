using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static DTOSettings GetDTOSettings(this IApplicationSettingsProvider settings)
        {
            return new DTOSettings(settings.GetGroup("aac8a446-047c-468b-809b-ca28989b558b"));
        }
    }

    public class DTOSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public DTOSettings(IGroupSettings groupSettings)
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
        public TypeOptions Type() => new TypeOptions(_groupSettings.GetSetting("1d1a8aae-d3f7-42ee-84b3-cf62d7da4d1e")?.Value);

        public class TypeOptions
        {
            public readonly string Value;

            public TypeOptions(string value)
            {
                Value = value;
            }

            public TypeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "class" => TypeOptionsEnum.Class,
                    "record" => TypeOptionsEnum.Record,
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

        public enum TypeOptionsEnum
        {
            Class,
            Record,
        }
        public PropertySetterAccessibilityOptions PropertySetterAccessibility() => new PropertySetterAccessibilityOptions(_groupSettings.GetSetting("1b43bdbc-c11b-4e81-8285-6ca5c0876158")?.Value);

        public class PropertySetterAccessibilityOptions
        {
            public readonly string Value;

            public PropertySetterAccessibilityOptions(string value)
            {
                Value = value;
            }

            public PropertySetterAccessibilityOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "init" => PropertySetterAccessibilityOptionsEnum.Init,
                    "internal" => PropertySetterAccessibilityOptionsEnum.Internal,
                    "private" => PropertySetterAccessibilityOptionsEnum.Private,
                    "protected" => PropertySetterAccessibilityOptionsEnum.Protected,
                    "public" => PropertySetterAccessibilityOptionsEnum.Public,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsInit()
            {
                return Value == "init";
            }

            public bool IsInternal()
            {
                return Value == "internal";
            }

            public bool IsPrivate()
            {
                return Value == "private";
            }

            public bool IsProtected()
            {
                return Value == "protected";
            }

            public bool IsPublic()
            {
                return Value == "public";
            }
        }

        public enum PropertySetterAccessibilityOptionsEnum
        {
            Init,
            Internal,
            Private,
            Protected,
            Public,
        }

        public bool Sealed() => bool.TryParse(_groupSettings.GetSetting("9fd57931-91b4-45ba-aca4-eb38d91b7f97")?.Value.ToPascalCase(), out var result) && result;
    }
}