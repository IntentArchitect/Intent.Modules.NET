using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET6.Application.Customers;
using AzureFunctions.NET6.Application.Interfaces.Queues;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.NET6.Application.Implementation.Queues
{
    [IntentManaged(Mode.Merge)]
    public class QueueService : IQueueService
    {
        [IntentManaged(Mode.Merge)]
        public QueueService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task CreateCustomerOp(CustomerDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateCustomerOp (QueueService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task CreateCustomerOpWrapped(CustomerDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateCustomerOpWrapped (QueueService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}