using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Server.AccountLayoutCodeBehindTemplate", Version = "1.0")]

namespace BlazorNoMudBlazor.Api.Components.Account.Shared
{
    public partial class AccountLayout
    {
        [CascadingParameter]
        private HttpContext? HttpContext { get; set; }

        protected override void OnParametersSet()
        {
            if (HttpContext is null)
            {
                NavigationManager.Refresh(forceReload: true);
            }
        }
    }
}