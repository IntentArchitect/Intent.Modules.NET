using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DtoSettings.Record.Public.Domain.Entities
{
    public class Customer : Person
    {
        public string Name { get; set; }

        public string Surname { get; set; }
    }
}