using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.TPH.InheritanceAssociations
{
    public class TPH_MiddleAbstract_Leaf : TPH_MiddleAbstract_Middle
    {
        public TPH_MiddleAbstract_Leaf()
        {
            LeafAttribute = null!;
        }
        public string LeafAttribute { get; set; }
    }
}