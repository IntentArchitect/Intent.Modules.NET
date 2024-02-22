using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.FindCustomerByName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FindCustomerByNameQueryHandler : IRequestHandler<FindCustomerByNameQuery, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public FindCustomerByNameQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> Handle(FindCustomerByNameQuery request, CancellationToken cancellationToken)
        {
            var entity = await _customerRepository.FindAsync(x => x.Name == request.Name, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Name}'");
            }
            return entity.MapToCustomerDto(_mapper);
        }
    }
}