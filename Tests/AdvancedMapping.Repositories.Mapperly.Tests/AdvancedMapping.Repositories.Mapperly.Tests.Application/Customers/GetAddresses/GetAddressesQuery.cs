using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.GetAddresses
{
    public class GetAddressesQuery : IRequest<List<AddressDto>>, IQuery
    {
        public GetAddressesQuery(Guid customerId)
        {
            CustomerId = customerId;
        }

        public Guid CustomerId { get; set; }
    }
}