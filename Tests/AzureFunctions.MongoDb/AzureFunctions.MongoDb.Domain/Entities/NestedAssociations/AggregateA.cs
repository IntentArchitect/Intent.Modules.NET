using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.NestedAssociations
{
    public class AggregateA
    {
        public AggregateA()
        {
            Id = null!;
            Attribute = null!;
            NestedCompositionA = null!;
        }

        public string Id { get; set; }

        public string Attribute { get; set; }

        public NestedCompositionA NestedCompositionA { get; set; }
    }
}