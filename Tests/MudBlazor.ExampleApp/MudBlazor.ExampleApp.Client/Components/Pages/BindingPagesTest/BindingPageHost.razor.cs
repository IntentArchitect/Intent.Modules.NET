using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Client.Pages.BindingPagesTest;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.PageCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Components.Pages.BindingPagesTest
{
    public partial class BindingPageHost
    {
        public string StringValue { get; set; }
        public int IntValue { get; set; }
        public TestModel ModelValue { get; set; }

        protected override async Task OnInitializedAsync()
        {
        }
    }
}