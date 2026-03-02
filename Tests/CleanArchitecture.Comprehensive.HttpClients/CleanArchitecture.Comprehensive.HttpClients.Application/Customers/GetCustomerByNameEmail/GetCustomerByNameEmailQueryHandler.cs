using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.GetCustomerByNameEmail
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerByNameEmailQueryHandler : IRequestHandler<GetCustomerByNameEmailQuery, List<CustomerDto>>
    {
        private readonly ICustomersService _customersService;

        [IntentManaged(Mode.Merge)]
        public GetCustomerByNameEmailQueryHandler(ICustomersService customersService)
        {
            _customersService = customersService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerDto>> Handle(
            GetCustomerByNameEmailQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _customersService.GetCustomerByNameEmailAsync(new IntegrationServices.Contracts.Services.Customers.GetCustomerByNameEmailQuery
            {
                Name = request.Name,
                Email = request.Email
            }, cancellationToken);

            // TODO: Implement return type mapping...
            throw new NotImplementedException("Implement return type mapping...");
        }
    }
}