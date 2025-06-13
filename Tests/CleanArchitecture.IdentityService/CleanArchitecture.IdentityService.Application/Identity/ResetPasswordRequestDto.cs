using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application
{
    public class ResetPasswordRequestDto
    {
        public ResetPasswordRequestDto()
        {
            Email = null!;
            ResetCode = null!;
            NewPassword = null!;
        }

        public string Email { get; set; }
        public string ResetCode { get; set; }
        public string NewPassword { get; set; }

        public static ResetPasswordRequestDto Create(string email, string resetCode, string newPassword)
        {
            return new ResetPasswordRequestDto
            {
                Email = email,
                ResetCode = resetCode,
                NewPassword = newPassword
            };
        }
    }
}