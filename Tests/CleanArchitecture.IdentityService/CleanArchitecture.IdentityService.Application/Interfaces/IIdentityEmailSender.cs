using CleanArchitecture.IdentityService.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IdentityService.IdentityEmailSenderInterface", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application.Interfaces
{
    public interface IIdentityEmailSender
    {
        Task SendConfirmationLinkAsync(ApplicationIdentityUser user, string email, string confirmationLink);
        Task SendPasswordResetLinkAsync(ApplicationIdentityUser user, string email, string resetLink);
        Task SendPasswordResetCodeAsync(ApplicationIdentityUser user, string email, string resetCode);
    }
}