using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveAuto.Jwt.Components.Account.Pages
{
    public partial class LoginJwt
    {
        private string? errorMessage;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [CascadingParameter]
        private HttpContext? HttpContext { get; set; } = default!;
        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();
        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        public async Task LoginUser()
        {
            await AuthService.Login(Input.Email, Input.Password, Input.RememberMe, ReturnUrl ?? string.Empty);
        }

        private void NavigateToForgotPassword()
        {
            NavigationManager.NavigateTo("Account/ForgotPassword");
        }

        private void NavigateToRegister()
        {
            NavigationManager.NavigateTo("Account/Register");
        }

        private void NavigateToResendEmailConfirmation()
        {
            NavigationManager.NavigateTo("Account/ResendEmailConfirmation");
        }

        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = "";
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }
    }
}