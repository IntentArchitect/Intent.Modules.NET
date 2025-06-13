using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application
{
    public class AccessTokenResponseDto
    {
        public AccessTokenResponseDto()
        {
            TokenType = null!;
            AccessToken = null!;
            RefreshToken = null!;
        }

        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiresIn { get; set; }
        public string RefreshToken { get; set; }

        public static AccessTokenResponseDto Create(
            string tokenType,
            string accessToken,
            DateTime expiresIn,
            string refreshToken)
        {
            return new AccessTokenResponseDto
            {
                TokenType = tokenType,
                AccessToken = accessToken,
                ExpiresIn = expiresIn,
                RefreshToken = refreshToken
            };
        }
    }
}