using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Events
{
    public class StockBaseEvent : DomainEvent
    {
        public StockBaseEvent(int total)
        {
            Total = total;
        }

        public int Total { get; }
    }
}