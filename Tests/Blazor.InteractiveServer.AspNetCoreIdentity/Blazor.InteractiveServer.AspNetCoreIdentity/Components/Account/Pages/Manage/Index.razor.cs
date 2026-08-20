using System.ComponentModel.DataAnnotations;
using Blazor.InteractiveServer.AspNetCoreIdentity.Data;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.AspNetCoreIdentity.Components.Account.Pages.Manage
{
    public partial class Index
    {
        private ApplicationUser user = default!;
        private string? username;
        private string? phoneNumber;

        [CascadingParameter]
        public HttpContext? HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            Input ??= new();
            user = await UserAccessor.GetRequiredUserAsync(HttpContext);
            username = await UserManager.GetUserNameAsync(user);
            phoneNumber = await UserManager.GetPhoneNumberAsync(user);

            Input.PhoneNumber ??= phoneNumber;
        }

        private async Task OnValidSubmitAsync()
        {
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await UserManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    RedirectManager.RedirectToCurrentPageWithStatus("Error: Failed to set phone number.", HttpContext);
                }
            }

            await SignInManager.RefreshSignInAsync(user);
            RedirectManager.RedirectToCurrentPageWithStatus("Your profile has been updated", HttpContext);
        }

        private sealed class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }
        }
    }
}
