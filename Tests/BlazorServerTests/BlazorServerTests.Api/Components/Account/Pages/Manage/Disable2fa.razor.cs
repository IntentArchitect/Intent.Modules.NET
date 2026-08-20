using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace BlazorServerTests.Api.Components.Account.Pages.Manage
{
    public partial class Disable2fa
    {
        private IdentityUser user = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [CascadingParameter]
        public HttpContext? HttpContext { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            user = await UserAccessor.GetRequiredUserAsync(HttpContext);

            if (HttpMethods.IsGet(HttpContext.Request.Method) && !await UserManager.GetTwoFactorEnabledAsync(user))
            {
                throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.");
            }
        }

        private void NavigateToResetAuthenticator()
        {
            NavigationManager.NavigateTo("Account/Manage/ResetAuthenticator");
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
