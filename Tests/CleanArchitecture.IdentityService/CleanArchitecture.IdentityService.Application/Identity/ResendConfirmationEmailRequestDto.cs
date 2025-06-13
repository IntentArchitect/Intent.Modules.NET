using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application
{
    public class ResendConfirmationEmailRequestDto
    {
        public ResendConfirmationEmailRequestDto()
        {
            Email = null!;
        }

        public string Email { get; set; }

        public static ResendConfirmationEmailRequestDto Create(string email)
        {
            return new ResendConfirmationEmailRequestDto
            {
                Email = email
            };
        }
    }
}