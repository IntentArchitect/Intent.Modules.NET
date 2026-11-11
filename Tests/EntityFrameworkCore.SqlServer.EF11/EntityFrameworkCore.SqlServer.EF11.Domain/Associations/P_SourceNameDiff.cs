using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Associations
{
    public class P_SourceNameDiff
    {
        public Guid Id { get; set; }

        public virtual ICollection<P_SourceNameDiffDependent> P_SourceNameDiffDependents { get; set; } = [];
    }
}