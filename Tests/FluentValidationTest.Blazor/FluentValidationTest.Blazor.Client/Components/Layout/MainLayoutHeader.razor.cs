using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorLayoutHeaderCodeBehindTemplate", Version = "1.0")]

namespace FluentValidationTest.Blazor.Client.Components.Layout
{
    public partial class MainLayoutHeader
    {
    }
}