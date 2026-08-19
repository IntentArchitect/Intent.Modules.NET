using IntegrationTesting.SQLLite.Tests.Domain.Common.Exceptions;
using IntegrationTesting.SQLLite.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.SQLLite.Tests.Application.Customers.UpdateCustomer
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}'");
            }

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;
            customer.PhoneNumber = request.PhoneNumber;
        }
    }
}
