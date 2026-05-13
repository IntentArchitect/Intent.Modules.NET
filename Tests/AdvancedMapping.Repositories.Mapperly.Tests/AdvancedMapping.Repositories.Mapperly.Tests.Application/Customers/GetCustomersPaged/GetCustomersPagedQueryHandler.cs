using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Pagination;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Customers;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories.Sales;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using static System.Linq.Dynamic.Core.DynamicQueryableExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.GetCustomersPaged
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersPagedQueryHandler : IRequestHandler<GetCustomersPagedQuery, PagedResult<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly CustomerDtoMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomersPagedQueryHandler(ICustomerRepository customerRepository, CustomerDtoMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<CustomerDto>> Handle(
            GetCustomersPagedQuery request,
            CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.FindAllAsync(request.PageNo, request.PageSize, queryOptions => queryOptions.OrderBy(request.OrderBy ?? "Id"), cancellationToken);
            return customers.MapToPagedResult(x => _mapper.CustomerToCustomerDto(x));
        }
    }
}