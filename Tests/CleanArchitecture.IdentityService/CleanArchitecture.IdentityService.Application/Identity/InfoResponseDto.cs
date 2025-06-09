using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application.Identity
{
    public class InfoResponseDto
    {
        public InfoResponseDto()
        {
            Email = null!;
        }

        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }

        public static InfoResponseDto Create(string email, bool isEmailConfirmed)
        {
            return new InfoResponseDto
            {
                Email = email,
                IsEmailConfirmed = isEmailConfirmed
            };
        }
    }
}