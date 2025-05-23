using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.GetCustomerByName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerByNameQueryHandler : IRequestHandler<GetCustomerByNameQuery, List<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomerByNameQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerDto>> Handle(GetCustomerByNameQuery request, CancellationToken cancellationToken)
        {
            IQueryable<ICustomerDocument> FilterCustomers(IQueryable<ICustomerDocument> queryable)
            {
                queryable = queryable.Where(x => x.Surname == request.Surname && x.IsActive);

                if (request.Name != null)
                {
                    queryable = queryable.Where(x => x.Name == request.Name);
                }
                return queryable;
            }

            var entity = await _customerRepository.FindAllAsync(FilterCustomers, cancellationToken);
            return entity.MapToCustomerDtoList(_mapper);
        }
    }
}