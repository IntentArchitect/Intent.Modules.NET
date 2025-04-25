using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.BindingPagesTest
{
    public partial class BindablePageComponent
    {
        [Parameter]
        public string StringParam { get; set; }
        [Parameter]
        public int IntParam { get; set; }
        [Parameter]
        public TestModel ModelParam { get; set; }

        protected override async Task OnInitializedAsync()
        {
        }
    }
}