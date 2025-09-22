using AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Customers;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Common.Exceptions;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.GetAddresses
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetAddressesQueryHandler : IRequestHandler<GetAddressesQuery, List<AddressDto>>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public GetAddressesQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<AddressDto>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.CustomerId}'");
            }

            var addresses = customer.Addresses;
            var mapper = new AddressDtoMapper();
            return mapper.AddressToAddressDtoList(addresses.ToList());
        }
    }
}