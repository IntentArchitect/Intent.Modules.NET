using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.Postgres.Domain.Entities.ExplicitKeys
{
    public class PK_PrimaryKeyInt
    {
        public int PrimaryKeyId { get; set; }
    }
}