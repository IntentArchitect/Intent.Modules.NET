using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users
{
    public class AddContactDetailsVODto
    {
        public AddContactDetailsVODto()
        {
            Cell = null!;
            Email = null!;
        }

        public string Cell { get; set; }
        public string Email { get; set; }

        public static AddContactDetailsVODto Create(string cell, string email)
        {
            return new AddContactDetailsVODto
            {
                Cell = cell,
                Email = email
            };
        }
    }
}