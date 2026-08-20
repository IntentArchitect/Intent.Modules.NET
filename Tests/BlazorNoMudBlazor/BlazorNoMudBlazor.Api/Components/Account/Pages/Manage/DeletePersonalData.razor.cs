using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace BlazorNoMudBlazor.Api.Components.Account.Pages.Manage
{
    public partial class DeletePersonalData
    {
        private string? message;
        private IdentityUser user = default!;
        private bool requirePassword;

        [CascadingParameter]
        public HttpContext? HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            Input ??= new();
            user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            requirePassword = await UserManager.HasPasswordAsync(user);
        }

        private async Task OnValidSubmitAsync()
        {
            if (requirePassword && !await UserManager.CheckPasswordAsync(user, Input.Password))
            {
                message = "Error: Incorrect password.";
                return;
            }

            var result = await UserManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Unexpected error occurred deleting user.");
            }

            await SignInManager.SignOutAsync();

            var userId = await UserManager.GetUserIdAsync(user);
            Logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            RedirectManager.RedirectToCurrentPage();
        }

        private sealed class InputModel
        {
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";
        }
    }
}
