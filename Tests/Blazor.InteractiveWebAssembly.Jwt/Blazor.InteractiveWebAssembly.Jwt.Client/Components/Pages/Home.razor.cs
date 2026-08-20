using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.PageCodeBehindTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.Jwt.Client.Components.Pages
{
    public partial class Home
    {
    }
}