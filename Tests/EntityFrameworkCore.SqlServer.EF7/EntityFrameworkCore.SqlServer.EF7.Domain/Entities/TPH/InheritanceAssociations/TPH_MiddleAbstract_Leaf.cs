using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPH.InheritanceAssociations
{
    public class TPH_MiddleAbstract_Leaf : TPH_MiddleAbstract_Middle
    {
        public string LeafAttribute { get; set; }
    }
}