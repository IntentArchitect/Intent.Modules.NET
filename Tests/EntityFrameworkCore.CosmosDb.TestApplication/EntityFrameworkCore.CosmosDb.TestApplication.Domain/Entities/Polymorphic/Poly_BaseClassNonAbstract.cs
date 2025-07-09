using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Polymorphic
{
    public class Poly_BaseClassNonAbstract
    {
        public Poly_BaseClassNonAbstract()
        {
            BaseField = null!;
            PartitionKey = null!;
        }

        public Guid Id { get; set; }

        public string BaseField { get; set; }

        public string PartitionKey { get; set; }
    }
}