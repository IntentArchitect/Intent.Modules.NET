using CleanArchitecture.Comprehensive.HttpClients.Application.Common.Pagination;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices;
using CleanArchitecture.Comprehensive.HttpClients.Application.IntegrationServices.Contracts.Services.Customers;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Customers.GetCustomersPaged
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersPagedQueryHandler : IRequestHandler<GetCustomersPagedQuery, PagedResult<CustomerDto>>
    {
        private readonly ICustomersService _customersService;

        [IntentManaged(Mode.Merge)]
        public GetCustomersPagedQueryHandler(ICustomersService customersService)
        {
            _customersService = customersService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<CustomerDto>> Handle(
            GetCustomersPagedQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _customersService.GetCustomersPagedAsync(new IntegrationServices.Contracts.Services.Customers.GetCustomersPagedQuery
            {
                PageNo = request.PageNo,
                PageSize = request.PageSize
            }, cancellationToken);

            // TODO: Implement return type mapping...
            throw new NotImplementedException("Implement return type mapping...");
        }
    }
}