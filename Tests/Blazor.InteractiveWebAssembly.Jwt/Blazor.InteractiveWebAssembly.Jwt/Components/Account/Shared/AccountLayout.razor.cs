using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.AccountLayoutCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.Jwt.Components.Account.Shared
{
    public partial class AccountLayout
    {
        [CascadingParameter]
        private HttpContext? HttpContext { get; set; }
        private bool IsDarkTheme => !(HttpContext?.Request.Cookies.TryGetValue("theme", out var theme) == true && theme == "light");

        protected override void OnParametersSet()
        {
            if (HttpContext is null)
            {
                NavigationManager.Refresh(forceReload: true);
            }
        }
    }
}