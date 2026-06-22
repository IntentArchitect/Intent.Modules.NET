using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.ConfirmEmailCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.Jwt.Components.Account.Pages
{
    public partial class ConfirmEmail
    {
        private string? statusMessage;
        [CascadingParameter]
        public HttpContext HttpContext { get; set; } = default!;
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
    }
}