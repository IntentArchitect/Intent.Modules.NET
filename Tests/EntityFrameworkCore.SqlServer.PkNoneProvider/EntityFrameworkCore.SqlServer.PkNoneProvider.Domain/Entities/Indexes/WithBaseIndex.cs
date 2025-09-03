using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities.Indexes
{
    public class WithBaseIndex : WithBaseIndexBase
    {
        public WithBaseIndex()
        {
            Name = null!;
        }

        public string Name { get; set; }
    }
}