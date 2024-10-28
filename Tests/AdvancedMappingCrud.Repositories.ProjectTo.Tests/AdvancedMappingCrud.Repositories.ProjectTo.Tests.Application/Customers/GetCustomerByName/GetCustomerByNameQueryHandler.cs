using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Customers.GetCustomerByName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerByNameQueryHandler : IRequestHandler<GetCustomerByNameQuery, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public GetCustomerByNameQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> Handle(GetCustomerByNameQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindProjectToAsync<CustomerDto>(x => x.Name == request.Name, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Name}'");
            }
            return customer;
        }
    }
}