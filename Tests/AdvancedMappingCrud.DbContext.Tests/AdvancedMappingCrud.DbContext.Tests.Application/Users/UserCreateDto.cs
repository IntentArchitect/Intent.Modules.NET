using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Users
{
    public class UserCreateDto
    {
        public UserCreateDto()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        public static UserCreateDto Create(string name, string surname, string email)
        {
            return new UserCreateDto
            {
                Name = name,
                Surname = surname,
                Email = email
            };
        }
    }
}