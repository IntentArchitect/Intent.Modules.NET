using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MinimalHostingModel.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    public class LoginDto
    {
        public LoginDto()
        {
            Email = null!;
            Password = null!;
        }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        public static LoginDto Create(string email, string password)
        {
            return new LoginDto
            {
                Email = email,
                Password = password
            };
        }
    }
}