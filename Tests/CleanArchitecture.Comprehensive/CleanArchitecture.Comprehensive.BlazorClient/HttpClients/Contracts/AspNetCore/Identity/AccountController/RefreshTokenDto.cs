using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    public class RefreshTokenDto
    {
        public RefreshTokenDto()
        {
            RefreshToken = null!;
        }

        [Required(ErrorMessage = "Refresh token is required.")]
        public string RefreshToken { get; set; }

        public static RefreshTokenDto Create(string refreshToken)
        {
            return new RefreshTokenDto
            {
                RefreshToken = refreshToken
            };
        }
    }
}