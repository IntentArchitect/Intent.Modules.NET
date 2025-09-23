using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.GetAddressById
{
    public class GetAddressByIdQuery : IRequest<AddressDto>, IQuery
    {
        public GetAddressByIdQuery(Guid customerId, Guid id)
        {
            CustomerId = customerId;
            Id = id;
        }

        public Guid CustomerId { get; set; }
        public Guid Id { get; set; }
    }
}