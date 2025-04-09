using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.People
{
    public class PersonDCDto
    {
        public PersonDCDto()
        {
            FirstName = null!;
            Surname = null!;
        }

        public string FirstName { get; set; }
        public string Surname { get; set; }

        public static PersonDCDto Create(string firstName, string surname)
        {
            return new PersonDCDto
            {
                FirstName = firstName,
                Surname = surname
            };
        }
    }
}