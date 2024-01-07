using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Pagination;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomersPaginated
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersPaginatedQueryHandler : IRequestHandler<GetCustomersPaginatedQuery, PagedResult<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomersPaginatedQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<CustomerDto>> Handle(
            GetCustomersPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.FindAllAsync(x => x.Name == request.Name && x.Surname == request.Surname && x.IsActive == request.IsActive, request.PageNo, request.PageSize, cancellationToken);
            return customers.MapToPagedResult(x => x.MapToCustomerDto(_mapper));
        }
    }
}