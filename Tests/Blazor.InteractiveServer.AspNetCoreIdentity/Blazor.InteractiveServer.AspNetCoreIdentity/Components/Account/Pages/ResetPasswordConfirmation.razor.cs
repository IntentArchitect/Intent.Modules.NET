using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.AspNetCoreIdentity.Components.Account.Pages
{
    public partial class ResetPasswordConfirmation
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private void NavigateToLogin()
        {
            NavigationManager.NavigateTo("Account/Login");
        }
    }
}
