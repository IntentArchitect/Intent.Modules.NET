using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Customers.GetCustomerBySearch
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerBySearchQueryHandler : IRequestHandler<GetCustomerBySearchQuery, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public GetCustomerBySearchQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> Handle(GetCustomerBySearchQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Customer> FilterCustomers(IQueryable<Customer> queryable)
            {
                if (request.Name != null)
                {
                    queryable = queryable.Where(x => x.Name == request.Name);
                }

                if (request.Email != null)
                {
                    queryable = queryable.Where(x => x.Email == request.Email);
                }
                return queryable;
            }

            var customer = await _customerRepository.FindProjectToAsync<CustomerDto>(FilterCustomers, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '({request.Name}, {request.Email})'");
            }
            return customer;
        }
    }
}