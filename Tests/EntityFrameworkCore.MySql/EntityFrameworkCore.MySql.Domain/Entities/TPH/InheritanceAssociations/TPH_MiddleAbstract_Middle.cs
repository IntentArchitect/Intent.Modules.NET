using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.TPH.InheritanceAssociations
{
    public abstract class TPH_MiddleAbstract_Middle : TPH_MiddleAbstract_Root
    {
        public string MiddleAttribute { get; set; }
    }
}