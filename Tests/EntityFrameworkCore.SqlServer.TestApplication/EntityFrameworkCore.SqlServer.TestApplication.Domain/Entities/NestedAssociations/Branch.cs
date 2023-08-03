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
    public class Branch
    {
        [IntentManaged(Mode.Fully)]
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

        public virtual Texture Texture { get; set; }

        public virtual Internode Internode { get; set; }

        public virtual ICollection<Inhabitant> Inhabitants { get; set; } = new List<Inhabitant>();

        public virtual ICollection<Leaf> Leaves { get; set; } = new List<Leaf>();

        public Guid TreeId { get; set; }

        public virtual Tree Tree { get; set; }
    }
}