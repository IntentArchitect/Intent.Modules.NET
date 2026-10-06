using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.Interfaces.ServiceBusTriggers
{
    /// <summary>
    /// Exercises Service Bus triggered Azure Functions on traditional service operations.
    /// </summary>
    public interface IServiceBusTriggerService
    {
        /// <summary>
        /// Service Bus queue trigger (no subscription).
        /// </summary>
        Task ProcessQueueMessage(PayloadDto payload, CancellationToken cancellationToken = default);
        /// <summary>
        /// Service Bus topic trigger using a subscription.
        /// </summary>
        Task ProcessTopicMessage(PayloadDto payload, CancellationToken cancellationToken = default);
    }
}