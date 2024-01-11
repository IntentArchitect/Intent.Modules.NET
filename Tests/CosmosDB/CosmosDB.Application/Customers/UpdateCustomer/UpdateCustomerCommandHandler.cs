using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.Customers.UpdateCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var existingCustomer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingCustomer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}'");
            }

            existingCustomer.Name = request.Name;
            existingCustomer.DeliveryAddress = CreateAddress(request.DeliveryAddress);

            _customerRepository.Update(existingCustomer);
        }

        [IntentManaged(Mode.Fully)]
        public static Address CreateAddress(UpdateCustomerAddressDto dto)
        {
            return new Address(line1: dto.Line1, line2: dto.Line2, city: dto.City, postalAddress: dto.PostalAddress);
        }
    }
}