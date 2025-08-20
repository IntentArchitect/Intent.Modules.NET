using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.IdentityNoOpEmailSenderTemplate", Version = "1.0")]

namespace BlazorNoMudBlazor.Api.Components.Account
{
    internal sealed class IdentityNoOpEmailSender : IEmailSender<IdentityUser>
    {
        private readonly IEmailSender emailSender = new NoOpEmailSender();

        public async Task SendConfirmationLinkAsync(IdentityUser user, string email, string confirmationLink)
        {
            await emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
        }

        public async Task SendPasswordResetLinkAsync(IdentityUser user, string email, string resetLink)
        {
            await emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
        }

        public async Task SendPasswordResetCodeAsync(IdentityUser user, string email, string resetCode)
        {
            await emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
        }
    }
}