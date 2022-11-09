using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.NestedAssociations
{
    public interface IBranch
    {
        string BranchAttribute { get; set; }

        ITexture Texture { get; set; }

        IInternode Internode { get; set; }

        ICollection<IInhabitant> Inhabitants { get; set; }

        ICollection<ILeaf> Leaves { get; set; }

        ITree Tree { get; set; }
    }
}