using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.ModelDefinitionTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.ModelDefinitionTest
{
    public class ModelOutsideComponent
    {
        public string Property { get; set; }
    }
}