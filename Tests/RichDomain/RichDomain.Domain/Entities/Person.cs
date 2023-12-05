using Intent.RoslynWeaver.Attributes;

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
        }

        public void UpdatePerson(string firstName, Department department)
        {
            FirstName = firstName;
            Department = department;
        }
    }
}