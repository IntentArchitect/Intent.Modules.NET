using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.RazorComponentCodeBehindTemplate", Version = "1.0")]

namespace BlazorNoMudBlazor.Api.Components.Account.Pages
{
    public partial class InvalidUser
    {
    }
}
