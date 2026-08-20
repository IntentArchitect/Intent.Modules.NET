using System.Text;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.AspNetCoreIdentity.Components.Account.Pages
{
    public partial class RegisterConfirmation
    {
        private string? emailConfirmationLink;
        private string? statusMessage;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [CascadingParameter]
        public HttpContext? HttpContext { get; set; } = default!;

        [SupplyParameterFromQuery]
        public string? Email { get; set; }

        [SupplyParameterFromQuery]
        public string? ReturnUrl { get; set; }

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
            else
            {
                if (EmailSender.GetType().Name == "IdentityNoOpEmailSender")
                {
                    var userId = await UserManager.GetUserIdAsync(user);
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    emailConfirmationLink = NavigationManager.GetUriWithQueryParameters(NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri, new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = ReturnUrl });
                }
            }
        }

        private void NavigateToConfirmEmail()
        {
            NavigationManager.NavigateTo("Account/ConfirmEmail");
        }
    }
}
