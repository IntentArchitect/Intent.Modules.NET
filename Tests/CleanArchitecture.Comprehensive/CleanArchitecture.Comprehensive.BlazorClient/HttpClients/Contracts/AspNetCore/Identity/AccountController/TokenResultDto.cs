using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.AspNetCore.Identity.AccountController
{
    public class TokenResultDto
    {
        public TokenResultDto()
        {
            TokenType = null!;
            AuthenticationToken = null!;
            RefreshToken = null!;
        }

        [Required(ErrorMessage = "Token type is required.")]
        public string TokenType { get; set; }
        [Required(ErrorMessage = "Authentication token is required.")]
        public string AuthenticationToken { get; set; }
        public int ExpiresIn { get; set; }
        [Required(ErrorMessage = "Refresh token is required.")]
        public string RefreshToken { get; set; }

        public static TokenResultDto Create(string tokenType, string authenticationToken, int expiresIn, string refreshToken)
        {
            return new TokenResultDto
            {
                TokenType = tokenType,
                AuthenticationToken = authenticationToken,
                ExpiresIn = expiresIn,
                RefreshToken = refreshToken
            };
        }
    }
}