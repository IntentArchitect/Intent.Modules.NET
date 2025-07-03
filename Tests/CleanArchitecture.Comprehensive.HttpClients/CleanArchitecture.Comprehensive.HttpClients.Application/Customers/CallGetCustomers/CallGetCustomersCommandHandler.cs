using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.CallGetCustomers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CallGetCustomersCommandHandler : IRequestHandler<CallGetCustomersCommand>
    {
        private readonly ICustomersService _customersService;

        [IntentManaged(Mode.Merge)]
        public CallGetCustomersCommandHandler(ICustomersService customersService)
        {
            _customersService = customersService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CallGetCustomersCommand request, CancellationToken cancellationToken)
        {
            var result = await _customersService.GetCustomersAsync(cancellationToken);
        }
    }
}