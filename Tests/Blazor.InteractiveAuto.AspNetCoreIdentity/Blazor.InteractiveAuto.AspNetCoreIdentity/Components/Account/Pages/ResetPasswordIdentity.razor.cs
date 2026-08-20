using System.ComponentModel.DataAnnotations;
using System.Text;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveAuto.AspNetCoreIdentity.Components.Account.Pages
{
    public partial class ResetPasswordIdentity
    {
        private System.Collections.Generic.IEnumerable<IdentityError>? identityErrors;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();
        [SupplyParameterFromQuery]
        private string? Code { get; set; }
        private string? Message => identityErrors is null ? null : $"Error: {string.Join(", ", identityErrors.Select(error => error.Description))}";

        protected override void OnInitialized()
        {
            if (Code is null)
            {
                RedirectManager.RedirectTo("Account/InvalidPasswordReset");
            }
            Input.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
        }

        private void NavigateToLogin()
        {
            NavigationManager.NavigateTo("Account/Login");
        }

        private void NavigateToInvalidPasswordReset()
        {
            NavigationManager.NavigateTo("Account/InvalidPasswordReset");
        }

        private void NavigateToResetPasswordConfirmation()
        {
            NavigationManager.NavigateTo("Account/ResetPasswordConfirmation");
        }

        private async Task OnValidSubmitAsync()
        {
            await AuthService.ResetPassword(Input.Email, Input.Code, Input.Password);
        }

        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = "";
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = "";
            [Required]
            public string Code { get; set; } = "";
        }
    }
}