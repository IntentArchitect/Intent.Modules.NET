using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.MassTransit.Api
{
    public static class MessageSubscribeAssocationTargetEndModelStereotypeExtensions
    {
        public static AzureServiceBusConsumerSettings GetAzureServiceBusConsumerSettings(this MessageSubscribeAssocationTargetEndModel model)
        {
            var stereotype = model.GetStereotype("Azure Service Bus Consumer Settings");
            return stereotype != null ? new AzureServiceBusConsumerSettings(stereotype) : null;
        }


        public static bool HasAzureServiceBusConsumerSettings(this MessageSubscribeAssocationTargetEndModel model)
        {
            return model.HasStereotype("Azure Service Bus Consumer Settings");
        }

        public static bool TryGetAzureServiceBusConsumerSettings(this MessageSubscribeAssocationTargetEndModel model, out AzureServiceBusConsumerSettings stereotype)
        {
            if (!HasAzureServiceBusConsumerSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AzureServiceBusConsumerSettings(model.GetStereotype("Azure Service Bus Consumer Settings"));
            return true;
        }

        public static RabbitMQConsumerSettings GetRabbitMQConsumerSettings(this MessageSubscribeAssocationTargetEndModel model)
        {
            var stereotype = model.GetStereotype("Rabbit MQ Consumer Settings");
            return stereotype != null ? new RabbitMQConsumerSettings(stereotype) : null;
        }


        public static bool HasRabbitMQConsumerSettings(this MessageSubscribeAssocationTargetEndModel model)
        {
            return model.HasStereotype("Rabbit MQ Consumer Settings");
        }

        public static bool TryGetRabbitMQConsumerSettings(this MessageSubscribeAssocationTargetEndModel model, out RabbitMQConsumerSettings stereotype)
        {
            if (!HasRabbitMQConsumerSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new RabbitMQConsumerSettings(model.GetStereotype("Rabbit MQ Consumer Settings"));
            return true;
        }

        [IntentManaged(Mode.Fully, Signature = Mode.Merge)]
        public class AzureServiceBusConsumerSettings : IAzureServiceBusConsumerSettings
        {
            private IStereotype _stereotype;

            public AzureServiceBusConsumerSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public int? PrefetchCount()
            {
                return _stereotype.GetProperty<int?>("Prefetch Count");
            }

            public bool RequiresSession()
            {
                return _stereotype.GetProperty<bool>("Requires Session");
            }

            public string DefaultMessageTimeToLive()
            {
                return _stereotype.GetProperty<string>("Default Message Time To Live");
            }

            public string LockDuration()
            {
                return _stereotype.GetProperty<string>("Lock Duration");
            }

            public bool RequiresDuplicateDetection()
            {
                return _stereotype.GetProperty<bool>("Requires Duplicate Detection");
            }

            public string DuplicateDetectionHistoryTimeWindow()
            {
                return _stereotype.GetProperty<string>("Duplicate Detection History Time Window");
            }

            public bool EnableBatchedOperations()
            {
                return _stereotype.GetProperty<bool>("Enable Batched Operations");
            }

            public bool EnableDeadLetteringOnMessageExpiration()
            {
                return _stereotype.GetProperty<bool>("Enable Dead-lettering On Message Expiration");
            }

            public int? MaxQueueSize()
            {
                return _stereotype.GetProperty<int?>("Max Queue Size");
            }

            public int? MaxDeliveryCount()
            {
                return _stereotype.GetProperty<int?>("Max Delivery Count");
            }

        }

        [IntentManaged(Mode.Fully, Signature = Mode.Merge)]
        public class RabbitMQConsumerSettings : IRabbitMQConsumerSettings
        {
            private IStereotype _stereotype;

            public RabbitMQConsumerSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public int? PrefetchCount()
            {
                return _stereotype.GetProperty<int?>("Prefetch Count");
            }

            public bool Lazy()
            {
                return _stereotype.GetProperty<bool>("Lazy");
            }

            public bool Durable()
            {
                return _stereotype.GetProperty<bool>("Durable");
            }

            public bool PurgeOnStartup()
            {
                return _stereotype.GetProperty<bool>("Purge On Startup");
            }

            public bool Exclusive()
            {
                return _stereotype.GetProperty<bool>("Exclusive");
            }

        }

    }
}