using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Entities
{
    public class DecimalKeySet
    {
        public decimal Id { get; set; }
    }
}