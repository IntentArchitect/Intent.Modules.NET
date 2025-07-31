using AutoMapper;
using EfCoreSoftDelete.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EfCoreSoftDelete.Application.Customers.GetDeletedCustomerById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDeletedCustomerByIdQueryHandler : IRequestHandler<GetDeletedCustomerByIdQuery, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetDeletedCustomerByIdQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> Handle(GetDeletedCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _customerRepository.FindDeletedByIdAsync(request.Id, cancellationToken);
            return result.MapToCustomerDto(_mapper);
        }
    }
}