using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Application.Interfaces.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.CallCreateCustomerName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CallCreateCustomerNameCommandHandler : IRequestHandler<CallCreateCustomerNameCommand>
    {
        private readonly ICustomersService _customersService;
        private readonly ICustomerService _customerService;

        [IntentManaged(Mode.Merge)]
        public CallCreateCustomerNameCommandHandler(ICustomersService customersService, ICustomerService customerService)
        {
            _customersService = customersService;
            _customerService = customerService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CallCreateCustomerNameCommand request, CancellationToken cancellationToken)
        {
            var result = await _customersService.GetCustomerByIdAsync(request.CustomerId, cancellationToken);
            await _customerService.CreateCustomerNameCommand(result.Name, cancellationToken);
        }
    }
}