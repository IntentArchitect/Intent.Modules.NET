using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.CallDeleteCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CallDeleteCustomerCommandHandler : IRequestHandler<CallDeleteCustomerCommand>
    {
        private readonly ICustomersService _customersService;

        [IntentManaged(Mode.Merge)]
        public CallDeleteCustomerCommandHandler(ICustomersService customersService)
        {
            _customersService = customersService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CallDeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            await _customersService.DeleteCustomerAsync(new DeleteCustomerCommand
            {
                Id = request.Id
            }, cancellationToken);
        }
    }
}