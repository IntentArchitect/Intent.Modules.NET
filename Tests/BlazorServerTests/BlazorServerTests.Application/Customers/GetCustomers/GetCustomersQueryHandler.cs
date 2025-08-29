using AutoMapper;
using BlazorServerTests.Application.Common.Pagination;
using BlazorServerTests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using static System.Linq.Dynamic.Core.DynamicQueryableExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace BlazorServerTests.Application.Customers.GetCustomers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResult<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomersQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            //[IntentIgnore]
            var customers = await _customerRepository.FindAllAsync(
                c => (request.IsActive == null || c.IsActive == request.IsActive) && (string.IsNullOrEmpty(request.SearchTerm) || c.Name.Contains(request.SearchTerm)),
                request.PageNo, 
                request.PageSize, 
                queryOptions => queryOptions.OrderBy(request.OrderBy ?? "Id"), 
                cancellationToken);
            return customers.MapToPagedResult(x => x.MapToCustomerDto(_mapper));
        }
    }
}