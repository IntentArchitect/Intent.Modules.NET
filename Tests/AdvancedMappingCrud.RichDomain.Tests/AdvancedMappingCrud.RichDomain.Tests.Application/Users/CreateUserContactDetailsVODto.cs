using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users
{
    public class CreateUserContactDetailsVODto
    {
        public CreateUserContactDetailsVODto()
        {
            Cell = null!;
            Email = null!;
        }

        public string Cell { get; set; }
        public string Email { get; set; }

        public static CreateUserContactDetailsVODto Create(string cell, string email)
        {
            return new CreateUserContactDetailsVODto
            {
                Cell = cell,
                Email = email
            };
        }
    }
}