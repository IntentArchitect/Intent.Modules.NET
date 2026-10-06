using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureServiceBus.Application.Interfaces.TriggerAppSettings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.Implementation.TriggerAppSettings
{
    /// <summary>
    /// Exercises app-setting trigger names for every non-Service Bus trigger type.
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class TriggerAppSettingsService : ITriggerAppSettingsService
    {
        [IntentManaged(Mode.Merge)]
        public TriggerAppSettingsService()
        {
        }

        /// <summary>
        /// Azure Storage Queue trigger.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task ProcessStorageQueueMessage(PayloadDto payload, CancellationToken cancellationToken = default)
        {
        }

        /// <summary>
        /// RabbitMQ trigger.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task ProcessRabbitMQMessage(PayloadDto payload, CancellationToken cancellationToken = default)
        {
        }

        /// <summary>
        /// Event Hub trigger (batched, as the isolated worker requires by default).
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task ProcessEventHubMessage(List<PayloadDto> payload, CancellationToken cancellationToken = default)
        {
        }

        /// <summary>
        /// Timer trigger.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task RunSchedule(CancellationToken cancellationToken = default)
        {
        }

        /// <summary>
        /// Cosmos DB change feed trigger.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task ProcessCosmosChanges(PayloadDto payload, CancellationToken cancellationToken = default)
        {
        }
    }
}
