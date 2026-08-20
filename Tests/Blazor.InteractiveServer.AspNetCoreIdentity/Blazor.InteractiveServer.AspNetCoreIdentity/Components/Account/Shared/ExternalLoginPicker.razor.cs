using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerComponentCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.AspNetCoreIdentity.Components.Account.Shared
{
    public partial class ExternalLoginPicker
    {
        private AuthenticationScheme[] externalLogins = [];
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [SupplyParameterFromQuery]
        public string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            externalLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync()).ToArray();
        }

        private void NavigateToExternalLogin()
        {
            NavigationManager.NavigateTo("Account/ExternalLogin");
        }
    }
}
