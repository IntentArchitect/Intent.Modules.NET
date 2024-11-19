using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DtoSettings.Class.Private.Domain.Entities
{
    public class Customer : Person
    {
        public Customer()
        {
            Name = null!;
            Surname = null!;
        }
        public string Name { get; set; }

        public string Surname { get; set; }
    }
}