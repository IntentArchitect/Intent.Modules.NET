using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.NestedAssociations
{
    public class Sun
    {
        public Guid Id { get; set; }

        public float Temp { get; set; }
    }
}