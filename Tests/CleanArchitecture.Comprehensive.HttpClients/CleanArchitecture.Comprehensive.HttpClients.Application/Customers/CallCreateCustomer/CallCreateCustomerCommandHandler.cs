using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.CallCreateCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CallCreateCustomerCommandHandler : IRequestHandler<CallCreateCustomerCommand>
    {
        private readonly ICustomersService _customersService;

        [IntentManaged(Mode.Merge)]
        public CallCreateCustomerCommandHandler(ICustomersService customersService)
        {
            _customersService = customersService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CallCreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var result = await _customersService.CreateCustomerAsync(new CreateCustomerCommand
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                Address = new CreateCustomerAddressDto
                {
                    Line1 = request.AddressLine1,
                    Line2 = request.AddressLine2,
                    City = request.AddressCity,
                    Postal = request.AddressPostal
                }
            }, cancellationToken);
        }
    }
}