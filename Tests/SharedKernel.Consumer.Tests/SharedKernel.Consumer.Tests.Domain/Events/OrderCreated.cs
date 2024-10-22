using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using SharedKernel.Consumer.Tests.Domain.Common;
using SharedKernel.Consumer.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Domain.Events
{
    public class OrderCreated : DomainEvent
    {
        public OrderCreated(Order order)
        {
            Order = order;
        }

        public Order Order { get; }
    }
}