using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Associations
{
    public class O_DestNameDiff
    {
        public Guid Id { get; set; }

        public virtual ICollection<O_DestNameDiffDependent> DestNameDiff { get; set; } = [];
    }
}