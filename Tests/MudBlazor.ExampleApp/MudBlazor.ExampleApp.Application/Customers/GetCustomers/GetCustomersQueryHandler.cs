using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Application.Common.Pagination;
using MudBlazor.ExampleApp.Domain.Entities;
using MudBlazor.ExampleApp.Domain.Repositories;
using static System.Linq.Dynamic.Core.DynamicQueryableExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Customers.GetCustomers
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
            IQueryable<Customer> FilterCustomers(IQueryable<Customer> queryable)
            {
                if (request.SearchText != null)
                {
                    queryable = queryable.Where(x => x.Name == request.SearchText);
                }
                return queryable;
            }

            var customers = await _customerRepository.FindAllAsync(request.PageNo, request.PageSize, q => FilterCustomers(q).OrderBy(request.OrderBy ?? "Id"), cancellationToken);
            return customers.MapToPagedResult(x => x.MapToCustomerDto(_mapper));
        }
    }
}