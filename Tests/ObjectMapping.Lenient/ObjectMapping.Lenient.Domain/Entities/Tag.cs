using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ObjectMapping.Lenient.Domain.Entities
{
    public class Tag
    {
        public Tag()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}