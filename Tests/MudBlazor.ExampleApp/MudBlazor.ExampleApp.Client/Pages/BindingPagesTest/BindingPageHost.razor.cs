using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.BindingPagesTest
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