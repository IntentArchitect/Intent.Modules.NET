using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.CallUpdateCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CallUpdateCustomerCommandHandler : IRequestHandler<CallUpdateCustomerCommand>
    {
        private readonly ICustomersService _customersService;

        [IntentManaged(Mode.Merge)]
        public CallUpdateCustomerCommandHandler(ICustomersService customersService)
        {
            _customersService = customersService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CallUpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            await _customersService.UpdateCustomerAsync(new UpdateCustomerCommand
            {
                Id = request.Id,
                Surname = request.Surname,
                Email = request.Email,
                Address = new UpdateCustomerAddressDto
                {
                    Line1 = request.AddressLine1,
                    Line2 = request.AddressLine2,
                    City = request.AddressCity,
                    Postal = request.AddressPostal,
                    Id = request.AddressId
                },
                Name = request.Name
            }, cancellationToken);
        }
    }
}