using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.AppSettings.Application.Interfaces
{
    /// <summary>
    /// Exercises Trigger Names From App Settings for every trigger type.
    /// </summary>
    public interface ITriggerAppSettingsService
    {
        /// <summary>
        /// Service Bus queue trigger.
        /// </summary>
        Task ProcessServiceBusQueueMessage(PayloadDto payload, CancellationToken cancellationToken = default);
        /// <summary>
        /// Azure Storage Queue trigger.
        /// </summary>
        Task ProcessStorageQueueMessage(PayloadDto payload, CancellationToken cancellationToken = default);
        /// <summary>
        /// Queue Name already entered as an app setting reference; must be emitted unchanged and not seeded.
        /// </summary>
        Task ProcessPreResolvedQueueMessage(PayloadDto payload, CancellationToken cancellationToken = default);
        /// <summary>
        /// RabbitMQ trigger.
        /// </summary>
        Task ProcessRabbitMQMessage(PayloadDto payload, CancellationToken cancellationToken = default);
        /// <summary>
        /// Event Hub trigger (batched, as the isolated worker requires by default).
        /// </summary>
        Task ProcessEventHubMessage(List<PayloadDto> payload, CancellationToken cancellationToken = default);
        /// <summary>
        /// Timer trigger.
        /// </summary>
        Task RunSchedule(CancellationToken cancellationToken = default);
        /// <summary>
        /// Cosmos DB change feed trigger; lease settings must stay literal.
        /// </summary>
        Task ProcessCosmosChanges(PayloadDto payload, CancellationToken cancellationToken = default);
        /// <summary>
        /// Http trigger; must be unaffected by the setting.
        /// </summary>
        Task<string> GetStatus(CancellationToken cancellationToken = default);
        /// <summary>
        /// Service Bus topic subscription trigger.
        /// </summary>
        Task ProcessServiceBusTopicMessage(PayloadDto payload, CancellationToken cancellationToken = default);
    }
}