using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class N_CompositeTwo
    {
        public N_CompositeTwo()
        {
            PartitionKey = null!;
            CompositeTwoAttr = null!;
        }
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string CompositeTwoAttr { get; set; }
    }
}