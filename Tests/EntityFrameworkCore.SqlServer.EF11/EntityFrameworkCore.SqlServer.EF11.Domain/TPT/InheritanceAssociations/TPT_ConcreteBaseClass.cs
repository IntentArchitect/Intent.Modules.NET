using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.TPT.InheritanceAssociations
{
    public class TPT_ConcreteBaseClass
    {
        public TPT_ConcreteBaseClass()
        {
            BaseAttribute = null!;
        }

        public Guid Id { get; set; }

        public string BaseAttribute { get; set; }
    }
}