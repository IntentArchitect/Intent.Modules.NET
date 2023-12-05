using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.Inheritance
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Derived : Base
    {
        public string DerivedField1 { get; set; }

        public Guid AssociatedId { get; set; }

        public virtual Associated Associated { get; set; }

        public virtual ICollection<Composite> Composites { get; set; } = new List<Composite>();
    }
}