using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AzureFunctions.TestApplication.Application.Customers;
using AzureFunctions.TestApplication.Application.Interfaces.Queues;
using AzureFunctions.TestApplication.Domain.Entities;
using AzureFunctions.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.Implementation.Queues
{
    [IntentManaged(Mode.Merge)]
    public class QueueService : IQueueService
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueueService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CreateCustomerOp(CustomerDto dto, CancellationToken cancellationToken = default)
        {
            var newCustomer = new Customer
            {
                Name = dto.Name,
            };
            _customerRepository.Add(newCustomer);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task CreateCustomerOpWrapped(CustomerDto dto, CancellationToken cancellationToken = default)
        {
            var newCustomer = new Customer
            {
                Name = dto.Name,
            };
            _customerRepository.Add(newCustomer);
        }

        public void Dispose()
        {
        }
    }
}