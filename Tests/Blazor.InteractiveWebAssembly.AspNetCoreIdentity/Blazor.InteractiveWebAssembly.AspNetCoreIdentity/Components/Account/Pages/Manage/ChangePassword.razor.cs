using System.ComponentModel.DataAnnotations;
using Blazor.InteractiveWebAssembly.AspNetCoreIdentity.Data;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.AspNetCoreIdentity.Components.Account.Pages.Manage
{
    public partial class ChangePassword
    {
        private string? message;
        private ApplicationUser user = default!;
        private bool hasPassword;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [CascadingParameter]
        public HttpContext? HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            Input ??= new();
            user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            hasPassword = await UserManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                RedirectManager.RedirectTo("Account/Manage/SetPassword");
            }
        }

        private void NavigateToSetPassword()
        {
            NavigationManager.NavigateTo("Account/Manage/SetPassword");
        }

        private async Task OnValidSubmitAsync()
        {
            var changePasswordResult = await UserManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                message = $"Error: {string.Join(",", changePasswordResult.Errors.Select(error => error.Description))}";
                return;
            }

            await SignInManager.RefreshSignInAsync(user);
            Logger.LogInformation("User changed their password successfully.");

            RedirectManager.RedirectToCurrentPageWithStatus("Your password has been changed", HttpContext);
        }

        private sealed class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; } = "";

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; } = "";

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = "";
        }
    }
}
