using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.TPC.InheritanceAssociations
{
    public abstract class TPC_AbstractBaseClass
    {
        public TPC_AbstractBaseClass()
        {
            BaseAttribute = null!;
        }
        public string BaseAttribute { get; set; }
    }
}