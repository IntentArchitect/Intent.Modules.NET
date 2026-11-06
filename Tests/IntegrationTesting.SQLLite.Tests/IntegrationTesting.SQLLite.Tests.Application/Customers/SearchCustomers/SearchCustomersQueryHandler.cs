using AutoMapper;
using IntegrationTesting.SQLLite.Tests.Application.Mappings.Customers;
using IntegrationTesting.SQLLite.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace IntegrationTesting.SQLLite.Tests.Application.Customers.SearchCustomers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SearchCustomersQueryHandler : IRequestHandler<SearchCustomersQuery, List<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public SearchCustomersQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// HTTP QUERY endpoint: the search criteria travel in the request body, and the request stays safe and idempotent.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<CustomerDto>> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.FindAllAsync(c => c.LastName == request.LastName, cancellationToken);
            return customers.MapToCustomerDtoList(_mapper);
        }
    }
}
