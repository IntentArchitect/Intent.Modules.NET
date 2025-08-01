using System;
using System.Collections.Generic;
using EntityFrameworkCore.CosmosDb.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Polymorphic
{
    public class Poly_SecondLevel : IHasDomainEvent
    {
        public Poly_SecondLevel()
        {
            SecondField = null!;
            PartitionKey = null!;
            BaseClassNonAbstracts = null!;
        }

        public Guid Id { get; set; }

        public string SecondField { get; set; }

        public string PartitionKey { get; set; }

        public virtual Poly_BaseClassNonAbstract BaseClassNonAbstracts { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}