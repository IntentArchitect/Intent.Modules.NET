using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.Sample.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace MudBlazor.Sample.Application.Customers.GetCustomersLookup
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersLookupQueryHandler : IRequestHandler<GetCustomersLookupQuery, List<CustomerLookupDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomersLookupQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerLookupDto>> Handle(
            GetCustomersLookupQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _customerRepository.FindAllAsync(cancellationToken);
            return entity.MapToCustomerLookupDtoList(_mapper);
        }
    }
}