using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.GetCustomersLinq
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersLinqQueryHandler : IRequestHandler<GetCustomersLinqQuery, List<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomersLinqQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerDto>> Handle(GetCustomersLinqQuery request, CancellationToken cancellationToken)
        {
            // IntentIgnore
            var customers = await _customerRepository.FindAllAsync(queryOptions => queryOptions.OrderBy(c => c.Name), cancellationToken);
            return customers.MapToCustomerDtoList(_mapper);
        }
    }
}