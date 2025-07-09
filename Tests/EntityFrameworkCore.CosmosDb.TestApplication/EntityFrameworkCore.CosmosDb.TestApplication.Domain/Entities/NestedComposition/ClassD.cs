using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.NestedComposition
{
    public class ClassD
    {
        public ClassD()
        {
            ClassE = null!;
        }

        public Guid Id { get; set; }

        public Guid ClassBId { get; set; }

        public virtual ClassE ClassE { get; set; }
    }
}