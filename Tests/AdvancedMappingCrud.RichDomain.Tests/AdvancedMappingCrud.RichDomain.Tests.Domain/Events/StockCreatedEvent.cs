using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Events
{
    public class StockCreatedEvent : StockBaseEvent
    {
        public StockCreatedEvent(string name, string addedUser, int total) : base(total)
        {
            Name = name;
            AddedUser = addedUser;
        }

        public string Name { get; }
        public string AddedUser { get; }
    }
}