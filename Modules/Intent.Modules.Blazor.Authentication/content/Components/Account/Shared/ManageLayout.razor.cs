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

        protected override void OnParametersSet()
        {
            if (HttpContext is null)
            {
                NavigationManager.Refresh(forceReload: true);
            }
        }
    }
}
