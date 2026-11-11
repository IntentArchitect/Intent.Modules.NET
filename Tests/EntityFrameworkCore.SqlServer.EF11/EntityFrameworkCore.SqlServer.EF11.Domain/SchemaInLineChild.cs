using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF11.Domain
{
    public class SchemaInLineChild
    {
        public SchemaInLineChild()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}