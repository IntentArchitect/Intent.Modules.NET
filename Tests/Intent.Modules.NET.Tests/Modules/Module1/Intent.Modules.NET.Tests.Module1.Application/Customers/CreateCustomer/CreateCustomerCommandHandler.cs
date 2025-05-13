using Intent.Modules.NET.Tests.Module1.Application.Contracts.Customers.CreateCustomer;
using Intent.Modules.NET.Tests.Module1.Domain.Entities;
using Intent.Modules.NET.Tests.Module1.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Customers.CreateCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = new Customer(
                name: request.Name);

            _customerRepository.Add(customer);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return customer.Id;
        }
    }
}