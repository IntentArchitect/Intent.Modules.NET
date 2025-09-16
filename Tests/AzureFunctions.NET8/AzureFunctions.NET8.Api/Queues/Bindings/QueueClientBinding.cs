using AzureFunctions.NET8.Application.Customers;
using AzureFunctions.NET8.Application.Queues.CreateCustomerMessage;
using AzureFunctions.NET8.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "2.0")]

namespace AzureFunctions.NET8.Api.Queues.Bindings
{
    public class QueueClientBinding
    {

        public QueueClientBinding()
        {
        }

        [Function("Queues_Bindings_QueueClientBinding")]
        [QueueOutput("out-queue")]
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<CustomerDto> Run(
            [QueueTrigger("queue1")] Application.Queues.CreateCustomerMessage.CreateCustomerMessage message,
            CancellationToken cancellationToken)
        {
            return default;
        }
    }
}