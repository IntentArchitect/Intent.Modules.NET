using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.PageCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Components.Pages
{
    public partial class Home
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private void NavigateToCustomerList()
        {
            NavigationManager.NavigateTo("customers");
        }
    }
}