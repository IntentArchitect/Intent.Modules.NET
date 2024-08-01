using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MySql.Domain.Entities.Indexes
{
    public class WithBaseIndex : WithBaseIndexBase
    {
        public string Name { get; set; }
    }
}