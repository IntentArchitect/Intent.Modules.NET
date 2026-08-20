using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.AspNetCoreIdentity.Components.Account.Pages.Manage
{
    public partial class ResetAuthenticator
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [CascadingParameter]
        public HttpContext? HttpContext { get; set; } = default!;

        private void NavigateToEnableAuthenticator()
        {
            NavigationManager.NavigateTo("Account/Manage/EnableAuthenticator");
        }

        private async Task OnSubmitAsync()
        {
            var user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            await UserManager.SetTwoFactorEnabledAsync(user, false);
            await UserManager.ResetAuthenticatorKeyAsync(user);
            var userId = await UserManager.GetUserIdAsync(user);
            Logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", userId);

            await SignInManager.RefreshSignInAsync(user);

            RedirectManager.RedirectToWithStatus(
                "Account/Manage/EnableAuthenticator",
                "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.",
                HttpContext);
        }
    }
}
