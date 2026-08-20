using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace BlazorServerTests.Api.Components.Account.Pages
{
    public partial class RegisterIdentity
    {
        private System.Collections.Generic.IEnumerable<IdentityError>? identityErrors;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();
        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }
        private string? Message => identityErrors is null ? null : $"Error: {string.Join(", ", identityErrors.Select(error => error.Description))}";

        public async Task RegisterUser()
        {
            identityErrors = await AuthService.Register(Input.Email, Input.Password, ReturnUrl ?? string.Empty);
        }

        private void NavigateToRegisterConfirmation()
        {
            NavigationManager.NavigateTo("Account/RegisterConfirmation");
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
        }
    }
}