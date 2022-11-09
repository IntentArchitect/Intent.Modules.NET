using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.NestedAssociations
{
    public partial class Leaf : ILeaf
    {
        public Guid Id { get; set; }

        public string LeafAttribute { get; set; }

        public Guid BranchId { get; set; }
    }
}