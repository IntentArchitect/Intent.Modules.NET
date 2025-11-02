using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.AzureQueueStorage.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static AzureQueueStorageSettings GetAzureQueueStorageSettings(this IApplicationSettingsProvider settings)
        {
            return new AzureQueueStorageSettings(settings.GetGroup("e71edb78-27b4-4326-9def-796a18064e8b"));
        }
    }

    public class AzureQueueStorageSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public AzureQueueStorageSettings(IGroupSettings groupSettings)
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
        public MessageEncodingOptions MessageEncoding() => new MessageEncodingOptions(_groupSettings.GetSetting("32d8dbd8-c478-48ef-8e1a-cdbed0e9da98")?.Value);

        public class MessageEncodingOptions
        {
            public readonly string Value;

            public MessageEncodingOptions(string value)
            {
                Value = value;
            }

            public MessageEncodingOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "none" => MessageEncodingOptionsEnum.None,
                    "base-64" => MessageEncodingOptionsEnum.Base64,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsNone()
            {
                return Value == "none";
            }

            public bool IsBase64()
            {
                return Value == "base-64";
            }
        }

        public enum MessageEncodingOptionsEnum
        {
            None,
            Base64,
        }
    }
}