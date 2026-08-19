using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.ObjectMapping.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static ObjectMapping GetObjectMapping(this IApplicationSettingsProvider settings)
        {
            return new ObjectMapping(settings.GetGroup("27e1bf22-ac74-4e15-b5d9-cc1342530dab"));
        }
    }

    public class ObjectMapping : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public ObjectMapping(IGroupSettings groupSettings)
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
        public NullPathHandlingOptions NullPathHandling() => new NullPathHandlingOptions(_groupSettings.GetSetting("0d9f5223-c911-4afd-bb7d-89e5bd13a48c")?.Value);

        public class NullPathHandlingOptions
        {
            public readonly string Value;

            public NullPathHandlingOptions(string value)
            {
                Value = value;
            }

            public NullPathHandlingOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "strict" => NullPathHandlingOptionsEnum.Strict,
                    "lenient" => NullPathHandlingOptionsEnum.Lenient,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsStrict()
            {
                return Value == "strict";
            }

            public bool IsLenient()
            {
                return Value == "lenient";
            }
        }

        public enum NullPathHandlingOptionsEnum
        {
            Strict,
            Lenient,
        }
    }
}