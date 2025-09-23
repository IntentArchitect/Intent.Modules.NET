using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Events
{
    public class StockLevelUpdatedEvent : DomainEvent
    {
        public StockLevelUpdatedEvent(Guid id, int total, DateTime dateUpdated)
        {
            Id = id;
            Total = total;
            DateUpdated = dateUpdated;
        }

        public Guid Id { get; }
        public int Total { get; }
        public DateTime DateUpdated { get; }
    }
}