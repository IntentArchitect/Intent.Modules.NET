using EfCoreSoftDelete.Domain;
using EfCoreSoftDelete.Domain.Entities;
using EfCoreSoftDelete.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EfCoreSoftDelete.Application.Customers.CreateCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = new Customer
            {
                Name = request.Name,
                OtherAddresses = request.OtherAddresses
                    .Select(oa => new Address(
                        line1: oa.Line1,
                        line2: oa.Line2,
                        city: oa.City,
                        postal: oa.Postal,
                        otherBuildings: oa.OtherBuildings
                            .Select(ob => new AddressBuilding(
                                name: ob.Name))
                            .ToList(),
                        primaryBuilding: new AddressBuilding(
                            name: oa.PrimaryBuilding.Name)))
                    .ToList(),
                PrimaryAddress = new Address(
                    line1: request.PrimaryAddress.Line1,
                    line2: request.PrimaryAddress.Line2,
                    city: request.PrimaryAddress.City,
                    postal: request.PrimaryAddress.Postal,
                    otherBuildings: request.OtherBuildings
                        .Select(ob => new AddressBuilding(
                            name: ob.Name))
                        .ToList(),
                    primaryBuilding: new AddressBuilding(
                        name: request.PrimaryBuilding.Name))
            };

            _customerRepository.Add(customer);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return customer.Id;
        }
    }
}