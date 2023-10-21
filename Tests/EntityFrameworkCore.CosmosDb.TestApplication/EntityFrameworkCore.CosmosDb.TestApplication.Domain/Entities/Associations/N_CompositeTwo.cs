using System;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Associations
{
    public class N_CompositeTwo
    {
        public Guid Id { get; set; }

        public string PartitionKey { get; set; }

        public string CompositeTwoAttr { get; set; }
    }
}