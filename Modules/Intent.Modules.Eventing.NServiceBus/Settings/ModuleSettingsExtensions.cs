using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static NServiceBusSettings GetNServiceBusSettings(this IApplicationSettingsProvider settings)
        {
            return new NServiceBusSettings(settings.GetGroup("76e5fbf4-95c3-4d3e-8343-0969c4a67df6"));
        }
    }

    public class NServiceBusSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public NServiceBusSettings(IGroupSettings groupSettings)
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

        public TransportOptions Transport() => new TransportOptions(_groupSettings.GetSetting("537d4def-c538-417c-bada-a785c14195b3")?.Value);

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
                    "rabbitmq" => TransportOptionsEnum.Rabbitmq,
                    "azure-service-bus" => TransportOptionsEnum.AzureServiceBus,
                    "amazon-sqs" => TransportOptionsEnum.AmazonSqs,
                    "sql-server" => TransportOptionsEnum.SqlServer,
                    "learning-transport" => TransportOptionsEnum.LearningTransport,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
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

            public bool IsSqlServer()
            {
                return Value == "sql-server";
            }

            public bool IsLearningTransport()
            {
                return Value == "learning-transport";
            }
        }

        public enum TransportOptionsEnum
        {
            Rabbitmq,
            AzureServiceBus,
            AmazonSqs,
            SqlServer,
            LearningTransport,
        }
        public RecoverabilityPolicyOptions RecoverabilityPolicy() => new RecoverabilityPolicyOptions(_groupSettings.GetSetting("4060477a-191f-43be-a2c1-f2dd94ff00e2")?.Value);

        public class RecoverabilityPolicyOptions
        {
            public readonly string Value;

            public RecoverabilityPolicyOptions(string value)
            {
                Value = value;
            }

            public RecoverabilityPolicyOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "none" => RecoverabilityPolicyOptionsEnum.None,
                    "immediate-only" => RecoverabilityPolicyOptionsEnum.ImmediateOnly,
                    "delayed-only" => RecoverabilityPolicyOptionsEnum.DelayedOnly,
                    "immediate-and-delayed" => RecoverabilityPolicyOptionsEnum.ImmediateAndDelayed,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsNone()
            {
                return Value == "none";
            }

            public bool IsImmediateOnly()
            {
                return Value == "immediate-only";
            }

            public bool IsDelayedOnly()
            {
                return Value == "delayed-only";
            }

            public bool IsImmediateAndDelayed()
            {
                return Value == "immediate-and-delayed";
            }
        }

        public enum RecoverabilityPolicyOptionsEnum
        {
            None,
            ImmediateOnly,
            DelayedOnly,
            ImmediateAndDelayed,
        }
    }
}