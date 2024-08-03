using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.NestedAssociations
{
    public class Branch
    {
        public Guid Id { get; set; }

        public string BranchAttribute { get; set; }

        public Guid TextureId { get; set; }

        public Guid TreeId { get; set; }

        public virtual Texture Texture { get; set; }

        public virtual Internode Internode { get; set; }

        public virtual ICollection<Inhabitant> Inhabitants { get; set; } = new List<Inhabitant>();

        public virtual ICollection<Leaf> Leaves { get; set; } = new List<Leaf>();

        public virtual Tree Tree { get; set; }
    }
}