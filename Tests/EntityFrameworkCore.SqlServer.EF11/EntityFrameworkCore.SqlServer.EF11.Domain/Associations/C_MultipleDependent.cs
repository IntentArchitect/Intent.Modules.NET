using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Associations
{
    public class C_MultipleDependent
    {
        public C_MultipleDependent()
        {
            MultipleDepAttr = null!;
        }

        public Guid Id { get; set; }

        public string MultipleDepAttr { get; set; }

        public Guid C_RequiredCompositeId { get; set; }
    }
}