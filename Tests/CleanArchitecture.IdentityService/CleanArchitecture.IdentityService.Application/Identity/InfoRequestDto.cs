using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application.Identity
{
    public class InfoRequestDto
    {
        public InfoRequestDto()
        {
            NewEmail = null!;
            NewPassword = null!;
            OldPassword = null!;
        }

        public string NewEmail { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }

        public static InfoRequestDto Create(string newEmail, string newPassword, string oldPassword)
        {
            return new InfoRequestDto
            {
                NewEmail = newEmail,
                NewPassword = newPassword,
                OldPassword = oldPassword
            };
        }
    }
}