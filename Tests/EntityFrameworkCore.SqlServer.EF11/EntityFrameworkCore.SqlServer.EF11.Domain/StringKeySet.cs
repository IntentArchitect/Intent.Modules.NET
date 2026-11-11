using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain
{
    public class StringKeySet
    {
        public StringKeySet()
        {
            Id = null!;
        }

        public string Id { get; set; }
    }
}