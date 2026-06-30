using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using <#= IdentityClassNamespace #>;
using System.ComponentModel.DataAnnotations;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace <#= Namespace #>Components.Account.Pages.Manage
{
    public partial class SetPassword
    {
        private string? message;
        private <#= IdentityClass #> user = default!;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            Input ??= new();
            user = await UserAccessor.GetRequiredUserAsync(HttpContext);

            var hasPassword = await UserManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                RedirectManager.RedirectTo("Account/Manage/ChangePassword");
            }
        }

        private async Task OnValidSubmitAsync()
        {
            var addPasswordResult = await UserManager.AddPasswordAsync(user, Input.NewPassword!);
            if (!addPasswordResult.Succeeded)
            {
                message = $"Error: {string.Join(",", addPasswordResult.Errors.Select(error => error.Description))}";
                return;
            }

            await SignInManager.RefreshSignInAsync(user);
            RedirectManager.RedirectToCurrentPageWithStatus("Your password has been set.", HttpContext);
        }

        private sealed class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string? NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }
        }
    }
}
