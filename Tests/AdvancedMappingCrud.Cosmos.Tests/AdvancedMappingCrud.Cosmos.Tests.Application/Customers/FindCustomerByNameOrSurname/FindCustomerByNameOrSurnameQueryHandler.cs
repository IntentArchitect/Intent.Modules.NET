using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.FindCustomerByNameOrSurname
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FindCustomerByNameOrSurnameQueryHandler : IRequestHandler<FindCustomerByNameOrSurnameQuery, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public FindCustomerByNameOrSurnameQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> Handle(
            FindCustomerByNameOrSurnameQuery request,
            CancellationToken cancellationToken)
        {
            // IntentIgnore
            IQueryable<ICustomerDocument> FilterCustomers(IQueryable<ICustomerDocument> queryable)
            {
                if (request.Name != null)
                {
                    queryable = queryable.Where(x => x.Name == request.Name);
                }

                if (request.Surname != null)
                {
                    queryable = queryable.Where(x => x.Surname == request.Surname);
                }
                return queryable;
            }

            var entity = await _customerRepository.FindAsync(FilterCustomers, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException($"Could not find Customer '({request.Name}, {request.Surname})'");
            }
            return entity.MapToCustomerDto(_mapper);
        }
    }
}