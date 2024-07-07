using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomersWithParams
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersWithParamsQueryHandler : IRequestHandler<GetCustomersWithParamsQuery, List<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomersWithParamsQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerDto>> Handle(
            GetCustomersWithParamsQuery request,
            CancellationToken cancellationToken)
        {
            IQueryable<Customer> FilterCustomers(IQueryable<Customer> queryable)
            {
                queryable = queryable.Where(x => x.IsActive == request.IsActive);

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

            var customers = await _customerRepository.FindAllAsync(FilterCustomers, cancellationToken);
            return customers.MapToCustomerDtoList(_mapper);
        }
    }
}