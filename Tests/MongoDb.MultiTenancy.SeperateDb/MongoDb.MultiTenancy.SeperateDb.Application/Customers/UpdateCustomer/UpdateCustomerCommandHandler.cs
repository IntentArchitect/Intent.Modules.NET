using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using MongoDb.MultiTenancy.SeperateDb.Domain.Common.Exceptions;
using MongoDb.MultiTenancy.SeperateDb.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Application.Customers.UpdateCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}'");
            }

            customer.Name = request.Name;

            _customerRepository.Update(customer);
        }
    }
}