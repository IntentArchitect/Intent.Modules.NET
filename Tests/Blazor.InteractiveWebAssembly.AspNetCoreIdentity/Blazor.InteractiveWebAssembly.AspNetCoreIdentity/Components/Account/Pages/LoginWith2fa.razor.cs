using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Blazor.InteractiveWebAssembly.AspNetCoreIdentity.Data;
using System.ComponentModel.DataAnnotations;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.AspNetCoreIdentity.Components.Account.Pages
{
    public partial class LoginWith2fa
    {
        private string? message;
        private ApplicationUser user = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        [SupplyParameterFromQuery]
        private bool RememberMe { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // Ensure the user has gone through the username & password screen first
            user = await SignInManager.GetTwoFactorAuthenticationUserAsync() ??
                throw new InvalidOperationException("Unable to load two-factor authentication user.");
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
