using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IdentityService.EmailSenderInterface", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application.Interfaces
{
    public interface IEmailSender<TUser>
        where TUser : class
    {
        Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink);
        Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink);
        Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode);
    }
}