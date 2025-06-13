using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application
{
    public class ForgotPasswordRequestDto
    {
        public ForgotPasswordRequestDto()
        {
            Email = null!;
        }

        public string Email { get; set; }

        public static ForgotPasswordRequestDto Create(string email)
        {
            return new ForgotPasswordRequestDto
            {
                Email = email
            };
        }
    }
}