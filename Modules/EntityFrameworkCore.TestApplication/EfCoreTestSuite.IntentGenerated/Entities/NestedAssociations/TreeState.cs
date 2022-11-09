using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.NestedAssociations
{
    public partial class Tree : ITree
    {
        public Guid Id { get; set; }

        public string TreeAttribute { get; set; }

        public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

        ICollection<IBranch> ITree.Branches
        {
            get => Branches.CreateWrapper<IBranch, Branch>();
            set => Branches = value.Cast<Branch>().ToList();
        }
    }
}