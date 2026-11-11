using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.TPT.InheritanceAssociations
{
    public abstract class TPT_AbstractBaseClass
    {
        public TPT_AbstractBaseClass()
        {
            BaseAttribute = null!;
        }

        public Guid Id { get; set; }

        public string BaseAttribute { get; set; }
    }
}