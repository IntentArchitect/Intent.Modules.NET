using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static WolverineMessageBusSettings GetWolverineMessageBusSettings(this IApplicationSettingsProvider settings)
        {
            return new WolverineMessageBusSettings(settings.GetGroup("a37422a7-64ad-446f-905b-75651043fe33"));
        }
    }

    public class WolverineMessageBusSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public WolverineMessageBusSettings(IGroupSettings groupSettings)
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
        public TransportOptions Transport() => new TransportOptions(_groupSettings.GetSetting("dc046cdc-e50f-4822-86f5-8372b2a6e2b0")?.Value);

        public class TransportOptions
        {
            public readonly string Value;

            public TransportOptions(string value)
            {
                Value = value;
            }

            public TransportOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "local" => TransportOptionsEnum.Local,
                    "rabbitmq" => TransportOptionsEnum.Rabbitmq,
                    "azure-service-bus" => TransportOptionsEnum.AzureServiceBus,
                    "amazon-sqs" => TransportOptionsEnum.AmazonSqs,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsLocal()
            {
                return Value == "local";
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

        public enum TransportOptionsEnum
        {
            Local,
            Rabbitmq,
            AzureServiceBus,
            AmazonSqs,
        }
        public TransactionalOutboxOptions TransactionalOutbox() => new TransactionalOutboxOptions(_groupSettings.GetSetting("2c31038a-8056-4b04-a183-9da3b504cbbe")?.Value);

        public class TransactionalOutboxOptions
        {
            public readonly string Value;

            public TransactionalOutboxOptions(string value)
            {
                Value = value;
            }

            public TransactionalOutboxOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "none" => TransactionalOutboxOptionsEnum.None,
                    "durable" => TransactionalOutboxOptionsEnum.Durable,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsNone()
            {
                return Value == "none";
            }

            public bool IsDurable()
            {
                return Value == "durable";
            }
        }

        public enum TransactionalOutboxOptionsEnum
        {
            None,
            Durable,
        }
        public ErrorHandlingPolicyOptions ErrorHandlingPolicy() => new ErrorHandlingPolicyOptions(_groupSettings.GetSetting("d016cdb6-f1ec-49f2-82af-4d6be226532b")?.Value);

        public class ErrorHandlingPolicyOptions
        {
            public readonly string Value;

            public ErrorHandlingPolicyOptions(string value)
            {
                Value = value;
            }

            public ErrorHandlingPolicyOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "none" => ErrorHandlingPolicyOptionsEnum.None,
                    "retry" => ErrorHandlingPolicyOptionsEnum.Retry,
                    "retry-with-cooldown" => ErrorHandlingPolicyOptionsEnum.RetryWithCooldown,
                    "schedule-retry" => ErrorHandlingPolicyOptionsEnum.ScheduleRetry,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsNone()
            {
                return Value == "none";
            }

            public bool IsRetry()
            {
                return Value == "retry";
            }

            public bool IsRetryWithCooldown()
            {
                return Value == "retry-with-cooldown";
            }

            public bool IsScheduleRetry()
            {
                return Value == "schedule-retry";
            }
        }

        public enum ErrorHandlingPolicyOptionsEnum
        {
            None,
            Retry,
            RetryWithCooldown,
            ScheduleRetry,
        }
    }
}