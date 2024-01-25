using Intent.RoslynWeaver.Attributes;
using RichDomain.Domain.Events;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace RichDomain.Domain.Entities
{
    public partial class Person : IPerson
    {
        public Person(string firstName)
        {
            FirstName = firstName;
        }

        public void UpdatePerson(string firstName)
        {
            FirstName = firstName;
            DomainEvents.Add(new PersonUpdatedEvent(person: this));
        }

        public void UpdatePerson(string firstName, Department department)
        {
            FirstName = firstName;
            Department = department;
            DomainEvents.Add(new PersonUpdatedEvent(person: this));
        }
    }
}