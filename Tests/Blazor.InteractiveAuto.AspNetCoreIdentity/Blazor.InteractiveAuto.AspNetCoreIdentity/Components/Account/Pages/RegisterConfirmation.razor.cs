using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveAuto.AspNetCoreIdentity.Components.Account.Pages
{
    public partial class RegisterConfirmation
    {
        private string? emailConfirmationLink;
        private string? statusMessage;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromQuery]
        private string? Email { get; set; }

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (Email is null)
            {
                RedirectManager.RedirectTo("");
            }

            var user = await UserManager.FindByEmailAsync(Email);
            if (user is null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                statusMessage = "Error finding user for unspecified email";
            }
            else if (EmailSender is IdentityNoOpEmailSender)
            {
                // Once you add a real email sender, you should remove this code that lets you confirm the account
                var userId = await UserManager.GetUserIdAsync(user);
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                emailConfirmationLink = NavigationManager.GetUriWithQueryParameters(
                    NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                    new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = ReturnUrl });
            }
        }
    }
}
