using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Pagination;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using static System.Linq.Dynamic.Core.DynamicQueryableExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.GetCustomersPagedFiltered
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersPagedFilteredQueryHandler : IRequestHandler<GetCustomersPagedFilteredQuery, PagedResult<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomersPagedFilteredQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<CustomerDto>> Handle(
            GetCustomersPagedFilteredQuery request,
            CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.FindAllAsync(x => x.IsActive == request.IsActive, request.PageNo, request.PageSize, queryOptions => queryOptions.OrderBy(request.OrderBy ?? "Id"), cancellationToken);
            return customers.MapToPagedResult(x => x.MapToCustomerDto(_mapper));
        }
    }
}