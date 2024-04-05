using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.TPC.InheritanceAssociations
{
    public abstract class TPC_AbstractBaseClass
    {
        public string BaseAttribute { get; set; }
    }
}