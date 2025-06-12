using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AzureFunctions.NET6.Application.Customers;
using AzureFunctions.NET6.Application.Interfaces.Queues;
using AzureFunctions.NET6.Domain.Entities;
using AzureFunctions.NET6.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.NET6.Application.Implementation.Queues
{
    [IntentManaged(Mode.Merge)]
    public class QueueService : IQueueService
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueueService()
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CreateCustomerOp(CustomerDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateCustomerOp (QueueService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CreateCustomerOpWrapped(CustomerDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateCustomerOpWrapped (QueueService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}