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
        public static EventingSettings GetEventingSettings(this IApplicationSettingsProvider settings)
        {
            return new EventingSettings(settings.GetGroup("b1c11f3f-63ce-4917-8ffb-b6c7698346c7"));
        }
    }

    public class EventingSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public EventingSettings(IGroupSettings groupSettings)
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
                    "azure-service-bus" => MessagingServiceProviderOptionsEnum.AzureServiceBus,
                    "amazon-sqs" => MessagingServiceProviderOptionsEnum.AmazonSqs,
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

            public bool IsAzureServiceBus()
            {
                return Value == "azure-service-bus";
            }

            public bool IsAmazonSqs()
            {
                return Value == "amazon-sqs";
            }
        }

        public enum MessagingServiceProviderOptionsEnum
        {
            InMemory,
            Rabbitmq,
            AzureServiceBus,
            AmazonSqs,
        }
        public OutboxPatternOptions OutboxPattern() => new OutboxPatternOptions(_groupSettings.GetSetting("52006faf-54dc-4b4c-8251-284cdaef7b89")?.Value);

        public class OutboxPatternOptions
        {
            public readonly string Value;

            public OutboxPatternOptions(string value)
            {
                Value = value;
            }

            public OutboxPatternOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "none" => OutboxPatternOptionsEnum.None,
                    "in-memory" => OutboxPatternOptionsEnum.InMemory,
                    "entity-framework" => OutboxPatternOptionsEnum.EntityFramework,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsNone()
            {
                return Value == "none";
            }

            public bool IsInMemory()
            {
                return Value == "in-memory";
            }

            public bool IsEntityFramework()
            {
                return Value == "entity-framework";
            }
        }

        public enum OutboxPatternOptionsEnum
        {
            None,
            InMemory,
            EntityFramework,
        }
        public RetryPolicyOptions RetryPolicy() => new RetryPolicyOptions(_groupSettings.GetSetting("4c41cfe0-a2a0-4b67-b8c3-878650f8639f")?.Value);

        public class RetryPolicyOptions
        {
            public readonly string Value;

            public RetryPolicyOptions(string value)
            {
                Value = value;
            }

            public RetryPolicyOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "retry-immediate" => RetryPolicyOptionsEnum.RetryImmediate,
                    "retry-interval" => RetryPolicyOptionsEnum.RetryInterval,
                    "retry-incremental" => RetryPolicyOptionsEnum.RetryIncremental,
                    "retry-exponential" => RetryPolicyOptionsEnum.RetryExponential,
                    "retry-none" => RetryPolicyOptionsEnum.RetryNone,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsRetryImmediate()
            {
                return Value == "retry-immediate";
            }

            public bool IsRetryInterval()
            {
                return Value == "retry-interval";
            }

            public bool IsRetryIncremental()
            {
                return Value == "retry-incremental";
            }

            public bool IsRetryExponential()
            {
                return Value == "retry-exponential";
            }

            public bool IsRetryNone()
            {
                return Value == "retry-none";
            }
        }

        public enum RetryPolicyOptionsEnum
        {
            RetryImmediate,
            RetryInterval,
            RetryIncremental,
            RetryExponential,
            RetryNone,
        }

        public bool EnableScheduledPublishing() => bool.TryParse(_groupSettings.GetSetting("917a5c7c-cc7d-458a-8ecf-82315778f038")?.Value.ToPascalCase(), out var result) && result;
    }
}