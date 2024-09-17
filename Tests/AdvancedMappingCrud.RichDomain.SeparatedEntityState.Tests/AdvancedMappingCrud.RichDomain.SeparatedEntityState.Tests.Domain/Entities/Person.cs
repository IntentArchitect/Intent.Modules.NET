using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class Person
    {
        public Person(PersonDetails details)
        {
            Details = details;
        }

        public void Update(PersonDetails details)
        {
            Details = details;
        }
    }
}