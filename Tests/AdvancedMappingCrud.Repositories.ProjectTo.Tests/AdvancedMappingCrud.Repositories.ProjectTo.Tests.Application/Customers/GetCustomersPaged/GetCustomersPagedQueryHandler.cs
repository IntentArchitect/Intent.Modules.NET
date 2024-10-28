using System;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Common.Pagination;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Customers.GetCustomersPaged
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersPagedQueryHandler : IRequestHandler<GetCustomersPagedQuery, Common.Pagination.PagedResult<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public GetCustomersPagedQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Common.Pagination.PagedResult<CustomerDto>> Handle(
            GetCustomersPagedQuery request,
            CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.FindAllProjectToAsync<CustomerDto>(request.PageNo, request.PageSize, queryOptions => queryOptions.OrderBy(request.OrderBy ?? "Id"), cancellationToken);
            return customers.MapToPagedResult();
        }
    }
}