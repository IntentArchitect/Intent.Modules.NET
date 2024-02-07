using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MinimalHostingModel.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    public class ForgotPasswordDto
    {
        public ForgotPasswordDto()
        {
            Email = null!;
        }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        public static ForgotPasswordDto Create(string email)
        {
            return new ForgotPasswordDto
            {
                Email = email
            };
        }
    }
}