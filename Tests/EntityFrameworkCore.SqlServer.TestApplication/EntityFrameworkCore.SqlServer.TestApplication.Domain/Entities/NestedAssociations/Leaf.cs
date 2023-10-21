using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.NestedAssociations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Leaf
    {
        public Guid Id { get; set; }

        public string LeafAttribute { get; set; }

        public Guid SunId { get; set; }

        public virtual ICollection<Worm> Worms { get; set; } = new List<Worm>();

        public virtual Sun Sun { get; set; }

        public Guid BranchId { get; set; }
    }
}