using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorLayoutHeaderCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveAuto.Oidc.Client.Components.Layout
{
    public partial class MainLayoutHeader
    {
    }
}