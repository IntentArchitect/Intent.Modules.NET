using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Associations
{
    public class H_OptionalAggregateNav
    {
        public H_OptionalAggregateNav()
        {
            OptionalAggrNavAttr = null!;
        }

        public Guid Id { get; set; }

        public string OptionalAggrNavAttr { get; set; }

        public virtual ICollection<H_MultipleDependent> H_MultipleDependents { get; set; } = [];
    }
}