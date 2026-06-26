using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace <#= Namespace #>Components.Account.Shared
{
    public partial class ManageLayout
    {
        [CascadingParameter]
        private HttpContext? HttpContext { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        // The manage shell is static SSR (no circuit), so the theme is derived from the theme cookie
        // on the server rather than a JS/theme service. Mirrors AccountLayout. (Used by the MudBlazor
        // shell's MudThemeProvider; harmless for the non-Mud shell.)
        private bool IsDarkTheme =>
            !(HttpContext?.Request.Cookies.TryGetValue("theme", out var theme) == true && theme == "light");

        protected override void OnParametersSet()
        {
            if (HttpContext is null)
            {
                NavigationManager.Refresh(forceReload: true);
            }
        }
    }
}
