using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.Interfaces.TriggerAppSettings
{
    /// <summary>
    /// Exercises app-setting trigger names for every non-Service Bus trigger type.
    /// </summary>
    public interface ITriggerAppSettingsService
    {
        /// <summary>
        /// Azure Storage Queue trigger.
        /// </summary>
        Task ProcessStorageQueueMessage(PayloadDto payload, CancellationToken cancellationToken = default);
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
        /// Cosmos DB change feed trigger.
        /// </summary>
        Task ProcessCosmosChanges(PayloadDto payload, CancellationToken cancellationToken = default);
    }
}