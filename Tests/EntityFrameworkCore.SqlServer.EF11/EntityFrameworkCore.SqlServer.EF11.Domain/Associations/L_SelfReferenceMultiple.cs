using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Associations
{
    public class L_SelfReferenceMultiple
    {
        public L_SelfReferenceMultiple()
        {
            SelfRefMultipleAttr = null!;
        }

        public Guid Id { get; set; }

        public string SelfRefMultipleAttr { get; set; }

        public virtual ICollection<L_SelfReferenceMultiple> L_SelfReferenceMultiplesDst { get; set; } = [];
    }
}