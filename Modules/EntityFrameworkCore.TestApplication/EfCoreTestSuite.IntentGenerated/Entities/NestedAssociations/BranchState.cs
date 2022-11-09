using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.NestedAssociations
{
    public partial class Branch : IBranch
    {
        public Guid Id { get; set; }

        public string BranchAttribute { get; set; }

        public Guid TextureId { get; set; }

        public virtual Texture Texture { get; set; }

        ITexture IBranch.Texture
        {
            get => Texture;
            set => Texture = (Texture)value;
        }

        public virtual Internode Internode { get; set; }

        IInternode IBranch.Internode
        {
            get => Internode;
            set => Internode = (Internode)value;
        }

        public virtual ICollection<Inhabitant> Inhabitants { get; set; } = new List<Inhabitant>();

        ICollection<IInhabitant> IBranch.Inhabitants
        {
            get => Inhabitants.CreateWrapper<IInhabitant, Inhabitant>();
            set => Inhabitants = value.Cast<Inhabitant>().ToList();
        }

        public virtual ICollection<Leaf> Leaves { get; set; } = new List<Leaf>();

        ICollection<ILeaf> IBranch.Leaves
        {
            get => Leaves.CreateWrapper<ILeaf, Leaf>();
            set => Leaves = value.Cast<Leaf>().ToList();
        }

        public Guid TreeId { get; set; }

        public virtual Tree Tree { get; set; }

        ITree IBranch.Tree
        {
            get => Tree;
            set => Tree = (Tree)value;
        }
    }
}