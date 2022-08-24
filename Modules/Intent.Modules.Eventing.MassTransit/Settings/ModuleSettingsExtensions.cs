using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static Eventing GetEventing(this IApplicationSettingsProvider settings)
        {
            return new Eventing(settings.GetGroup("b1c11f3f-63ce-4917-8ffb-b6c7698346c7"));
        }
    }

    public class Eventing : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public Eventing(IGroupSettings groupSettings)
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
        public MessagingServiceProviderOptions MessagingServiceProvider() => new MessagingServiceProviderOptions(_groupSettings.GetSetting("2888b373-0419-4d33-ba56-2d8d0bf98eb9")?.Value);

        public class MessagingServiceProviderOptions
        {
            public readonly string Value;

            public MessagingServiceProviderOptions(string value)
            {
                Value = value;
            }

            public MessagingServiceProviderOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "in-memory" => MessagingServiceProviderOptionsEnum.InMemory,
                    "rabbitmq" => MessagingServiceProviderOptionsEnum.Rabbitmq,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsInMemory()
            {
                return Value == "in-memory";
            }

            public bool IsRabbitmq()
            {
                return Value == "rabbitmq";
            }
        }

        public enum MessagingServiceProviderOptionsEnum
        {
            InMemory,
            Rabbitmq,
        }
    }
}