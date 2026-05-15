using AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Customers;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories.Sales;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.GetCustomersLean
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersLeanQueryHandler : IRequestHandler<GetCustomersLeanQuery, List<CustomerLeanDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly CustomerLeanDtoMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomersLeanQueryHandler(ICustomerRepository customerRepository, CustomerLeanDtoMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerLeanDto>> Handle(GetCustomersLeanQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.FindAllAsync(cancellationToken);
            return _mapper.CustomerToCustomerLeanDtoList(customers);
        }
    }
}