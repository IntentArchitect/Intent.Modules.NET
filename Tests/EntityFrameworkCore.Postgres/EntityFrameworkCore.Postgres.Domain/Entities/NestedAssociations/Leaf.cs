using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.NestedAssociations
{
    public class Leaf
    {
        public Guid Id { get; set; }

        public string LeafAttribute { get; set; }

        public Guid SunId { get; set; }

        public Guid BranchId { get; set; }

        public virtual ICollection<Worm> Worms { get; set; } = new List<Worm>();

        public virtual Sun Sun { get; set; }
    }
}