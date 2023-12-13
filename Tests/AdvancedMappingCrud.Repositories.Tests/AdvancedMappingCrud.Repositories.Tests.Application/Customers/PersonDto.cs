using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers
{
    public class PersonDto
    {
        public PersonDto()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        public static PersonDto Create(string name, string surname, string email)
        {
            return new PersonDto
            {
                Name = name,
                Surname = surname,
                Email = email
            };
        }
    }
}