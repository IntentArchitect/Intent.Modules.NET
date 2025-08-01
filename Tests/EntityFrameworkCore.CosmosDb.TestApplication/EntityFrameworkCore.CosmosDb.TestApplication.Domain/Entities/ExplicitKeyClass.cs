using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities
{
    public class ExplicitKeyClass : IHasDomainEvent
    {
        public ExplicitKeyClass()
        {
            Attribute = null!;
        }

        public Guid Id { get; set; }

        public string Attribute { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}