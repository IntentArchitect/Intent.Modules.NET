using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.CallGetCustomerExtraFieldsQuery
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CallGetCustomerExtraFieldsQueryHandler : IRequestHandler<CallGetCustomerExtraFieldsQuery, List<CustomerDto>>
    {
        private readonly ICustomersService _customersService;

        [IntentManaged(Mode.Merge)]
        public CallGetCustomerExtraFieldsQueryHandler(ICustomersService customersService)
        {
            _customersService = customersService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerDto>> Handle(
            CallGetCustomerExtraFieldsQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _customersService.GetCustomerExtraFieldsAsync(
                new GetCustomerExtraFieldsQuery
                {
                    Id = request.Id,
                    Field1 = request.Field1,
                    Field2 = request.Field2
                }, cancellationToken);

            // TODO: Implement return type mapping...
            throw new NotImplementedException("Implement return type mapping...");
        }
    }
}