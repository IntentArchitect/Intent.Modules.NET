using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.ModelDefinitionTest
{
    public partial class ModelDefinitionInComponent
    {
        protected override async Task OnInitializedAsync()
        {
        }

        public class ModelInComponent
        {
            public string Property { get; set; }
        }
    }
}