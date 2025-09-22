using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Common.Exceptions;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Entities;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.CreateAddress
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public CreateAddressCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.CustomerId}'");
            }
            var address = new Address
            {
                Line1 = request.Line1,
                Line2 = request.Line2,
                City = request.City,
                Province = request.Province,
                PostalCode = request.PostalCode,
                Country = request.Country,
                CustomerId = request.CustomerId
            };

            customer.Addresses.Add(address);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return address.Id;
        }
    }
}