using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain.ExplicitKeys
{
    public class PK_PrimaryKeyInt
    {
        public int PrimaryKeyId { get; set; }
    }
}