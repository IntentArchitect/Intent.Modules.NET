using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Events
{
    public class StockCreatedEvent : DomainEvent
    {
        public StockCreatedEvent(string name, int total, string addedUser)
        {
            Name = name;
            Total = total;
            AddedUser = addedUser;
        }

        public string Name { get; }
        public int Total { get; }
        public string AddedUser { get; }
    }
}