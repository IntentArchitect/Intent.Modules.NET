using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Customers.GetCustomerById
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