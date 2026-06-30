using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Blazor.InteractiveServer.AspNetCoreIdentity.Data;
using System.ComponentModel.DataAnnotations;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.AspNetCoreIdentity.Components.Account.Pages.Manage
{
    public partial class ChangePassword
    {
        private string? message;
        private ApplicationUser user = default!;
        private bool hasPassword;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

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
