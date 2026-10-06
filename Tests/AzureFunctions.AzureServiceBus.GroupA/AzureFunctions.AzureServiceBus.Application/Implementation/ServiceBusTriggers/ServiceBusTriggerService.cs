using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.AzureServiceBus.Application.Interfaces.ServiceBusTriggers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.Implementation.ServiceBusTriggers
{
    /// <summary>
    /// Exercises Service Bus triggered Azure Functions on traditional service operations.
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class ServiceBusTriggerService : IServiceBusTriggerService
    {
        [IntentManaged(Mode.Merge)]
        public ServiceBusTriggerService()
        {
        }

        /// <summary>
        /// Service Bus queue trigger (no subscription).
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task ProcessQueueMessage(PayloadDto payload, CancellationToken cancellationToken = default)
        {
        }

        /// <summary>
        /// Service Bus topic trigger using a subscription.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task ProcessTopicMessage(PayloadDto payload, CancellationToken cancellationToken = default)
        {
        }
    }
}
