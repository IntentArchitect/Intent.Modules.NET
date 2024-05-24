using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    public class UserCreateUserContactDetailsVODto
    {
        public UserCreateUserContactDetailsVODto()
        {
            Cell = null!;
            Email = null!;
        }

        public string Cell { get; set; }
        public string Email { get; set; }

        public static UserCreateUserContactDetailsVODto Create(string cell, string email)
        {
            return new UserCreateUserContactDetailsVODto
            {
                Cell = cell,
                Email = email
            };
        }
    }
}