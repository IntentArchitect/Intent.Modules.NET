using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MinimalHostingModel.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    public class UpdateInfoDto
    {
        public UpdateInfoDto()
        {
            NewEmail = null!;
            NewPassword = null!;
            OldPassword = null!;
        }

        [Required(ErrorMessage = "New email is required.")]
        public string NewEmail { get; set; }
        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Old password is required.")]
        public string OldPassword { get; set; }

        public static UpdateInfoDto Create(string newEmail, string newPassword, string oldPassword)
        {
            return new UpdateInfoDto
            {
                NewEmail = newEmail,
                NewPassword = newPassword,
                OldPassword = oldPassword
            };
        }
    }
}