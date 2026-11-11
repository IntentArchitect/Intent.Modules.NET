using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Indexes
{
    public class DeviationIndex
    {
        public DeviationIndex()
        {
            AttributeName = null!;
        }

        public Guid Id { get; set; }

        public string AttributeName { get; set; }
    }
}