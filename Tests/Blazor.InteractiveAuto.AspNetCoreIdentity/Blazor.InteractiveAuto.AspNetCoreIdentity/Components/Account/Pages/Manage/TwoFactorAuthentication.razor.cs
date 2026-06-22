using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveAuto.AspNetCoreIdentity.Components.Account.Pages.Manage
{
    public partial class TwoFactorAuthentication
    {
        private bool canTrack;
        private bool hasAuthenticator;
        private int recoveryCodesLeft;
        private bool is2faEnabled;
        private bool isMachineRemembered;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            var user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            canTrack = HttpContext.Features.Get<ITrackingConsentFeature>()?.CanTrack ?? true;
            hasAuthenticator = await UserManager.GetAuthenticatorKeyAsync(user) is not null;
            is2faEnabled = await UserManager.GetTwoFactorEnabledAsync(user);
            isMachineRemembered = await SignInManager.IsTwoFactorClientRememberedAsync(user);
            recoveryCodesLeft = await UserManager.CountRecoveryCodesAsync(user);
        }

        private async Task OnSubmitForgetBrowserAsync()
        {
            await SignInManager.ForgetTwoFactorClientAsync();

            RedirectManager.RedirectToCurrentPageWithStatus(
                "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.",
                HttpContext);
        }
    }
}
