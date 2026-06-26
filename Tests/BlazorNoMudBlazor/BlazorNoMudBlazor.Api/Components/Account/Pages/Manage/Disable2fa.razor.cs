using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace BlazorNoMudBlazor.Api.Components.Account.Pages.Manage
{
    public partial class Disable2fa
    {
        private IdentityUser user = default!;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            user = await UserAccessor.GetRequiredUserAsync(HttpContext);

            if (HttpMethods.IsGet(HttpContext.Request.Method) && !await UserManager.GetTwoFactorEnabledAsync(user))
            {
                throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.");
            }
        }

        private async Task OnSubmitAsync()
        {
            var disable2faResult = await UserManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new InvalidOperationException("Unexpected error occurred disabling 2FA.");
            }

            var userId = await UserManager.GetUserIdAsync(user);
            Logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", userId);
            RedirectManager.RedirectToWithStatus(
                "Account/Manage/TwoFactorAuthentication",
                "2fa has been disabled. You can reenable 2fa when you setup an authenticator app",
                HttpContext);
        }
    }
}
