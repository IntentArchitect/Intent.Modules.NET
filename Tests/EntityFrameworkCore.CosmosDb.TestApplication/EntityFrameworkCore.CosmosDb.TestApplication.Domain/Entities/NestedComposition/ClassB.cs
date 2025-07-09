using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.NestedComposition
{
    public class ClassB
    {
        public ClassB()
        {
            ClassBAttr = null!;
            ClassC = null!;
        }

        public Guid Id { get; set; }

        public string ClassBAttr { get; set; }

        public Guid ClassAId { get; set; }

        public virtual ClassC ClassC { get; set; }

        public virtual ICollection<ClassD> ClassDS { get; set; } = [];
    }
}