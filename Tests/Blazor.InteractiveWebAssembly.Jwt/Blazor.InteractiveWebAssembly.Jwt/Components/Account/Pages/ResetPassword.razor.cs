using System.ComponentModel.DataAnnotations;
using System.Text;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.ResetPasswordCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.Jwt.Components.Account.Pages
{
    public partial class ResetPassword
    {
        private IEnumerable<IdentityError>? identityErrors;
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