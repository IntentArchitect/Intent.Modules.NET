using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.RazorServerComponentCodeBehindTemplate", Version = "1.0")]

namespace BlazorServerTests.Api.Components.Account.Shared
{
    public partial class StatusMessage
    {
        private string? messageFromCookie;

        [Parameter]
        public string? Message { get; set; }

        [CascadingParameter]
        private HttpContext? HttpContext { get; set; } = default!;

        private string? DisplayMessage => Message ?? messageFromCookie;

        protected override void OnInitialized()
        {
            messageFromCookie = HttpContext.Request.Cookies[IdentityRedirectManager.StatusCookieName];

            if (messageFromCookie is not null)
            {
                HttpContext.Response.Cookies.Delete(IdentityRedirectManager.StatusCookieName);
            }
        }
    }
}
