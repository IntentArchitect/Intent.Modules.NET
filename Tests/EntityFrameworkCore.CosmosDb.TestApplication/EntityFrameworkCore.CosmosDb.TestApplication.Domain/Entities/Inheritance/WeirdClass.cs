using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Inheritance
{
    public class WeirdClass : Composite, IHasDomainEvent
    {
        public WeirdClass()
        {
            WeirdField = null!;
        }

        public string WeirdField { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}