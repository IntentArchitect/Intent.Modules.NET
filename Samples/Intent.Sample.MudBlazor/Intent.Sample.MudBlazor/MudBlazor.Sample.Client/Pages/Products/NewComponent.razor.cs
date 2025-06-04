using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.Sample.Client.Pages.Products
{
    public partial class NewComponent
    {
        protected override async Task OnInitializedAsync()
        {
        }

        private void Operation()
        {
        }

        public class MyNewModelDefinition
        {
            public string Field1 { get; set; }
        }
    }
}