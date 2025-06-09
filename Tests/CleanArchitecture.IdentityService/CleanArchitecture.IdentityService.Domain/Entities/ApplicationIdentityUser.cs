using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Identity;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IdentityService.ApplicationIdentityUser", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Domain.Entities
{
    public class ApplicationIdentityUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpired { get; set; }
    }
}