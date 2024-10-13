using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Services
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OrderDomainService : IOrderDomainService
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public OrderDomainService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void UpdateLineItems(Guid orderId, IEnumerable<OrderItemUpdateDC> orderLineItems)
        {
            // TODO: Implement UpdateLineItems (OrderDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}