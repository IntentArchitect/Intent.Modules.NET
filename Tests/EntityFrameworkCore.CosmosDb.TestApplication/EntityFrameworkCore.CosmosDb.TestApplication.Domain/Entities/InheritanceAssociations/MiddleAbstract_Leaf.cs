using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.CosmosDb.TestApplication.Domain.Entities.InheritanceAssociations
{
    public class MiddleAbstract_Leaf : MiddleAbstract_Middle
    {
        public MiddleAbstract_Leaf()
        {
            LeafAttribute = null!;
        }

        public string LeafAttribute { get; set; }
    }
}