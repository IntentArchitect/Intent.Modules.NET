using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Common.Exceptions;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.UpdateAddress
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateAddressCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.CustomerId}'");
            }

            var address = customer.Addresses.FirstOrDefault(x => x.Id == request.Id);
            if (address is null)
            {
                throw new NotFoundException($"Could not find Address '{request.Id}'");
            }

            address.Line1 = request.Line1;
            address.Line2 = request.Line2;
            address.City = request.City;
            address.Province = request.Province;
            address.PostalCode = request.PostalCode;
            address.Country = request.Country;
            address.CustomerId = request.CustomerId;
        }
    }
}