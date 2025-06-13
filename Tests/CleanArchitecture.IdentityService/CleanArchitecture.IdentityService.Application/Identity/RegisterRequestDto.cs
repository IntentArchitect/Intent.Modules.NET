using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application
{
    public class RegisterRequestDto
    {
        public RegisterRequestDto()
        {
            Email = null!;
            Password = null!;
        }

        public string Email { get; set; }
        public string Password { get; set; }

        public static RegisterRequestDto Create(string email, string password)
        {
            return new RegisterRequestDto
            {
                Email = email,
                Password = password
            };
        }
    }
}