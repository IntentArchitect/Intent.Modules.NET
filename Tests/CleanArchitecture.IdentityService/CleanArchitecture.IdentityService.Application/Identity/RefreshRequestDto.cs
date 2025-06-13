using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application
{
    public class RefreshRequestDto
    {
        public RefreshRequestDto()
        {
            RefreshToken = null!;
        }

        public string RefreshToken { get; set; }

        public static RefreshRequestDto Create(string refreshToken)
        {
            return new RefreshRequestDto
            {
                RefreshToken = refreshToken
            };
        }
    }
}