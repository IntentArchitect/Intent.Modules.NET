using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.PkNoneProvider.Domain.Entities
{
    public class StringKeyDefault
    {
        public StringKeyDefault()
        {
            Id = null!;
        }

        public string Id { get; set; }
    }
}