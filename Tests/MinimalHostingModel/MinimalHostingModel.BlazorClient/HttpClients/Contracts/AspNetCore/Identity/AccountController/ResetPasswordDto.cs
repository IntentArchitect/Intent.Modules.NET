using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MinimalHostingModel.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    public class ResetPasswordDto
    {
        public ResetPasswordDto()
        {
            Email = null!;
            ResetCode = null!;
            NewPassword = null!;
        }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Reset code is required.")]
        public string ResetCode { get; set; }
        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; }

        public static ResetPasswordDto Create(string email, string resetCode, string newPassword)
        {
            return new ResetPasswordDto
            {
                Email = email,
                ResetCode = resetCode,
                NewPassword = newPassword
            };
        }
    }
}