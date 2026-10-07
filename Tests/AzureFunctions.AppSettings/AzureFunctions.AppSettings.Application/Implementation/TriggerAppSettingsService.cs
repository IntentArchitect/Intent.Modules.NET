using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AppSettings.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.AppSettings.Application.Implementation
{
    /// <summary>
    /// Exercises Trigger Names From App Settings for every trigger type.
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class TriggerAppSettingsService : ITriggerAppSettingsService
    {
        [IntentManaged(Mode.Merge)]
        public TriggerAppSettingsService()
        {
        }

        /// <summary>
        /// Service Bus queue trigger.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task ProcessServiceBusQueueMessage(PayloadDto payload, CancellationToken cancellationToken = default)
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
        /// Queue Name already entered as an app setting reference; must be emitted unchanged and not seeded.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task ProcessPreResolvedQueueMessage(PayloadDto payload, CancellationToken cancellationToken = default)
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
        /// Cosmos DB change feed trigger; lease settings must stay literal.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task ProcessCosmosChanges(PayloadDto payload, CancellationToken cancellationToken = default)
        {
        }

        /// <summary>
        /// Http trigger; must be unaffected by the setting.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> GetStatus(CancellationToken cancellationToken = default)
        {
            return "OK";
        }

        /// <summary>
        /// Service Bus topic subscription trigger.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task ProcessServiceBusTopicMessage(PayloadDto payload, CancellationToken cancellationToken = default)
        {
        }
    }
}