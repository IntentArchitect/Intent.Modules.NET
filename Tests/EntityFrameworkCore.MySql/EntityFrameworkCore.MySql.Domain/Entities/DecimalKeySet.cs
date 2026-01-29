using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities
{
    public class DecimalKeySet
    {
        public decimal Id { get; set; }
    }
}