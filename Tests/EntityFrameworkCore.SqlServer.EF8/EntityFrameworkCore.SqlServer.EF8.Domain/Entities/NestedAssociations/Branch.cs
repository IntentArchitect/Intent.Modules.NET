using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF8.Domain.Entities.NestedAssociations
{
    public class Branch
    {
        public Branch()
        {
            BranchAttribute = null!;
            Texture = null!;
            Internode = null!;
            Tree = null!;
        }
        public Guid Id { get; set; }

        public string BranchAttribute { get; set; }

        public Guid TextureId { get; set; }

        public Guid TreeId { get; set; }

        public virtual Texture Texture { get; set; }

        public virtual Internode Internode { get; set; }

        public virtual ICollection<Inhabitant> Inhabitants { get; set; } = [];

        public virtual ICollection<Leaf> Leaves { get; set; } = [];

        public virtual Tree Tree { get; set; }
    }
}