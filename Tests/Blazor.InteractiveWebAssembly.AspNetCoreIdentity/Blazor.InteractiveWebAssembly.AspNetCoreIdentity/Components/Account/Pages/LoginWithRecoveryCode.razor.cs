using System.ComponentModel.DataAnnotations;
using Blazor.InteractiveWebAssembly.AspNetCoreIdentity.Data;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.AspNetCoreIdentity.Components.Account.Pages
{
    public partial class LoginWithRecoveryCode
    {
        private string? message;
        private ApplicationUser user = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = default!;

        [SupplyParameterFromQuery]
        public string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Input ??= new();
            // Ensure the user has gone through the username & password screen first
            user = await SignInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");
        }

        private void NavigateToLockout()
        {
            NavigationManager.NavigateTo("Account/Lockout");
        }

        private async Task OnValidSubmitAsync()
        {
            var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty);

            var result = await SignInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            var userId = await UserManager.GetUserIdAsync(user);

            if (result.Succeeded)
            {
                Logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", userId);
                RedirectManager.RedirectTo(ReturnUrl);
            }
            else if (result.IsLockedOut)
            {
                Logger.LogWarning("User account locked out.");
                RedirectManager.RedirectTo("Account/Lockout");
            }
            else
            {
                Logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", userId);
                message = "Error: Invalid recovery code entered.";
            }
        }

        private sealed class InputModel
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Recovery Code")]
            public string RecoveryCode { get; set; } = "";
        }
    }
}
