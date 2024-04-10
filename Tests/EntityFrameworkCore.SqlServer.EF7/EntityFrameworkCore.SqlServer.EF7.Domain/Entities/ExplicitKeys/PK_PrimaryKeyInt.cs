using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Domain.Entities.ExplicitKeys
{
    public class PK_PrimaryKeyInt
    {
        public int PrimaryKeyId { get; set; }
    }
}