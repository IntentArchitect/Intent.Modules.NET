using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Domain.Common.Exceptions;
using MudBlazor.ExampleApp.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Customers.GetCustomerById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}'");
            }
            return customer.MapToCustomerDto(_mapper);
        }
    }
}