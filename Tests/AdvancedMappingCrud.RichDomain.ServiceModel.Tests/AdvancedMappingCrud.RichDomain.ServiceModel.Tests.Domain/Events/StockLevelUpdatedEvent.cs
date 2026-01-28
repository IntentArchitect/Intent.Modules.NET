using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Events
{
    public class StockLevelUpdatedEvent : StockBaseEvent
    {
        public StockLevelUpdatedEvent(Guid id, DateTime dateUpdated, int total) : base(total)
        {
            Id = id;
            DateUpdated = dateUpdated;
        }

        public Guid Id { get; }
        public DateTime DateUpdated { get; }
    }
}