using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.NestedAssociations
{
    public class NestedCompositionA
    {
        public NestedCompositionA()
        {
            Attribute = null!;
            AggregateBId = null!;
        }

        public string Attribute { get; set; }

        public string AggregateBId { get; set; }
    }
}