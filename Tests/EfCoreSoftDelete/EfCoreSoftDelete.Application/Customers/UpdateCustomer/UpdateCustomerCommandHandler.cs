using EfCoreSoftDelete.Domain;
using EfCoreSoftDelete.Domain.Common;
using EfCoreSoftDelete.Domain.Common.Exceptions;
using EfCoreSoftDelete.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EfCoreSoftDelete.Application.Customers.UpdateCustomer
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
            var customer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}'");
            }

            customer.Name = request.Name;
            customer.OtherAddresses = UpdateHelper.CreateOrUpdateCollection(customer.OtherAddresses, request.OtherAddresses, (e, d) => e.Equals(new Address(
                line1: d.Line1,
                line2: d.Line2,
                city: d.City,
                postal: d.Postal,
                otherBuildings: d.OtherBuildings
                    .Select(ob => new AddressBuilding(
                        name: ob.Name))
                    .ToList(),
                primaryBuilding: new AddressBuilding(
                    name: d.PrimaryBuilding.Name))), CreateOrUpdateAddress);
            customer.PrimaryAddress = new Address(
                line1: request.PrimaryAddress.Line1,
                line2: request.PrimaryAddress.Line2,
                city: request.PrimaryAddress.City,
                postal: request.PrimaryAddress.Postal,
                otherBuildings: request.OtherBuildings
                    .Select(ob => new AddressBuilding(
                        name: ob.Name))
                    .ToList(),
                primaryBuilding: new AddressBuilding(
                    name: request.PrimaryBuilding.Name));
        }

        [IntentManaged(Mode.Fully)]
        private static Address CreateOrUpdateAddress(Address? valueObject, UpdateCustomerCommandOtherAddressesDto dto)
        {
            if (valueObject is null)
            {
                return new Address(
                    line1: dto.Line1,
                    line2: dto.Line2,
                    city: dto.City,
                    postal: dto.Postal,
                    otherBuildings: dto.OtherBuildings
                        .Select(ob => new AddressBuilding(
                            name: ob.Name))
                        .ToList(),
                    primaryBuilding: new AddressBuilding(
                        name: dto.PrimaryBuilding.Name));
            }
            return valueObject;
        }
    }
}