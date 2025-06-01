using AutoMapper;
using EntityFramework.Application.LinqExtensions.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFramework.Application.LinqExtensions.Tests.Application.Customers.GetCustomersWithStatus
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersWithStatusQueryHandler : IRequestHandler<GetCustomersWithStatusQuery, List<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomersWithStatusQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerDto>> Handle(
            GetCustomersWithStatusQuery request,
            CancellationToken cancellationToken)
        {
            // [IntentIgnore]
            var customers = await _customerRepository.FindAllAsync(c => c.IsActive == request.IsActive, o => o.AsNoTracking(),  cancellationToken);
            return customers.MapToCustomerDtoList(_mapper);
        }
    }
}