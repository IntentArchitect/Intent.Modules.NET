using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.NestedAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Leaf
    {
        [IntentManaged(Mode.Fully)]
        public Leaf()
        {
            LeafAttribute = null!;
            Sun = null!;
        }
        public Guid Id { get; set; }

        public string LeafAttribute { get; set; }

        public Guid SunId { get; set; }

        public virtual ICollection<Worm> Worms { get; set; } = new List<Worm>();

        public virtual Sun Sun { get; set; }

        public Guid BranchId { get; set; }
    }
}