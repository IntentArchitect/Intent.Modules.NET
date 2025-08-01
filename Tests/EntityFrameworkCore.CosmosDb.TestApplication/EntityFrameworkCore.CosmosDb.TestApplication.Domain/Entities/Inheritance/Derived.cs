using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Inheritance
{
    public class Derived : Base
    {
        public Derived()
        {
            DerivedField1 = null!;
            Associated = null!;
        }

        public string DerivedField1 { get; set; }

        public Guid AssociatedId { get; set; }

        public virtual Associated Associated { get; set; }

        public virtual ICollection<Composite> Composites { get; set; } = [];
    }
}