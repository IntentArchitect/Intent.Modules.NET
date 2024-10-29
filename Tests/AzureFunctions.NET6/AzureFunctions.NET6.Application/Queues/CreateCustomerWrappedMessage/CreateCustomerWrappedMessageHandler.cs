using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.NET6.Domain.Entities;
using AzureFunctions.NET6.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Queues.CreateCustomerWrappedMessage
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCustomerWrappedMessageHandler : IRequestHandler<CreateCustomerWrappedMessage>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCustomerWrappedMessageHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateCustomerWrappedMessage request, CancellationToken cancellationToken)
        {
            var newCustomer = new Customer
            {
                Name = request.Name,
            };

            _customerRepository.Add(newCustomer);
        }
    }
}