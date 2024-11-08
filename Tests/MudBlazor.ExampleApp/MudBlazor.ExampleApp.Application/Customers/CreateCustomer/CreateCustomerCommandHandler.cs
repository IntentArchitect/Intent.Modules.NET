using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Domain;
using MudBlazor.ExampleApp.Domain.Entities;
using MudBlazor.ExampleApp.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MudBlazor.ExampleApp.Application.Customers.CreateCustomer
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
            var customer = new Customer
            {
                Name = request.Name,
                AccountNo = request.AccountNo,
                Address = new Address(
                    line1: request.Address.Line1,
                    line2: request.Address.Line2,
                    city: request.Address.City,
                    country: request.Address.Country,
                    postal: request.Address.Postal)
            };

            _customerRepository.Add(customer);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return customer.Id;
        }
    }
}