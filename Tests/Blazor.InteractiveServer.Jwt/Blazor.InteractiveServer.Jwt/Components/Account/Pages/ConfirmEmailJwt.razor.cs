using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerPageCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.Jwt.Components.Account.Pages
{
    public partial class ConfirmEmailJwt
    {
        private string? statusMessage;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [CascadingParameter]
        public HttpContext? HttpContext { get; set; } = default!;
        [SupplyParameterFromQuery]
        public string? UserId { get; set; }
        [SupplyParameterFromQuery]
        public string? Code { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (UserId is null || Code is null)
            {
                RedirectManager.RedirectTo("");
            }
            statusMessage = await AuthService.ConfirmEmail(UserId, Code);
        }

        private void NavigateToLogin()
        {
            NavigationManager.NavigateTo("Account/Login");
        }
    }
}