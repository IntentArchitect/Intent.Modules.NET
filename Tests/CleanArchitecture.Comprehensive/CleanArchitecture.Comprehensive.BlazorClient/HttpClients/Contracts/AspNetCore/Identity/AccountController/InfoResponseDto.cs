using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    public class InfoResponseDto
    {
        public InfoResponseDto()
        {
            Email = null!;
        }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        public static InfoResponseDto Create(string email)
        {
            return new InfoResponseDto
            {
                Email = email
            };
        }
    }
}