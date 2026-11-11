using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.Associations
{
    public class A_OptionalDependent
    {
        public A_OptionalDependent()
        {
            OptionalDepAttr = null!;
        }

        public Guid Id { get; set; }

        public string OptionalDepAttr { get; set; }
    }
}