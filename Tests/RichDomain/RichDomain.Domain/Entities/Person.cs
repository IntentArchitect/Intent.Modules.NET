using Intent.RoslynWeaver.Attributes;

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