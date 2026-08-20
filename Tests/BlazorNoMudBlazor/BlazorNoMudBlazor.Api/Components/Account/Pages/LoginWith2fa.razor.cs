using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace BlazorNoMudBlazor.Api.Components.Account.Pages
{
    public partial class LoginWith2fa
    {
        private string? message;
        private IdentityUser user = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = default!;

        [SupplyParameterFromQuery]
        public string? ReturnUrl { get; set; }

        [SupplyParameterFromQuery]
        public bool RememberMe { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Input ??= new();
            // Ensure the user has gone through the username & password screen first
            user = await SignInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");
        }

        private void NavigateToLoginWithRecoveryCode()
        {
            NavigationManager.NavigateTo("Account/LoginWithRecoveryCode");
        }

        private void NavigateToLockout()
        {
            NavigationManager.NavigateTo("Account/Lockout");
        }

        private async Task OnValidSubmitAsync()
        {
            var authenticatorCode = Input.TwoFactorCode!.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await SignInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, RememberMe, Input.RememberMachine);
            var userId = await UserManager.GetUserIdAsync(user);

            if (result.Succeeded)
            {
                Logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", userId);
                RedirectManager.RedirectTo(ReturnUrl);
            }
            else if (result.IsLockedOut)
            {
                Logger.LogWarning("User with ID '{UserId}' account locked out.", userId);
                RedirectManager.RedirectTo("Account/Lockout");
            }
            else
            {
                Logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", userId);
                message = "Error: Invalid authenticator code.";
            }
        }

        private sealed class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator code")]
            public string? TwoFactorCode { get; set; }

            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }
        }
    }
}
