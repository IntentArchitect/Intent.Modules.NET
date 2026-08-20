using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorLayoutSiderCodeBehindTemplate", Version = "1.0")]

namespace BlazorServerTests.Api.Components.Layout
{
    public partial class MainLayoutSider
    {
    }
}