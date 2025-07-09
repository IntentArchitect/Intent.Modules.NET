using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.NestedComposition
{
    public class ClassC
    {
        public ClassC()
        {
            ClassCAttr = null!;
        }

        public Guid Id { get; set; }

        public string ClassCAttr { get; set; }
    }
}