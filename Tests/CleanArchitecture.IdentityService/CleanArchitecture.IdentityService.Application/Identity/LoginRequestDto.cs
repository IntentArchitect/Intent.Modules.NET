using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application
{
    public class LoginRequestDto
    {
        public LoginRequestDto()
        {
            Email = null!;
            Password = null!;
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public string? TwoFactorCode { get; set; }
        public string? TwoFactorRecoveryCode { get; set; }

        public static LoginRequestDto Create(
            string email,
            string password,
            string? twoFactorCode,
            string? twoFactorRecoveryCode)
        {
            return new LoginRequestDto
            {
                Email = email,
                Password = password,
                TwoFactorCode = twoFactorCode,
                TwoFactorRecoveryCode = twoFactorRecoveryCode
            };
        }
    }
}