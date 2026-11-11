using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.ExplicitKeys
{
    public class ChildNonStdId
    {
        public ChildNonStdId()
        {
            Name = null!;
        }

        public Guid DiffId { get; set; }

        public string Name { get; set; }
    }
}