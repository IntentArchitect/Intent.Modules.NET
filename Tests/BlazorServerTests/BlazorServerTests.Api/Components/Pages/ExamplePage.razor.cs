using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace BlazorServerTests.Api.Components.Pages
{
    public partial class ExamplePage
    {
        [Parameter]
        public string Title { get; set; } = "Example Page";
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
        }

        private void NavigateToCustomerSearch()
        {
            NavigationManager.NavigateTo("customer-search");
        }
    }
}