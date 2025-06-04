using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.Sample.Domain;
using MudBlazor.Sample.Domain.Common.Exceptions;
using MudBlazor.Sample.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MudBlazor.Sample.Application.Customers.UpdateCustomer
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
            customer.AccountNo = request.AccountNo;
            customer.Address = new Address(
                line1: request.Address.Line1,
                line2: request.Address.Line2,
                city: request.Address.City,
                country: request.Address.Country,
                postal: request.Address.Postal);
        }
    }
}