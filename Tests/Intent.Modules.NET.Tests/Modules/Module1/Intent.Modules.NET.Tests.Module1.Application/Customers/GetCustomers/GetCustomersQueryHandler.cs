using AutoMapper;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Customers;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Customers.GetCustomers;
using Intent.Modules.NET.Tests.Module1.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Customers.GetCustomers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<CustomerDto>>
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
        public async Task<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.FindAllAsync(cancellationToken);
            return customers.MapToCustomerDtoList(_mapper);
        }
    }
}