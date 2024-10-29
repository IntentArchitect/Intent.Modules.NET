using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AzureFunctions.NET6.Application.Common.Pagination;
using AzureFunctions.NET6.Domain.Repositories;
using AzureFunctions.NET8.Application.Common.Pagination;
using AzureFunctions.NET8.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AzureFunctions.NET8.Application.Customers.GetCustomersPaged
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersPagedQueryHandler : IRequestHandler<GetCustomersPagedQuery, PagedResult<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetCustomersPagedQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<CustomerDto>> Handle(
            GetCustomersPagedQuery request,
            CancellationToken cancellationToken)
        {
            var results = await _customerRepository.FindAllAsync(
                pageNo: request.PageNo,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);
            return results.MapToPagedResult(x => x.MapToCustomerDto(_mapper));
        }
    }
}